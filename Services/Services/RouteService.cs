using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Services.Constants;
using CourseWork.Services.Interfaces;
using CourseWork.Services.Validators;
using CourseWork.Services.Exceptions;

namespace CourseWork.Services.Services
{
    public class RouteService : BaseService, IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly RouteValidator _validator;

        public RouteService(
            IRouteRepository routeRepository,
            ITripRepository tripRepository)
        {
            _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _validator = new RouteValidator(_routeRepository);
        }

        public void AddRoute(Route route)
        {
            _validator.ValidateForAdd(route);

            HandleRepositoryOperation(
                () => _routeRepository.Add(route),
                $"Ошибка при добавлении маршрута {route.RouteCode}"
            );
        }

        public void UpdateRoute(Route route)
        {
            _validator.ValidateForUpdate(route);

            HandleRepositoryOperation(
                () => _routeRepository.Update(route),
                $"Ошибка при обновлении маршрута {route.RouteCode}"
            );
        }

        public void RemoveRoute(string routeCode)
        {
            _validator.ValidateRouteCode(routeCode);

            // Проверяем, нет ли выполненных рейсов по этому маршруту
            var routeTrips = GetTripsByRoute(routeCode);
            if (routeTrips.Any())
                throw new BusinessRuleException($"Невозможно удалить маршрут {routeCode}: по нему есть выполненные рейсы");

            var route = GetRouteByCode(routeCode);

            HandleRepositoryOperation(
                () => _routeRepository.Remove(route),
                $"Ошибка при удалении маршрута {routeCode}"
            );
        }

        public Route GetRouteByCode(string routeCode)
        {
            _validator.ValidateRouteCode(routeCode);

            return HandleRepositoryOperation(
                () => _routeRepository.GetById(routeCode),
                $"Ошибка при получении маршрута {routeCode}"
            );
        }

        public IEnumerable<Route> GetAllRoutes()
        {
            return HandleRepositoryOperation(
                () => _routeRepository.GetAll(),
                "Ошибка при получении списка маршрутов"
            );
        }

        public IEnumerable<Route> GetRoutesByDay(DayOfWeek day)
        {
            return HandleRepositoryOperation(
                () => _routeRepository.GetRoutesByDay(day),
                $"Ошибка при получении маршрутов на день {day}"
            );
        }

        public IEnumerable<Route> GetRoutesByPoint(string pointName)
        {
            if (string.IsNullOrWhiteSpace(pointName))
                throw new ValidationException("Название пункта не может быть пустым");

            return HandleRepositoryOperation(
                () => _routeRepository.GetRoutesByPoint(pointName),
                $"Ошибка при получении маршрутов по пункту {pointName}"
            );
        }

        public IEnumerable<Route> GetRoutesByTimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime > endTime)
                throw new ValidationException("Начальное время не может быть больше конечного");

            return HandleRepositoryOperation(
                () => _routeRepository.GetRoutesByTimeRange(startTime, endTime),
                "Ошибка при получении маршрутов по диапазону времени"
            );
        }

        public IEnumerable<Route> SearchRoutes(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllRoutes();

            var allRoutes = GetAllRoutes();
            return allRoutes.Where(route =>
                route.RouteCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                route.StartPoint.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                route.EndPoint.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                route.IntermediatePoints.Any(p => p.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            );
        }

        public IEnumerable<string> GetAllRouteCodes()
        {
            return GetAllRoutes()
                .Select(r => r.RouteCode)
                .OrderBy(code => code);
        }

        public Dictionary<DayOfWeek, int> GetRouteFrequencyByDay()
        {
            return HandleRepositoryOperation(
                () => _routeRepository.GetDayStatistics(),
                "Ошибка при получении статистики маршрутов по дням"
            );
        }

        public bool RouteExists(string routeCode)
        {
            _validator.ValidateRouteCode(routeCode);

            return HandleRepositoryOperation(
                () => _routeRepository.Exists(routeCode),
                $"Ошибка при проверке существования маршрута {routeCode}"
            );
        }

        public IEnumerable<Route> GetRoutesByTravelTimeRange(TimeSpan minTime, TimeSpan maxTime)
        {
            if (minTime <= TimeSpan.Zero)
                throw new ValidationException("Минимальное время в пути должно быть положительным");

            if (maxTime < minTime)
                throw new ValidationException("Максимальное время в пути не может быть меньше минимального");

            return HandleRepositoryOperation(
                () => _routeRepository.GetRoutesByTravelTimeRange(minTime, maxTime),
                "Ошибка при получении маршрутов по диапазону времени в пути"
            );
        }

        public IEnumerable<string> GetAllStartPoints()
        {
            return HandleRepositoryOperation(
                () => _routeRepository.GetAllStartPoints(),
                "Ошибка при получении всех начальных пунктов"
            );
        }

        public IEnumerable<string> GetAllEndPoints()
        {
            return HandleRepositoryOperation(
                () => _routeRepository.GetAllEndPoints(),
                "Ошибка при получении всех конечных пунктов"
            );
        }

        public TimeSpan GetAverageRouteTravelTime()
        {
            return HandleRepositoryOperation(
                () => _routeRepository.GetAverageTravelTime(),
                "Ошибка при расчете среднего времени в пути маршрутов"
            );
        }

        private IEnumerable<Trip> GetTripsByRoute(string routeCode)
        {
            return HandleRepositoryOperation(
                () => _tripRepository.GetByRoute(routeCode),
                $"Ошибка при получении рейсов по маршруту {routeCode}"
            );
        }
    }
}