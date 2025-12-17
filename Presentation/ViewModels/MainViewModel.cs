using CourseWork.Presentation.Common;
using CourseWork.Presentation.ViewModels.Bus;
using CourseWork.Services.Interfaces;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IBusService _busService;
        private readonly IDriverService _driverService;
        private readonly IRouteService _routeService;
        private readonly ITripService _tripService;

        private ObservableObject _currentViewModel;

        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            private set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand ShowBusViewCommand { get; }
        public ICommand ShowDriverViewCommand { get; }
        public ICommand ShowRouteViewCommand { get; }
        public ICommand ShowTripViewCommand { get; }

        public MainViewModel(
            IBusService busService,
            IDriverService driverService,
            IRouteService routeService,
            ITripService tripService)
        {
            _busService = busService;
            _driverService = driverService;
            _routeService = routeService;
            _tripService = tripService;

            ShowBusViewCommand = new RelayCommand(ShowBusView);
            ShowDriverViewCommand = new RelayCommand(ShowDriverView);
            ShowRouteViewCommand = new RelayCommand(ShowRouteView);
            ShowTripViewCommand = new RelayCommand(ShowTripView);

            // По умолчанию показываем список автобусов
            ShowBusView();
        }

        private void ShowBusView()
        {
            CurrentViewModel = new BusListViewModel(_busService);
        }

        private void ShowDriverView()
        {
            CurrentViewModel = new PlaceholderViewModel("Водители");
        }

        private void ShowRouteView()
        {
            CurrentViewModel = new PlaceholderViewModel("Маршруты");
        }

        private void ShowTripView()
        {
            CurrentViewModel = new PlaceholderViewModel("Рейсы и статистика");
        }
    }
}