using CourseWork.Domain.Constants;
using System.Collections.ObjectModel;

namespace CourseWork.Domain.Models
{
    public class Route
    {
        private string _routeCode;
        private string _startPoint;
        private string _endPoint;
        private readonly List<string> _intermediatePoints = new List<string>();
        private TimeSpan _departureTime;
        private readonly List<DayOfWeek> _departureDays = new List<DayOfWeek>();
        private TimeSpan _travelTime;

        public Route(
            string routeCode,
            string startPoint,
            string endPoint,
            TimeSpan departureTime,
            TimeSpan travelTime)
        {
            RouteCode = routeCode;
            StartPoint = startPoint;
            EndPoint = endPoint;
            DepartureTime = departureTime;
            TravelTime = travelTime;
        }

        public Route(
            string routeCode,
            string startPoint,
            string endPoint,
            IEnumerable<string> intermediatePoints,
            TimeSpan departureTime,
            IEnumerable<DayOfWeek> departureDays,
            TimeSpan travelTime)
            : this(routeCode, startPoint, endPoint, departureTime, travelTime)
        {
            foreach (var point in intermediatePoints ?? Enumerable.Empty<string>())
            {
                AddIntermediatePoint(point);
            }

            foreach (var day in departureDays ?? Enumerable.Empty<DayOfWeek>())
            {
                AddDepartureDay(day);
            }
        }

        public string RouteCode
        {
            get => _routeCode;
            private set
            {
                ValidateRouteCode(value);
                _routeCode = value;
            }
        }

        public string StartPoint
        {
            get => _startPoint;
            private set
            {
                ValidatePoint(value, "Начальный пункт");
                _startPoint = value;
            }
        }

        public string EndPoint
        {
            get => _endPoint;
            private set
            {
                ValidatePoint(value, "Конечный пункт");
                _endPoint = value;
            }
        }

        public ReadOnlyCollection<string> IntermediatePoints => _intermediatePoints.AsReadOnly();

        public TimeSpan DepartureTime
        {
            get => _departureTime;
            set
            {
                ValidateDepartureTime(value);
                _departureTime = value;
            }
        }

        public ReadOnlyCollection<DayOfWeek> DepartureDays => _departureDays.AsReadOnly();

        public TimeSpan TravelTime
        {
            get => _travelTime;
            private set
            {
                ValidateTravelTime(value);
                _travelTime = value;
            }
        }

        public void AddIntermediatePoint(string point)
        {
            ValidatePoint(point, "Промежуточный пункт");

            if (_intermediatePoints.Count >= RouteConstants.MaximumIntermediatePoints)
                throw new ArgumentException($"Количество промежуточных пунктов не может превышать {RouteConstants.MaximumIntermediatePoints}");

            _intermediatePoints.Add(point);
        }

        public void RemoveIntermediatePoint(string point)
        {
            _intermediatePoints.Remove(point);
        }

        public void ClearIntermediatePoints()
        {
            _intermediatePoints.Clear();
        }

        public void AddDepartureDay(DayOfWeek day)
        {
            if (!_departureDays.Contains(day))
            {
                if (_departureDays.Count >= RouteConstants.MaximumDepartureDays)
                    throw new ArgumentException($"Количество дней отправления не может превышать {RouteConstants.MaximumDepartureDays}");

                _departureDays.Add(day);
            }
        }

        public void RemoveDepartureDay(DayOfWeek day)
        {
            _departureDays.Remove(day);
        }

        public void ClearDepartureDays()
        {
            _departureDays.Clear();
        }

        public bool OperatesOnDay(DayOfWeek day) => _departureDays.Contains(day);

        private void ValidateRouteCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Шифр маршрута не может быть пустым");

            if (code.Length < RouteConstants.MinimumRouteCodeLength)
                throw new ArgumentException($"Шифр маршрута должен содержать не менее {RouteConstants.MinimumRouteCodeLength} символов");

            if (code.Length > RouteConstants.MaximumRouteCodeLength)
                throw new ArgumentException($"Шифр маршрута не может превышать {RouteConstants.MaximumRouteCodeLength} символов");
        }

        private void ValidatePoint(string point, string pointName)
        {
            if (string.IsNullOrWhiteSpace(point))
                throw new ArgumentException($"{pointName} не может быть пустым");

            if (point.Length < RouteConstants.MinimumPointNameLength)
                throw new ArgumentException($"{pointName} должен содержать не менее {RouteConstants.MinimumPointNameLength} символов");

            if (point.Length > RouteConstants.MaximumPointNameLength)
                throw new ArgumentException($"{pointName} не может превышать {RouteConstants.MaximumPointNameLength} символов");
        }

        private void ValidateDepartureTime(TimeSpan time)
        {
            if (time < TimeSpan.Zero || time >= TimeSpan.FromDays(1))
                throw new ArgumentException("Время отправления должно быть в пределах суток");
        }

        private void ValidateTravelTime(TimeSpan time)
        {
            if (time <= TimeSpan.Zero)
                throw new ArgumentException("Время в пути должно быть положительным");

            if (time > TimeSpan.FromHours(RouteConstants.MaximumTravelTimeHours))
                throw new ArgumentException($"Время в пути не может превышать {RouteConstants.MaximumTravelTimeHours} часов");
        }
    }
}