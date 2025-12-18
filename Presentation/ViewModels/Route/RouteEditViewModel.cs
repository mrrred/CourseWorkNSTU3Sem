using CourseWork.Domain.Models;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Route
{
    public class RouteEditViewModel : ObservableObject
    {
        private readonly IRouteService _routeService;
        private readonly IDialogService _dialogService;

        private RouteItemViewModel _route;
        private bool _isEditMode;
        private string _newIntermediatePoint;
        private DayOfWeek _selectedDay;

        public RouteItemViewModel Route
        {
            get => _route;
            set => SetProperty(ref _route, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string NewIntermediatePoint
        {
            get => _newIntermediatePoint;
            set => SetProperty(ref _newIntermediatePoint, value);
        }

        public DayOfWeek SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        public string WindowTitle => IsEditMode ? "Редактирование маршрута" : "Добавление маршрута";

        public DayOfWeek[] AllDays => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToArray();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddIntermediatePointCommand { get; }
        public ICommand RemoveIntermediatePointCommand { get; }
        public ICommand AddDepartureDayCommand { get; }
        public ICommand RemoveDepartureDayCommand { get; }

        public RouteEditViewModel(
            IRouteService routeService,
            IDialogService dialogService,
            RouteItemViewModel route = null)
        {
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            Route = route ?? new RouteItemViewModel
            {
                IntermediatePoints = new ObservableCollection<string>(),
                DepartureDays = new ObservableCollection<DayOfWeek>()
            };

            IsEditMode = route != null;
            SelectedDay = DayOfWeek.Monday;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            AddIntermediatePointCommand = new RelayCommand(AddIntermediatePoint, CanAddIntermediatePoint);
            RemoveIntermediatePointCommand = new RelayCommand<string>(RemoveIntermediatePoint);
            AddDepartureDayCommand = new RelayCommand(AddDepartureDay, CanAddDepartureDay);
            RemoveDepartureDayCommand = new RelayCommand<DayOfWeek>(RemoveDepartureDay);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Route.RouteCode) &&
                   !string.IsNullOrWhiteSpace(Route.StartPoint) &&
                   !string.IsNullOrWhiteSpace(Route.EndPoint) &&
                   Route.DepartureDays.Count > 0 &&
                   Route.TravelTime > TimeSpan.Zero;
        }

        private bool CanAddIntermediatePoint()
        {
            return !string.IsNullOrWhiteSpace(NewIntermediatePoint);
        }

        private bool CanAddDepartureDay()
        {
            return !Route.DepartureDays.Contains(SelectedDay);
        }

        private void Save()
        {
            try
            {
                var domainRoute = ConvertToDomainModel(Route);

                if (IsEditMode)
                {
                    _routeService.UpdateRoute(domainRoute);
                }
                else
                {
                    _routeService.AddRoute(domainRoute);
                }

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при сохранении маршрута: {ex.Message}");
            }
        }

        private Domain.Models.Route ConvertToDomainModel(RouteItemViewModel item)
        {
            return new Domain.Models.Route(
                item.RouteCode,
                item.StartPoint,
                item.EndPoint,
                item.IntermediatePoints,
                item.DepartureTime,
                item.DepartureDays,
                item.TravelTime
            );
        }

        private void Cancel()
        {
            CloseWindow(false);
        }

        private void AddIntermediatePoint()
        {
            if (!string.IsNullOrWhiteSpace(NewIntermediatePoint))
            {
                Route.IntermediatePoints.Add(NewIntermediatePoint);
                NewIntermediatePoint = string.Empty;
            }
        }

        private void RemoveIntermediatePoint(string point)
        {
            if (point != null)
            {
                Route.IntermediatePoints.Remove(point);
            }
        }

        private void AddDepartureDay()
        {
            if (!Route.DepartureDays.Contains(SelectedDay))
            {
                Route.DepartureDays.Add(SelectedDay);
            }
        }

        private void RemoveDepartureDay(DayOfWeek day)
        {
            Route.DepartureDays.Remove(day);
        }

        private void CloseWindow(bool? dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}