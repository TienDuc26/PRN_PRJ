using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Services.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Implementations;

public class TourService : ITourService
{
    private readonly AppDbContext _db;
    public TourService(AppDbContext db) => _db = db;

    public async Task<PagedResult<Tour>> GetToursAsync(TourFilterViewModel f)
    {
        var query = _db.Tours.Include(t => t.Destination).AsQueryable();
        if (!string.IsNullOrWhiteSpace(f.Keyword))
            query = query.Where(t => t.Name.Contains(f.Keyword) || t.Code.Contains(f.Keyword));
        if (f.DestinationId.HasValue) query = query.Where(t => t.DestinationId == f.DestinationId.Value);
        if (f.TourType.HasValue) query = query.Where(t => t.TourType == f.TourType.Value);
        if (f.Status.HasValue) query = query.Where(t => t.Status == f.Status.Value);
        if (f.MinPrice.HasValue) query = query.Where(t => t.BasePrice >= f.MinPrice.Value);
        if (f.MaxPrice.HasValue) query = query.Where(t => t.BasePrice <= f.MaxPrice.Value);
        if (f.MinDays.HasValue) query = query.Where(t => t.DurationDays >= f.MinDays.Value);
        if (f.MaxDays.HasValue) query = query.Where(t => t.DurationDays <= f.MaxDays.Value);

        query = f.SortBy switch
        {
            "price_asc" => query.OrderBy(t => t.BasePrice),
            "price_desc" => query.OrderByDescending(t => t.BasePrice),
            "newest" => query.OrderByDescending(t => t.CreatedAt),
            "rating" => query.OrderByDescending(t => t.Reviews.Any() ? t.Reviews.Average(r => (double)r.Rating) : 0),
            "popular" => query.OrderByDescending(t => t.Reviews.Count),
            _ => query.OrderBy(t => t.Name)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToListAsync();
        return new PagedResult<Tour> { Items = items, Page = f.Page, PageSize = f.PageSize, TotalItems = total };
    }

    public Task<Tour?> GetTourByIdAsync(int id) => _db.Tours.Include(t => t.Destination).FirstOrDefaultAsync(t => t.Id == id);
    public Task<Tour?> GetTourWithDetailsAsync(int id) =>
        _db.Tours.Include(t => t.Destination).Include(t => t.Itineraries.OrderBy(i => i.DayNumber))
            .Include(t => t.Schedules.Where(s => s.StartDate >= DateTime.UtcNow.Date))
            .Include(t => t.Reviews.Where(r => r.Status == 1))
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Tour> CreateTourAsync(Tour tour)
    {
        tour.CreatedAt = DateTime.UtcNow;
        tour.UpdatedAt = DateTime.UtcNow;
        _db.Tours.Add(tour);
        await _db.SaveChangesAsync();
        return tour;
    }

    public async Task UpdateTourAsync(Tour tour)
    {
        var existing = await _db.Tours.FirstAsync(t => t.Id == tour.Id);
        existing.Name = tour.Name;
        existing.Code = tour.Code;
        existing.Description = tour.Description;
        existing.DestinationId = tour.DestinationId;
        existing.DurationDays = tour.DurationDays;
        existing.DurationNights = tour.DurationNights;
        existing.BasePrice = tour.BasePrice;
        existing.Thumbnail = tour.Thumbnail;
        existing.IncludedServices = tour.IncludedServices;
        existing.ExcludedServices = tour.ExcludedServices;
        existing.Policy = tour.Policy;
        existing.TourType = tour.TourType;
        existing.Status = tour.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteTourAsync(int id)
    {
        var t = await _db.Tours.FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return false;
        var hasBookings = await _db.Bookings.AnyAsync(b => b.Schedule!.TourId == t.Id);
        if (hasBookings) return false; // cannot hard delete
        _db.Tours.Remove(t);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteTourAsync(int id)
    {
        var t = await _db.Tours.FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return false;
        t.Status = 2;
        t.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var t = await _db.Tours.FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return false;
        t.Status = t.Status == 1 ? 2 : 1;
        t.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> CountActiveToursAsync() => _db.Tours.CountAsync(t => t.Status == 1);
}

public class DestinationService : IDestinationService
{
    private readonly AppDbContext _db;
    public DestinationService(AppDbContext db) => _db = db;

    public Task<List<Destination>> GetAllAsync() => _db.Destinations.OrderBy(d => d.Name).ToListAsync();

    public async Task<PagedResult<Destination>> GetPagedAsync(string? keyword, int page, int pageSize)
    {
        var query = _db.Destinations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(d => d.Name.Contains(keyword) || d.City.Contains(keyword) || d.Country.Contains(keyword));
        var total = await query.CountAsync();
        var items = await query.OrderBy(d => d.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Destination> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public Task<Destination?> GetByIdAsync(int id) => _db.Destinations.FirstOrDefaultAsync(d => d.Id == id);

    public async Task CreateAsync(Destination dest)
    {
        dest.CreatedAt = DateTime.UtcNow;
        dest.UpdatedAt = DateTime.UtcNow;
        _db.Destinations.Add(dest);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Destination dest)
    {
        var d = await _db.Destinations.FirstAsync(x => x.Id == dest.Id);
        d.Name = dest.Name;
        d.City = dest.City;
        d.Country = dest.Country;
        d.Description = dest.Description;
        d.Image = dest.Image;
        d.Status = dest.Status;
        d.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var d = await _db.Destinations.Include(x => x.Tours).FirstOrDefaultAsync(x => x.Id == id);
        if (d == null) return false;
        if (d.Tours.Any()) return false;
        _db.Destinations.Remove(d);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> CountActiveAsync() => _db.Destinations.CountAsync(d => d.Status == 1);
}

public class ItineraryService : IItineraryService
{
    private readonly AppDbContext _db;
    public ItineraryService(AppDbContext db) => _db = db;

    public Task<List<TourItinerary>> GetByTourAsync(int tourId) =>
        _db.TourItineraries.Where(i => i.TourId == tourId).OrderBy(i => i.DayNumber).ToListAsync();

    public Task<TourItinerary?> GetByIdAsync(int id) => _db.TourItineraries.FirstOrDefaultAsync(i => i.Id == id);

    public async Task CreateAsync(TourItinerary item)
    {
        _db.TourItineraries.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(TourItinerary item)
    {
        var existing = await _db.TourItineraries.FirstAsync(x => x.Id == item.Id);
        existing.DayNumber = item.DayNumber;
        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.Location = item.Location;
        existing.TimeInfo = item.TimeInfo;
        existing.Meals = item.Meals;
        existing.Hotel = item.Hotel;
        existing.Notes = item.Notes;
        existing.Image = item.Image;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var x = await _db.TourItineraries.FirstOrDefaultAsync(i => i.Id == id);
        if (x != null) { _db.TourItineraries.Remove(x); await _db.SaveChangesAsync(); }
    }

    public async Task ReorderAsync(int tourId, List<int> orderedIds)
    {
        var items = await _db.TourItineraries.Where(i => i.TourId == tourId).ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var item = items.FirstOrDefault(x => x.Id == orderedIds[i]);
            if (item != null) item.DayNumber = i + 1;
        }
        await _db.SaveChangesAsync();
    }
}

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _db;
    public ScheduleService(AppDbContext db) => _db = db;

    public async Task<PagedResult<TourSchedule>> GetPagedAsync(ScheduleFilterViewModel f)
    {
        var query = _db.TourSchedules.Include(s => s.Tour).ThenInclude(t => t!.Destination).AsQueryable();
        if (f.TourId.HasValue) query = query.Where(s => s.TourId == f.TourId.Value);
        if (f.Status.HasValue) query = query.Where(s => s.Status == f.Status.Value);
        if (f.FromDate.HasValue) query = query.Where(s => s.StartDate >= f.FromDate.Value);
        if (f.ToDate.HasValue) query = query.Where(s => s.StartDate <= f.ToDate.Value);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(s => s.StartDate).Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToListAsync();
        return new PagedResult<TourSchedule> { Items = items, Page = f.Page, PageSize = f.PageSize, TotalItems = total };
    }

    public Task<List<TourSchedule>> GetByTourAsync(int tourId) =>
        _db.TourSchedules.Where(s => s.TourId == tourId).OrderBy(s => s.StartDate).ToListAsync();

    public Task<TourSchedule?> GetByIdAsync(int id) =>
        _db.TourSchedules.Include(s => s.Tour).Include(s => s.Bookings).Include(s => s.GuideAssignments).ThenInclude(g => g.Guide)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<TourSchedule> CreateAsync(TourSchedule schedule)
    {
        schedule.CreatedAt = DateTime.UtcNow;
        schedule.UpdatedAt = DateTime.UtcNow;
        schedule.BookedGuests = 0;
        _db.TourSchedules.Add(schedule);
        await _db.SaveChangesAsync();
        return schedule;
    }

    public async Task UpdateAsync(TourSchedule schedule)
    {
        var existing = await _db.TourSchedules.FirstAsync(s => s.Id == schedule.Id);
        existing.StartDate = schedule.StartDate;
        existing.EndDate = schedule.EndDate;
        existing.MeetingTime = schedule.MeetingTime;
        existing.MeetingPoint = schedule.MeetingPoint;
        existing.MaxGuests = schedule.MaxGuests;
        existing.Price = schedule.Price;
        existing.Status = schedule.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var s = await _db.TourSchedules.Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return false;
        if (s.Bookings.Any()) return false;
        _db.TourSchedules.Remove(s);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(int id)
    {
        var s = await _db.TourSchedules.FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return false;
        s.Status = 4; // CANCELLED
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseAsync(int id)
    {
        var s = await _db.TourSchedules.FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return false;
        s.Status = 3; // CLOSED
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateAvailableSeatsAsync(int scheduleId)
    {
        var s = await _db.TourSchedules.FirstOrDefaultAsync(x => x.Id == scheduleId);
        if (s == null) return;
        if (s.BookedGuests >= s.MaxGuests && s.Status == 1)
        {
            s.Status = 2; // FULL
        }
        else if (s.BookedGuests < s.MaxGuests && s.Status == 2)
        {
            s.Status = 1; // OPEN
        }
        await _db.SaveChangesAsync();
    }

    public Task<int> CountUpcomingAsync() => _db.TourSchedules.CountAsync(s => s.StartDate >= DateTime.UtcNow.Date && s.Status == 1);
    public Task<int> CountActiveAsync() => _db.TourSchedules.CountAsync(s => s.Status == 1);
}