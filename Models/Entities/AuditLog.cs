using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    [Required, StringLength(100)]
    public string Action { get; set; } = string.Empty;
    [StringLength(100)]
    public string? EntityType { get; set; }
    [StringLength(100)]
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    [StringLength(50)]
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
}