using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "ADMIN,STAFF")]
public class ReportController : Controller
{
    private readonly AppDbContext _db;
    private readonly IDashboardService _dashboardService;

    public ReportController(AppDbContext db, IDashboardService dashboardService)
    {
        _db = db;
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(ReportFilterViewModel filter)
    {
        var fromDate = filter.FromDate ?? DateTime.UtcNow.Date.AddMonths(-1);
        var toDate = filter.ToDate ?? DateTime.UtcNow.Date;
        var groupBy = filter.GroupBy ?? "day";

        var data = await _dashboardService.GetRevenueSeriesAsync(fromDate, toDate, groupBy);

        var revenueReport = data.Select(d => new RevenueReportItem
        {
            Date = d.Date,
            Label = groupBy == "month" ? d.Date.ToString("MM/yyyy") : d.Date.ToString("dd/MM"),
            BookingCount = d.BookingCount,
            Revenue = d.Amount
        }).ToList();

        var tourReport = await _db.Tours.Select(t => new TourReportItem
        {
            TourId = t.Id,
            TourCode = t.Code,
            TourName = t.Name,
            BookingCount = _db.Bookings.Count(b => b.Schedule!.TourId == t.Id && b.Status != (int)BookingStatus.CANCELLED),
            GuestCount = _db.Bookings.Where(b => b.Schedule!.TourId == t.Id && b.Status != (int)BookingStatus.CANCELLED).Sum(b => (int?)(b.Adults + b.Children)) ?? 0,
            Revenue = _db.Bookings.Where(b => b.Schedule!.TourId == t.Id && b.PaymentStatus == (int)PaymentStatus.PAID).Sum(b => (decimal?)b.TotalAmount) ?? 0,
            TotalSeats = _db.TourSchedules.Where(s => s.TourId == t.Id).Sum(s => (int?)s.MaxGuests) ?? 0,
            BookedSeats = _db.TourSchedules.Where(s => s.TourId == t.Id).Sum(s => (int?)s.BookedGuests) ?? 0
        }).OrderByDescending(x => x.Revenue).ToListAsync();

        ViewBag.RevenueReport = revenueReport;
        ViewBag.TourReport = tourReport;
        ViewBag.TotalRevenue = revenueReport.Sum(r => r.Revenue);
        ViewBag.TotalBookings = revenueReport.Sum(r => r.BookingCount);
        return View(filter);
    }

    public IActionResult ExportRevenue(ReportFilterViewModel filter)
    {
        // CSV xuất báo cáo doanh thu
        var fromDate = filter.FromDate ?? DateTime.UtcNow.Date.AddMonths(-1);
        var toDate = filter.ToDate ?? DateTime.UtcNow.Date;

        var data = _db.Bookings
            .Include(b => b.Schedule).ThenInclude(s => s!.Tour)
            .Where(b => b.PaymentStatus == (int)PaymentStatus.PAID && b.BookedAt >= fromDate && b.BookedAt <= toDate.AddDays(1))
            .OrderBy(b => b.BookedAt)
            .Select(b => new { b.BookingCode, b.BookedAt, TourName = b.Schedule!.Tour!.Name, b.TotalAmount, b.Status })
            .ToList();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Ma don,Ngay dat,Tour,So tien,Trang thai");
        foreach (var r in data)
        {
            sb.AppendLine($"{r.BookingCode},{r.BookedAt:yyyy-MM-dd HH:mm},\"{r.TourName}\",{r.TotalAmount},{r.Status}");
        }
        return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"revenue_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv");
    }
}