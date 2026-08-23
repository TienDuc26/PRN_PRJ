using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Guide
{
    public int Id { get; set; }
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    [StringLength(20)]
    public string? Phone { get; set; }
    [StringLength(100)]
    public string? Email { get; set; }
    [StringLength(300)]
    public string? Address { get; set; }
    public int? ExperienceYears { get; set; }
    [StringLength(300)]
    public string? Languages { get; set; }
    public string? Bio { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<GuideAssignment> Assignments { get; set; } = new List<GuideAssignment>();
}