using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;
    [StringLength(300)]
    public string? Link { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
}