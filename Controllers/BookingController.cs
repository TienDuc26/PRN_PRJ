using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IScheduleService _scheduleService;
    private readonly ITourService _tourService;
    private readonly IPromotionService _promoService;
    private readonly IReviewService _reviewService;
    private readonly IPaymentService _paymentService;
    private readonly AppDbContext _db;

    public BookingController(IBookingService bookingService, IScheduleService scheduleService, ITourService tourService,
        IPromotionService promoService, IReviewService reviewService, IPaymentService paymentService, AppDbContext db)
    {
        _bookingService = bookingService;
        _scheduleService = scheduleService;
        _tourService = tourService;
        _promoService = promoService;
        _reviewService = reviewService;
        _paymentService = paymentService;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int scheduleId)
    {
        var schedule = await _scheduleService.GetByIdAsync(scheduleId);
        if (schedule == null || schedule.Status != (int)ScheduleStatus.OPEN || schedule.StartDate.Date < DateTime.UtcNow.Date)
        {
            TempData["Error"] = "Lịch khởi hành không khả dụng";
            return RedirectToAction("Details", "Tour", new { id = schedule?.TourId });
        }
        var tour = await _tourService.GetTourByIdAsync(schedule.TourId);
        if (tour == null) return NotFound();
        var model = new BookingCreateViewModel { ScheduleId = scheduleId };
        ViewBag.Schedule = schedule;
        ViewBag.Tour = tour;
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
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

    public async Task<IActionResult> Index(int page = 1, int? status = null)
    {
        var userId = User.GetUserId();
        var result = await _bookingService.GetUserBookingsAsync(userId, page, 10, status);
        ViewBag.Status = status;
        return View(result);
    }

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
    public async Task<IActionResult> CheckPromo(string code, decimal amount)
    {
        var result = await _promoService.ValidateAsync(code, amount);
        return Json(new { success = result.Success, message = result.Message, discount = result.Discount });
    }

    [HttpGet]
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
            // Làm tròn số tiền về nguyên (chống lệch do decimal precision)
            var amount = Math.Round(model.Amount, 0, MidpointRounding.AwayFromZero);
            // Snap lên remaining nếu người dùng nhập gần đúng (lệch ≤ 100đ)
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