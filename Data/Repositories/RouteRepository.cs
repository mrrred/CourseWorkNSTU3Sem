using CourseWork.Data.Dtos;
using CourseWork.Data.Mappings;
using CourseWork.Data.Xml;
using CourseWork.Data.Exceptions;
using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;

namespace CourseWork.Data.Repositories
{
    public class RouteRepository : XmlBaseRepository<Route, RouteDto>, IRouteRepository
    {
        public RouteRepository(
            IXmlDataManager<RouteDto> xmlDataManager,
            IMapper<Route, RouteDto> mapper)
            : base(xmlDataManager, mapper)
        {
        }

        public void Add(Route item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();

            if (dtos.Any(d => CompareKeys(GetKey(d), item.RouteCode)))
                throw new DataException($"Маршрут с шифром {item.RouteCode} уже существует");

            dtos.Add(_mapper.ToDto(item));
            SaveAllDtos(dtos);
        }

        public void Update(Route item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var existingDtoIndex = dtos.FindIndex(d => CompareKeys(GetKey(d), item.RouteCode));

            if (existingDtoIndex == -1)
                throw new DataException($"Маршрут с шифром {item.RouteCode} не найден");

            dtos[existingDtoIndex] = _mapper.ToDto(item);
            SaveAllDtos(dtos);
        }

        public void Remove(Route item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var count = dtos.RemoveAll(d => CompareKeys(GetKey(d), item.RouteCode));

            if (count == 0)
                throw new DataException($"Маршрут с шифром {item.RouteCode} не найден");

            SaveAllDtos(dtos);
        }

        public Route GetById(object id)
        {
            if (id is not string routeCode)
                throw new ArgumentException("Идентификатор должен быть строкой", nameof(id));

            var dtos = LoadAllDtos();
            var dto = dtos.FirstOrDefault(d => CompareKeys(GetKey(d), routeCode));

            if (dto == null)
                throw new DataException($"Маршрут с шифром {routeCode} не найден");

            return _mapper.ToDomain(dto);
        }

        public IEnumerable<Route> GetAll()
        {
            var dtos = LoadAllDtos();
            return dtos.Select(_mapper.ToDomain);
        }

        public bool Exists(object id)
        {
            if (id is not string routeCode)
                throw new ArgumentException("Идентификатор должен быть строкой", nameof(id));

            var dtos = LoadAllDtos();
            return dtos.Any(d => CompareKeys(GetKey(d), routeCode));
        }

        public IEnumerable<Route> GetRoutesByDay(DayOfWeek day)
        {
            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.DepartureDays.Contains(day))
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Route> GetRoutesByPoint(string point)
        {
            if (string.IsNullOrWhiteSpace(point))
                throw new ArgumentException("Пункт не может быть пустым", nameof(point));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.StartPoint.Contains(point, StringComparison.OrdinalIgnoreCase) ||
                            d.EndPoint.Contains(point, StringComparison.OrdinalIgnoreCase) ||
                            d.IntermediatePoints.Any(p => p.Contains(point, StringComparison.OrdinalIgnoreCase)))
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Route> GetRoutesByTimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime < TimeSpan.Zero || startTime >= TimeSpan.FromDays(1))
                throw new ArgumentException("Начальное время должно быть в пределах суток", nameof(startTime));

            if (endTime < TimeSpan.Zero || endTime >= TimeSpan.FromDays(1))
                throw new ArgumentException("Конечное время должно быть в пределах суток", nameof(endTime));

            if (startTime > endTime)
                throw new ArgumentException("Начальное время не может быть больше конечного", nameof(startTime));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.DepartureTime >= startTime && d.DepartureTime <= endTime)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Route> GetRoutesByTravelTimeRange(TimeSpan minTime, TimeSpan maxTime)
        {
            if (minTime <= TimeSpan.Zero)
                throw new ArgumentException("Минимальное время в пути должно быть положительным", nameof(minTime));

            if (maxTime < minTime)
                throw new ArgumentException("Максимальное время в пути не может быть меньше минимального", nameof(maxTime));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.TravelTime >= minTime && d.TravelTime <= maxTime)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<string> GetAllStartPoints()
        {
            var dtos = LoadAllDtos();
            return dtos
                .Select(d => d.StartPoint)
                .Distinct()
                .OrderBy(p => p);
        }

        public IEnumerable<string> GetAllEndPoints()
        {
            var dtos = LoadAllDtos();
            return dtos
                .Select(d => d.EndPoint)
                .Distinct()
                .OrderBy(p => p);
        }

        public Dictionary<DayOfWeek, int> GetDayStatistics()
        {
            var dtos = LoadAllDtos();
            var statistics = new Dictionary<DayOfWeek, int>();

            foreach (var day in Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>())
            {
                statistics[day] = dtos.Count(d => d.DepartureDays.Contains(day));
            }

            return statistics;
        }

        public TimeSpan GetAverageTravelTime()
        {
            var dtos = LoadAllDtos();
            if (dtos.Count == 0) return TimeSpan.Zero;

            var totalTicks = dtos.Sum(d => d.TravelTime.Ticks);
            return TimeSpan.FromTicks(totalTicks / dtos.Count);
        }

        protected override string GetKey(Route domain)
        {
            return domain.RouteCode;
        }

        protected override string GetKey(RouteDto dto)
        {
            return dto.RouteCode;
        }
    }
}