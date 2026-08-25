using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public CustomerService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<PagedResult<ApplicationUser>> GetCustomersAsync(string? keyword, int? status, int page, int pageSize)
    {
        // lấy user thuộc role CUSTOMER
        var customerRoleId = await _db.Roles.Where(r => r.Name == "CUSTOMER").Select(r => r.Id).FirstOrDefaultAsync();
        var userRoleIds = await _db.UserRoles.Where(ur => ur.RoleId == customerRoleId).Select(ur => ur.UserId).ToListAsync();
        var query = _db.Users.Where(u => userRoleIds.Contains(u.Id));
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(u => u.FullName.Contains(keyword) || (u.Email != null && u.Email.Contains(keyword)) || (u.PhoneNumber != null && u.PhoneNumber.Contains(keyword)));
        if (status.HasValue) query = query.Where(u => u.Status == status.Value);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(u => u.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<ApplicationUser> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public Task<ApplicationUser?> GetByIdAsync(int id) => _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<ApplicationUser?> GetWithBookingsAsync(int id) =>
        _db.Users.Include(u => u.Bookings).ThenInclude(b => b.Schedule).ThenInclude(s => s!.Tour)
            .Include(u => u.Reviews).FirstOrDefaultAsync(u => u.Id == id);

    public async Task UpdateProfileAsync(int id, ProfileUpdateViewModel m)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return;
        u.FullName = m.FullName;
        u.PhoneNumber = m.Phone;
        u.Address = m.Address;
        u.DateOfBirth = m.DateOfBirth;
        u.Gender = m.Gender;
        if (!string.IsNullOrEmpty(m.AvatarPath)) u.Avatar = m.AvatarPath;
        u.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> LockAsync(int id)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return false;
        u.Status = 2;
        u.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        u.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        // Đổi SecurityStamp để các phiên đang đăng nhập bị vô hiệu ngay
        await _userManager.UpdateSecurityStampAsync(u);
        return true;
    }

    public async Task<bool> UnlockAsync(int id)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return false;
        u.Status = 1;
        u.LockoutEnd = null;
        u.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _userManager.UpdateSecurityStampAsync(u);
        return true;
    }

    public Task<int> CountAsync() => _db.Users.CountAsync();
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var model = new DashboardViewModel();
        model.TotalTours = await _db.Tours.CountAsync();
        model.ActiveTours = await _db.Tours.CountAsync(t => t.Status == 1);
        model.TotalDestinations = await _db.Destinations.CountAsync();
        model.TotalCustomers = await _db.Users.CountAsync();
        model.TotalBookings = await _db.Bookings.CountAsync();
        model.PendingBookings = await _db.Bookings.CountAsync(b => b.Status == (int)BookingStatus.PENDING);
        model.ConfirmedBookings = await _db.Bookings.CountAsync(b => b.Status == (int)BookingStatus.CONFIRMED);
        model.CompletedBookings = await _db.Bookings.CountAsync(b => b.Status == (int)BookingStatus.COMPLETED);
        model.CancelledBookings = await _db.Bookings.CountAsync(b => b.Status == (int)BookingStatus.CANCELLED);
        model.TotalRevenue = await _db.Bookings.Where(b => b.PaymentStatus == (int)PaymentStatus.PAID).SumAsync(b => (decimal?)b.TotalAmount) ?? 0;
        model.UpcomingSchedules = await _db.TourSchedules.CountAsync(s => s.StartDate >= DateTime.UtcNow.Date && s.Status != 4);

        // Tour bán chạy
        model.TopTours = await _db.Tours
            .Select(t => new TopTourItem
            {
                TourId = t.Id,
                TourName = t.Name,
                Thumbnail = t.Thumbnail,
                BookingCount = _db.Bookings.Count(b => b.Schedule!.TourId == t.Id && b.Status != (int)BookingStatus.CANCELLED),
                Revenue = _db.Bookings.Where(b => b.Schedule!.TourId == t.Id && b.PaymentStatus == (int)PaymentStatus.PAID).Sum(b => (decimal?)b.TotalAmount) ?? 0
            })
            .OrderByDescending(x => x.BookingCount).Take(5).ToListAsync();

        // Điểm đến phổ biến
        model.PopularDestinations = await _db.Destinations
            .Select(d => new PopularDestinationItem
            {
                DestinationId = d.Id,
                Name = d.Name,
                Image = d.Image,
                TourCount = _db.Tours.Count(t => t.DestinationId == d.Id),
                BookingCount = _db.Bookings.Count(b => b.Schedule!.Tour!.DestinationId == d.Id)
            })
            .OrderByDescending(x => x.BookingCount).Take(5).ToListAsync();

        // Booking gần đây
        model.RecentBookings = await _db.Bookings
            .Include(b => b.User).Include(b => b.Schedule).ThenInclude(s => s!.Tour)
            .OrderByDescending(b => b.BookedAt).Take(10).ToListAsync();

        // Doanh thu 7 ngày
        var fromDate = DateTime.UtcNow.Date.AddDays(-6);
        model.RevenueSeries = await GetRevenueSeriesAsync(fromDate, DateTime.UtcNow.Date, "day");

        // Phân bố trạng thái booking
        model.BookingsByStatus = new()
        {
            { "PENDING", model.PendingBookings },
            { "CONFIRMED", model.ConfirmedBookings },
            { "PAID", await _db.Bookings.CountAsync(b => b.Status == (int)BookingStatus.PAID) },
            { "COMPLETED", model.CompletedBookings },
            { "CANCELLED", model.CancelledBookings }
        };

        return model;
    }

    public async Task<List<RevenuePoint>> GetRevenueSeriesAsync(DateTime from, DateTime to, string groupBy)
    {
        var bookings = await _db.Bookings
            .Where(b => b.PaymentStatus == (int)PaymentStatus.PAID && b.BookedAt >= from && b.BookedAt <= to.AddDays(1))
            .ToListAsync();

        IEnumerable<IGrouping<DateTime, Booking>> grouped = groupBy switch
        {
            "month" => bookings.GroupBy(b => new DateTime(b.BookedAt.Year, b.BookedAt.Month, 1)),
            "year" => bookings.GroupBy(b => new DateTime(b.BookedAt.Year, 1, 1)),
            _ => bookings.GroupBy(b => b.BookedAt.Date)
        };

        var result = grouped.Select(g => new RevenuePoint
        {
            Date = g.Key,
            Amount = g.Sum(b => b.TotalAmount),
            BookingCount = g.Count()
        }).OrderBy(x => x.Date).ToList();

        // Fill missing dates
        var all = new List<RevenuePoint>();
        var cursor = from.Date;
        while (cursor <= to)
        {
            var item = result.FirstOrDefault(r => r.Date == cursor);
            all.Add(item ?? new RevenuePoint { Date = cursor, Amount = 0, BookingCount = 0 });
            cursor = cursor.AddDays(1);
        }
        return all;
    }
}

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _db;
    public AuditLogService(AppDbContext db) => _db = db;

    public async Task LogAsync(int? userId, string action, string? entityType, string? entityId, string? oldVal, string? newVal, string? ip, string? role = null)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldVal,
            NewValue = newVal,
            IpAddress = ip,
            Role = role,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResult<AuditLog>> GetPagedAsync(int page, int pageSize, string? action, string? role, string? userKeyword, DateTime? fromDate, DateTime? toDate)
    {
        var query = _db.AuditLogs.Include(a => a.User).AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action.Contains(action));
        
        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(a => a.Role == role);
        
        if (!string.IsNullOrWhiteSpace(userKeyword))
            query = query.Where(a => a.User != null && (a.User.FullName != null && a.User.FullName.Contains(userKeyword) || (a.User.Email != null && a.User.Email.Contains(userKeyword))));
        
        if (fromDate.HasValue)
            query = query.Where(a => a.CreatedAt >= fromDate.Value);
        
        if (toDate.HasValue)
            query = query.Where(a => a.CreatedAt <= toDate.Value.AddDays(1));
        
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<AuditLog> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }
}