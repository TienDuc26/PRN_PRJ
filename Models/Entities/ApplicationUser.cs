using Microsoft.AspNetCore.Identity;

namespace TourManagement.Web.Models.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public string? Address { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}