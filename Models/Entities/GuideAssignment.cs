using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models.Entities;

public class GuideAssignment
{
    public int Id { get; set; }
    public int GuideId { get; set; }
    public int ScheduleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    [StringLength(300)]
    public string? Note { get; set; }

    public Guide? Guide { get; set; }
    public TourSchedule? Schedule { get; set; }
}