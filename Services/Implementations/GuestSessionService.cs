using TourManagement.Web.Services.Interfaces;

namespace TourManagement.Web.Services.Implementations;

public class GuestSessionService : IGuestSessionService
{
    private const string SessionKey = "GuestBookingSelection";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GuestSessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SaveBookingSelection(int tourId, int scheduleId, int adults, int children, string? promoCode)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return;

        var selection = new GuestBookingSelection
        {
            TourId = tourId,
            ScheduleId = scheduleId,
            Adults = adults,
            Children = children,
            PromoCode = promoCode,
            SavedAt = DateTime.UtcNow
        };

        session.SetString(SessionKey, System.Text.Json.JsonSerializer.Serialize(selection));
    }

    public GuestBookingSelection? GetBookingSelection()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return null;

        var raw = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(raw)) return null;

        try
        {
            var sel = System.Text.Json.JsonSerializer.Deserialize<GuestBookingSelection>(raw);
            if (sel == null) return null;

            // Hết hạn sau 30 phút
            if ((DateTime.UtcNow - sel.SavedAt).TotalMinutes > 30)
            {
                ClearBookingSelection();
                return null;
            }

            return sel;
        }
        catch
        {
            return null;
        }
    }

    public void ClearBookingSelection()
    {
        _httpContextAccessor.HttpContext?.Session.Remove(SessionKey);
    }
}
