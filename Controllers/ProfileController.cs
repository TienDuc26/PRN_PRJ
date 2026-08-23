using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICustomerService _customerService;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICustomerService customerService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        var model = new ProfileUpdateViewModel
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            AvatarPath = user.Avatar
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProfileUpdateViewModel model, IFormFile? avatar)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (avatar != null && avatar.Length > 0)
        {
            var path = await FileUploadHelper.SaveImageAsync(avatar, "avatars");
            if (!string.IsNullOrEmpty(user.Avatar)) FileUploadHelper.DeleteImage(user.Avatar);
            model.AvatarPath = path;
        }
        else
        {
            model.AvatarPath = user.Avatar;
        }

        await _customerService.UpdateProfileAsync(user.Id, model);
        // Cập nhật Claim FullName
        var existingClaims = await _userManager.GetClaimsAsync(user);
        foreach (var c in existingClaims.Where(c => c.Type == "FullName")) await _userManager.RemoveClaimAsync(user, c);
        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", model.FullName));
        await _signInManager.RefreshSignInAsync(user);
        TempData["Success"] = "Cập nhật hồ sơ thành công";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Đổi mật khẩu thành công";
            return RedirectToAction(nameof(Index));
        }
        foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
        return View(model);
    }
}