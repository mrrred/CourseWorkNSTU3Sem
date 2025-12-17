using CourseWork.Data.Dtos;
using CourseWork.Domain.Models;
using CourseWork.Domain.Services;

namespace CourseWork.Data.Mappings
{
    public class DriverMapper : IMapper<Driver, DriverDto>
    {
        private readonly ITimeService _timeService;

        public DriverMapper(ITimeService timeService)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
        }

        public DriverDto ToDto(Driver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));

            return new DriverDto
            {
                FullName = driver.FullName,
                PersonnelNumber = driver.PersonnelNumber,
                BirthYear = driver.BirthYear,
                ExperienceYears = driver.ExperienceYears,
                LicenseCategory = driver.LicenseCategory,
                DriverClass = driver.DriverClass
            };
        }

        public Driver ToDomain(DriverDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var driver = new Driver(
                _timeService,
                dto.FullName,
                dto.PersonnelNumber,
                dto.BirthYear,
                dto.ExperienceYears,
                dto.LicenseCategory,
                dto.DriverClass
            );

            return driver;
        }
    }
}