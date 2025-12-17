using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Domain.Services;
using CourseWork.Services.Constants;
using CourseWork.Services.Exceptions;
using CourseWork.Services.Interfaces;
using CourseWork.Services.Validators;

namespace CourseWork.Services.Services
{
    public class DriverService : BaseService, IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ITripRepository _tripRepository;
        private readonly DriverValidator _validator;
        private readonly ITimeService _timeService;

        public DriverService(
            IDriverRepository driverRepository,
            ITripRepository tripRepository,
            ITimeService timeService)
        {
            _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _validator = new DriverValidator(_driverRepository);
        }

        public void AddDriver(Driver driver)
        {
            _validator.ValidateForAdd(driver);

            HandleRepositoryOperation(
                () => _driverRepository.Add(driver),
                $"Ошибка при добавлении водителя {driver.PersonnelNumber}"
            );
        }

        public void UpdateDriver(Driver driver)
        {
            _validator.ValidateForUpdate(driver);

            HandleRepositoryOperation(
                () => _driverRepository.Update(driver),
                $"Ошибка при обновлении водителя {driver.PersonnelNumber}"
            );
        }

        public void RemoveDriver(string personnelNumber)
        {
            _validator.ValidatePersonnelNumber(personnelNumber);

            // Проверяем, нет ли у водителя выполненных рейсов
            var driverTrips = GetTripsByDriver(personnelNumber);
            if (driverTrips.Any())
                throw new BusinessRuleException($"Невозможно удалить водителя {personnelNumber}: у него есть выполненные рейсы");

            var driver = GetDriverByPersonnelNumber(personnelNumber);

            HandleRepositoryOperation(
                () => _driverRepository.Remove(driver),
                $"Ошибка при удалении водителя {personnelNumber}"
            );
        }

        public Driver GetDriverByPersonnelNumber(string personnelNumber)
        {
            _validator.ValidatePersonnelNumber(personnelNumber);

            return HandleRepositoryOperation(
                () => _driverRepository.GetById(personnelNumber),
                $"Ошибка при получении водителя {personnelNumber}"
            );
        }

        public IEnumerable<Driver> GetAllDrivers()
        {
            return HandleRepositoryOperation(
                () => _driverRepository.GetAll(),
                "Ошибка при получении списка водителей"
            );
        }

        public IEnumerable<Driver> SearchDriversByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Имя для поиска не может быть пустым");

            return HandleRepositoryOperation(
                () => _driverRepository.GetByFullName(name),
                $"Ошибка при поиске водителей по имени {name}"
            );
        }

        public IEnumerable<Driver> GetDriversByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ValidationException("Категория не может быть пустой");

            return HandleRepositoryOperation(
                () => _driverRepository.GetByCategory(category),
                $"Ошибка при получении водителей по категории {category}"
            );
        }

        public IEnumerable<Driver> GetDriversByExperienceRange(int minExperience, int maxExperience)
        {
            if (minExperience < 0 || maxExperience < minExperience)
                throw new ValidationException("Некорректный диапазон стажа");

            var allDrivers = GetAllDrivers();
            return allDrivers.Where(d => d.ExperienceYears >= minExperience && d.ExperienceYears <= maxExperience);
        }

        public IEnumerable<Driver> GetDriversByClass(int driverClass)
        {
            if (driverClass < 1 || driverClass > 3)
                throw new ValidationException("Классность должна быть от 1 до 3");

            return HandleRepositoryOperation(
                () => _driverRepository.GetByClass(driverClass),
                $"Ошибка при получении водителей по классности {driverClass}"
            );
        }

        public IEnumerable<Driver> GetAvailableDriversForDate(DateTime date)
        {
            // Получаем всех водителей
            var allDrivers = GetAllDrivers();

            // Получаем водителей, которые уже имеют рейсы на эту дату
            var busyDrivers = GetTripsByDateRange(date, date)
                .Select(t => t.DriverPersonnelNumber)
                .Distinct();

            // Возвращаем водителей, которые не заняты в эту дату
            return allDrivers.Where(d => !busyDrivers.Contains(d.PersonnelNumber));
        }

        public Dictionary<string, int> GetDriverStatisticsByCategory()
        {
            return HandleRepositoryOperation(
                () => _driverRepository.GetCategoryStatistics(),
                "Ошибка при получении статистики водителей по категориям"
            );
        }

        public Dictionary<int, int> GetDriverStatisticsByClass()
        {
            return HandleRepositoryOperation(
                () => _driverRepository.GetClassStatistics(),
                "Ошибка при получении статистики водителей по классам"
            );
        }

        public bool DriverExists(string personnelNumber)
        {
            _validator.ValidatePersonnelNumber(personnelNumber);

            return HandleRepositoryOperation(
                () => _driverRepository.Exists(personnelNumber),
                $"Ошибка при проверке существования водителя {personnelNumber}"
            );
        }

        public IEnumerable<Driver> GetExperiencedDrivers(int minExperience = BusinessConstants.Driver.MinimumExperienceForSenior)
        {
            if (minExperience < 0)
                throw new ValidationException("Минимальный стаж не может быть отрицательным");

            return HandleRepositoryOperation(
                () => _driverRepository.GetByExperience(minExperience),
                $"Ошибка при получении опытных водителей со стажем от {minExperience} лет"
            );
        }

        public double GetAverageDriverExperience()
        {
            return HandleRepositoryOperation(
                () => _driverRepository.GetAverageExperience(),
                "Ошибка при расчете среднего стажа водителей"
            );
        }

        private IEnumerable<Trip> GetTripsByDriver(string personnelNumber)
        {
            return HandleRepositoryOperation(
                () => _tripRepository.GetByDriver(personnelNumber),
                $"Ошибка при получении рейсов водителя {personnelNumber}"
            );
        }

        private IEnumerable<Trip> GetTripsByDateRange(DateTime startDate, DateTime endDate)
        {
            return HandleRepositoryOperation(
                () => _tripRepository.GetByDateRange(startDate, endDate),
                "Ошибка при получении рейсов по диапазону дат"
            );
        }
    }
}