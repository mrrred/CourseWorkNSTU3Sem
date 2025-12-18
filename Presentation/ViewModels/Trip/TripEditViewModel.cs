using CourseWork.Domain.Models;
using CourseWork.Domain.Services;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Trip
{
    public class TripEditViewModel : ObservableObject
    {
        private readonly ITripService _tripService;
        private readonly IDialogService _dialogService;
        private readonly ITimeService _timeService;

        private TripItemViewModel _trip;

        public TripItemViewModel Trip
        {
            get => _trip;
            set => SetProperty(ref _trip, value);
        }

        private DateTime _maxDate = DateTime.Now;
        public DateTime MaxDate
        {
            get => _maxDate;
            private set => SetProperty(ref _maxDate, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public TripEditViewModel(
            ITripService tripService,
            IDialogService dialogService)
        {
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _timeService = new SystemTimeService();

            Trip = new TripItemViewModel
            {
                TripDate = DateTime.Now.Date // Устанавливаем только дату
            };

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave()
        {
            return Trip.TripDate <= DateTime.Now &&
                   !string.IsNullOrWhiteSpace(Trip.RouteCode) &&
                   !string.IsNullOrWhiteSpace(Trip.DriverPersonnelNumber) &&
                   Trip.TicketsSold >= 0 &&
                   Trip.TotalRevenue >= 0;
        }

        private void Save()
        {
            try
            {
                // Убедимся, что сохраняем только дату
                var domainTrip = ConvertToDomainModel(Trip);
                _tripService.AddTrip(domainTrip);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при сохранении рейса: {ex.Message}");
            }
        }

        private Domain.Models.Trip ConvertToDomainModel(TripItemViewModel item)
        {
            // Убедимся, что сохраняем только дату
            return new Domain.Models.Trip(
                _timeService,
                item.TripDate.Date, // Сохраняем только дату
                item.RouteCode,
                item.DriverPersonnelNumber,
                item.TicketsSold,
                item.TotalRevenue
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