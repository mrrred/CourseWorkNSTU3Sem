using CourseWork.Domain.Models;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Presentation.Views.Route;
using CourseWork.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Route
{
    public class RouteListViewModel : ObservableObject
    {
        private readonly IRouteService _routeService;
        private readonly IDialogService _dialogService;

        private string _searchText;
        private ObservableCollection<RouteItemViewModel> _routes;
        private RouteItemViewModel _selectedRoute;
        private ObservableCollection<RouteItemViewModel> _filteredRoutes;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterRoutes();
                }
            }
        }

        public ObservableCollection<RouteItemViewModel> Routes
        {
            get => _routes;
            private set => SetProperty(ref _routes, value);
        }

        public RouteItemViewModel SelectedRoute
        {
            get => _selectedRoute;
            set => SetProperty(ref _selectedRoute, value);
        }

        public ObservableCollection<RouteItemViewModel> FilteredRoutes
        {
            get => _filteredRoutes;
            private set => SetProperty(ref _filteredRoutes, value);
        }

        public int TotalRouteCount => Routes?.Count ?? 0;
        public int FilteredRouteCount => FilteredRoutes?.Count ?? 0;

        public ICommand AddRouteCommand { get; }
        public ICommand EditRouteCommand { get; }
        public ICommand DeleteRouteCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand RefreshCommand { get; }

        public RouteListViewModel(IRouteService routeService)
        {
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
            _dialogService = new DialogService();

            Routes = new ObservableCollection<RouteItemViewModel>();
            FilteredRoutes = new ObservableCollection<RouteItemViewModel>();

            AddRouteCommand = new RelayCommand(AddRoute);
            EditRouteCommand = new RelayCommand(EditRoute, () => SelectedRoute != null);
            DeleteRouteCommand = new RelayCommand(DeleteRoute, () => SelectedRoute != null);
            ViewDetailsCommand = new RelayCommand(ViewDetails, () => SelectedRoute != null);
            RefreshCommand = new RelayCommand(Refresh);

            LoadRoutes();
        }

        private void LoadRoutes()
        {
            try
            {
                var routes = _routeService.GetAllRoutes();
                Routes.Clear();

                foreach (var route in routes)
                {
                    Routes.Add(ConvertToItemViewModel(route));
                }

                FilterRoutes();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при загрузке маршрутов: {ex.Message}");
            }
        }

        private RouteItemViewModel ConvertToItemViewModel(Domain.Models.Route route)
        {
            var intermediatePoints = new ObservableCollection<string>(route.IntermediatePoints);
            var departureDays = new ObservableCollection<DayOfWeek>(route.DepartureDays);

            return new RouteItemViewModel
            {
                RouteCode = route.RouteCode,
                StartPoint = route.StartPoint,
                EndPoint = route.EndPoint,
                IntermediatePoints = intermediatePoints,
                DepartureTime = route.DepartureTime,
                DepartureDays = departureDays,
                TravelTime = route.TravelTime
            };
        }

        private void FilterRoutes()
        {
            FilteredRoutes.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var route in Routes)
                {
                    FilteredRoutes.Add(route);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var route in Routes.Where(r =>
                    r.RouteCode.ToLower().Contains(searchLower) ||
                    r.StartPoint.ToLower().Contains(searchLower) ||
                    r.EndPoint.ToLower().Contains(searchLower) ||
                    r.IntermediatePoints.Any(p => p.ToLower().Contains(searchLower))))
                {
                    FilteredRoutes.Add(route);
                }
            }

            OnPropertyChanged(nameof(FilteredRouteCount));
        }

        private void AddRoute()
        {
            var editWindow = new RouteEditWindow();
            var viewModel = new RouteEditViewModel(_routeService, _dialogService);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadRoutes();
                _dialogService.ShowMessageDialog($"Маршрут {viewModel.Route.RouteCode} успешно добавлен", "Успех");
            }
        }

        private void EditRoute()
        {
            if (SelectedRoute == null) return;

            var selectedRouteCode = SelectedRoute.RouteCode;
            var routeCopy = SelectedRoute.Clone();
            var editWindow = new RouteEditWindow();
            var viewModel = new RouteEditViewModel(_routeService, _dialogService, routeCopy);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadRoutes();

                // Восстанавливаем выбранный маршрут
                SelectedRoute = FilteredRoutes.FirstOrDefault(r => r.RouteCode == selectedRouteCode);

                string message = SelectedRoute != null
                    ? $"Маршрут {SelectedRoute.RouteCode} успешно обновлен"
                    : "Маршрут успешно обновлен";

                _dialogService.ShowMessageDialog(message, "Успех");
            }
        }

        private void DeleteRoute()
        {
            if (SelectedRoute == null) return;

            if (_dialogService.ShowConfirmationDialog($"Вы уверены, что хотите удалить маршрут {SelectedRoute.RouteCode}?"))
            {
                try
                {
                    _routeService.RemoveRoute(SelectedRoute.RouteCode);
                    Routes.Remove(SelectedRoute);
                    FilterRoutes();
                    _dialogService.ShowMessageDialog("Маршрут успешно удален", "Успех");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowErrorDialog($"Ошибка при удалении маршрута: {ex.Message}");
                }
            }
        }

        private void ViewDetails()
        {
            if (SelectedRoute == null) return;

            var detailsWindow = new RouteDetailsWindow();
            var viewModel = new RouteDetailsViewModel(SelectedRoute);
            detailsWindow.DataContext = viewModel;
            detailsWindow.Owner = Application.Current.MainWindow;
            detailsWindow.ShowDialog();
        }

        private void Refresh()
        {
            LoadRoutes();
        }
    }
}