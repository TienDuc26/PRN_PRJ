using TourManagement.Web.Models.Entities;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Services.Interfaces;

public interface ITourService
{
    Task<PagedResult<Tour>> GetToursAsync(TourFilterViewModel filter);
    Task<Tour?> GetTourByIdAsync(int id);
    Task<Tour?> GetTourWithDetailsAsync(int id);
    Task<Tour> CreateTourAsync(Tour tour);
    Task UpdateTourAsync(Tour tour);
    Task<bool> DeleteTourAsync(int id); // returns true if hard deleted
    Task<bool> SoftDeleteTourAsync(int id);
    Task<bool> ToggleStatusAsync(int id);
    Task<int> CountActiveToursAsync();
}

public interface IDestinationService
{
    Task<List<Destination>> GetAllAsync();
    Task<PagedResult<Destination>> GetPagedAsync(string? keyword, int page, int pageSize);
    Task<Destination?> GetByIdAsync(int id);
    Task CreateAsync(Destination dest);
    Task UpdateAsync(Destination dest);
    Task<bool> DeleteAsync(int id);
    Task<int> CountActiveAsync();
}

public interface IItineraryService
{
    Task<List<TourItinerary>> GetByTourAsync(int tourId);
    Task<TourItinerary?> GetByIdAsync(int id);
    Task CreateAsync(TourItinerary item);
    Task UpdateAsync(TourItinerary item);
    Task DeleteAsync(int id);
    Task ReorderAsync(int tourId, List<int> orderedIds);
}

public interface IScheduleService
{
    Task<PagedResult<TourSchedule>> GetPagedAsync(ScheduleFilterViewModel filter);
    Task<List<TourSchedule>> GetByTourAsync(int tourId);
    Task<TourSchedule?> GetByIdAsync(int id);
    Task<TourSchedule> CreateAsync(TourSchedule schedule);
    Task UpdateAsync(TourSchedule schedule);
    Task<bool> DeleteAsync(int id);
    Task<bool> CancelAsync(int id);
    Task<bool> CloseAsync(int id);
    Task UpdateAvailableSeatsAsync(int scheduleId);
    Task<int> CountUpcomingAsync();
    Task<int> CountActiveAsync();
}