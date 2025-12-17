using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Domain.Services;
using CourseWork.Services.Constants;
using CourseWork.Services.Exceptions;
using CourseWork.Services.Interfaces;
using CourseWork.Services.Validators;

namespace CourseWork.Services.Services
{
    public class TripService : BaseService, ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly TripValidator _validator;
        private readonly ITimeService _timeService;

        public TripService(
            ITripRepository tripRepository,
            IDriverRepository driverRepository,
            IRouteRepository routeRepository,
            ITimeService timeService)
        {
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _validator = new TripValidator(tripRepository, driverRepository, routeRepository);
        }

        public void AddTrip(Trip trip)
        {
            _validator.ValidateForAdd(trip);

            // Дополнительная бизнес-проверка: не превышает ли водитель лимит рейсов в день
            var driverTripsToday = GetTripsByDriverAndDate(trip.DriverPersonnelNumber, trip.TripDate);
            if (driverTripsToday.Count() >= BusinessConstants.Driver.MaximumTripsPerDay)
            {
                throw new BusinessRuleException($"Водитель {trip.DriverPersonnelNumber} " +
                    $"не может выполнить более {BusinessConstants.Driver.MaximumTripsPerDay} рейсов в день");
            }

            HandleRepositoryOperation(
                () => _tripRepository.Add(trip),
                $"Ошибка при добавлении рейса на {trip.TripDate}"
            );
        }

        public void UpdateTrip(Trip trip)
        {
            _validator.ValidateForUpdate(trip);

            HandleRepositoryOperation(
                () => _tripRepository.Update(trip),
                $"Ошибка при обновлении рейса на {trip.TripDate}"
            );
        }

        public void RemoveTrip(DateTime tripDate, string routeCode, string driverPersonnelNumber)
        {
            var trip = GetTrip(tripDate, routeCode, driverPersonnelNumber);

            HandleRepositoryOperation(
                () => _tripRepository.Remove(trip),
                $"Ошибка при удалении рейса на {tripDate}"
            );
        }

        public Trip GetTrip(DateTime tripDate, string routeCode, string driverPersonnelNumber)
        {
            var tripKey = new TripKey(tripDate, routeCode, driverPersonnelNumber);

            return HandleRepositoryOperation(
                () => _tripRepository.GetById(tripKey),
                $"Ошибка при получении рейса на {tripDate}"
            );
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            return HandleRepositoryOperation(
                () => _tripRepository.GetAll(),
                "Ошибка при получении всех рейсов"
            );
        }

        public IEnumerable<Trip> GetTripsByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetByDateRange(startDate, endDate),
                "Ошибка при получении рейсов по диапазону дат"
            );
        }

        public IEnumerable<Trip> GetTripsByRoute(string routeCode)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
                throw new ValidationException("Шифр маршрута не может быть пустым");

            return HandleRepositoryOperation(
                () => _tripRepository.GetByRoute(routeCode),
                $"Ошибка при получении рейсов по маршруту {routeCode}"
            );
        }

        public IEnumerable<Trip> GetTripsByDriver(string driverPersonnelNumber)
        {
            if (string.IsNullOrWhiteSpace(driverPersonnelNumber))
                throw new ValidationException("Табельный номер водителя не может быть пустым");

            return HandleRepositoryOperation(
                () => _tripRepository.GetByDriver(driverPersonnelNumber),
                $"Ошибка при получении рейсов водителя {driverPersonnelNumber}"
            );
        }

        public TripStatistics GetTripStatistics(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            var trips = GetTripsByDateRange(startDate, endDate).ToList();

            if (trips.Count == 0)
            {
                return new TripStatistics
                {
                    TotalTrips = 0,
                    TotalTicketsSold = 0,
                    TotalRevenue = 0,
                    AverageRevenuePerTrip = 0,
                    AverageTicketsPerTrip = 0
                };
            }

            return new TripStatistics
            {
                TotalTrips = trips.Count,
                TotalTicketsSold = trips.Sum(t => t.TicketsSold),
                TotalRevenue = trips.Sum(t => t.TotalRevenue),
                AverageRevenuePerTrip = trips.Average(t => t.TotalRevenue),
                AverageTicketsPerTrip = trips.Average(t => t.TicketsSold)
            };
        }

        public IEnumerable<Trip> GetTopPerformingTrips(int count, DateTime startDate, DateTime endDate)
        {
            if (count <= 0)
                throw new ValidationException("Количество должно быть положительным");

            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetTopRevenueTrips(startDate, endDate, count),
                "Ошибка при получении топ-рейсов по выручке"
            );
        }

        public IEnumerable<Trip> GetTopPerformingTripsDefault()
        {
            int count = BusinessConstants.Trip.TopPerformingTripsCount;
            DateTime endDate = _timeService.GetCurrentDate();
            DateTime startDate = endDate.AddDays(-BusinessConstants.Statistics.DefaultReportDays);

            return GetTopPerformingTrips(count, startDate, endDate);
        }

        public Dictionary<string, decimal> GetRevenueByRoute(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetRevenueByRoute(startDate, endDate),
                "Ошибка при получении выручки по маршрутам"
            );
        }

        public Dictionary<string, decimal> GetRevenueByDriver(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetRevenueByDriver(startDate, endDate),
                "Ошибка при получении выручки по водителям"
            );
        }

        public bool TripExists(DateTime tripDate, string routeCode, string driverPersonnelNumber)
        {
            var tripKey = new TripKey(tripDate, routeCode, driverPersonnelNumber);

            return HandleRepositoryOperation(
                () => _tripRepository.Exists(tripKey),
                $"Ошибка при проверке существования рейса на {tripDate}"
            );
        }

        public decimal GetTotalRevenueForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetTotalRevenueForPeriod(startDate, endDate),
                "Ошибка при получении общей выручки за период"
            );
        }

        public int GetTotalTicketsSoldForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetTotalTicketsSoldForPeriod(startDate, endDate),
                "Ошибка при получении общего количества проданных билетов за период"
            );
        }

        public int GetTotalTripsForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetTotalTripsForPeriod(startDate, endDate),
                "Ошибка при получении общего количества рейсов за период"
            );
        }

        public decimal GetAverageRevenuePerTrip(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetAverageRevenuePerTrip(startDate, endDate),
                "Ошибка при получении средней выручки за рейс"
            );
        }

        public double GetAverageTicketsSoldPerTrip(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            return HandleRepositoryOperation(
                () => _tripRepository.GetAverageTicketsSoldPerTrip(startDate, endDate),
                "Ошибка при получении среднего количества проданных билетов за рейс"
            );
        }

        public IEnumerable<Trip> GetProfitableTrips(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("Начальная дата не может быть больше конечной");

            var allTrips = GetTripsByDateRange(startDate, endDate);
            return allTrips.Where(trip =>
                trip.TicketsSold >= BusinessConstants.Trip.MinimumTicketsForProfitableTrip &&
                (trip.TotalRevenue / Math.Max(trip.TicketsSold, 1)) >= BusinessConstants.Trip.MinimumRevenuePerTicket
            );
        }

        private IEnumerable<Trip> GetTripsByDriverAndDate(string driverPersonnelNumber, DateTime date)
        {
            var trips = GetTripsByDriver(driverPersonnelNumber);
            return trips.Where(t => t.TripDate.Date == date.Date);
        }

        internal record TripKey(DateTime TripDate, string RouteCode, string DriverPersonnelNumber);
    }
}