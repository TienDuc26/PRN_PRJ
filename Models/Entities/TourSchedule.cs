using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class TourSchedule
{
    public int Id { get; set; }
    public int TourId { get; set; }
    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan MeetingTime { get; set; }
    [StringLength(300)]
    public string MeetingPoint { get; set; } = string.Empty;
    public int MaxGuests { get; set; }
    public int BookedGuests { get; set; }
    public decimal Price { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public byte[]? RowVersion { get; set; }

    public Tour? Tour { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<GuideAssignment> GuideAssignments { get; set; } = new List<GuideAssignment>();

    public int AvailableSeats => MaxGuests - BookedGuests;
}