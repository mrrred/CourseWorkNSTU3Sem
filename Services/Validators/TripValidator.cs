using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Services.Exceptions;

namespace CourseWork.Services.Validators
{
    public class TripValidator
    {
        private readonly ITripRepository _tripRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IRouteRepository _routeRepository;

        public TripValidator(
            ITripRepository tripRepository,
            IDriverRepository driverRepository,
            IRouteRepository routeRepository)
        {
            _tripRepository = tripRepository;
            _driverRepository = driverRepository;
            _routeRepository = routeRepository;
        }

        public void ValidateForAdd(Trip trip)
        {
            if (trip == null)
                throw new ValidationException("Рейс не может быть null");

            var tripKey = new TripKey(trip.TripDate, trip.RouteCode, trip.DriverPersonnelNumber);
            if (_tripRepository.Exists(tripKey))
                throw new BusinessRuleException($"Рейс на дату {trip.TripDate} по маршруту {trip.RouteCode} уже существует");

            ValidateDriverExists(trip.DriverPersonnelNumber);
            ValidateRouteExists(trip.RouteCode);
        }

        public void ValidateForUpdate(Trip trip)
        {
            if (trip == null)
                throw new ValidationException("Рейс не может быть null");

            var tripKey = new TripKey(trip.TripDate, trip.RouteCode, trip.DriverPersonnelNumber);
            if (!_tripRepository.Exists(tripKey))
                throw new BusinessRuleException($"Рейс на дату {trip.TripDate} по маршруту {trip.RouteCode} не найден");

            ValidateDriverExists(trip.DriverPersonnelNumber);
            ValidateRouteExists(trip.RouteCode);
        }

        private void ValidateDriverExists(string personnelNumber)
        {
            if (!_driverRepository.Exists(personnelNumber))
                throw new BusinessRuleException($"Водитель с табельным номером {personnelNumber} не найден");
        }

        private void ValidateRouteExists(string routeCode)
        {
            if (!_routeRepository.Exists(routeCode))
                throw new BusinessRuleException($"Маршрут с шифром {routeCode} не найден");
        }
    }

    internal record TripKey(DateTime TripDate, string RouteCode, string DriverPersonnelNumber);
}