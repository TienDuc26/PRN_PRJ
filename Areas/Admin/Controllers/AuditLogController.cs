using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN")]
public class AuditLogController : Controller
{
    private readonly IAuditLogService _service;
    public AuditLogController(IAuditLogService service) => _service = service;

    public async Task<IActionResult> Index(string? action, int page = 1)
    {
        var result = await _service.GetPagedAsync(page, 20, action);
        ViewBag.Action = action;
        return View(result);
    }
}