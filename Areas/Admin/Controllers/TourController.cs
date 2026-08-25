using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class TourController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly ITourService _tourService;
    private readonly IDestinationService _destinationService;
    private readonly IItineraryService _itineraryService;
    private readonly IAuditLogService _auditLog;

    public TourController(ITourService tourService, IDestinationService destinationService, IItineraryService itineraryService, IAuditLogService auditLog)
    {
        _tourService = tourService;
        _destinationService = destinationService;
        _itineraryService = itineraryService;
        _auditLog = auditLog;
    }

    private string? GetUserRole() => User?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

    public async Task<IActionResult> Index(TourFilterViewModel filter)
    {
        if (filter.Page < 1) filter.Page = 1;
        var result = await _tourService.GetToursAsync(filter);
        ViewBag.Destinations = await _destinationService.GetAllAsync();
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var tour = await _tourService.GetTourWithDetailsAsync(id);
        if (tour == null) return NotFound();
        return View(tour);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Destinations = await _destinationService.GetAllAsync();
        return View(new TourFormViewModel { Code = $"T-{DateTime.UtcNow:yyyyMMddHHmmss}" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TourFormViewModel model, IFormFile? thumbnail)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Destinations = await _destinationService.GetAllAsync();
            return View(model);
        }
        var tour = new Tour
        {
            Code = model.Code,
            Name = model.Name,
            DestinationId = model.DestinationId,
            Description = model.Description,
            DurationDays = model.DurationDays,
            DurationNights = model.DurationNights,
            BasePrice = model.BasePrice,
            IncludedServices = model.IncludedServices,
            ExcludedServices = model.ExcludedServices,
            Policy = model.Policy,
            TourType = model.TourType,
            Status = model.Status,
            Thumbnail = await FileUploadHelper.SaveImageAsync(thumbnail, "tours")
        };
        var created = await _tourService.CreateTourAsync(tour);
        await _auditLog.LogAsync(User.GetUserId(), "CREATE_TOUR", "Tour", created.Id.ToString(), null, created.Name, HttpContext.Connection.RemoteIpAddress?.ToString(), GetUserRole());
        TempData["Success"] = "Tạo tour thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _tourService.GetTourByIdAsync(id);
        if (tour == null) return NotFound();
        var model = new TourFormViewModel
        {
            Id = tour.Id,
            Code = tour.Code,
            Name = tour.Name,
            DestinationId = tour.DestinationId,
            Description = tour.Description,
            DurationDays = tour.DurationDays,
            DurationNights = tour.DurationNights,
            BasePrice = tour.BasePrice,
            IncludedServices = tour.IncludedServices,
            ExcludedServices = tour.ExcludedServices,
            Policy = tour.Policy,
            TourType = tour.TourType,
            Status = tour.Status,
            Thumbnail = tour.Thumbnail
        };
        ViewBag.Destinations = await _destinationService.GetAllAsync();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TourFormViewModel model, IFormFile? thumbnail)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Destinations = await _destinationService.GetAllAsync();
            return View(model);
        }
        var tour = new Tour
        {
            Id = model.Id,
            Code = model.Code,
            Name = model.Name,
            DestinationId = model.DestinationId,
            Description = model.Description,
            DurationDays = model.DurationDays,
            DurationNights = model.DurationNights,
            BasePrice = model.BasePrice,
            IncludedServices = model.IncludedServices,
            ExcludedServices = model.ExcludedServices,
            Policy = model.Policy,
            TourType = model.TourType,
            Status = model.Status
        };
        if (thumbnail != null && thumbnail.Length > 0)
        {
            var old = await _tourService.GetTourByIdAsync(model.Id);
            if (!string.IsNullOrEmpty(old?.Thumbnail)) FileUploadHelper.DeleteImage(old.Thumbnail);
            tour.Thumbnail = await FileUploadHelper.SaveImageAsync(thumbnail, "tours");
        }
        await _tourService.UpdateTourAsync(tour);
        await _auditLog.LogAsync(User.GetUserId(), "UPDATE_TOUR", "Tour", model.Id.ToString(), null, model.Name, HttpContext.Connection.RemoteIpAddress?.ToString(), GetUserRole());
        TempData["Success"] = "Cập nhật tour thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _tourService.DeleteTourAsync(id);
        if (!deleted)
        {
            await _tourService.SoftDeleteTourAsync(id);
            TempData["Success"] = "Tour đã có booking - đã chuyển sang INACTIVE";
        }
        else
        {
            TempData["Success"] = "Đã xóa tour";
        }
        await _auditLog.LogAsync(User.GetUserId(), "DELETE_TOUR", "Tour", id.ToString(), null, null, HttpContext.Connection.RemoteIpAddress?.ToString(), GetUserRole());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        await _tourService.ToggleStatusAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // Itineraries
    [HttpGet]
    public async Task<IActionResult> Itineraries(int tourId)
    {
        var tour = await _tourService.GetTourByIdAsync(tourId);
        if (tour == null) return NotFound();
        var items = await _itineraryService.GetByTourAsync(tourId);
        ViewBag.Tour = tour;
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> AddItinerary(int tourId)
    {
        var tour = await _tourService.GetTourByIdAsync(tourId);
        if (tour == null) return NotFound();
        ViewBag.Tour = tour;
        return View(new ItineraryFormViewModel { TourId = tourId, DayNumber = (await _itineraryService.GetByTourAsync(tourId)).Count + 1 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItinerary(ItineraryFormViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Tour = await _tourService.GetTourByIdAsync(model.TourId);
            return View(model);
        }
        var item = new TourItinerary
        {
            TourId = model.TourId,
            DayNumber = model.DayNumber,
            Title = model.Title,
            Description = model.Description,
            Location = model.Location,
            TimeInfo = model.TimeInfo,
            Meals = model.Meals,
            Hotel = model.Hotel,
            Notes = model.Notes,
            Image = await FileUploadHelper.SaveImageAsync(image, "tours")
        };
        await _itineraryService.CreateAsync(item);
        TempData["Success"] = "Đã thêm lịch trình";
        return RedirectToAction(nameof(Itineraries), new { tourId = model.TourId });
    }

    [HttpGet]
    public async Task<IActionResult> EditItinerary(int id)
    {
        var item = await _itineraryService.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewBag.Tour = await _tourService.GetTourByIdAsync(item.TourId);
        var model = new ItineraryFormViewModel
        {
            Id = item.Id,
            TourId = item.TourId,
            DayNumber = item.DayNumber,
            Title = item.Title,
            Description = item.Description,
            Location = item.Location,
            TimeInfo = item.TimeInfo,
            Meals = item.Meals,
            Hotel = item.Hotel,
            Notes = item.Notes,
            Image = item.Image
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItinerary(ItineraryFormViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Tour = await _tourService.GetTourByIdAsync(model.TourId);
            return View(model);
        }
        var item = new TourItinerary
        {
            Id = model.Id,
            TourId = model.TourId,
            DayNumber = model.DayNumber,
            Title = model.Title,
            Description = model.Description,
            Location = model.Location,
            TimeInfo = model.TimeInfo,
            Meals = model.Meals,
            Hotel = model.Hotel,
            Notes = model.Notes
        };
        if (image != null && image.Length > 0) item.Image = await FileUploadHelper.SaveImageAsync(image, "tours");
        else
        {
            var old = await _itineraryService.GetByIdAsync(model.Id);
            item.Image = old?.Image;
        }
        await _itineraryService.UpdateAsync(item);
        TempData["Success"] = "Đã cập nhật lịch trình";
        return RedirectToAction(nameof(Itineraries), new { tourId = model.TourId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItinerary(int id, int tourId)
    {
        await _itineraryService.DeleteAsync(id);
        return RedirectToAction(nameof(Itineraries), new { tourId });
    }

    [HttpPost]
    public async Task<IActionResult> ReorderItineraries(int tourId, int[] orderedIds)
    {
        await _itineraryService.ReorderAsync(tourId, orderedIds.ToList());
        return RedirectToAction(nameof(Itineraries), new { tourId });
    }
}