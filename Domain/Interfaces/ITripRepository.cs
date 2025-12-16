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
    }
}