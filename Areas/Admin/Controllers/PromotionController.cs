using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class PromotionController : Controller
{
    private readonly IPromotionService _service;
    private readonly IAuditLogService _audit;
    public PromotionController(IPromotionService service, IAuditLogService audit) { _service = service; _audit = audit; }

    public async Task<IActionResult> Index(string? keyword, int page = 1)
    {
        var result = await _service.GetPagedAsync(keyword, page, 10);
        ViewBag.Keyword = keyword;
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new PromotionFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PromotionFormViewModel model)
    {
        if (model.EndAt <= model.StartAt) ModelState.AddModelError("EndAt", "Ngày kết thúc phải sau ngày bắt đầu");
        if (!ModelState.IsValid) return View(model);
        var p = new Promotion
        {
            Code = model.Code,
            Name = model.Name,
            Description = model.Description,
            DiscountType = model.DiscountType,
            DiscountValue = model.DiscountValue,
            MaxDiscount = model.MaxDiscount,
            MinOrderValue = model.MinOrderValue,
            StartAt = model.StartAt,
            EndAt = model.EndAt,
            UsageLimit = model.UsageLimit,
            Status = model.Status
        };
        await _service.CreateAsync(p);
        await _audit.LogAsync(User.GetUserId(), "CREATE_PROMOTION", "Promotion", p.Id.ToString(), null, p.Code, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Tạo khuyến mãi thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _service.GetByIdAsync(id);
        if (p == null) return NotFound();
        var model = new PromotionFormViewModel
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            MaxDiscount = p.MaxDiscount,
            MinOrderValue = p.MinOrderValue,
            StartAt = p.StartAt,
            EndAt = p.EndAt,
            UsageLimit = p.UsageLimit,
            Status = p.Status
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PromotionFormViewModel model)
    {
        if (model.EndAt <= model.StartAt) ModelState.AddModelError("EndAt", "Ngày kết thúc phải sau ngày bắt đầu");
        if (!ModelState.IsValid) return View(model);
        var p = new Promotion
        {
            Id = model.Id,
            Code = model.Code,
            Name = model.Name,
            Description = model.Description,
            DiscountType = model.DiscountType,
            DiscountValue = model.DiscountValue,
            MaxDiscount = model.MaxDiscount,
            MinOrderValue = model.MinOrderValue,
            StartAt = model.StartAt,
            EndAt = model.EndAt,
            UsageLimit = model.UsageLimit,
            Status = model.Status
        };
        await _service.UpdateAsync(p);
        await _audit.LogAsync(User.GetUserId(), "UPDATE_PROMOTION", "Promotion", p.Id.ToString(), null, p.Code, HttpContext.Connection.RemoteIpAddress?.ToString());
        TempData["Success"] = "Cập nhật khuyến mãi";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        await _service.ToggleStatusAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) TempData["Error"] = "Không thể xóa khuyến mãi đã được sử dụng";
        else TempData["Success"] = "Đã xóa khuyến mãi";
        return RedirectToAction(nameof(Index));
    }
}