using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class ItineraryController : Controller
{
    private readonly IItineraryService _service;
    private readonly ITourService _tourService;

    public ItineraryController(IItineraryService service, ITourService tourService)
    {
        _service = service;
        _tourService = tourService;
    }

    public async Task<IActionResult> Index(int tourId)
    {
        var tour = await _tourService.GetTourByIdAsync(tourId);
        if (tour == null) return NotFound();
        var items = await _service.GetByTourAsync(tourId);
        ViewBag.Tour = tour;
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int tourId)
    {
        var tour = await _tourService.GetTourByIdAsync(tourId);
        if (tour == null) return NotFound();
        ViewBag.Tour = tour;
        return View(new ItineraryFormViewModel { TourId = tourId, DayNumber = (await _service.GetByTourAsync(tourId)).Count + 1 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ItineraryFormViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid) { ViewBag.Tour = await _tourService.GetTourByIdAsync(model.TourId); return View(model); }
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
        await _service.CreateAsync(item);
        TempData["Success"] = "Đã thêm ngày";
        return RedirectToAction(nameof(Index), new { tourId = model.TourId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
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
    public async Task<IActionResult> Edit(ItineraryFormViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid) { ViewBag.Tour = await _tourService.GetTourByIdAsync(model.TourId); return View(model); }
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
        else { var old = await _service.GetByIdAsync(model.Id); item.Image = old?.Image; }
        await _service.UpdateAsync(item);
        TempData["Success"] = "Đã cập nhật ngày";
        return RedirectToAction(nameof(Index), new { tourId = model.TourId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, int tourId)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { tourId });
    }

    [HttpPost]
    public async Task<IActionResult> Reorder(int tourId, int[] orderedIds)
    {
        await _service.ReorderAsync(tourId, orderedIds.ToList());
        return RedirectToAction(nameof(Index), new { tourId });
    }
}