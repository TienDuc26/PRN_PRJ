using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Implementations;

public class GuideService : IGuideService
{
    private readonly AppDbContext _db;
    public GuideService(AppDbContext db) => _db = db;

    public async Task<PagedResult<Guide>> GetPagedAsync(string? keyword, int? status, int page, int pageSize)
    {
        var query = _db.Guides.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(g => g.FullName.Contains(keyword) || (g.Phone != null && g.Phone.Contains(keyword)) || (g.Email != null && g.Email.Contains(keyword)));
        if (status.HasValue) query = query.Where(g => g.Status == status.Value);
        var total = await query.CountAsync();
        var items = await query.OrderBy(g => g.FullName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Guide> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public Task<Guide?> GetByIdAsync(int id) => _db.Guides.FirstOrDefaultAsync(g => g.Id == id);
    public Task<List<Guide>> GetActiveGuidesAsync() => _db.Guides.Where(g => g.Status == 1).OrderBy(g => g.FullName).ToListAsync();

    public async Task<List<TourSchedule>> GetGuideScheduleAsync(int guideId)
    {
        var ids = await _db.GuideAssignments.Where(a => a.GuideId == guideId).Select(a => a.ScheduleId).ToListAsync();
        return await _db.TourSchedules.Include(s => s.Tour).ThenInclude(t => t!.Destination)
            .Where(s => ids.Contains(s.Id)).OrderByDescending(s => s.StartDate).ToListAsync();
    }

    public async Task CreateAsync(Guide guide)
    {
        guide.CreatedAt = DateTime.UtcNow;
        guide.UpdatedAt = DateTime.UtcNow;
        _db.Guides.Add(guide);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guide guide)
    {
        var g = await _db.Guides.FirstAsync(x => x.Id == guide.Id);
        g.FullName = guide.FullName;
        g.DateOfBirth = guide.DateOfBirth;
        g.Phone = guide.Phone;
        g.Email = guide.Email;
        g.Address = guide.Address;
        g.ExperienceYears = guide.ExperienceYears;
        g.Languages = guide.Languages;
        g.Bio = guide.Bio;
        g.Status = guide.Status;
        g.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var g = await _db.Guides.Include(x => x.Assignments).FirstOrDefaultAsync(x => x.Id == id);
        if (g == null) return false;
        if (g.Assignments.Any()) return false;
        _db.Guides.Remove(g);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var g = await _db.Guides.FirstOrDefaultAsync(x => x.Id == id);
        if (g == null) return false;
        g.Status = g.Status == 1 ? 2 : 1;
        g.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignGuideAsync(int guideId, int scheduleId, string? note)
    {
        if (await HasScheduleConflictAsync(guideId, scheduleId))
            throw new InvalidOperationException("Hướng dẫn viên đã có lịch trùng thời gian");
        var exists = await _db.GuideAssignments.AnyAsync(x => x.GuideId == guideId && x.ScheduleId == scheduleId);
        if (exists) return false;
        _db.GuideAssignments.Add(new GuideAssignment
        {
            GuideId = guideId,
            ScheduleId = scheduleId,
            AssignedAt = DateTime.UtcNow,
            Note = note
        });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnassignAsync(int assignmentId)
    {
        var a = await _db.GuideAssignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
        if (a == null) return false;
        _db.GuideAssignments.Remove(a);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasScheduleConflictAsync(int guideId, int scheduleId)
    {
        var target = await _db.TourSchedules.FirstOrDefaultAsync(s => s.Id == scheduleId);
        if (target == null) return false;
        var scheduleIds = await _db.GuideAssignments.Where(a => a.GuideId == guideId).Select(a => a.ScheduleId).ToListAsync();
        var conflictSchedules = await _db.TourSchedules
            .Where(s => scheduleIds.Contains(s.Id))
            .Where(s => s.StartDate <= target.EndDate && s.EndDate >= target.StartDate)
            .AnyAsync();
        return conflictSchedules;
    }
}

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;
    public ReviewService(AppDbContext db) => _db = db;

    public async Task<PagedResult<Review>> GetByTourAsync(int tourId, int page, int pageSize, bool visibleOnly)
    {
        var query = _db.Reviews.Include(r => r.User).Where(r => r.TourId == tourId);
        if (visibleOnly) query = query.Where(r => r.Status == 1);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Review> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<Review>> GetPagedAdminAsync(int page, int pageSize, int? status)
    {
        var query = _db.Reviews.Include(r => r.User).Include(r => r.Tour).Include(r => r.Booking).AsQueryable();
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Review> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public Task<Review?> GetByIdAsync(int id) => _db.Reviews.Include(r => r.User).Include(r => r.Tour).FirstOrDefaultAsync(r => r.Id == id);
    public Task<Review?> GetByBookingAsync(int bookingId) => _db.Reviews.FirstOrDefaultAsync(r => r.BookingId == bookingId);

    public async Task<Review> CreateAsync(int userId, int bookingId, int rating, string content, IFormFile? image)
    {
        var booking = await _db.Bookings.Include(b => b.Schedule).FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null) throw new InvalidOperationException("Không tìm thấy booking");
        if (booking.UserId != userId) throw new UnauthorizedAccessException();
        if (booking.Status != (int)BookingStatus.COMPLETED) throw new InvalidOperationException("Chỉ đánh giá sau khi hoàn thành tour");
        if (await _db.Reviews.AnyAsync(r => r.BookingId == bookingId))
            throw new InvalidOperationException("Bạn đã đánh giá đơn này");

        var imgPath = await FileUploadHelper.SaveImageAsync(image, "reviews");
        var review = new Review
        {
            BookingId = bookingId,
            UserId = userId,
            TourId = booking.Schedule!.TourId,
            Rating = rating,
            Content = content,
            Image = imgPath,
            Status = 1,
            CreatedAt = DateTime.UtcNow
        };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return review;
    }

    public async Task<bool> ToggleVisibilityAsync(int id)
    {
        var r = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return false;
        r.Status = r.Status == 1 ? 2 : 1;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var r = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return false;
        if (!string.IsNullOrEmpty(r.Image)) FileUploadHelper.DeleteImage(r.Image);
        _db.Reviews.Remove(r);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<double> GetAverageRatingAsync(int tourId)
    {
        var ratings = await _db.Reviews.Where(r => r.TourId == tourId && r.Status == 1).Select(r => (double?)r.Rating).ToListAsync();
        return ratings.Any() ? ratings.Average() ?? 0 : 0;
    }

    public Task<int> GetReviewCountAsync(int tourId) => _db.Reviews.CountAsync(r => r.TourId == tourId && r.Status == 1);

    public async Task<bool> CanReviewAsync(int userId, int bookingId)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
        if (b == null) return false;
        if (b.UserId != userId) return false;
        if (b.Status != (int)BookingStatus.COMPLETED) return false;
        return !await _db.Reviews.AnyAsync(r => r.BookingId == bookingId);
    }
}

public class NotificationService : INotificationService
{
    private readonly AppDbContext _db;
    public NotificationService(AppDbContext db) => _db = db;

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId, int page, int pageSize)
    {
        return await _db.Notifications.Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public Task<int> CountUnreadAsync(int userId) =>
        _db.Notifications.CountAsync(n => n.UserId == userId && n.Status == 1);

    public async Task MarkReadAsync(int id, int userId)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (n != null) { n.Status = 2; await _db.SaveChangesAsync(); }
    }

    public async Task MarkAllReadAsync(int userId)
    {
        await _db.Notifications.Where(n => n.UserId == userId && n.Status == 1)
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.Status, 2));
    }

    public async Task CreateAsync(int userId, string title, string content, string? link)
    {
        _db.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            Link = link,
            Status = 1,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }
}