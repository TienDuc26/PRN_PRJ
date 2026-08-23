using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class BookingParticipant
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    [StringLength(30)]
    public string? IdentityNumber { get; set; }
    [StringLength(20)]
    public string? Phone { get; set; }
    [StringLength(100)]
    public string? Email { get; set; }
    public bool IsAdult { get; set; } = true;
    [StringLength(300)]
    public string? Note { get; set; }

    public Booking? Booking { get; set; }
}