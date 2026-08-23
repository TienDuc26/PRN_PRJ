using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN")]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly AppDbContext _db;
    private readonly IAuditLogService _audit;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, AppDbContext db, IAuditLogService audit)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _audit = audit;
    }

    public async Task<IActionResult> Index(string? keyword, string? role)
    {
        var users = _db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            users = users.Where(u => u.FullName.Contains(keyword) || (u.Email != null && u.Email.Contains(keyword)));
        var list = await users.OrderByDescending(u => u.CreatedAt).Take(200).ToListAsync();
        var userIds = list.Select(u => u.Id).ToList();
        var roleMap = await _db.UserRoles.Where(ur => userIds.Contains(ur.UserId)).ToListAsync();
        var roles = await _db.Roles.ToListAsync();
        ViewBag.RoleMap = roleMap.ToDictionary(r => r.UserId, r => roles.FirstOrDefault(x => x.Id == r.RoleId)?.Name ?? "");
        ViewBag.Keyword = keyword;
        ViewBag.Role = role;
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string fullName, string email, string phone, string password, string role)
    {
        if (await _userManager.FindByEmailAsync(email) != null)
        {
            TempData["Error"] = "Email đã tồn tại";
            return RedirectToAction(nameof(Create));
        }
        var u = new ApplicationUser { UserName = email, Email = email, PhoneNumber = phone, FullName = fullName, Status = 1, EmailConfirmed = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var result = await _userManager.CreateAsync(u, password);
        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Create));
        }
        if (await _roleManager.RoleExistsAsync(role)) await _userManager.AddToRoleAsync(u, role);
        await _audit.LogAsync(User.GetUserId(), "CREATE_USER", "User", u.Id.ToString(), null, $"{u.Email}/{role}", HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Tạo user thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(int userId, string newRole)
    {
        var u = await _userManager.FindByIdAsync(userId.ToString());
        if (u == null) return NotFound();
        var currentRoles = await _userManager.GetRolesAsync(u);
        await _userManager.RemoveFromRolesAsync(u, currentRoles);
        if (await _roleManager.RoleExistsAsync(newRole)) await _userManager.AddToRoleAsync(u, newRole);
        await _audit.LogAsync(User.GetUserId(), "CHANGE_ROLE", "User", u.Id.ToString(), string.Join(",", currentRoles), newRole, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Đã đổi quyền";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Lock(int id)
    {
        var u = await _userManager.FindByIdAsync(id.ToString());
        if (u == null) return NotFound();
        u.Status = 2; u.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        await _userManager.UpdateAsync(u);
        // Đổi SecurityStamp để vô hiệu hóa tất cả session đang đăng nhập
        await _userManager.UpdateSecurityStampAsync(u);
        await _audit.LogAsync(User.GetUserId(), "LOCK_USER", "User", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Đã khóa";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Unlock(int id)
    {
        var u = await _userManager.FindByIdAsync(id.ToString());
        if (u == null) return NotFound();
        u.Status = 1; u.LockoutEnd = null;
        await _userManager.UpdateAsync(u);
        await _userManager.UpdateSecurityStampAsync(u);
        await _audit.LogAsync(User.GetUserId(), "UNLOCK_USER", "User", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Đã mở khóa";
        return RedirectToAction(nameof(Index));
    }
}