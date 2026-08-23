using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

public class TourFilterViewModel
{
    public string? Keyword { get; set; }
    public int? DestinationId { get; set; }
    public int? TourType { get; set; }
    public int? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinDays { get; set; }
    public int? MaxDays { get; set; }
    public DateTime? StartDate { get; set; }
    public string SortBy { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 9;
}

public class TourFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int DestinationId { get; set; }

    public string? Description { get; set; }
    public string? IncludedServices { get; set; }
    public string? ExcludedServices { get; set; }
    public string? Policy { get; set; }
    public string? Thumbnail { get; set; }

    [Range(1, 60, ErrorMessage = "Số ngày từ 1-60")]
    public int DurationDays { get; set; }

    [Range(0, 59)]
    public int DurationNights { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
    public decimal BasePrice { get; set; }

    public int TourType { get; set; } = 1;
    public int Status { get; set; } = 1;
}

public class DestinationViewModel
{
    public int Id { get; set; }
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string Country { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int Status { get; set; } = 1;
}

public class ItineraryFormViewModel
{
    public int Id { get; set; }
    public int TourId { get; set; }
    [Range(1, 60)]
    public int DayNumber { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [StringLength(250)]
    public string? Location { get; set; }
    public string? TimeInfo { get; set; }
    [StringLength(100)]
    public string? Meals { get; set; }
    [StringLength(200)]
    public string? Hotel { get; set; }
    public string? Notes { get; set; }
    public string? Image { get; set; }
}

public class ScheduleFilterViewModel
{
    public int? TourId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ScheduleFormViewModel
{
    public int Id { get; set; }
    public int TourId { get; set; }
    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date.AddDays(7);

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddDays(10);

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan MeetingTime { get; set; } = new TimeSpan(8, 0, 0);

    [Required, StringLength(300)]
    public string MeetingPoint { get; set; } = string.Empty;

    [Range(1, 200)]
    public int MaxGuests { get; set; } = 20;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public int Status { get; set; } = 1;
}