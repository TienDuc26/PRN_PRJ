using TourManagement.Web.Models.Entities;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Interfaces;

public interface ICustomerService
{
    Task<PagedResult<ApplicationUser>> GetCustomersAsync(string? keyword, int? status, int page, int pageSize);
    Task<ApplicationUser?> GetByIdAsync(int id);
    Task<ApplicationUser?> GetWithBookingsAsync(int id);
    Task UpdateProfileAsync(int id, ProfileUpdateViewModel model);
    Task<bool> LockAsync(int id);
    Task<bool> UnlockAsync(int id);
    Task<int> CountAsync();
}

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
    Task<List<RevenuePoint>> GetRevenueSeriesAsync(DateTime from, DateTime to, string groupBy);
}

public class RevenuePoint
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int BookingCount { get; set; }
}

public interface IAuditLogService
{
    Task LogAsync(int? userId, string action, string? entityType, string? entityId, string? oldVal, string? newVal, string? ip);
    Task<PagedResult<AuditLog>> GetPagedAsync(int page, int pageSize, string? action);
}