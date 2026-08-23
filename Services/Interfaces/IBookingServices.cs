using TourManagement.Web.Models.Entities;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Interfaces;

public interface IBookingService
{
    Task<PagedResult<Booking>> GetUserBookingsAsync(int userId, int page, int pageSize, int? status);
    Task<PagedResult<Booking>> GetAllBookingsAsync(BookingFilterViewModel filter);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByCodeAsync(string code);
    Task<Booking> CreateBookingAsync(int userId, BookingCreateViewModel model);
    Task<bool> CancelBookingAsync(int bookingId, int userId, bool isStaff);
    Task<bool> ConfirmBookingAsync(int bookingId);
    Task<bool> CompleteBookingAsync(int bookingId);
    Task<bool> UpdateStatusAsync(int bookingId, int status);
    Task<int> CountByStatusAsync(int status);
    Task<int> CountAllAsync();
    Task<decimal> GetTotalRevenueAsync();
}

public interface IPaymentService
{
    Task<List<Payment>> GetByBookingAsync(int bookingId);
    Task<Payment?> GetByIdAsync(int id);
    Task<Payment> CreatePaymentAsync(int bookingId, decimal amount, int method, string? note, string? processedBy);
    Task<bool> RefundAsync(int paymentId, decimal amount, string? note, string? processedBy);
    Task<decimal> GetPaidAmountAsync(int bookingId);
}

public interface IPromotionService
{
    Task<PagedResult<Promotion>> GetPagedAsync(string? keyword, int page, int pageSize);
    Task<Promotion?> GetByCodeAsync(string code);
    Task<Promotion?> GetByIdAsync(int id);
    Task CreateAsync(Promotion p);
    Task UpdateAsync(Promotion p);
    Task<bool> DeleteAsync(int id);
    Task<bool> ToggleStatusAsync(int id);
    Task<PromotionValidationResult> ValidateAsync(string code, decimal orderAmount, DateTime? now = null);
    Task<bool> IncrementUsageAsync(int promotionId);
}

public class PromotionValidationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Promotion? Promotion { get; set; }
    public decimal Discount { get; set; }
}