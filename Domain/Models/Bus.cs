using CourseWork.Domain.Constants;
using CourseWork.Domain.Services;

namespace CourseWork.Domain.Models
{
    public class Bus
    {
        private readonly ITimeService _timeService;

        private string _governmentNumber;
        private int _capacity;
        private int _yearOfManufacture;
        private int? _yearOfOverhaul;
        private int _mileageAtYearStart;

        public Bus(
            ITimeService timeService,
            string governmentNumber,
            string brandModel,
            int capacity,
            int yearOfManufacture,
            int mileageAtYearStart)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));

            GovernmentNumber = governmentNumber;
            BrandModel = brandModel;
            Capacity = capacity;
            YearOfManufacture = yearOfManufacture;
            MileageAtYearStart = mileageAtYearStart;
        }

        public Bus(
            ITimeService timeService,
            string governmentNumber,
            string brandModel,
            int capacity,
            int yearOfManufacture,
            int mileageAtYearStart,
            int? yearOfOverhaul,
            string? photoPath)
            : this(timeService, governmentNumber, brandModel, capacity, yearOfManufacture, mileageAtYearStart)
        {
            YearOfOverhaul = yearOfOverhaul;
            PhotoPath = photoPath;
        }

        public string GovernmentNumber
        {
            get => _governmentNumber;
            private set
            {
                ValidateGovernmentNumber(value);
                _governmentNumber = value;
            }
        }

        public string BrandModel { get; private set; }

        public int Capacity
        {
            get => _capacity;
            private set
            {
                ValidateCapacity(value);
                _capacity = value;
            }
        }

        public int YearOfManufacture
        {
            get => _yearOfManufacture;
            private set
            {
                ValidateYearOfManufacture(value);
                _yearOfManufacture = value;
            }
        }

        public int? YearOfOverhaul
        {
            get => _yearOfOverhaul;
            set
            {
                ValidateYearOfOverhaul(value);
                _yearOfOverhaul = value;
            }
        }

        public int MileageAtYearStart
        {
            get => _mileageAtYearStart;
            private set
            {
                ValidateMileage(value);
                _mileageAtYearStart = value;
            }
        }

        public string? PhotoPath { get; set; }

        private void ValidateGovernmentNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Государственный номер не может быть пустым");

            if (number.Length < BusConstants.MinimumGovernmentNumberLength)
                throw new ArgumentException($"Государственный номер должен содержать не менее {BusConstants.MinimumGovernmentNumberLength} символов");

            if (number.Length > BusConstants.MaximumGovernmentNumberLength)
                throw new ArgumentException($"Государственный номер не может превышать {BusConstants.MaximumGovernmentNumberLength} символов");
        }

        private void ValidateCapacity(int capacity)
        {
            if (capacity < BusConstants.MinimumCapacity)
                throw new ArgumentException($"Вместимость должна быть не менее {BusConstants.MinimumCapacity} мест");

            if (capacity > BusConstants.MaximumCapacity)
                throw new ArgumentException($"Вместимость не может превышать {BusConstants.MaximumCapacity} мест");
        }

        private void ValidateYearOfManufacture(int year)
        {
            int currentYear = _timeService.GetCurrentYear();
            if (year < BusConstants.MinimumYear || year > currentYear)
                throw new ArgumentException($"Год выпуска должен быть между {BusConstants.MinimumYear} и {currentYear}");
        }

        private void ValidateYearOfOverhaul(int? year)
        {
            if (!year.HasValue) return;

            if (year < YearOfManufacture)
                throw new ArgumentException("Год капитального ремонта не может быть раньше года выпуска");

            int currentYear = _timeService.GetCurrentYear();
            if (year > currentYear)
                throw new ArgumentException("Год капитального ремонта не может быть в будущем");
        }

        private void ValidateMileage(int mileage)
        {
            if (mileage < 0)
                throw new ArgumentException("Пробег не может быть отрицательным");
        }
    }
}