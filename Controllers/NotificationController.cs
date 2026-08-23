using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notiService;
    public NotificationController(INotificationService notiService) => _notiService = notiService;

    public async Task<IActionResult> Index(int page = 1)
    {
        var userId = User.GetUserId();
        var items = await _notiService.GetUserNotificationsAsync(userId, page, 20);
        ViewBag.Unread = await _notiService.CountUnreadAsync(userId);
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _notiService.MarkReadAsync(id, User.GetUserId());
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllRead()
    {
        await _notiService.MarkAllReadAsync(User.GetUserId());
        TempData["Success"] = "Đã đánh dấu tất cả đã đọc";
        return RedirectToAction(nameof(Index));
    }
}