using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    [Required, StringLength(40)]
    public string TransactionCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public int Status { get; set; } = 1;
    public DateTime? PaidAt { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }
    [StringLength(100)]
    public string? ProcessedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Booking? Booking { get; set; }
}