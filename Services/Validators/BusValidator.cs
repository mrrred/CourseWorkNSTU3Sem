using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Services.Exceptions;

namespace CourseWork.Services.Validators
{
    public class BusValidator
    {
        private readonly IBusRepository _busRepository;

        public BusValidator(IBusRepository busRepository)
        {
            _busRepository = busRepository;
        }

        public void ValidateForAdd(Bus bus)
        {
            if (bus == null)
                throw new ValidationException("Автобус не может быть null");

            if (_busRepository.Exists(bus.GovernmentNumber))
                throw new BusinessRuleException($"Автобус с государственным номером {bus.GovernmentNumber} уже существует");
        }

        public void ValidateForUpdate(Bus bus)
        {
            if (bus == null)
                throw new ValidationException("Автобус не может быть null");

            if (!_busRepository.Exists(bus.GovernmentNumber))
                throw new BusinessRuleException($"Автобус с государственным номером {bus.GovernmentNumber} не найден");
        }

        public void ValidateGovernmentNumber(string governmentNumber)
        {
            if (string.IsNullOrWhiteSpace(governmentNumber))
                throw new ValidationException("Государственный номер не может быть пустым");
        }
    }
}