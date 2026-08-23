using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class DestinationController : Controller
{
    private readonly IDestinationService _service;
    private readonly IAuditLogService _audit;
    public DestinationController(IDestinationService service, IAuditLogService audit) { _service = service; _audit = audit; }

    public async Task<IActionResult> Index(string? keyword, int page = 1)
    {
        var result = await _service.GetPagedAsync(keyword, page, 10);
        ViewBag.Keyword = keyword;
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new DestinationViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DestinationViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid) return View(model);
        var dest = new Destination
        {
            Name = model.Name,
            City = model.City,
            Country = model.Country,
            Description = model.Description,
            Status = model.Status,
            Image = await FileUploadHelper.SaveImageAsync(image, "tours")
        };
        await _service.CreateAsync(dest);
        await _audit.LogAsync(User.GetUserId(), "CREATE_DESTINATION", "Destination", dest.Id.ToString(), null, dest.Name, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Tạo điểm đến thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var d = await _service.GetByIdAsync(id);
        if (d == null) return NotFound();
        var model = new DestinationViewModel { Id = d.Id, Name = d.Name, City = d.City, Country = d.Country, Description = d.Description, Status = d.Status, Image = d.Image };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DestinationViewModel model, IFormFile? image)
    {
        if (!ModelState.IsValid) return View(model);
        var dest = new Destination
        {
            Id = model.Id,
            Name = model.Name,
            City = model.City,
            Country = model.Country,
            Description = model.Description,
            Status = model.Status
        };
        if (image != null && image.Length > 0)
        {
            var old = await _service.GetByIdAsync(model.Id);
            if (!string.IsNullOrEmpty(old?.Image)) FileUploadHelper.DeleteImage(old.Image);
            dest.Image = await FileUploadHelper.SaveImageAsync(image, "tours");
        }
        await _service.UpdateAsync(dest);
        await _audit.LogAsync(User.GetUserId(), "UPDATE_DESTINATION", "Destination", dest.Id.ToString(), null, dest.Name, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Cập nhật điểm đến thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
        {
            var d = await _service.GetByIdAsync(id);
            if (d != null) { d.Status = 2; await _service.UpdateAsync(d); }
            TempData["Success"] = "Điểm đến đã có tour - đã chuyển sang INACTIVE";
        }
        else
        {
            TempData["Success"] = "Đã xóa điểm đến";
        }
        return RedirectToAction(nameof(Index));
    }
}