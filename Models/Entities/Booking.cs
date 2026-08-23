using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ScheduleId { get; set; }
    [Required, StringLength(30)]
    public string BookingCode { get; set; } = string.Empty;
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Surcharge { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public int Status { get; set; } = 1;
    public int PaymentStatus { get; set; } = 1;
    public int? PromotionId { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
    public TourSchedule? Schedule { get; set; }
    public Promotion? Promotion { get; set; }
    public ICollection<BookingParticipant> Participants { get; set; } = new List<BookingParticipant>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Review? Review { get; set; }

    public int TotalGuests => Adults + Children;
}