using CourseWork.Data.Dtos;
using CourseWork.Data.Mappings;
using CourseWork.Data.Xml;
using CourseWork.Data.Exceptions;
using CourseWork.Domain;
using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWork.Data.Repositories
{
    public class TripRepository : XmlBaseRepository<Trip, TripDto>, ITripRepository
    {
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
            var key = new TripKey(item.TripDate, item.RouteCode, item.DriverPersonnelNumber);

            if (dtos.Any(d => GetTripKey(d) == key))
                throw new DataException($"Рейс на дату {item.TripDate} по маршруту {item.RouteCode} " +
                    $"с водителем {item.DriverPersonnelNumber} уже существует");

            dtos.Add(_mapper.ToDto(item));
            SaveAllDtos(dtos);
        }

        public void Update(Trip item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var key = new TripKey(item.TripDate, item.RouteCode, item.DriverPersonnelNumber);
            var existingDtoIndex = dtos.FindIndex(d => GetTripKey(d) == key);

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
            var key = new TripKey(item.TripDate, item.RouteCode, item.DriverPersonnelNumber);
            var count = dtos.RemoveAll(d => GetTripKey(d) == key);

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
                .Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date)
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
                .Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date)
                .Sum(d => d.TotalRevenue);
        }

        public int GetTotalTicketsSoldForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date)
                .Sum(d => d.TicketsSold);
        }

        public int GetTotalTripsForPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Count(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date);
        }

        public decimal GetAverageRevenuePerTrip(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            var trips = dtos.Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date).ToList();

            if (trips.Count == 0) return 0;
            return trips.Average(d => d.TotalRevenue);
        }

        public double GetAverageTicketsSoldPerTrip(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            var trips = dtos.Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date).ToList();

            if (trips.Count == 0) return 0;
            return trips.Average(d => d.TicketsSold);
        }

        public Dictionary<string, decimal> GetRevenueByRoute(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date)
                .GroupBy(d => d.RouteCode)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.TotalRevenue));
        }

        public Dictionary<string, decimal> GetRevenueByDriver(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Начальная дата не может быть больше конечной", nameof(startDate));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date)
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
                .Where(d => d.TripDate.Date >= startDate.Date && d.TripDate.Date <= endDate.Date)
                .OrderByDescending(d => d.TotalRevenue)
                .Take(count)
                .Select(_mapper.ToDomain);
        }

        private TripKey GetTripKey(TripDto dto)
        {
            return new TripKey(dto.TripDate, dto.RouteCode, dto.DriverPersonnelNumber);
        }

        protected override string GetKey(Trip domain)
        {
            return new TripKey(domain.TripDate, domain.RouteCode, domain.DriverPersonnelNumber).ToString();
        }

        protected override string GetKey(TripDto dto)
        {
            return new TripKey(dto.TripDate, dto.RouteCode, dto.DriverPersonnelNumber).ToString();
        }
    }
}