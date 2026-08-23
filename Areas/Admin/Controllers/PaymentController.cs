using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IBookingService _bookingService;
    private readonly IAuditLogService _audit;

    public PaymentController(IPaymentService paymentService, IBookingService bookingService, IAuditLogService audit)
    {
        _paymentService = paymentService;
        _bookingService = bookingService;
        _audit = audit;
    }

    public async Task<IActionResult> Index(int bookingId)
    {
        var booking = await _bookingService.GetByIdAsync(bookingId);
        if (booking == null) return NotFound();
        var payments = await _paymentService.GetByBookingAsync(bookingId);
        ViewBag.Booking = booking;
        ViewBag.PaidAmount = await _paymentService.GetPaidAmountAsync(bookingId);
        ViewBag.Remaining = booking.TotalAmount - ViewBag.PaidAmount;
        return View(payments);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int bookingId)
    {
        var booking = await _bookingService.GetByIdAsync(bookingId);
        if (booking == null) return NotFound();
        var paid = await _paymentService.GetPaidAmountAsync(bookingId);
        var model = new PaymentCreateViewModel { BookingId = bookingId, Amount = booking.TotalAmount - paid };
        ViewBag.Booking = booking;
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Booking = await _bookingService.GetByIdAsync(model.BookingId);
            return View(model);
        }
        try
        {
            await _paymentService.CreatePaymentAsync(model.BookingId, model.Amount, model.Method, model.Note, User.GetFullName());
            await _audit.LogAsync(User.GetUserId(), "CREATE_PAYMENT", "Booking", model.BookingId.ToString(), null, $"{model.Amount}", HttpContext.Connection.RemoteIpAddress?.ToString());
            TempData["Success"] = "Đã ghi nhận thanh toán";
            return RedirectToAction(nameof(Index), new { bookingId = model.BookingId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index), new { bookingId = model.BookingId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Refund(int paymentId, decimal amount, string? note)
    {
        var ok = await _paymentService.RefundAsync(paymentId, amount, note, User.GetFullName());
        if (ok) TempData["Success"] = "Đã hoàn tiền";
        else TempData["Error"] = "Không thể hoàn tiền";
        return RedirectToAction("Details", "Booking", new { id = (await _paymentService.GetByIdAsync(paymentId))?.BookingId });
    }
}