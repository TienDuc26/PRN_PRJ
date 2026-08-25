using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class ScheduleController : Controller
{
    private readonly IScheduleService _service;
    private readonly ITourService _tourService;
    private readonly IAuditLogService _audit;

    public ScheduleController(IScheduleService service, ITourService tourService, IAuditLogService audit)
    {
        _service = service;
        _tourService = tourService;
        _audit = audit;
    }

    public async Task<IActionResult> Index(ScheduleFilterViewModel filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        var result = await _service.GetPagedAsync(filter);
        ViewBag.Tours = await _tourService.GetToursAsync(new TourFilterViewModel { PageSize = 100 });
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? tourId)
    {
        var tours = await _tourService.GetToursAsync(new TourFilterViewModel { PageSize = 100 });
        ViewBag.Tours = tours.Items;
        var model = new ScheduleFormViewModel { TourId = tourId ?? tours.Items.FirstOrDefault()?.Id ?? 0 };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleFormViewModel model)
    {
        if (model.EndDate < model.StartDate) ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
        if (model.StartDate.Date < DateTime.UtcNow.Date) ModelState.AddModelError("StartDate", "Không thể tạo lịch trong quá khứ");
        if (!ModelState.IsValid)
        {
            ViewBag.Tours = (await _tourService.GetToursAsync(new TourFilterViewModel { PageSize = 100 })).Items;
            return View(model);
        }
        var sch = new TourSchedule
        {
            TourId = model.TourId,
            Code = model.Code,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            MeetingTime = model.MeetingTime,
            MeetingPoint = model.MeetingPoint,
            MaxGuests = model.MaxGuests,
            Price = model.Price,
            Status = model.Status
        };
        var created = await _service.CreateAsync(sch);
        await _audit.LogAsync(User.GetUserId(), "CREATE_SCHEDULE", "Schedule", created.Id.ToString(), null, created.Code, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Tạo lịch khởi hành thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var s = await _service.GetByIdAsync(id);
        if (s == null) return NotFound();
        var model = new ScheduleFormViewModel
        {
            Id = s.Id,
            TourId = s.TourId,
            Code = s.Code,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            MeetingTime = s.MeetingTime,
            MeetingPoint = s.MeetingPoint,
            MaxGuests = s.MaxGuests,
            Price = s.Price,
            Status = s.Status
        };
        ViewBag.Tours = (await _tourService.GetToursAsync(new TourFilterViewModel { PageSize = 100 })).Items;
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ScheduleFormViewModel model)
    {
        if (model.EndDate < model.StartDate) ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
        if (model.StartDate.Date < DateTime.UtcNow.Date) ModelState.AddModelError("StartDate", "Không thể đặt lịch trong quá khứ");
        if (!ModelState.IsValid)
        {
            ViewBag.Tours = (await _tourService.GetToursAsync(new TourFilterViewModel { PageSize = 100 })).Items;
            return View(model);
        }
        var sch = new TourSchedule
        {
            Id = model.Id,
            TourId = model.TourId,
            Code = model.Code,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            MeetingTime = model.MeetingTime,
            MeetingPoint = model.MeetingPoint,
            MaxGuests = model.MaxGuests,
            Price = model.Price,
            Status = model.Status
        };
        await _service.UpdateAsync(sch);
        await _audit.LogAsync(User.GetUserId(), "UPDATE_SCHEDULE", "Schedule", sch.Id.ToString(), null, sch.Code, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Cập nhật lịch thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) TempData["Error"] = "Không thể xóa lịch đã có booking";
        else
        {
            await _audit.LogAsync(User.GetUserId(), "DELETE_SCHEDULE", "Schedule", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
            TempData["Success"] = "Đã xóa lịch";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        await _service.CancelAsync(id);
        await _audit.LogAsync(User.GetUserId(), "CANCEL_SCHEDULE", "Schedule", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Close(int id)
    {
        await _service.CloseAsync(id);
        await _audit.LogAsync(User.GetUserId(), "CLOSE_SCHEDULE", "Schedule", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        return RedirectToAction(nameof(Index));
    }
}