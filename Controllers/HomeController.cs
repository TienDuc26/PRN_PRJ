using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITourService _tourService;
    private readonly IDestinationService _destinationService;

    public HomeController(ITourService tourService, IDestinationService destinationService)
    {
        _tourService = tourService;
        _destinationService = destinationService;
    }

    public async Task<IActionResult> Index()
    {
        var destinations = await _destinationService.GetAllAsync();
        var tours = await _tourService.GetToursAsync(new ViewModels.TourFilterViewModel { Status = 1, PageSize = 8, SortBy = "newest" });
        ViewBag.Destinations = destinations.Take(6).ToList();
        ViewBag.Tours = tours.Items;
        return View();
    }

    public IActionResult About() => View();
    public IActionResult Contact() => View();

    public IActionResult Error()
    {
        return View(new Models.ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}