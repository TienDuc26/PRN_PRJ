using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    public async Task<IActionResult> Index()
    {
        var model = await _dashboardService.GetDashboardAsync();
        return View(model);
    }
}