using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Domain.Services;
using CourseWork.Services.Constants;
using CourseWork.Services.Exceptions;
using CourseWork.Services.Interfaces;
using CourseWork.Services.Validators;

namespace CourseWork.Services.Services
{
    public class BusService : BaseService, IBusService
    {
        private readonly IBusRepository _busRepository;
        private readonly BusValidator _validator;
        private readonly ITimeService _timeService;

        public BusService(
            IBusRepository busRepository,
            ITimeService timeService)
        {
            _busRepository = busRepository ?? throw new ArgumentNullException(nameof(busRepository));
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            _validator = new BusValidator(_busRepository);
        }

        public void AddBus(Bus bus)
        {
            _validator.ValidateForAdd(bus);

            HandleRepositoryOperation(
                () => _busRepository.Add(bus),
                $"Ошибка при добавлении автобуса {bus.GovernmentNumber}"
            );
        }

        public void UpdateBus(Bus bus)
        {
            _validator.ValidateForUpdate(bus);

            HandleRepositoryOperation(
                () => _busRepository.Update(bus),
                $"Ошибка при обновлении автобуса {bus.GovernmentNumber}"
            );
        }

        public void RemoveBus(string governmentNumber)
        {
            _validator.ValidateGovernmentNumber(governmentNumber);

            var bus = GetBusByGovernmentNumber(governmentNumber);

            HandleRepositoryOperation(
                () => _busRepository.Remove(bus),
                $"Ошибка при удалении автобуса {governmentNumber}"
            );
        }

        public Bus GetBusByGovernmentNumber(string governmentNumber)
        {
            _validator.ValidateGovernmentNumber(governmentNumber);

            return HandleRepositoryOperation(
                () => _busRepository.GetById(governmentNumber),
                $"Ошибка при получении автобуса {governmentNumber}"
            );
        }

        public IEnumerable<Bus> GetAllBuses()
        {
            return HandleRepositoryOperation(
                () => _busRepository.GetAll(),
                "Ошибка при получении списка автобусов"
            );
        }

        public IEnumerable<Bus> SearchBusesByBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ValidationException("Марка для поиска не может быть пустой");

            return HandleRepositoryOperation(
                () => _busRepository.GetByBrand(brand),
                $"Ошибка при поиске автобусов по марке {brand}"
            );
        }

        public IEnumerable<Bus> GetBusesByCapacityRange(int minCapacity, int maxCapacity)
        {
            if (minCapacity < 0 || maxCapacity < minCapacity)
                throw new ValidationException("Некорректный диапазон вместимости");

            return HandleRepositoryOperation(
                () => _busRepository.GetByCapacityRange(minCapacity, maxCapacity),
                "Ошибка при получении автобусов по диапазону вместимости"
            );
        }

        public IEnumerable<Bus> GetBusesByYearRange(int startYear, int endYear)
        {
            if (startYear < BusinessConstants.Bus.MinimumYearForStatistics || endYear < startYear)
                throw new ValidationException("Некорректный диапазон годов");

            return HandleRepositoryOperation(
                () => _busRepository.GetByYearRange(startYear, endYear),
                "Ошибка при получении автобусов по диапазону годов"
            );
        }

        public IEnumerable<Bus> GetBusesRequiringOverhaul(int yearsThreshold = BusinessConstants.Bus.OverhaulYearsThreshold)
        {
            if (yearsThreshold <= 0)
                throw new ValidationException("Пороговое значение должно быть положительным");

            var currentYear = _timeService.GetCurrentYear();
            var allBuses = GetAllBuses();

            return allBuses.Where(bus =>
            {
                if (!bus.YearOfOverhaul.HasValue)
                {
                    // Автобус никогда не ремонтировался
                    return (currentYear - bus.YearOfManufacture) >= yearsThreshold;
                }
                else
                {
                    // Проверяем сколько лет прошло с последнего ремонта
                    return (currentYear - bus.YearOfOverhaul.Value) >= yearsThreshold;
                }
            });
        }

        public int GetTotalFleetCapacity()
        {
            return HandleRepositoryOperation(
                () => _busRepository.GetTotalCapacity(),
                "Ошибка при расчете общей вместимости автопарка"
            );
        }

        public IEnumerable<string> GetAvailableBrands()
        {
            return HandleRepositoryOperation(
                () => _busRepository.GetAllBrands(),
                "Ошибка при получении списка марок автобусов"
            );
        }

        public bool BusExists(string governmentNumber)
        {
            _validator.ValidateGovernmentNumber(governmentNumber);

            return HandleRepositoryOperation(
                () => _busRepository.Exists(governmentNumber),
                $"Ошибка при проверке существования автобуса {governmentNumber}"
            );
        }

        public double GetAverageFleetCapacity()
        {
            return HandleRepositoryOperation(
                () => _busRepository.GetAverageCapacity(),
                "Ошибка при расчете средней вместимости автопарка"
            );
        }

        public IEnumerable<Bus> GetBusesByOverhaulStatus(bool hasOverhaul)
        {
            return HandleRepositoryOperation(
                () => _busRepository.GetByOverhaulStatus(hasOverhaul),
                "Ошибка при получении автобусов по статусу капремонта"
            );
        }
    }
}