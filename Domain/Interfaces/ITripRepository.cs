using CourseWork.Domain.Models;
using System;

namespace CourseWork.Domain.Interfaces
{
    public interface ITripRepository : IRepository<Trip>
    {
        IEnumerable<Trip> GetByDateRange(DateTime startDate, DateTime endDate);
        IEnumerable<Trip> GetByRoute(string routeCode);
        IEnumerable<Trip> GetByDriver(string personnelNumber);
        decimal GetTotalRevenueForPeriod(DateTime startDate, DateTime endDate);
        int GetTotalTicketsSoldForPeriod(DateTime startDate, DateTime endDate);
        int GetTotalTripsForPeriod(DateTime startDate, DateTime endDate);
        decimal GetAverageRevenuePerTrip(DateTime startDate, DateTime endDate);
        double GetAverageTicketsSoldPerTrip(DateTime startDate, DateTime endDate);
        Dictionary<string, decimal> GetRevenueByRoute(DateTime startDate, DateTime endDate);
        Dictionary<string, decimal> GetRevenueByDriver(DateTime startDate, DateTime endDate);
        IEnumerable<Trip> GetTopRevenueTrips(DateTime startDate, DateTime endDate, int count);
    }
}