using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Promotion
{
    public int Id { get; set; }
    [Required, StringLength(30)]
    public string Code { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DiscountType { get; set; } = 1;
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscount { get; set; }
    public decimal MinOrderValue { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}