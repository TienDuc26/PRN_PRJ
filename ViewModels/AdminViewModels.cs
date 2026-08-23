using System.ComponentModel.DataAnnotations;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.ViewModels;

public class GuideFormViewModel
{
    public int Id { get; set; }
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    [Phone]
    public string? Phone { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [StringLength(300)]
    public string? Address { get; set; }
    public int? ExperienceYears { get; set; }
    [StringLength(300)]
    public string? Languages { get; set; }
    public string? Bio { get; set; }
    public int Status { get; set; } = 1;
}

public class GuideAssignViewModel
{
    public int ScheduleId { get; set; }
    [Required] public int GuideId { get; set; }
    public string? Note { get; set; }
}

public class DashboardViewModel
{
    public int TotalTours { get; set; }
    public int ActiveTours { get; set; }
    public int TotalDestinations { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UpcomingSchedules { get; set; }
    public List<TopTourItem> TopTours { get; set; } = new();
    public List<PopularDestinationItem> PopularDestinations { get; set; } = new();
    public List<Booking> RecentBookings { get; set; } = new();
    public List<RevenuePoint> RevenueSeries { get; set; } = new();
    public Dictionary<string, int> BookingsByStatus { get; set; } = new();
}

public class TopTourItem
{
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
}

public class PopularDestinationItem
{
    public int DestinationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int TourCount { get; set; }
    public int BookingCount { get; set; }
}

public class ReportFilterViewModel
{
    public DateTime? FromDate { get; set; } = DateTime.UtcNow.Date.AddMonths(-1);
    public DateTime? ToDate { get; set; } = DateTime.UtcNow.Date;
    public string GroupBy { get; set; } = "day";
}

public class RevenueReportItem
{
    public DateTime Date { get; set; }
    public string Label { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
}

public class TourReportItem
{
    public int TourId { get; set; }
    public string TourCode { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public int GuestCount { get; set; }
    public decimal Revenue { get; set; }
    public int TotalSeats { get; set; }
    public int BookedSeats { get; set; }
    public double FillRate => TotalSeats == 0 ? 0 : Math.Round((double)BookedSeats / TotalSeats * 100, 2);
}