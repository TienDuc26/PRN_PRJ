using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

public class BookingCreateViewModel
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
}

public class ParticipantInputModel
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }
    public bool IsAdult { get; set; } = true;

    [StringLength(30)]
    public string? IdentityNumber { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(300)]
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

public class PaymentCreateViewModel
{
    public int BookingId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public int Method { get; set; } = 2;

    public string? Note { get; set; }
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