using CourseWork.Domain.Models;

namespace CourseWork.Services.Interfaces
{
    public interface IRouteService
    {
        void AddRoute(Route route);
        void UpdateRoute(Route route);
        void RemoveRoute(string routeCode);
        Route GetRouteByCode(string routeCode);
        IEnumerable<Route> GetAllRoutes();
        IEnumerable<Route> GetRoutesByDay(DayOfWeek day);
        IEnumerable<Route> GetRoutesByPoint(string pointName);
        IEnumerable<Route> GetRoutesByTimeRange(TimeSpan startTime, TimeSpan endTime);
        IEnumerable<Route> SearchRoutes(string searchTerm);
        IEnumerable<string> GetAllRouteCodes();
        Dictionary<DayOfWeek, int> GetRouteFrequencyByDay();
        bool RouteExists(string routeCode);
    }
}