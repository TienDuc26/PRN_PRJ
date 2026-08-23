using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Controllers;

public class TourController : Controller
{
    private readonly ITourService _tourService;
    private readonly IDestinationService _destinationService;
    private readonly IScheduleService _scheduleService;
    private readonly IReviewService _reviewService;
    private readonly AppDbContext _db;

    public TourController(ITourService tourService, IDestinationService destinationService, IScheduleService scheduleService,
        IReviewService reviewService, AppDbContext db)
    {
        _tourService = tourService;
        _destinationService = destinationService;
        _scheduleService = scheduleService;
        _reviewService = reviewService;
        _db = db;
    }

    public async Task<IActionResult> Index(TourFilterViewModel filter)
    {
        filter.Status = 1;
        if (filter.Page < 1) filter.Page = 1;
        var result = await _tourService.GetToursAsync(filter);
        ViewBag.Destinations = await _destinationService.GetAllAsync();
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var tour = await _tourService.GetTourWithDetailsAsync(id);
        if (tour == null || tour.Status != 1) return NotFound();

        var schedules = await _scheduleService.GetByTourAsync(id);
        var upcoming = schedules.Where(s => s.StartDate >= DateTime.UtcNow.Date && s.Status != 4).ToList();
        ViewBag.Schedules = upcoming;

        var reviews = await _reviewService.GetByTourAsync(id, 1, 5, true);
        ViewBag.Reviews = reviews;
        ViewBag.AvgRating = await _reviewService.GetAverageRatingAsync(id);
        ViewBag.ReviewCount = await _reviewService.GetReviewCountAsync(id);

        return View(tour);
    }

    [HttpGet]
    public async Task<IActionResult> ByDestination(int id)
    {
        var dest = await _destinationService.GetByIdAsync(id);
        if (dest == null) return NotFound();
        ViewBag.Destination = dest;
        var filter = new TourFilterViewModel { DestinationId = id, Status = 1 };
        var result = await _tourService.GetToursAsync(filter);
        return View(result);
    }
}