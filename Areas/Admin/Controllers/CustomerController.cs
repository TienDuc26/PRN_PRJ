using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class CustomerController : Controller
{
    private readonly ICustomerService _service;
    private readonly IAuditLogService _audit;
    public CustomerController(ICustomerService service, IAuditLogService audit) { _service = service; _audit = audit; }

    public async Task<IActionResult> Index(string? keyword, int? status, int page = 1)
    {
        var result = await _service.GetCustomersAsync(keyword, status, page, 10);
        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var c = await _service.GetWithBookingsAsync(id);
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpPost]
    public async Task<IActionResult> Lock(int id)
    {
        await _service.LockAsync(id);
        await _audit.LogAsync(User.GetUserId(), "LOCK_USER", "User", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Đã khóa tài khoản";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Unlock(int id)
    {
        await _service.UnlockAsync(id);
        await _audit.LogAsync(User.GetUserId(), "UNLOCK_USER", "User", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Đã mở khóa tài khoản";
        return RedirectToAction(nameof(Index));
    }
}