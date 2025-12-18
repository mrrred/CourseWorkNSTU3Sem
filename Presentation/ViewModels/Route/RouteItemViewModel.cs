using CourseWork.Presentation.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CourseWork.Presentation.ViewModels.Route
{
    public class RouteItemViewModel : ObservableObject
    {
        private string _routeCode;
        private string _startPoint;
        private string _endPoint;
        private ObservableCollection<string> _intermediatePoints;
        private TimeSpan _departureTime;
        private ObservableCollection<DayOfWeek> _departureDays;
        private TimeSpan _travelTime;

        public string RouteCode
        {
            get => _routeCode;
            set => SetProperty(ref _routeCode, value);
        }

        public string StartPoint
        {
            get => _startPoint;
            set => SetProperty(ref _startPoint, value);
        }

        public string EndPoint
        {
            get => _endPoint;
            set => SetProperty(ref _endPoint, value);
        }

        public ObservableCollection<string> IntermediatePoints
        {
            get => _intermediatePoints;
            set => SetProperty(ref _intermediatePoints, value);
        }

        public TimeSpan DepartureTime
        {
            get => _departureTime;
            set => SetProperty(ref _departureTime, value);
        }

        public ObservableCollection<DayOfWeek> DepartureDays
        {
            get => _departureDays;
            set => SetProperty(ref _departureDays, value);
        }

        public TimeSpan TravelTime
        {
            get => _travelTime;
            set => SetProperty(ref _travelTime, value);
        }

        public string DepartureTimeDisplay => DepartureTime.ToString(@"hh\:mm");
        public string TravelTimeDisplay => TravelTime.ToString(@"hh\:mm");
        public string DepartureDaysDisplay => string.Join(", ", DepartureDays.Select(d => GetDayName(d)));
        public string RouteDisplay => $"{StartPoint} → {EndPoint}";

        private string GetDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return "Пн";
                case DayOfWeek.Tuesday: return "Вт";
                case DayOfWeek.Wednesday: return "Ср";
                case DayOfWeek.Thursday: return "Чт";
                case DayOfWeek.Friday: return "Пт";
                case DayOfWeek.Saturday: return "Сб";
                case DayOfWeek.Sunday: return "Вс";
                default: return day.ToString();
            }
        }

        public RouteItemViewModel Clone()
        {
            return new RouteItemViewModel
            {
                RouteCode = this.RouteCode,
                StartPoint = this.StartPoint,
                EndPoint = this.EndPoint,
                IntermediatePoints = new ObservableCollection<string>(this.IntermediatePoints),
                DepartureTime = this.DepartureTime,
                DepartureDays = new ObservableCollection<DayOfWeek>(this.DepartureDays),
                TravelTime = this.TravelTime
            };
        }
    }
}