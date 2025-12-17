using CourseWork.Presentation.Common;

namespace CourseWork.Presentation.ViewModels.Bus
{
    public class BusItemViewModel : ObservableObject
    {
        private string _governmentNumber;
        private string _brandModel;
        private int _capacity;
        private int _yearOfManufacture;
        private int? _yearOfOverhaul;
        private int _mileageAtYearStart;
        private string _photoPath;

        public string GovernmentNumber
        {
            get => _governmentNumber;
            set => SetProperty(ref _governmentNumber, value);
        }

        public string BrandModel
        {
            get => _brandModel;
            set => SetProperty(ref _brandModel, value);
        }

        public int Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
        }

        public int YearOfManufacture
        {
            get => _yearOfManufacture;
            set => SetProperty(ref _yearOfManufacture, value);
        }

        public int? YearOfOverhaul
        {
            get => _yearOfOverhaul;
            set => SetProperty(ref _yearOfOverhaul, value);
        }

        public int MileageAtYearStart
        {
            get => _mileageAtYearStart;
            set => SetProperty(ref _mileageAtYearStart, value);
        }

        public string PhotoPath
        {
            get => _photoPath;
            set => SetProperty(ref _photoPath, value);
        }

        public string YearOfOverhaulDisplay => YearOfOverhaul?.ToString() ?? "Не было";
        public string PhotoStatus => string.IsNullOrEmpty(PhotoPath) ? "Нет фото" : "Есть фото";

        // Метод для клонирования объекта (нужен для редактирования)
        public BusItemViewModel Clone()
        {
            return new BusItemViewModel
            {
                GovernmentNumber = this.GovernmentNumber,
                BrandModel = this.BrandModel,
                Capacity = this.Capacity,
                YearOfManufacture = this.YearOfManufacture,
                YearOfOverhaul = this.YearOfOverhaul,
                MileageAtYearStart = this.MileageAtYearStart,
                PhotoPath = this.PhotoPath
            };
        }
    }
}