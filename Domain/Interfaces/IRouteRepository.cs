using CourseWork.Domain.Models;
using System;

namespace CourseWork.Domain.Interfaces
{
    public interface IRouteRepository : IRepository<Route>
    {
        IEnumerable<Route> GetRoutesByDay(DayOfWeek day);
        IEnumerable<Route> GetRoutesByPoint(string point);
        IEnumerable<Route> GetRoutesByTimeRange(TimeSpan startTime, TimeSpan endTime);
        IEnumerable<Route> GetRoutesByTravelTimeRange(TimeSpan minTime, TimeSpan maxTime);
        IEnumerable<string> GetAllStartPoints();
        IEnumerable<string> GetAllEndPoints();
        Dictionary<DayOfWeek, int> GetDayStatistics();
        TimeSpan GetAverageTravelTime();
    }
}