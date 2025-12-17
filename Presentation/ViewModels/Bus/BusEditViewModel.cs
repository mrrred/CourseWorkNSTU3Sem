using CourseWork.Domain.Models;
using CourseWork.Domain.Services;
using CourseWork.Presentation.Common;
using CourseWork.Presentation.Services;
using CourseWork.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace CourseWork.Presentation.ViewModels.Bus
{
    public class BusEditViewModel : ObservableObject
    {
        private readonly IBusService _busService;
        private readonly IImageService _imageService;
        private readonly IDialogService _dialogService;
        private readonly ITimeService _timeService;

        private BusItemViewModel _bus;
        private bool _isEditMode;

        public BusItemViewModel Bus
        {
            get => _bus;
            set => SetProperty(ref _bus, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string WindowTitle => IsEditMode ? "Редактирование автобуса" : "Добавление автобуса";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadImageCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public BusEditViewModel(
            IBusService busService,
            IImageService imageService,
            IDialogService dialogService,
            BusItemViewModel bus = null)
        {
            _busService = busService ?? throw new ArgumentNullException(nameof(busService));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _timeService = new SystemTimeService(); // Создаем экземпляр ITimeService

            Bus = bus ?? new BusItemViewModel();
            IsEditMode = bus != null;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            LoadImageCommand = new RelayCommand(LoadImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
        }

        private bool CanSave()
        {
            // Проверяем обязательные поля
            return !string.IsNullOrWhiteSpace(Bus.GovernmentNumber) &&
                   !string.IsNullOrWhiteSpace(Bus.BrandModel) &&
                   Bus.Capacity > 0 &&
                   Bus.YearOfManufacture > 1900 &&
                   Bus.YearOfManufacture <= _timeService.GetCurrentYear() &&
                   Bus.MileageAtYearStart >= 0;
        }

        private void Save()
        {
            // Дополнительная валидация
            if (Bus.YearOfOverhaul.HasValue)
            {
                if (Bus.YearOfOverhaul < Bus.YearOfManufacture)
                {
                    _dialogService.ShowErrorDialog("Год капитального ремонта не может быть раньше года выпуска");
                    return;
                }

                if (Bus.YearOfOverhaul > _timeService.GetCurrentYear())
                {
                    _dialogService.ShowErrorDialog("Год капитального ремонта не может быть в будущем");
                    return;
                }
            }

            try
            {
                // Преобразуем в доменную модель
                var domainBus = ConvertToDomainModel(Bus);

                if (IsEditMode)
                {
                    _busService.UpdateBus(domainBus);
                }
                else
                {
                    _busService.AddBus(domainBus);
                }

                // Закрываем окно с успехом
                CloseWindow(true);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Ошибка при сохранении автобуса: {ex.Message}");
            }
        }

        private Domain.Models.Bus ConvertToDomainModel(BusItemViewModel item)
        {
            return new Domain.Models.Bus(
                _timeService,
                item.GovernmentNumber,
                item.BrandModel,
                item.Capacity,
                item.YearOfManufacture,
                item.MileageAtYearStart,
                item.YearOfOverhaul,
                item.PhotoPath
            );
        }

        private void Cancel()
        {
            // Закрываем окно с отменой
            CloseWindow(false);
        }

        private void LoadImage()
        {
            string filePath = _dialogService.ShowOpenFileDialog("Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif");
            if (!string.IsNullOrEmpty(filePath))
            {
                // Проверяем, является ли файл изображением
                if (_imageService.IsValidImageFile(filePath))
                {
                    // Копируем изображение в папку приложения
                    string newPath = _imageService.CopyImageToAppData(filePath);
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        // Удаляем старое фото если оно было
                        if (!string.IsNullOrEmpty(Bus.PhotoPath))
                        {
                            _imageService.DeleteImage(Bus.PhotoPath);
                        }

                        Bus.PhotoPath = newPath;
                    }
                    else
                    {
                        _dialogService.ShowErrorDialog("Не удалось загрузить изображение");
                    }
                }
                else
                {
                    _dialogService.ShowErrorDialog("Выбранный файл не является изображением");
                }
            }
        }

        private void RemoveImage()
        {
            if (!string.IsNullOrEmpty(Bus.PhotoPath))
            {
                if (_dialogService.ShowConfirmationDialog("Удалить фотографию автобуса?"))
                {
                    _imageService.DeleteImage(Bus.PhotoPath);
                    Bus.PhotoPath = null;
                }
            }
        }

        private void CloseWindow(bool? dialogResult)
        {
            // Находим окно, к которому привязана эта ViewModel
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