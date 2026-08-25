using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TourManagement.Web.Models.Enums;

namespace TourManagement.Web.ViewModels;

public class BookingCreateViewModel : IValidatableObject
{
    public int ScheduleId { get; set; }

    [Range(1, 100, ErrorMessage = "Số người lớn tối thiểu 1")]
    public int Adults { get; set; } = 1;

    [Range(0, 100)]
    public int Children { get; set; }

    public string? PromotionCode { get; set; }
    public string? Note { get; set; }

    [Required]
    public List<ParticipantInputModel> Participants { get; set; } = new();

    public DateTime? TourStartDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        var expectedTotal = Adults + Children;
        if (Participants.Count != expectedTotal)
        {
            results.Add(new ValidationResult($"Tổng số người tham gia ({Participants.Count}) phải khớp với số lượng đã chọn ({expectedTotal})"));
        }

        for (int i = 0; i < Participants.Count; i++)
        {
            Participants[i].Index = i + 1;
        }

        var identityNumbers = new List<(string value, int index)>();
        var phoneNumbers = new List<(string value, int index)>();

        for (int i = 0; i < Participants.Count; i++)
        {
            var p = Participants[i];

            if (!string.IsNullOrWhiteSpace(p.IdentityNumber))
            {
                identityNumbers.Add((p.IdentityNumber, i + 1));
            }

            if (!string.IsNullOrWhiteSpace(p.Phone))
            {
                phoneNumbers.Add((p.Phone, i + 1));
            }
        }

        var dupIdentity = identityNumbers.GroupBy(x => x.value)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        if (dupIdentity.Count > 0)
        {
            var indices = string.Join(", ", dupIdentity.Select(x => $"Người thứ {x.index}"));
            results.Add(new ValidationResult($"CMND/CCCD bị trùng lặp giữa: {indices}"));
        }

        var dupPhone = phoneNumbers.GroupBy(x => x.value)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        if (dupPhone.Count > 0)
        {
            var indices = string.Join(", ", dupPhone.Select(x => $"Người thứ {x.index}"));
            results.Add(new ValidationResult($"Số điện thoại bị trùng lặp giữa: {indices}"));
        }

        return results;
    }
}

public class ParticipantInputModel : IValidatableObject
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên từ 2-100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Giới tính là bắt buộc")]
    [Range(1, 2, ErrorMessage = "Vui lòng chọn giới tính")]
    public int? Gender { get; set; }

    public bool IsAdult { get; set; } = true;

    [StringLength(12, MinimumLength = 0, ErrorMessage = "CMND/CCCD từ 9-12 số")]
    public string? IdentityNumber { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? Note { get; set; }

    public int? Index { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Validate FullName: no numbers, Vietnamese chars allowed
        if (!string.IsNullOrWhiteSpace(FullName))
        {
            var nameRegex = new Regex(@"^[\p{L}\s'-]+$", RegexOptions.Compiled);
            if (!nameRegex.IsMatch(FullName))
            {
                results.Add(new ValidationResult("Họ tên không được chứa số hoặc ký tự đặc biệt", new[] { nameof(FullName) }));
            }

            if (string.IsNullOrWhiteSpace(FullName.Replace(" ", "").Replace("-", "").Replace("'", "")))
            {
                results.Add(new ValidationResult("Họ tên không được chỉ toàn khoảng trắng", new[] { nameof(FullName) }));
            }
        }

        // Validate DateOfBirth
        if (DateOfBirth.HasValue)
        {
            var dob = DateOfBirth.Value;

            // Not in future
            if (dob > DateTime.UtcNow.Date)
            {
                results.Add(new ValidationResult("Ngày sinh không được là ngày tương lai", new[] { nameof(DateOfBirth) }));
            }

            // Not more than 120 years ago
            if (dob < DateTime.UtcNow.Date.AddYears(-120))
            {
                results.Add(new ValidationResult("Ngày sinh không hợp lý (quá 120 năm)", new[] { nameof(DateOfBirth) }));
            }
        }

        // Validate IdentityNumber (CMND/CCCD) - chỉ khi có nhập
        if (!string.IsNullOrWhiteSpace(IdentityNumber))
        {
            var cccdRegex = new Regex(@"^(\d{9}|\d{12})$", RegexOptions.Compiled);
            if (!cccdRegex.IsMatch(IdentityNumber))
            {
                results.Add(new ValidationResult("CMND/CCCD phải là 9 hoặc 12 chữ số", new[] { nameof(IdentityNumber) }));
            }
        }

        // Validate Phone - chỉ khi có nhập
        if (!string.IsNullOrWhiteSpace(Phone))
        {
            var phoneRegex = new Regex(@"^0[3|5|7|8|9][0-9]{8}$", RegexOptions.Compiled);
            if (!phoneRegex.IsMatch(Phone))
            {
                results.Add(new ValidationResult("Số điện thoại không đúng định dạng Việt Nam (10 số, bắt đầu bằng 03/05/07/08/09)", new[] { nameof(Phone) }));
            }
        }

        // Validate required fields theo loại người
        // CMND/CCCD bắt buộc cho người lớn
        if (IsAdult && string.IsNullOrWhiteSpace(IdentityNumber))
        {
            results.Add(new ValidationResult("CMND/CCCD là bắt buộc cho người lớn", new[] { nameof(IdentityNumber) }));
        }

        // Phone bắt buộc cho người lớn
        if (IsAdult && string.IsNullOrWhiteSpace(Phone))
        {
            results.Add(new ValidationResult("Điện thoại là bắt buộc cho người lớn", new[] { nameof(Phone) }));
        }

        return results;
    }
}

public class PaymentCreateViewModel
{
    public int BookingId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public int Method { get; set; } = 2;

    public string? Note { get; set; }
}

public class BookingFilterViewModel
{
    public string? Keyword { get; set; }
    public int? Status { get; set; }
    public int? PaymentStatus { get; set; }
    public int? TourId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PromotionFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int DiscountType { get; set; } = 1;

    [Range(0.01, double.MaxValue)]
    public decimal DiscountValue { get; set; }

    public decimal? MaxDiscount { get; set; }
    public decimal MinOrderValue { get; set; }

    [Required, DataType(DataType.DateTime)]
    public DateTime StartAt { get; set; } = DateTime.UtcNow;

    [Required, DataType(DataType.DateTime)]
    public DateTime EndAt { get; set; } = DateTime.UtcNow.AddMonths(1);

    [Range(1, int.MaxValue)]
    public int UsageLimit { get; set; } = 100;

    public int Status { get; set; } = 1;
}

public class ReviewCreateViewModel
{
    public int BookingId { get; set; }

    [Range(1, 5, ErrorMessage = "Đánh giá từ 1-5 sao")]
    public int Rating { get; set; } = 5;

    [Required, StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}
