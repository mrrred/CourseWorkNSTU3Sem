using CourseWork.Data.Dtos;
using CourseWork.Data.Mappings;
using CourseWork.Data.Xml;
using CourseWork.Data.Exceptions;
using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;

namespace CourseWork.Data.Repositories
{
    public class TripRepository : XmlBaseRepository<Trip, TripDto>, ITripRepository
    {
        private record TripKey(DateTime TripDate, string RouteCode, string DriverPersonnelNumber);

        public TripRepository(
            IXmlDataManager<TripDto> xmlDataManager,
            IMapper<Trip, TripDto> mapper)
            : base(xmlDataManager, mapper)
        {
        }

        public void Add(Trip item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();

            if (dtos.Any(d => GetTripKey(d) == GetTripKey(item)))
                throw new DataException($"Рейс на дату {item.TripDate} по маршруту {item.RouteCode} " +
                    $"с водителем {item.DriverPersonnelNumber} уже существует");

            dtos.Add(_mapper.ToDto(item));
            SaveAllDtos(dtos);
        }

        public void Update(Trip item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var existingDtoIndex = dtos.FindIndex(d => GetTripKey(d) == GetTripKey(item));

            if (existingDtoIndex == -1)
                throw new DataException($"Рейс на дату {item.TripDate} по маршруту {item.RouteCode} " +
                    $"с водителем {item.DriverPersonnelNumber} не найден");

            dtos[existingDtoIndex] = _mapper.ToDto(item);
            SaveAllDtos(dtos);
        }

        public void Remove(Trip item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var count = dtos.RemoveAll(d => GetTripKey(d) == GetTripKey(item));

            if (count == 0)
                throw new DataException($"Рейс на дату {item.TripDate} по маршруту {item.RouteCode} " +
                    $"с водителем {item.DriverPersonnelNumber} не найден");

            SaveAllDtos(dtos);
        }

        public Trip GetById(object id)
        {
            if (id is not TripKey key)
                throw new ArgumentException("Идентификатор должен быть TripKey", nameof(id));

            var dtos = LoadAllDtos();
            var dto = dtos.FirstOrDefault(d => GetTripKey(d) == key);

            if (dto == null)
                throw new DataException($"Рейс на дату {key.TripDate} по маршруту {key.RouteCode} " +
                    $"с водителем {key.DriverPersonnelNumber} не найден");

            return _mapper.ToDomain(dto);
        }

        public IEnumerable<Trip> GetAll()
        {
            var dtos = LoadAllDtos();
            return dtos.Select(_mapper.ToDomain);
        }

        public bool Exists(object id)
        {
            if (id is not TripKey key)
                throw new ArgumentException("Идентификатор должен быть TripKey", nameof(id));

            var dtos = LoadAllDtos();
            return dtos.Any(d => GetTripKey(d) == key);
        }

        public IEnumerable<Trip> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate >= startDate && d.TripDate <= endDate)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Trip> GetByRoute(string routeCode)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
                throw new ArgumentException("Шифр маршрута не может быть пустым", nameof(routeCode));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.RouteCode == routeCode)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Trip> GetByDriver(string personnelNumber)
        {
            if (string.IsNullOrWhiteSpace(personnelNumber))
                throw new ArgumentException("Табельный номер водителя не может быть пустым", nameof(personnelNumber));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.DriverPersonnelNumber == personnelNumber)
                .Select(_mapper.ToDomain);
        }

        public decimal GetTotalRevenueForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate >= startDate && d.TripDate <= endDate)
                .Sum(d => d.TotalRevenue);
        }

        public int GetTotalTicketsSoldForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate >= startDate && d.TripDate <= endDate)
                .Sum(d => d.TicketsSold);
        }

        public int GetTotalTripsForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Count(d => d.TripDate >= startDate && d.TripDate <= endDate);
        }

        public decimal GetAverageRevenuePerTrip(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            var trips = dtos.Where(d => d.TripDate >= startDate && d.TripDate <= endDate).ToList();

            if (trips.Count == 0) return 0;
            return trips.Average(d => d.TotalRevenue);
        }

        public double GetAverageTicketsSoldPerTrip(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            var trips = dtos.Where(d => d.TripDate >= startDate && d.TripDate <= endDate).ToList();

            if (trips.Count == 0) return 0;
            return trips.Average(d => d.TicketsSold);
        }

        public Dictionary<string, decimal> GetRevenueByRoute(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate >= startDate && d.TripDate <= endDate)
                .GroupBy(d => d.RouteCode)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.TotalRevenue));
        }

        public Dictionary<string, decimal> GetRevenueByDriver(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate >= startDate && d.TripDate <= endDate)
                .GroupBy(d => d.DriverPersonnelNumber)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.TotalRevenue));
        }

        public IEnumerable<Trip> GetTopRevenueTrips(DateTime startDate, DateTime endDate, int count)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            if (count <= 0)
                throw new ArgumentException("Количество должно быть положительным", nameof(count));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate >= startDate && d.TripDate <= endDate)
                .OrderByDescending(d => d.TotalRevenue)
                .Take(count)
                .Select(_mapper.ToDomain);
        }

        private TripKey GetTripKey(Trip trip)
        {
            return new TripKey(trip.TripDate, trip.RouteCode, trip.DriverPersonnelNumber);
        }

        private TripKey GetTripKey(TripDto dto)
        {
            return new TripKey(dto.TripDate, dto.RouteCode, dto.DriverPersonnelNumber);
        }

        protected override string GetKey(Trip domain)
        {
            return $"{domain.TripDate:yyyyMMdd}|{domain.RouteCode}|{domain.DriverPersonnelNumber}";
        }

        protected override string GetKey(TripDto dto)
        {
            return $"{dto.TripDate:yyyyMMdd}|{dto.RouteCode}|{dto.DriverPersonnelNumber}";
        }
    }
}