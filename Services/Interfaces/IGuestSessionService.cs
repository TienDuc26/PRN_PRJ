namespace TourManagement.Web.Services.Interfaces;

public interface IGuestSessionService
{
    void SaveBookingSelection(int tourId, int scheduleId, int adults, int children, string? promoCode);
    GuestBookingSelection? GetBookingSelection();
    void ClearBookingSelection();
}

public class GuestBookingSelection
{
    public int TourId { get; set; }
    public int ScheduleId { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public string? PromoCode { get; set; }
    public DateTime SavedAt { get; set; }
}
