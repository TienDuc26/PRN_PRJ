using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class GuideController : Controller
{
    private readonly IGuideService _service;
    private readonly IScheduleService _scheduleService;
    private readonly IAuditLogService _audit;

    public GuideController(IGuideService service, IScheduleService scheduleService, IAuditLogService audit)
    {
        _service = service;
        _scheduleService = scheduleService;
        _audit = audit;
    }

    public async Task<IActionResult> Index(string? keyword, int? status, int page = 1)
    {
        var result = await _service.GetPagedAsync(keyword, status, page, 10);
        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new GuideFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GuideFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var g = new Guide
        {
            FullName = model.FullName,
            DateOfBirth = model.DateOfBirth,
            Phone = model.Phone,
            Email = model.Email,
            Address = model.Address,
            ExperienceYears = model.ExperienceYears,
            Languages = model.Languages,
            Bio = model.Bio,
            Status = model.Status
        };
        await _service.CreateAsync(g);
        await _audit.LogAsync(User.GetUserId(), "CREATE_GUIDE", "Guide", g.Id.ToString(), null, g.FullName, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Tạo HDV thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var g = await _service.GetByIdAsync(id);
        if (g == null) return NotFound();
        var model = new GuideFormViewModel
        {
            Id = g.Id,
            FullName = g.FullName,
            DateOfBirth = g.DateOfBirth,
            Phone = g.Phone,
            Email = g.Email,
            Address = g.Address,
            ExperienceYears = g.ExperienceYears,
            Languages = g.Languages,
            Bio = g.Bio,
            Status = g.Status
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GuideFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var g = new Guide
        {
            Id = model.Id,
            FullName = model.FullName,
            DateOfBirth = model.DateOfBirth,
            Phone = model.Phone,
            Email = model.Email,
            Address = model.Address,
            ExperienceYears = model.ExperienceYears,
            Languages = model.Languages,
            Bio = model.Bio,
            Status = model.Status
        };
        await _service.UpdateAsync(g);
        await _audit.LogAsync(User.GetUserId(), "UPDATE_GUIDE", "Guide", g.Id.ToString(), null, g.FullName, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Cập nhật HDV thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) TempData["Error"] = "Không thể xóa HDV đã phân công";
        else TempData["Success"] = "Đã xóa HDV";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        await _service.ToggleStatusAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Schedule(int id)
    {
        var g = await _service.GetByIdAsync(id);
        if (g == null) return NotFound();
        var schedules = await _service.GetGuideScheduleAsync(id);
        ViewBag.Guide = g;
        return View(schedules);
    }

    [HttpGet]
    public async Task<IActionResult> Assign(int scheduleId)
    {
        var s = await _scheduleService.GetByIdAsync(scheduleId);
        if (s == null) return NotFound();
        var guides = await _service.GetActiveGuidesAsync();
        ViewBag.Schedule = s;
        ViewBag.Guides = guides;
        return View(new GuideAssignViewModel { ScheduleId = scheduleId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(GuideAssignViewModel model)
    {
        try
        {
            await _service.AssignGuideAsync(model.GuideId, model.ScheduleId, model.Note);
            await _audit.LogAsync(User.GetUserId(), "ASSIGN_GUIDE", "Schedule", model.ScheduleId.ToString(), null, $"Guide:{model.GuideId}", HttpContext.Connection.RemoteIpAddress?.ToString());
            TempData["Success"] = "Phân công thành công";
            return RedirectToAction("Schedule", new { id = model.GuideId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Assign", new { scheduleId = model.ScheduleId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Unassign(int assignmentId, int guideId)
    {
        await _service.UnassignAsync(assignmentId);
        return RedirectToAction("Schedule", new { id = guideId });
    }
}