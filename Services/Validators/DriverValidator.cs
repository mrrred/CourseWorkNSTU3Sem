using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;
using CourseWork.Services.Exceptions;

namespace CourseWork.Services.Validators
{
    public class DriverValidator
    {
        private readonly IDriverRepository _driverRepository;

        public DriverValidator(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        public void ValidateForAdd(Driver driver)
        {
            if (driver == null)
                throw new ValidationException("Водитель не может быть null");

            if (_driverRepository.Exists(driver.PersonnelNumber))
                throw new BusinessRuleException($"Водитель с табельным номером {driver.PersonnelNumber} уже существует");
        }

        public void ValidateForUpdate(Driver driver)
        {
            if (driver == null)
                throw new ValidationException("Водитель не может быть null");

            if (!_driverRepository.Exists(driver.PersonnelNumber))
                throw new BusinessRuleException($"Водитель с табельным номером {driver.PersonnelNumber} не найден");
        }

        public void ValidatePersonnelNumber(string personnelNumber)
        {
            if (string.IsNullOrWhiteSpace(personnelNumber))
                throw new ValidationException("Табельный номер не может быть пустым");
        }
    }
}