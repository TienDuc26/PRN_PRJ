using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICustomerService _customerService;
    private readonly IAuditLogService _auditLog;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ICustomerService customerService, IAuditLogService auditLog)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerService = customerService;
        _auditLog = auditLog;
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

        // FR-AUTH: phone trùng nếu đã đăng ký
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
        await _auditLog.LogAsync(user.Id, "REGISTER", "User", user.Id.ToString(), null, user.Email, HttpContext.Connection.RemoteIpAddress?.ToString());
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
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            return View(model);
        }
        // FR-CUSTOMER-03: chặn tài khoản bị khóa ngay từ đầu, không cho thử mật khẩu
        if (user.Status == 2)
        {
            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.");
            return View(model);
        }
        var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                ModelState.AddModelError("", "Tài khoản tạm thời bị khóa do đăng nhập sai nhiều lần. Thử lại sau 5 phút.");
            else
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            return View(model);
        }
        // Re-check status sau khi sign-in thành công (tránh trường hợp bị khóa giữa lúc xử lý)
        var fresh = await _userManager.FindByIdAsync(user.Id.ToString());
        if (fresh != null && fresh.Status == 2)
        {
            await _signInManager.SignOutAsync();
            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa");
            return View(model);
        }
        await _auditLog.LogAsync(user.Id, "LOGIN", "User", user.Id.ToString(), null, user.Email, HttpContext.Connection.RemoteIpAddress?.ToString());
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        if (await _userManager.IsInRoleAsync(user, "ADMIN") || await _userManager.IsInRoleAsync(user, "STAFF"))
            return RedirectToAction("Index", "Home", new { area = "Admin" });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var uid = User.GetUserId();
        await _signInManager.SignOutAsync();
        if (uid > 0) await _auditLog.LogAsync(uid, "LOGOUT", "User", uid.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
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