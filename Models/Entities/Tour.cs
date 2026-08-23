using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Tour
{
    public int Id { get; set; }
    public int DestinationId { get; set; }
    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;
    [Required, StringLength(250)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public decimal BasePrice { get; set; }
    public string? Thumbnail { get; set; }
    public string? IncludedServices { get; set; }
    public string? ExcludedServices { get; set; }
    public string? Policy { get; set; }
    public int TourType { get; set; } = 1;
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public byte[]? RowVersion { get; set; }

    public Destination? Destination { get; set; }
    public ICollection<TourItinerary> Itineraries { get; set; } = new List<TourItinerary>();
    public ICollection<TourSchedule> Schedules { get; set; } = new List<TourSchedule>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}