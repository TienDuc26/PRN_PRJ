using TourManagement.Web.Models.Entities;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Interfaces;

public interface IGuideService
{
    Task<PagedResult<Guide>> GetPagedAsync(string? keyword, int? status, int page, int pageSize);
    Task<Guide?> GetByIdAsync(int id);
    Task CreateAsync(Guide guide);
    Task UpdateAsync(Guide guide);
    Task<bool> DeleteAsync(int id);
    Task<bool> ToggleStatusAsync(int id);
    Task<List<Guide>> GetActiveGuidesAsync();
    Task<List<TourSchedule>> GetGuideScheduleAsync(int guideId);
    Task<bool> AssignGuideAsync(int guideId, int scheduleId, string? note);
    Task<bool> UnassignAsync(int assignmentId);
    Task<bool> HasScheduleConflictAsync(int guideId, int scheduleId);
}

public interface IReviewService
{
    Task<PagedResult<Review>> GetByTourAsync(int tourId, int page, int pageSize, bool visibleOnly);
    Task<PagedResult<Review>> GetPagedAdminAsync(int page, int pageSize, int? status);
    Task<Review?> GetByIdAsync(int id);
    Task<Review?> GetByBookingAsync(int bookingId);
    Task<Review> CreateAsync(int userId, int bookingId, int rating, string content, IFormFile? image);
    Task<bool> ToggleVisibilityAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<double> GetAverageRatingAsync(int tourId);
    Task<int> GetReviewCountAsync(int tourId);
    Task<bool> CanReviewAsync(int userId, int bookingId);
}

public interface INotificationService
{
    Task<List<Notification>> GetUserNotificationsAsync(int userId, int page, int pageSize);
    Task<int> CountUnreadAsync(int userId);
    Task MarkReadAsync(int id, int userId);
    Task MarkAllReadAsync(int userId);
    Task CreateAsync(int userId, string title, string content, string? link);
}