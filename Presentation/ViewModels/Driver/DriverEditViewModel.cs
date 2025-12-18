using CourseWork.Domain.Models;
using CourseWork.Domain.Services;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Services.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Driver
{
    public class DriverEditViewModel : ObservableObject
    {
        private readonly IDriverService _driverService;
        private readonly IDialogService _dialogService;
        private readonly ITimeService _timeService;

        private DriverItemViewModel _driver;
        private bool _isEditMode;

        public DriverItemViewModel Driver
        {
            get => _driver;
            set => SetProperty(ref _driver, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string WindowTitle => IsEditMode ? "Редактирование водителя" : "Добавление водителя";

        public int CurrentYear => DateTime.Now.Year;
        public int[] ValidDriverClasses => new[] { 1, 2, 3 };
        public string[] ValidLicenseCategories => new[] { "D", "E" };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public DriverEditViewModel(
            IDriverService driverService,
            IDialogService dialogService,
            DriverItemViewModel driver = null)
        {
            _driverService = driverService ?? throw new ArgumentNullException(nameof(driverService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _timeService = new SystemTimeService();

            Driver = driver ?? new DriverItemViewModel();
            IsEditMode = driver != null;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Driver.FullName) &&
                   !string.IsNullOrWhiteSpace(Driver.PersonnelNumber) &&
                   Driver.BirthYear > 1900 &&
                   Driver.BirthYear <= CurrentYear &&
                   Driver.ExperienceYears >= 0 &&
                   !string.IsNullOrWhiteSpace(Driver.LicenseCategory) &&
                   ValidLicenseCategories.Contains(Driver.LicenseCategory) &&
                   ValidDriverClasses.Contains(Driver.DriverClass);
        }

        private void Save()
        {
            try
            {
                var domainDriver = ConvertToDomainModel(Driver);

                if (IsEditMode)
                {
                    _driverService.UpdateDriver(domainDriver);
                }
                else
                {
                    _driverService.AddDriver(domainDriver);
                }

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при сохранении водителя: {ex.Message}");
            }
        }

        private Domain.Models.Driver ConvertToDomainModel(DriverItemViewModel item)
        {
            return new Domain.Models.Driver(
                _timeService,
                item.FullName,
                item.PersonnelNumber,
                item.BirthYear,
                item.ExperienceYears,
                item.LicenseCategory,
                item.DriverClass
            );
        }

        private void Cancel()
        {
            CloseWindow(false);
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