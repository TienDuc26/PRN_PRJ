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

    public async Task<IActionResult> Index(
        string? action,
        string? role,
        string? userKeyword,
        DateTime? fromDate,
        DateTime? toDate,
        int page = 1)
    {
        if (page < 1) page = 1;
        
        var result = await _service.GetPagedAsync(page, 20, action, role, userKeyword, fromDate, toDate);
        
        ViewBag.Action = action;
        ViewBag.Role = role;
        ViewBag.UserKeyword = userKeyword;
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
        
        return View(result);
    }
    
    public async Task<IActionResult> LoginHistory(int page = 1)
    {
        if (page < 1) page = 1;
        
        var result = await _service.GetPagedAsync(page, 20, "LOGIN", null, null, null, null);
        ViewBag.LoginOnly = true;
        ViewBag.Action = "LOGIN";
        
        return View("Index", result);
    }
}