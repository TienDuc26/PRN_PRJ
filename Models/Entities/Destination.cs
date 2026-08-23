using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class Destination
{
    public int Id { get; set; }
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string Country { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}