using CourseWork.Domain.Models;

namespace CourseWork.Services.Interfaces
{
    public interface ITripService
    {
        void AddTrip(Trip trip);
        void UpdateTrip(Trip trip);
        void RemoveTrip(DateTime tripDate, string routeCode, string driverPersonnelNumber);
        Trip GetTrip(DateTime tripDate, string routeCode, string driverPersonnelNumber);
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsByDateRange(DateTime startDate, DateTime endDate);
        IEnumerable<Trip> GetTripsByRoute(string routeCode);
        IEnumerable<Trip> GetTripsByDriver(string driverPersonnelNumber);
        TripStatistics GetTripStatistics(DateTime startDate, DateTime endDate);
        IEnumerable<Trip> GetTopPerformingTrips(int count, DateTime startDate, DateTime endDate);
        Dictionary<string, decimal> GetRevenueByRoute(DateTime startDate, DateTime endDate);
        Dictionary<string, decimal> GetRevenueByDriver(DateTime startDate, DateTime endDate);
        bool TripExists(DateTime tripDate, string routeCode, string driverPersonnelNumber);
        decimal GetTotalRevenueForPeriod(DateTime startDate, DateTime endDate);
        int GetTotalTicketsSoldForPeriod(DateTime startDate, DateTime endDate);
        int GetTotalTripsForPeriod(DateTime startDate, DateTime endDate);
        decimal GetAverageRevenuePerTrip(DateTime startDate, DateTime endDate);
        double GetAverageTicketsSoldPerTrip(DateTime startDate, DateTime endDate);
        IEnumerable<Trip> GetProfitableTrips(DateTime startDate, DateTime endDate);
        IEnumerable<Trip> GetTopPerformingTripsDefault();
    }

    public class TripStatistics
    {
        public int TotalTrips { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRevenuePerTrip { get; set; }
        public double AverageTicketsPerTrip { get; set; }
    }
}