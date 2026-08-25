using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;
using System.Text.Json;

namespace TourManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICustomerService _customerService;
    private readonly IAuditLogService _auditLog;
    private readonly IGuestSessionService _guestSession;
    private readonly AppDbContext _db;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ICustomerService customerService, IAuditLogService auditLog, IGuestSessionService guestSession, AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerService = customerService;
        _auditLog = auditLog;
        _guestSession = guestSession;
        _db = db;
    }

    private async Task<string?> GetUserRoleAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }

    private string? GetUserRoleFromClaims()
    {
        return User?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
    }

    private async Task WriteLogoutLogAsync(int userId, string? role)
    {
        try
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = "LOGOUT",
                EntityType = "User",
                EntityId = userId.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };
            _db.AuditLogs.Add(log);
            var saved = await _db.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"[LOGOUT] Saved {saved} record(s), UserId={userId}, Action=LOGOUT");
            
            // Detach to avoid conflicts with SignOutAsync
            _db.Entry(log).State = EntityState.Detached;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LOGOUT ERROR] {ex.Message}");
            throw;
        }
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            ModelState.AddModelError("Email", "Email đã được sử dụng");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Phone))
        {
            var existingPhone = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == model.Phone);
            if (existingPhone != null)
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng");
                return View(model);
            }
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.Phone,
            FullName = model.FullName,
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }
        await _userManager.AddToRoleAsync(user, "CUSTOMER");
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        var role = await GetUserRoleAsync(user);
        await _auditLog.LogAsync(user.Id, "REGISTER", "User", user.Id.ToString(), null, 
            JsonSerializer.Serialize(new { email = user.Email, role }), 
            HttpContext.Connection.RemoteIpAddress?.ToString(), role);

        // Guest flow: nếu có lựa chọn đặt tour đã lưu trong Session → redirect về ContinueFromGuest
        if (_guestSession.GetBookingSelection() != null)
        {
            return RedirectToAction("ContinueFromGuest", "Booking");
        }

        TempData["Success"] = "Đăng ký thành công! Chào mừng bạn.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Log đăng nhập thất bại - không tìm thấy user
            await _auditLog.LogAsync(null, "LOGIN_FAILED", "User", null, null,
                JsonSerializer.Serialize(new { email = model.Email, reason = "Không tìm thấy tài khoản" }),
                HttpContext.Connection.RemoteIpAddress?.ToString(), null);
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            return View(model);
        }
        
        // FR-CUSTOMER-03: chặn tài khoản bị khóa ngay từ đầu, không cho thử mật khẩu
        if (user.Status == 2)
        {
            // Log tài khoản bị khóa cố đăng nhập
            var userRole = await GetUserRoleAsync(user);
            await _auditLog.LogAsync(user.Id, "LOGIN_BLOCKED", "User", user.Id.ToString(), null,
                JsonSerializer.Serialize(new { email = user.Email, reason = "Tài khoản bị khóa" }),
                HttpContext.Connection.RemoteIpAddress?.ToString(), userRole);
            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.");
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                // Log tài khoản bị khóa tạm thời do đăng nhập sai nhiều lần
                var userRole = await GetUserRoleAsync(user);
                await _auditLog.LogAsync(user.Id, "LOGIN_LOCKOUT", "User", user.Id.ToString(), null,
                    JsonSerializer.Serialize(new { email = user.Email, reason = "Tài khoản tạm khóa do đăng nhập sai nhiều lần" }),
                    HttpContext.Connection.RemoteIpAddress?.ToString(), userRole);
                ModelState.AddModelError("", "Tài khoản tạm thời bị khóa do đăng nhập sai nhiều lần. Thử lại sau 5 phút.");
            }
            else
            {
                // Log đăng nhập thất bại - sai mật khẩu
                await _auditLog.LogAsync(user.Id, "LOGIN_FAILED", "User", user.Id.ToString(), null,
                    JsonSerializer.Serialize(new { email = user.Email, reason = "Sai mật khẩu" }),
                    HttpContext.Connection.RemoteIpAddress?.ToString(), null);
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            }
            return View(model);
        }
        
        // Re-check status sau khi sign-in thành công (tránh trường hợp bị khóa giữa lúc xử lý)
        var fresh = await _userManager.FindByIdAsync(user.Id.ToString());
        if (fresh != null && fresh.Status == 2)
        {
            await _signInManager.SignOutAsync();
            var userRole = await GetUserRoleAsync(fresh);
            await _auditLog.LogAsync(user.Id, "LOGIN_BLOCKED", "User", user.Id.ToString(), null,
                JsonSerializer.Serialize(new { email = user.Email, reason = "Tài khoản bị khóa sau khi đăng nhập" }),
                HttpContext.Connection.RemoteIpAddress?.ToString(), userRole);
            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa");
            return View(model);
        }
        
        var role = await GetUserRoleAsync(user);
        await _auditLog.LogAsync(user.Id, "LOGIN_SUCCESS", "User", user.Id.ToString(), null,
            JsonSerializer.Serialize(new { email = user.Email, role }),
            HttpContext.Connection.RemoteIpAddress?.ToString(), role);
            
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        // Guest flow: sau khi đăng nhập thành công, kiểm tra có lựa chọn đặt tour đã lưu trong Session không
        if (_guestSession.GetBookingSelection() != null)
            return RedirectToAction("ContinueFromGuest", "Booking");

        if (await _userManager.IsInRoleAsync(user, "ADMIN") || await _userManager.IsInRoleAsync(user, "STAFF"))
            return RedirectToAction("Index", "Home", new { area = "Admin" });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var uid = User.GetUserId();
        var role = GetUserRoleFromClaims();
        
        // Ghi log TRƯỚC KHI SignOut để đảm bảo còn user info
        if (uid > 0)
        {
            await WriteLogoutLogAsync(uid, role);
        }
        
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            TempData["Success"] = "Link đặt lại mật khẩu đã được gửi đến email của bạn (mô phỏng)";
            TempData["ResetToken"] = token;
            TempData["ResetEmail"] = model.Email;
        }
        else
        {
            TempData["Success"] = "Nếu email tồn tại, link đặt lại đã được gửi.";
        }
        return RedirectToAction("ResetPassword");
    }

    [HttpGet]
    public IActionResult ResetPassword(string? email = null, string? token = null)
    {
        var model = new ResetPasswordViewModel
        {
            Token = token ?? TempData["ResetToken"]?.ToString() ?? ""
        };
        ViewBag.Email = email ?? TempData["ResetEmail"]?.ToString();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string email)
    {
        if (!ModelState.IsValid) { ViewBag.Email = email; return View(model); }
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["Error"] = "Email không tồn tại";
            return RedirectToAction("Login");
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
        foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
        ViewBag.Email = email;
        return View(model);
    }
}