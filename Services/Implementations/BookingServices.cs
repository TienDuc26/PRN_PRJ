using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Helpers;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    private readonly IPromotionService _promoService;
    private readonly INotificationService _notiService;
    private readonly IAuditLogService _auditLog;

    public BookingService(AppDbContext db, IPromotionService promoService, INotificationService notiService, IAuditLogService auditLog)
    {
        _db = db;
        _promoService = promoService;
        _notiService = notiService;
        _auditLog = auditLog;
    }

    public async Task<PagedResult<Booking>> GetUserBookingsAsync(int userId, int page, int pageSize, int? status)
    {
        var query = _db.Bookings.Include(b => b.Schedule).ThenInclude(s => s!.Tour).ThenInclude(t => t!.Destination)
            .Include(b => b.Payments).Include(b => b.Participants).Include(b => b.Review)
            .Where(b => b.UserId == userId);
        if (status.HasValue) query = query.Where(b => b.Status == status.Value);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.BookedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Booking> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<PagedResult<Booking>> GetAllBookingsAsync(BookingFilterViewModel f)
    {
        var query = _db.Bookings.Include(b => b.User).Include(b => b.Schedule).ThenInclude(s => s!.Tour).ThenInclude(t => t!.Destination)
            .Include(b => b.Payments).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Keyword))
            query = query.Where(b => b.BookingCode.Contains(f.Keyword) || (b.User!.FullName != null && b.User.FullName.Contains(f.Keyword)));
        if (f.Status.HasValue) query = query.Where(b => b.Status == f.Status.Value);
        if (f.PaymentStatus.HasValue) query = query.Where(b => b.PaymentStatus == f.PaymentStatus.Value);
        if (f.TourId.HasValue) query = query.Where(b => b.Schedule!.TourId == f.TourId.Value);
        if (f.FromDate.HasValue) query = query.Where(b => b.BookedAt >= f.FromDate.Value);
        if (f.ToDate.HasValue) query = query.Where(b => b.BookedAt <= f.ToDate.Value);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.BookedAt).Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToListAsync();
        return new PagedResult<Booking> { Items = items, Page = f.Page, PageSize = f.PageSize, TotalItems = total };
    }

    public Task<Booking?> GetByIdAsync(int id) =>
        _db.Bookings.Include(b => b.User).Include(b => b.Schedule).ThenInclude(s => s!.Tour).ThenInclude(t => t!.Destination)
            .Include(b => b.Participants).Include(b => b.Payments).Include(b => b.Promotion).Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Id == id);

    public Task<Booking?> GetByCodeAsync(string code) =>
        _db.Bookings.Include(b => b.Schedule).ThenInclude(s => s!.Tour)
            .Include(b => b.Payments).Include(b => b.Participants).FirstOrDefaultAsync(b => b.BookingCode == code);

    public async Task<Booking> CreateBookingAsync(int userId, BookingCreateViewModel m)
    {
        // BR-01, BR-02: validate schedule open, not past, has enough seats
        var schedule = await _db.TourSchedules.FirstOrDefaultAsync(s => s.Id == m.ScheduleId);
        if (schedule == null) throw new InvalidOperationException("Không tìm thấy lịch khởi hành");
        if (schedule.Status != (int)ScheduleStatus.OPEN) throw new InvalidOperationException("Lịch khởi hành không mở bán");
        if (schedule.StartDate.Date < DateTime.UtcNow.Date) throw new InvalidOperationException("Lịch đã khởi hành");
        var totalGuests = m.Adults + m.Children;
        if (totalGuests <= 0) throw new InvalidOperationException("Số khách phải > 0");
        if (schedule.AvailableSeats < totalGuests) throw new InvalidOperationException("Không đủ chỗ trống");

        decimal discount = 0;
        Promotion? promo = null;
        if (!string.IsNullOrWhiteSpace(m.PromotionCode))
        {
            var pvr = await _promoService.ValidateAsync(m.PromotionCode, schedule.Price * totalGuests);
            if (!pvr.Success) throw new InvalidOperationException(pvr.Message);
            promo = pvr.Promotion;
            discount = pvr.Discount;
        }

        // Sử dụng transaction với raw SQL update để chống overbooking (atomic)
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            // Atomic increment with availability check (concurrency safe)
            var affected = await _db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE TourSchedules SET BookedGuests = BookedGuests + {totalGuests} WHERE Id = {schedule.Id} AND (MaxGuests - BookedGuests) >= {totalGuests}");

            if (affected == 0)
                throw new InvalidOperationException("Lịch khởi hành vừa hết chỗ, vui lòng thử lại");

            var calc = PriceCalculator.Calculate(
                schedule.Price, m.Adults, m.Children,
                discountValue: (decimal?)promo?.DiscountValue,
                discountType: (int?)promo?.DiscountType,
                maxDiscount: promo?.MaxDiscount,
                minOrderValue: promo?.MinOrderValue,
                surcharge: 0);

            var booking = new Booking
            {
                UserId = userId,
                ScheduleId = schedule.Id,
                BookingCode = await GenerateUniqueBookingCodeAsync(),
                Adults = m.Adults,
                Children = m.Children,
                Subtotal = calc.subtotal,
                Discount = discount,
                Surcharge = calc.surcharge,
                TotalAmount = calc.total,
                PaidAmount = 0,
                Status = (int)BookingStatus.PENDING,
                PaymentStatus = (int)PaymentStatus.UNPAID,
                PromotionId = promo?.Id,
                Note = m.Note,
                BookedAt = DateTime.UtcNow
            };
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            foreach (var p in m.Participants)
            {
                var participant = new BookingParticipant
                {
                    BookingId = booking.Id,
                    FullName = p.FullName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    IdentityNumber = p.IdentityNumber,
                    Phone = p.Phone,
                    Email = p.Email,
                    Note = p.Note,
                    IsAdult = p.IsAdult
                };
                _db.BookingParticipants.Add(participant);
            }
            await _db.SaveChangesAsync();

            if (promo != null)
                await _promoService.IncrementUsageAsync(promo.Id);

            // Update schedule status if full
            var updated = await _db.TourSchedules.AsNoTracking().FirstAsync(s => s.Id == schedule.Id);
            if (updated.BookedGuests >= updated.MaxGuests)
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE TourSchedules SET Status = 2 WHERE Id = {schedule.Id}");
            }

            await tx.CommitAsync();

            await _notiService.CreateAsync(userId, "Đặt tour thành công",
                $"Đơn hàng {booking.BookingCode} đã được tạo. Vui lòng thanh toán để hoàn tất.", $"/Booking/Details/{booking.Id}");

            await _auditLog.LogAsync(userId, "CREATE_BOOKING", "Booking", booking.Id.ToString(), null, booking.BookingCode, null);
            return booking;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private async Task<string> GenerateUniqueBookingCodeAsync()
    {
        string code;
        bool exists;
        do
        {
            code = CodeGenerator.GenerateBookingCode();
            exists = await _db.Bookings.AnyAsync(b => b.BookingCode == code);
        } while (exists);
        return code;
    }

    public async Task<bool> CancelBookingAsync(int bookingId, int userId, bool isStaff)
    {
        var b = await _db.Bookings.Include(x => x.Schedule).Include(x => x.Payments).FirstOrDefaultAsync(x => x.Id == bookingId);
        if (b == null) return false;
        if (!isStaff && b.UserId != userId) return false;
        if (b.Status == (int)BookingStatus.CANCELLED || b.Status == (int)BookingStatus.COMPLETED) return false;

        // BR-01: không cho hủy nếu tour đã khởi hành
        if (b.Schedule == null) return false;
        if (b.Schedule.StartDate.Date < DateTime.UtcNow.Date) return false;

        // BR-03: cancellation policy - tính theo số ngày còn lại trước khởi hành
        var daysToStart = (b.Schedule.StartDate.Date - DateTime.UtcNow.Date).TotalDays;
        decimal refundRate = 0;
        if (daysToStart >= 15) refundRate = 1.0m;
        else if (daysToStart >= 7) refundRate = 0.7m;
        else refundRate = 0;

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var totalGuests = b.Adults + b.Children;
            // giải phóng ghế
            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE TourSchedules SET BookedGuests = BookedGuests - {totalGuests}, Status = CASE WHEN Status = 2 AND (MaxGuests - BookedGuests + {totalGuests}) > 0 THEN 1 ELSE Status END WHERE Id = {b.ScheduleId}");

            b.Status = (int)BookingStatus.CANCELLED;
            b.CancelledAt = DateTime.UtcNow;

            // Hoàn tiền theo tỷ lệ
            var refundAmount = b.PaidAmount * refundRate;
            if (refundAmount > 0)
            {
                var refund = new Payment
                {
                    BookingId = b.Id,
                    TransactionCode = CodeGenerator.GenerateTransactionCode(),
                    Amount = -refundAmount,
                    Method = b.Payments.FirstOrDefault()?.Method ?? (int)PaymentMethod.BANK_TRANSFER,
                    Status = (int)PaymentStatus.REFUNDED,
                    PaidAt = DateTime.UtcNow,
                    Note = $"Hoàn tiền theo chính sách ({refundRate * 100}%)"
                };
                _db.Payments.Add(refund);
                b.PaidAmount -= refundAmount;
                if (b.PaidAmount <= 0) b.PaymentStatus = (int)PaymentStatus.REFUNDED;
                else b.PaymentStatus = (int)PaymentStatus.PARTIAL_PAID;
            }
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            await _notiService.CreateAsync(b.UserId, "Đơn hàng đã bị hủy",
                $"Đơn {b.BookingCode} đã được hủy. Hoàn tiền: {refundAmount:N0} VNĐ", $"/Booking/Details/{b.Id}");
            await _auditLog.LogAsync(userId, "CANCEL_BOOKING", "Booking", b.Id.ToString(), null, $"Refund: {refundAmount}", null);
            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ConfirmBookingAsync(int bookingId)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
        if (b == null) return false;
        b.Status = (int)BookingStatus.CONFIRMED;
        b.ConfirmedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _notiService.CreateAsync(b.UserId, "Đơn hàng đã xác nhận", $"Đơn {b.BookingCode} đã được xác nhận.", $"/Booking/Details/{b.Id}");
        return true;
    }

    public async Task<bool> CompleteBookingAsync(int bookingId)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
        if (b == null) return false;
        b.Status = (int)BookingStatus.COMPLETED;
        b.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _notiService.CreateAsync(b.UserId, "Tour đã hoàn thành", $"Cảm ơn bạn đã tham gia tour {b.BookingCode}. Hãy đánh giá!", $"/Booking/Details/{b.Id}");
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int bookingId, int status)
    {
        var b = await _db.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
        if (b == null) return false;
        // Validate chuyển trạng thái hợp lệ
        // PENDING -> CONFIRMED|CANCELLED
        // CONFIRMED -> PAID|CANCELLED
        // PAID -> COMPLETED|CANCELLED
        // COMPLETED -> (terminal)
        // CANCELLED -> (terminal)
        var current = b.Status;
        var valid = (current, status) switch
        {
            ((int)BookingStatus.PENDING, (int)BookingStatus.CONFIRMED) => true,
            ((int)BookingStatus.PENDING, (int)BookingStatus.CANCELLED) => true,
            ((int)BookingStatus.PENDING, (int)BookingStatus.PAID) => true, // thanh toán full -> PAID
            ((int)BookingStatus.CONFIRMED, (int)BookingStatus.PAID) => true,
            ((int)BookingStatus.CONFIRMED, (int)BookingStatus.CANCELLED) => true,
            ((int)BookingStatus.PAID, (int)BookingStatus.COMPLETED) => true,
            ((int)BookingStatus.PAID, (int)BookingStatus.CANCELLED) => true,
            _ => false
        };
        if (!valid) return false;

        b.Status = status;
        b.UpdatedAt = DateTime.UtcNow;
        if (status == (int)BookingStatus.COMPLETED) b.CompletedAt = DateTime.UtcNow;
        if (status == (int)BookingStatus.CANCELLED) b.CancelledAt = DateTime.UtcNow;
        if (status == (int)BookingStatus.CONFIRMED) b.ConfirmedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> CountByStatusAsync(int status) => _db.Bookings.CountAsync(b => b.Status == status);
    public Task<int> CountAllAsync() => _db.Bookings.CountAsync();
    public async Task<decimal> GetTotalRevenueAsync()
    {
        var sum = await _db.Bookings.Where(b => b.PaymentStatus == (int)PaymentStatus.PAID).SumAsync(b => (decimal?)b.TotalAmount);
        return sum ?? 0m;
    }
}

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _db;
    private readonly INotificationService _notiService;
    public PaymentService(AppDbContext db, INotificationService notiService) { _db = db; _notiService = notiService; }

    public Task<List<Payment>> GetByBookingAsync(int bookingId) =>
        _db.Payments.Where(p => p.BookingId == bookingId).OrderByDescending(p => p.CreatedAt).ToListAsync();

    public Task<Payment?> GetByIdAsync(int id) => _db.Payments.Include(p => p.Booking).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Payment> CreatePaymentAsync(int bookingId, decimal amount, int method, string? note, string? processedBy)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null) throw new InvalidOperationException("Không tìm thấy booking");
        if (amount <= 0) throw new InvalidOperationException("Số tiền không hợp lệ");

        var pay = new Payment
        {
            BookingId = bookingId,
            TransactionCode = CodeGenerator.GenerateTransactionCode(),
            Amount = amount,
            Method = method,
            Status = (int)PaymentStatus.PAID,
            PaidAt = DateTime.UtcNow,
            Note = note,
            ProcessedBy = processedBy
        };
        _db.Payments.Add(pay);

        booking.PaidAmount += amount;
        if (booking.PaidAmount >= booking.TotalAmount)
        {
            booking.PaidAmount = booking.TotalAmount;
            booking.PaymentStatus = (int)PaymentStatus.PAID;
            if (booking.Status == (int)BookingStatus.PENDING)
            {
                booking.Status = (int)BookingStatus.PAID;
            }
        }
        else
        {
            booking.PaymentStatus = (int)PaymentStatus.PARTIAL_PAID;
            if (booking.Status == (int)BookingStatus.PENDING) booking.Status = (int)BookingStatus.CONFIRMED;
        }
        booking.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _notiService.CreateAsync(booking.UserId, "Thanh toán thành công",
            $"Đã nhận {amount:N0} VNĐ cho đơn {booking.BookingCode}.", $"/Booking/Details/{booking.Id}");
        return pay;
    }

    public async Task<bool> RefundAsync(int paymentId, decimal amount, string? note, string? processedBy)
    {
        var p = await _db.Payments.Include(x => x.Booking).FirstOrDefaultAsync(x => x.Id == paymentId);
        if (p == null) return false;
        if (p.Booking == null) return false;
        if (amount <= 0 || amount > p.Booking.PaidAmount) return false;

        var refund = new Payment
        {
            BookingId = p.BookingId,
            TransactionCode = CodeGenerator.GenerateTransactionCode(),
            Amount = -amount,
            Method = p.Method,
            Status = (int)PaymentStatus.REFUNDED,
            PaidAt = DateTime.UtcNow,
            Note = note ?? "Hoàn tiền",
            ProcessedBy = processedBy
        };
        _db.Payments.Add(refund);
        p.Booking.PaidAmount -= amount;
        if (p.Booking.PaidAmount <= 0)
        {
            p.Booking.PaidAmount = 0;
            p.Booking.PaymentStatus = (int)PaymentStatus.REFUNDED;
        }
        else p.Booking.PaymentStatus = (int)PaymentStatus.PARTIAL_PAID;
        await _db.SaveChangesAsync();
        await _notiService.CreateAsync(p.Booking.UserId, "Hoàn tiền",
            $"Đã hoàn {amount:N0} VNĐ cho đơn {p.Booking.BookingCode}.", $"/Booking/Details/{p.BookingId}");
        return true;
    }

    public async Task<decimal> GetPaidAmountAsync(int bookingId)
    {
        var amounts = await _db.Payments.Where(p => p.BookingId == bookingId && p.Amount > 0).Select(p => (decimal?)p.Amount).ToListAsync();
        return amounts.Sum() ?? 0m;
    }
}

public class PromotionService : IPromotionService
{
    private readonly AppDbContext _db;
    public PromotionService(AppDbContext db) => _db = db;

    public async Task<PagedResult<Promotion>> GetPagedAsync(string? keyword, int page, int pageSize)
    {
        var query = _db.Promotions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword)) query = query.Where(p => p.Code.Contains(keyword) || p.Name.Contains(keyword));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.StartAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Promotion> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public Task<Promotion?> GetByCodeAsync(string code) =>
        _db.Promotions.FirstOrDefaultAsync(p => p.Code.ToUpper() == code.ToUpper());
    public Task<Promotion?> GetByIdAsync(int id) => _db.Promotions.FirstOrDefaultAsync(p => p.Id == id);

    public async Task CreateAsync(Promotion p)
    {
        p.Code = p.Code.ToUpper();
        p.CreatedAt = DateTime.UtcNow;
        p.UpdatedAt = DateTime.UtcNow;
        _db.Promotions.Add(p);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Promotion p)
    {
        var existing = await _db.Promotions.FirstAsync(x => x.Id == p.Id);
        existing.Code = p.Code.ToUpper();
        existing.Name = p.Name;
        existing.Description = p.Description;
        existing.DiscountType = p.DiscountType;
        existing.DiscountValue = p.DiscountValue;
        existing.MaxDiscount = p.MaxDiscount;
        existing.MinOrderValue = p.MinOrderValue;
        existing.StartAt = p.StartAt;
        existing.EndAt = p.EndAt;
        existing.UsageLimit = p.UsageLimit;
        existing.Status = p.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _db.Promotions.Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return false;
        if (p.Bookings.Any()) return false;
        _db.Promotions.Remove(p);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var p = await _db.Promotions.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return false;
        p.Status = p.Status == 1 ? 2 : 1;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<PromotionValidationResult> ValidateAsync(string code, decimal orderAmount, DateTime? now = null)
    {
        var result = new PromotionValidationResult();
        var p = await GetByCodeAsync(code);
        if (p == null) { result.Message = "Mã khuyến mãi không tồn tại"; return result; }
        if (p.Status != 1) { result.Message = "Mã khuyến mãi đang tắt"; return result; }
        var n = now ?? DateTime.UtcNow;
        if (n < p.StartAt) { result.Message = "Mã chưa bắt đầu áp dụng"; return result; }
        if (n > p.EndAt) { result.Message = "Mã đã hết hạn"; return result; }
        if (p.UsageCount >= p.UsageLimit) { result.Message = "Mã đã hết lượt sử dụng"; return result; }
        if (orderAmount < p.MinOrderValue) { result.Message = $"Đơn tối thiểu {p.MinOrderValue:N0} VNĐ"; return result; }

        decimal discount = p.DiscountType == 1
            ? Math.Min(orderAmount * p.DiscountValue / 100m, p.MaxDiscount ?? decimal.MaxValue)
            : Math.Min(p.DiscountValue, p.MaxDiscount ?? decimal.MaxValue);

        result.Success = true;
        result.Message = "Áp dụng thành công";
        result.Promotion = p;
        result.Discount = discount;
        return result;
    }

    public async Task<bool> IncrementUsageAsync(int promotionId)
    {
        // BR-07: Atomic update để tránh race condition khi nhiều booking cùng áp mã
        var now = DateTime.UtcNow;
        var affected = await _db.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Promotions SET UsageCount = UsageCount + 1, UpdatedAt = {now} WHERE Id = {promotionId} AND UsageCount < UsageLimit");
        return affected > 0;
    }
}