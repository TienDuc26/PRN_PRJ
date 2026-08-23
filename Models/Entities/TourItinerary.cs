using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class TourItinerary
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int DayNumber { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [StringLength(250)]
    public string? Location { get; set; }
    public string? TimeInfo { get; set; }
    [StringLength(100)]
    public string? Meals { get; set; }
    [StringLength(200)]
    public string? Hotel { get; set; }
    public string? Notes { get; set; }
    public string? Image { get; set; }

    public Tour? Tour { get; set; }
}