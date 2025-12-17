using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Services.Exceptions;

namespace CourseWork.Services.Validators
{
    public class RouteValidator
    {
        private readonly IRouteRepository _routeRepository;

        public RouteValidator(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public void ValidateForAdd(Route route)
        {
            if (route == null)
                throw new ValidationException("Маршрут не может быть null");

            if (_routeRepository.Exists(route.RouteCode))
                throw new BusinessRuleException($"Маршрут с шифром {route.RouteCode} уже существует");
        }

        public void ValidateForUpdate(Route route)
        {
            if (route == null)
                throw new ValidationException("Маршрут не может быть null");

            if (!_routeRepository.Exists(route.RouteCode))
                throw new BusinessRuleException($"Маршрут с шифром {route.RouteCode} не найден");
        }

        public void ValidateRouteCode(string routeCode)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
                throw new ValidationException("Шифр маршрута не может быть пустым");
        }
    }
}