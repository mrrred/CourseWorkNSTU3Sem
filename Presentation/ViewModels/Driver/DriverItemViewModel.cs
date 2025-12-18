using CourseWork.Presentation.Common;
using System;

namespace CourseWork.Presentation.ViewModels.Driver
{
    public class DriverItemViewModel : ObservableObject
    {
        private string _fullName;
        private string _personnelNumber;
        private int _birthYear;
        private int _experienceYears;
        private string _licenseCategory;
        private int _driverClass;

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string PersonnelNumber
        {
            get => _personnelNumber;
            set => SetProperty(ref _personnelNumber, value);
        }

        public int BirthYear
        {
            get => _birthYear;
            set => SetProperty(ref _birthYear, value);
        }

        public int ExperienceYears
        {
            get => _experienceYears;
            set => SetProperty(ref _experienceYears, value);
        }

        public string LicenseCategory
        {
            get => _licenseCategory;
            set => SetProperty(ref _licenseCategory, value);
        }

        public int DriverClass
        {
            get => _driverClass;
            set => SetProperty(ref _driverClass, value);
        }

        public int Age => DateTime.Now.Year - BirthYear;
        public string DriverClassDisplay => DriverClass.ToString();
        public string ExperienceDisplay => $"{ExperienceYears} лет";

        public DriverItemViewModel Clone()
        {
            return new DriverItemViewModel
            {
                FullName = this.FullName,
                PersonnelNumber = this.PersonnelNumber,
                BirthYear = this.BirthYear,
                ExperienceYears = this.ExperienceYears,
                LicenseCategory = this.LicenseCategory,
                DriverClass = this.DriverClass
            };
        }
    }
}