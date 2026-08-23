using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class BookingController : Controller
{
    private readonly IBookingService _service;
    private readonly ITourService _tourService;
    private readonly IAuditLogService _audit;

    public BookingController(IBookingService service, ITourService tourService, IAuditLogService audit)
    {
        _service = service;
        _tourService = tourService;
        _audit = audit;
    }

    public async Task<IActionResult> Index(BookingFilterViewModel filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        var result = await _service.GetAllBookingsAsync(filter);
        ViewBag.Tours = (await _tourService.GetToursAsync(new TourFilterViewModel { PageSize = 100 })).Items;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var booking = await _service.GetByIdAsync(id);
        if (booking == null) return NotFound();
        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(int id)
    {
        await _service.ConfirmBookingAsync(id);
        await _audit.LogAsync(User.GetUserId(), "CONFIRM_BOOKING", "Booking", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        await _service.CompleteBookingAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        await _service.CancelBookingAsync(id, User.GetUserId(), true);
        await _audit.LogAsync(User.GetUserId(), "CANCEL_BOOKING", "Booking", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString());
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> SetStatus(int id, int status)
    {
        await _service.UpdateStatusAsync(id, status);
        return RedirectToAction(nameof(Details), new { id });
    }
}