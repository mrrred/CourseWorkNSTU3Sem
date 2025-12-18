using CourseWork.Domain.Models;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Bus
{
    public class BusListViewModel : ObservableObject
    {
        private readonly IBusService _busService;
        private readonly IImageService _imageService;
        private readonly IDialogService _dialogService;

        private string _searchText;
        private ObservableCollection<BusItemViewModel> _buses;
        private BusItemViewModel _selectedBus;
        private ObservableCollection<BusItemViewModel> _filteredBuses;
        private int _totalBusCount;
        private int _filteredBusCount;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterBuses();
                }
            }
        }

        public ObservableCollection<BusItemViewModel> Buses
        {
            get => _buses;
            private set
            {
                if (SetProperty(ref _buses, value))
                {
                    UpdateCounts();
                }
            }
        }

        public BusItemViewModel SelectedBus
        {
            get => _selectedBus;
            set => SetProperty(ref _selectedBus, value);
        }

        public ObservableCollection<BusItemViewModel> FilteredBuses
        {
            get => _filteredBuses;
            private set
            {
                if (SetProperty(ref _filteredBuses, value))
                {
                    UpdateCounts();
                }
            }
        }

        public int TotalBusCount
        {
            get => _totalBusCount;
            private set => SetProperty(ref _totalBusCount, value);
        }

        public int FilteredBusCount
        {
            get => _filteredBusCount;
            private set => SetProperty(ref _filteredBusCount, value);
        }

        public ICommand AddBusCommand { get; }
        public ICommand EditBusCommand { get; }
        public ICommand DeleteBusCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand RefreshCommand { get; }

        public BusListViewModel(IBusService busService)
        {
            _busService = busService ?? throw new ArgumentNullException(nameof(busService));
            _imageService = new ImageService();
            _dialogService = new DialogService();

            Buses = new ObservableCollection<BusItemViewModel>();
            FilteredBuses = new ObservableCollection<BusItemViewModel>();

            AddBusCommand = new RelayCommand(AddBus);
            EditBusCommand = new RelayCommand(EditBus, () => SelectedBus != null);
            DeleteBusCommand = new RelayCommand(DeleteBus, () => SelectedBus != null);
            ViewDetailsCommand = new RelayCommand(ViewDetails, () => SelectedBus != null);
            RefreshCommand = new RelayCommand(Refresh);

            LoadBuses();
        }

        private void LoadBuses()
        {
            try
            {
                var buses = _busService.GetAllBuses();
                Buses.Clear();

                foreach (var bus in buses)
                {
                    Buses.Add(ConvertToItemViewModel(bus));
                }

                FilterBuses();
                UpdateCounts();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при загрузке автобусов: {ex.Message}");
            }
        }

        private BusItemViewModel ConvertToItemViewModel(Domain.Models.Bus bus)
        {
            return new BusItemViewModel
            {
                GovernmentNumber = bus.GovernmentNumber,
                BrandModel = bus.BrandModel,
                Capacity = bus.Capacity,
                YearOfManufacture = bus.YearOfManufacture,
                YearOfOverhaul = bus.YearOfOverhaul,
                MileageAtYearStart = bus.MileageAtYearStart,
                PhotoPath = bus.PhotoPath
            };
        }

        private Domain.Models.Bus ConvertToDomainModel(BusItemViewModel item)
        {
            // Получаем ITimeService через сервисы - заглушка
            // В реальном приложении нужно инжектировать ITimeService
            var timeService = new Domain.Services.SystemTimeService();

            return new Domain.Models.Bus(
                timeService,
                item.GovernmentNumber,
                item.BrandModel,
                item.Capacity,
                item.YearOfManufacture,
                item.MileageAtYearStart,
                item.YearOfOverhaul,
                item.PhotoPath
            );
        }

        private void FilterBuses()
        {
            FilteredBuses.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var bus in Buses)
                {
                    FilteredBuses.Add(bus);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var bus in Buses.Where(b =>
                    b.GovernmentNumber.ToLower().Contains(searchLower) ||
                    b.BrandModel.ToLower().Contains(searchLower)))
                {
                    FilteredBuses.Add(bus);
                }
            }

            UpdateCounts();
        }

        private void UpdateCounts()
        {
            TotalBusCount = Buses?.Count ?? 0;
            FilteredBusCount = FilteredBuses?.Count ?? 0;
        }

        private void AddBus()
        {
            var editWindow = new Views.Bus.BusEditWindow();
            var viewModel = new BusEditViewModel(_busService, _imageService, _dialogService);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true && viewModel.Bus != null)
            {
                // Перезагружаем список
                LoadBuses();

                _dialogService.ShowMessageDialog($"Автобус {viewModel.Bus.GovernmentNumber} успешно добавлен", "Успех");
            }
        }

        private void EditBus()
        {
            if (SelectedBus == null) return;

            var selectedGovernmentNumber = SelectedBus.GovernmentNumber; // Сохраняем
            var busCopy = SelectedBus.Clone();
            var editWindow = new Views.Bus.BusEditWindow();
            var viewModel = new BusEditViewModel(_busService, _imageService, _dialogService, busCopy);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadBuses();

                // Восстанавливаем выбранный автобус
                SelectedBus = FilteredBuses.FirstOrDefault(b => b.GovernmentNumber == selectedGovernmentNumber);

                string message = SelectedBus != null
                    ? $"Автобус {SelectedBus.GovernmentNumber} успешно обновлен"
                    : "Автобус успешно обновлен";

                _dialogService.ShowMessageDialog(message, "Успех");
            }
        }

        private void DeleteBus()
        {
            if (SelectedBus == null) return;

            if (_dialogService.ShowConfirmationDialog($"Вы уверены, что хотите удалить автобус {SelectedBus.GovernmentNumber}?"))
            {
                try
                {
                    // Удаляем через сервис
                    _busService.RemoveBus(SelectedBus.GovernmentNumber);

                    // Удаляем фото если оно есть
                    if (!string.IsNullOrEmpty(SelectedBus.PhotoPath))
                    {
                        _imageService.DeleteImage(SelectedBus.PhotoPath);
                    }

                    // Удаляем из коллекции
                    Buses.Remove(SelectedBus);
                    FilterBuses();

                    _dialogService.ShowMessageDialog($"Автобус успешно удален", "Успех");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowErrorDialog($"Ошибка при удалении автобуса: {ex.Message}");
                }
            }
        }

        private void ViewDetails()
        {
            if (SelectedBus == null) return;

            var detailsWindow = new Views.Bus.BusDetailsWindow();
            var viewModel = new BusDetailsViewModel(_imageService, SelectedBus);
            detailsWindow.DataContext = viewModel;
            detailsWindow.Owner = Application.Current.MainWindow;
            detailsWindow.ShowDialog();
        }

        private void Refresh()
        {
            LoadBuses();
        }
    }
}