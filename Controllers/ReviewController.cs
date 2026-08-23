using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService) => _reviewService = reviewService;

    [HttpGet]
    public async Task<IActionResult> Create(int bookingId)
    {
        var canReview = await _reviewService.CanReviewAsync(User.GetUserId(), bookingId);
        if (!canReview)
        {
            TempData["Error"] = "Bạn không thể đánh giá đơn này";
            return RedirectToAction("Details", "Booking", new { id = bookingId });
        }
        var model = new ReviewCreateViewModel { BookingId = bookingId };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var review = await _reviewService.CreateAsync(User.GetUserId(), model.BookingId, model.Rating, model.Content, model.Image);
            TempData["Success"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction("Details", "Tour", new { id = review.TourId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }
}