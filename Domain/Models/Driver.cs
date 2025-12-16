using CourseWork.Domain.Constants;
using CourseWork.Domain.Services;
using System.Linq;

namespace CourseWork.Domain.Models
{
    public class Driver
    {
        private readonly ITimeService _timeService;

        private string _fullName;
        private string _personnelNumber;
        private int _birthYear;
        private int _experienceYears;
        private string _licenseCategory;
        private int _driverClass;

        public Driver(
            ITimeService timeService,
            string fullName,
            string personnelNumber,
            int birthYear,
            int experienceYears,
            string licenseCategory,
            int driverClass)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));

            FullName = fullName;
            PersonnelNumber = personnelNumber;
            BirthYear = birthYear;
            ExperienceYears = experienceYears;
            LicenseCategory = licenseCategory;
            DriverClass = driverClass;
        }

        public string FullName
        {
            get => _fullName;
            private set
            {
                ValidateFullName(value);
                _fullName = value;
            }
        }

        public string PersonnelNumber
        {
            get => _personnelNumber;
            private set
            {
                ValidatePersonnelNumber(value);
                _personnelNumber = value;
            }
        }

        public int BirthYear
        {
            get => _birthYear;
            private set
            {
                ValidateBirthYear(value);
                _birthYear = value;
            }
        }

        public int ExperienceYears
        {
            get => _experienceYears;
            set
            {
                ValidateExperienceYears(value);
                _experienceYears = value;
            }
        }

        public string LicenseCategory
        {
            get => _licenseCategory;
            set
            {
                ValidateLicenseCategory(value);
                _licenseCategory = value;
            }
        }

        public int DriverClass
        {
            get => _driverClass;
            set
            {
                ValidateDriverClass(value);
                _driverClass = value;
            }
        }

        private void ValidateFullName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("ФИО не может быть пустым");

            if (name.Length < DriverConstants.MinimumNameLength)
                throw new ArgumentException($"ФИО должно содержать не менее {DriverConstants.MinimumNameLength} символов");

            if (name.Length > DriverConstants.MaximumNameLength)
                throw new ArgumentException($"ФИО не может превышать {DriverConstants.MaximumNameLength} символов");
        }

        private void ValidatePersonnelNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Табельный номер не может быть пустым");

            if (number.Length < DriverConstants.MinimumPersonnelNumberLength)
                throw new ArgumentException($"Табельный номер должен содержать не менее {DriverConstants.MinimumPersonnelNumberLength} символов");

            if (number.Length > DriverConstants.MaximumPersonnelNumberLength)
                throw new ArgumentException($"Табельный номер не может превышать {DriverConstants.MaximumPersonnelNumberLength} символов");

            if (!number.All(char.IsLetterOrDigit))
                throw new ArgumentException("Табельный номер должен содержать только буквы и цифры");
        }

        private void ValidateBirthYear(int year)
        {
            int currentYear = _timeService.GetCurrentYear();
            int minYear = currentYear - DriverConstants.MaximumDrivingAge;
            int maxYear = currentYear - DriverConstants.MinimumDrivingAge;

            if (year < minYear || year > maxYear)
                throw new ArgumentException($"Год рождения должен быть между {minYear} и {maxYear}");
        }

        private void ValidateExperienceYears(int experience)
        {
            if (experience < 0)
                throw new ArgumentException("Стаж работы не может быть отрицательным");

            if (experience > DriverConstants.MaximumExperienceYears)
                throw new ArgumentException($"Стаж работы не может превышать {DriverConstants.MaximumExperienceYears} лет");

            int age = _timeService.GetCurrentYear() - BirthYear;
            int maxPossibleExperience = age - DriverConstants.MinimumDrivingAge;

            if (maxPossibleExperience > 0 && experience > maxPossibleExperience)
                throw new ArgumentException($"Стаж работы не может превышать {maxPossibleExperience} лет для данного возраста");
        }

        private void ValidateLicenseCategory(string category)
        {
            if (!CategoryConstants.ValidDriverCategories.Contains(category))
                throw new ArgumentException($"Категория водителя должна быть одной из: {string.Join(", ", CategoryConstants.ValidDriverCategories)}");
        }

        private void ValidateDriverClass(int driverClass)
        {
            if (!CategoryConstants.ValidDriverClasses.Contains(driverClass))
                throw new ArgumentException($"Классность водителя должна быть одной из: {string.Join(", ", CategoryConstants.ValidDriverClasses)}");
        }
    }
}