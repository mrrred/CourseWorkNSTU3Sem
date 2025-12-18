using CourseWork.Domain.Models;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Presentation.Views.Trip;
using CourseWork.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Trip
{
    public class TripListViewModel : ObservableObject
    {
        private readonly ITripService _tripService;
        private readonly IDialogService _dialogService;

        private DateTime _startDate = DateTime.Now.AddDays(-30);
        private DateTime _endDate = DateTime.Now;
        private string _searchText;
        private ObservableCollection<TripItemViewModel> _trips;
        private TripItemViewModel _selectedTrip;
        private ObservableCollection<TripItemViewModel> _filteredTrips;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    LoadTrips();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    LoadTrips();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterTrips();
                }
            }
        }

        public ObservableCollection<TripItemViewModel> Trips
        {
            get => _trips;
            private set => SetProperty(ref _trips, value);
        }

        public TripItemViewModel SelectedTrip
        {
            get => _selectedTrip;
            set => SetProperty(ref _selectedTrip, value);
        }

        public ObservableCollection<TripItemViewModel> FilteredTrips
        {
            get => _filteredTrips;
            private set => SetProperty(ref _filteredTrips, value);
        }

        public int TotalTripCount => Trips?.Count ?? 0;
        public int FilteredTripCount => FilteredTrips?.Count ?? 0;

        public decimal TotalRevenue => FilteredTrips?.Sum(t => t.TotalRevenue) ?? 0;
        public int TotalTicketsSold => FilteredTrips?.Sum(t => t.TicketsSold) ?? 0;
        public decimal AverageRevenuePerTrip => FilteredTripCount > 0 ? TotalRevenue / FilteredTripCount : 0;

        public ICommand AddTripCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand DeleteTripCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ShowStatisticsCommand { get; }

        public TripListViewModel(ITripService tripService)
        {
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _dialogService = new DialogService();

            Trips = new ObservableCollection<TripItemViewModel>();
            FilteredTrips = new ObservableCollection<TripItemViewModel>();

            AddTripCommand = new RelayCommand(AddTrip);
            ViewDetailsCommand = new RelayCommand(ViewDetails, () => SelectedTrip != null);
            DeleteTripCommand = new RelayCommand(DeleteTrip, () => SelectedTrip != null);
            RefreshCommand = new RelayCommand(Refresh);
            ShowStatisticsCommand = new RelayCommand(ShowStatistics);

            LoadTrips();
        }

        private void LoadTrips()
        {
            try
            {
                var trips = _tripService.GetTripsByDateRange(StartDate, EndDate);
                Trips.Clear();

                foreach (var trip in trips)
                {
                    Trips.Add(ConvertToItemViewModel(trip));
                }

                FilterTrips();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при загрузке рейсов: {ex.Message}");
            }
        }

        private TripItemViewModel ConvertToItemViewModel(Domain.Models.Trip trip)
        {
            return new TripItemViewModel
            {
                TripDate = trip.TripDate,
                RouteCode = trip.RouteCode,
                DriverPersonnelNumber = trip.DriverPersonnelNumber,
                TicketsSold = trip.TicketsSold,
                TotalRevenue = trip.TotalRevenue
            };
        }

        private void FilterTrips()
        {
            FilteredTrips.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var trip in Trips)
                {
                    FilteredTrips.Add(trip);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var trip in Trips.Where(t =>
                    t.RouteCode.ToLower().Contains(searchLower) ||
                    t.DriverPersonnelNumber.ToLower().Contains(searchLower)))
                {
                    FilteredTrips.Add(trip);
                }
            }

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            OnPropertyChanged(nameof(TotalTripCount));
            OnPropertyChanged(nameof(FilteredTripCount));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalTicketsSold));
            OnPropertyChanged(nameof(AverageRevenuePerTrip));
        }

        private void AddTrip()
        {
            var editWindow = new TripEditWindow();
            var viewModel = new TripEditViewModel(_tripService, _dialogService);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadTrips();
                _dialogService.ShowMessageDialog($"Рейс успешно добавлен", "Успех");
            }
        }

        private void ViewDetails()
        {
            if (SelectedTrip == null) return;

            var detailsWindow = new TripDetailsWindow();
            var viewModel = new TripDetailsViewModel(SelectedTrip);
            detailsWindow.DataContext = viewModel;
            detailsWindow.Owner = Application.Current.MainWindow;
            detailsWindow.ShowDialog();
        }

        private void DeleteTrip()
        {
            if (SelectedTrip == null) return;

            if (_dialogService.ShowConfirmationDialog($"Вы уверены, что хотите удалить рейс от {SelectedTrip.TripDateDisplay}?"))
            {
                try
                {
                    _tripService.RemoveTrip(SelectedTrip.TripDate, SelectedTrip.RouteCode, SelectedTrip.DriverPersonnelNumber);
                    Trips.Remove(SelectedTrip);
                    FilterTrips();
                    _dialogService.ShowMessageDialog("Рейс успешно удален", "Успех");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowErrorDialog($"Ошибка при удалении рейса: {ex.Message}");
                }
            }
        }

        private void ShowStatistics()
        {
            try
            {
                var statistics = _tripService.GetTripStatistics(StartDate, EndDate);

                string message = $"Статистика за период с {StartDate:dd.MM.yyyy} по {EndDate:dd.MM.yyyy}:\n\n" +
                                $"Всего рейсов: {statistics.TotalTrips}\n" +
                                $"Всего проданных билетов: {statistics.TotalTicketsSold}\n" +
                                $"Общая выручка: {statistics.TotalRevenue:C}\n" +
                                $"Средняя выручка за рейс: {statistics.AverageRevenuePerTrip:C}\n" +
                                $"Среднее количество билетов за рейс: {statistics.AverageTicketsPerTrip:F2}";

                _dialogService.ShowMessageDialog(message, "Статистика рейсов");
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при получении статистики: {ex.Message}");
            }
        }

        private void Refresh()
        {
            LoadTrips();
        }
    }
}