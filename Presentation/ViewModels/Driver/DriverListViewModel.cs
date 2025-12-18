using CourseWork.Domain.Models;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Presentation.Views.Driver;
using CourseWork.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Driver
{
    public class DriverListViewModel : ObservableObject
    {
        private readonly IDriverService _driverService;
        private readonly IDialogService _dialogService;

        private string _searchText;
        private ObservableCollection<DriverItemViewModel> _drivers;
        private DriverItemViewModel _selectedDriver;
        private ObservableCollection<DriverItemViewModel> _filteredDrivers;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterDrivers();
                }
            }
        }

        public ObservableCollection<DriverItemViewModel> Drivers
        {
            get => _drivers;
            private set => SetProperty(ref _drivers, value);
        }

        public DriverItemViewModel SelectedDriver
        {
            get => _selectedDriver;
            set => SetProperty(ref _selectedDriver, value);
        }

        public ObservableCollection<DriverItemViewModel> FilteredDrivers
        {
            get => _filteredDrivers;
            private set => SetProperty(ref _filteredDrivers, value);
        }

        public int TotalDriverCount => Drivers?.Count ?? 0;
        public int FilteredDriverCount => FilteredDrivers?.Count ?? 0;

        public ICommand AddDriverCommand { get; }
        public ICommand EditDriverCommand { get; }
        public ICommand DeleteDriverCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand RefreshCommand { get; }

        public DriverListViewModel(IDriverService driverService)
        {
            _driverService = driverService ?? throw new ArgumentNullException(nameof(driverService));
            _dialogService = new DialogService();

            Drivers = new ObservableCollection<DriverItemViewModel>();
            FilteredDrivers = new ObservableCollection<DriverItemViewModel>();

            AddDriverCommand = new RelayCommand(AddDriver);
            EditDriverCommand = new RelayCommand(EditDriver, () => SelectedDriver != null);
            DeleteDriverCommand = new RelayCommand(DeleteDriver, () => SelectedDriver != null);
            ViewDetailsCommand = new RelayCommand(ViewDetails, () => SelectedDriver != null);
            RefreshCommand = new RelayCommand(Refresh);

            LoadDrivers();
        }

        private void LoadDrivers()
        {
            try
            {
                var drivers = _driverService.GetAllDrivers();
                Drivers.Clear();

                foreach (var driver in drivers)
                {
                    Drivers.Add(ConvertToItemViewModel(driver));
                }

                FilterDrivers();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при загрузке водителей: {ex.Message}");
            }
        }

        private DriverItemViewModel ConvertToItemViewModel(Domain.Models.Driver driver)
        {
            return new DriverItemViewModel
            {
                FullName = driver.FullName,
                PersonnelNumber = driver.PersonnelNumber,
                BirthYear = driver.BirthYear,
                ExperienceYears = driver.ExperienceYears,
                LicenseCategory = driver.LicenseCategory,
                DriverClass = driver.DriverClass
            };
        }

        private void FilterDrivers()
        {
            FilteredDrivers.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var driver in Drivers)
                {
                    FilteredDrivers.Add(driver);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var driver in Drivers.Where(d =>
                    d.FullName.ToLower().Contains(searchLower) ||
                    d.PersonnelNumber.ToLower().Contains(searchLower) ||
                    d.LicenseCategory.ToLower().Contains(searchLower)))
                {
                    FilteredDrivers.Add(driver);
                }
            }

            OnPropertyChanged(nameof(FilteredDriverCount));
        }

        private void AddDriver()
        {
            var editWindow = new DriverEditWindow();
            var viewModel = new DriverEditViewModel(_driverService, _dialogService);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadDrivers();
                _dialogService.ShowMessageDialog($"Водитель {viewModel.Driver.FullName} успешно добавлен", "Успех");
            }
        }

        private void EditDriver()
        {
            if (SelectedDriver == null) return;

            var selectedPersonnelNumber = SelectedDriver.PersonnelNumber; // Сохраняем
            var driverCopy = SelectedDriver.Clone();
            var editWindow = new DriverEditWindow();
            var viewModel = new DriverEditViewModel(_driverService, _dialogService, driverCopy);
            editWindow.DataContext = viewModel;
            editWindow.Owner = Application.Current.MainWindow;

            if (editWindow.ShowDialog() == true)
            {
                LoadDrivers();

                // Восстанавливаем выбранного водителя
                SelectedDriver = FilteredDrivers.FirstOrDefault(d => d.PersonnelNumber == selectedPersonnelNumber);

                string message = SelectedDriver != null
                    ? $"Водитель {SelectedDriver.FullName} успешно обновлен"
                    : "Водитель успешно обновлен";

                _dialogService.ShowMessageDialog(message, "Успех");
            }
        }

        private void DeleteDriver()
        {
            if (SelectedDriver == null) return;

            if (_dialogService.ShowConfirmationDialog($"Вы уверены, что хотите удалить водителя {SelectedDriver.FullName}?"))
            {
                try
                {
                    _driverService.RemoveDriver(SelectedDriver.PersonnelNumber);
                    Drivers.Remove(SelectedDriver);
                    FilterDrivers();
                    _dialogService.ShowMessageDialog("Водитель успешно удален", "Успех");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowErrorDialog($"Ошибка при удалении водителя: {ex.Message}");
                }
            }
        }

        private void ViewDetails()
        {
            if (SelectedDriver == null) return;

            var detailsWindow = new DriverDetailsWindow();
            var viewModel = new DriverDetailsViewModel(SelectedDriver);
            detailsWindow.DataContext = viewModel;
            detailsWindow.Owner = Application.Current.MainWindow;
            detailsWindow.ShowDialog();
        }

        private void Refresh()
        {
            LoadDrivers();
        }
    }
}