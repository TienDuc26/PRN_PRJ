using System.ComponentModel.DataAnnotations;
using TourManagement.Web.Models.Enums;

namespace TourManagement.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password), Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ProfileUpdateViewModel
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }
    public string? AvatarPath { get; set; }
}

public class ChangePasswordViewModel
{
    [Required, DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password), Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}