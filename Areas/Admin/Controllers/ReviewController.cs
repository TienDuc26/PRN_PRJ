using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class ReviewController : Controller
{
    private readonly IReviewService _service;
    private readonly IAuditLogService _audit;
    public ReviewController(IReviewService service, IAuditLogService audit) { _service = service; _audit = audit; }

    public async Task<IActionResult> Index(int? status, int page = 1)
    {
        var result = await _service.GetPagedAdminAsync(page, 10, status);
        ViewBag.Status = status;
        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        await _service.ToggleVisibilityAsync(id);
        await _audit.LogAsync(User.GetUserId(), "TOGGLE_REVIEW_VISIBILITY", "Review", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        await _audit.LogAsync(User.GetUserId(), "DELETE_REVIEW", "Review", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        return RedirectToAction(nameof(Index));
    }
}