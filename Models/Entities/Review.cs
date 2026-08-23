using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int TourId { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    [Required, StringLength(1000)]
    public string Content { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Booking? Booking { get; set; }
    public ApplicationUser? User { get; set; }
    public Tour? Tour { get; set; }
}