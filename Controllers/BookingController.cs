using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Controllers;

public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IScheduleService _scheduleService;
    private readonly ITourService _tourService;
    private readonly IPromotionService _promoService;
    private readonly IReviewService _reviewService;
    private readonly IPaymentService _paymentService;
    private readonly IGuestSessionService _guestSession;
    private readonly AppDbContext _db;

    public BookingController(IBookingService bookingService, IScheduleService scheduleService, ITourService tourService,
        IPromotionService promoService, IReviewService reviewService, IPaymentService paymentService,
        IGuestSessionService guestSession, AppDbContext db)
    {
        _bookingService = bookingService;
        _scheduleService = scheduleService;
        _tourService = tourService;
        _promoService = promoService;
        _reviewService = reviewService;
        _paymentService = paymentService;
        _guestSession = guestSession;
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Create(int scheduleId, int adults = 1, int children = 0, string? promoCode = null)
    {
        var schedule = await _scheduleService.GetByIdAsync(scheduleId);
        if (schedule == null || schedule.Status != (int)ScheduleStatus.OPEN || schedule.StartDate.Date < DateTime.UtcNow.Date)
        {
            TempData["Error"] = "Lịch khởi hành không khả dụng";
            return RedirectToAction("Details", "Tour", new { id = schedule?.TourId });
        }
        var tour = await _tourService.GetTourByIdAsync(schedule.TourId);
        if (tour == null) return NotFound();

        var model = new BookingCreateViewModel
        {
            ScheduleId = scheduleId,
            Adults = adults,
            Children = children,
            PromotionCode = promoCode
        };
        ViewBag.Schedule = schedule;
        ViewBag.Tour = tour;
        ViewBag.IsGuest = !(User.Identity?.IsAuthenticated ?? false);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(BookingCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var schedule = await _scheduleService.GetByIdAsync(model.ScheduleId);
            var tour = await _tourService.GetTourByIdAsync(schedule!.TourId);
            ViewBag.Schedule = schedule; ViewBag.Tour = tour;
            return View(model);
        }
        if (model.Participants.Count != (model.Adults + model.Children))
        {
            ModelState.AddModelError("", $"Số người tham gia phải khớp với {model.Adults + model.Children}");
            var schedule = await _scheduleService.GetByIdAsync(model.ScheduleId);
            var tour = await _tourService.GetTourByIdAsync(schedule!.TourId);
            ViewBag.Schedule = schedule; ViewBag.Tour = tour;
            return View(model);
        }

        try
        {
            var userId = User.GetUserId();
            var booking = await _bookingService.CreateBookingAsync(userId, model);
            _guestSession.ClearBookingSelection();
            TempData["Success"] = $"Đặt tour thành công! Mã đơn: {booking.BookingCode}";
            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            var schedule = await _scheduleService.GetByIdAsync(model.ScheduleId);
            var tour = await _tourService.GetTourByIdAsync(schedule!.TourId);
            ViewBag.Schedule = schedule; ViewBag.Tour = tour;
            return View(model);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(int page = 1, int? status = null)
    {
        var userId = User.GetUserId();
        var result = await _bookingService.GetUserBookingsAsync(userId, page, 10, status);
        ViewBag.Status = status;
        return View(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();
        if (booking.UserId != User.GetUserId() && !User.IsInRole("ADMIN") && !User.IsInRole("STAFF"))
            return Forbid();

        ViewBag.CanReview = booking.Status == (int)BookingStatus.COMPLETED && booking.Review == null;
        ViewBag.AvgRating = await _reviewService.GetAverageRatingAsync(booking.Schedule!.TourId);
        ViewBag.ReviewCount = await _reviewService.GetReviewCountAsync(booking.Schedule!.TourId);
        return View(booking);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var ok = await _bookingService.CancelBookingAsync(id, User.GetUserId(), false);
            if (ok) TempData["Success"] = "Đã hủy đơn đặt tour";
            else TempData["Error"] = "Không thể hủy đơn này";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> CheckPromo(string code, decimal amount)
    {
        var result = await _promoService.ValidateAsync(code, amount);
        return Json(new { success = result.Success, message = result.Message, discount = result.Discount });
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult SaveGuestSelection([FromBody] GuestBookingSelection selection)
    {
        if (selection == null || selection.ScheduleId <= 0 || selection.Adults <= 0)
            return BadRequest();
        _guestSession.SaveBookingSelection(
            selection.TourId, selection.ScheduleId,
            selection.Adults, selection.Children, selection.PromoCode);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ContinueFromGuest()
    {
        var sel = _guestSession.GetBookingSelection();
        if (sel == null)
        {
            TempData["Warning"] = "Phiên đặt tour đã hết hạn. Vui lòng chọn lại tour.";
            return RedirectToAction("Index", "Tour");
        }

        var schedule = await _scheduleService.GetByIdAsync(sel.ScheduleId);
        if (schedule == null || schedule.Status != (int)ScheduleStatus.OPEN || schedule.StartDate.Date < DateTime.UtcNow.Date)
        {
            _guestSession.ClearBookingSelection();
            TempData["Warning"] = "Lịch khởi hành không còn khả dụng. Vui lòng chọn lịch khác.";
            return RedirectToAction("Details", "Tour", new { id = sel.TourId });
        }
        if (schedule.AvailableSeats < (sel.Adults + sel.Children))
        {
            _guestSession.ClearBookingSelection();
            TempData["Warning"] = $"Số chỗ không đủ. Lịch này chỉ còn {schedule.AvailableSeats} chỗ.";
            return RedirectToAction("Details", "Tour", new { id = sel.TourId });
        }

        if (!string.IsNullOrWhiteSpace(sel.PromoCode))
        {
            var amount = schedule.Price * sel.Adults + (int)(schedule.Price * 0.7m * sel.Children);
            var promoResult = await _promoService.ValidateAsync(sel.PromoCode, amount);
            if (!promoResult.Success)
                sel.PromoCode = null;
        }

        var promoCodeToPass = sel.PromoCode;
        _guestSession.ClearBookingSelection();

        return RedirectToAction(nameof(Create), new
        {
            scheduleId = sel.ScheduleId,
            adults = sel.Adults,
            children = sel.Children,
            promoCode = promoCodeToPass
        });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Pay(int id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();
        if (booking.UserId != User.GetUserId()) return Forbid();
        if (booking.PaymentStatus == (int)PaymentStatus.PAID)
        {
            TempData["Success"] = "Đơn này đã thanh toán đủ";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (booking.Status == (int)BookingStatus.CANCELLED || booking.Status == (int)BookingStatus.COMPLETED)
        {
            TempData["Error"] = "Đơn không thể thanh toán";
            return RedirectToAction(nameof(Details), new { id });
        }

        var paid = await _paymentService.GetPaidAmountAsync(id);
        var remaining = booking.TotalAmount - paid;
        var model = new PaymentCreateViewModel
        {
            BookingId = id,
            Amount = remaining,
            Method = (int)PaymentMethod.BANK_TRANSFER
        };
        ViewBag.Booking = booking;
        ViewBag.Paid = paid;
        ViewBag.Remaining = remaining;
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Pay(PaymentCreateViewModel model)
    {
        var booking = await _bookingService.GetByIdAsync(model.BookingId);
        if (booking == null) return NotFound();
        if (booking.UserId != User.GetUserId()) return Forbid();

        var paid = await _paymentService.GetPaidAmountAsync(model.BookingId);
        var remaining = booking.TotalAmount - paid;
        if (model.Amount <= 0 || model.Amount > remaining)
        {
            TempData["Error"] = $"Số tiền không hợp lệ. Tối đa: {remaining:N0} đ";
            return RedirectToAction(nameof(Pay), new { id = model.BookingId });
        }

        try
        {
            var amount = Math.Round(model.Amount, 0, MidpointRounding.AwayFromZero);
            if (remaining - amount >= 0 && remaining - amount < 100m)
                amount = remaining;
            await _paymentService.CreatePaymentAsync(model.BookingId, amount, model.Method, model.Note, User.GetFullName());
            TempData["Success"] = $"Thanh toán {amount:N0} đ thành công";
            return RedirectToAction(nameof(Details), new { id = model.BookingId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Pay), new { id = model.BookingId });
        }
    }
}
