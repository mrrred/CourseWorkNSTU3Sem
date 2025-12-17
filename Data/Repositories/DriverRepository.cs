using CourseWork.Data.Dtos;
using CourseWork.Data.Mappings;
using CourseWork.Data.Xml;
using CourseWork.Data.Exceptions;
using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;

namespace CourseWork.Data.Repositories
{
    public class DriverRepository : XmlBaseRepository<Driver, DriverDto>, IDriverRepository
    {
        public DriverRepository(
            IXmlDataManager<DriverDto> xmlDataManager,
            IMapper<Driver, DriverDto> mapper)
            : base(xmlDataManager, mapper)
        {
        }

        public void Add(Driver item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();

            if (dtos.Any(d => CompareKeys(GetKey(d), item.PersonnelNumber)))
                throw new DataException($"Водитель с табельным номером {item.PersonnelNumber} уже существует");

            dtos.Add(_mapper.ToDto(item));
            SaveAllDtos(dtos);
        }

        public void Update(Driver item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var existingDtoIndex = dtos.FindIndex(d => CompareKeys(GetKey(d), item.PersonnelNumber));

            if (existingDtoIndex == -1)
                throw new DataException($"Водитель с табельным номером {item.PersonnelNumber} не найден");

            dtos[existingDtoIndex] = _mapper.ToDto(item);
            SaveAllDtos(dtos);
        }

        public void Remove(Driver item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var count = dtos.RemoveAll(d => CompareKeys(GetKey(d), item.PersonnelNumber));

            if (count == 0)
                throw new DataException($"Водитель с табельным номером {item.PersonnelNumber} не найден");

            SaveAllDtos(dtos);
        }

        public Driver GetById(object id)
        {
            if (id is not string personnelNumber)
                throw new ArgumentException("Идентификатор должен быть строкой", nameof(id));

            var dtos = LoadAllDtos();
            var dto = dtos.FirstOrDefault(d => CompareKeys(GetKey(d), personnelNumber));

            if (dto == null)
                throw new DataException($"Водитель с табельным номером {personnelNumber} не найден");

            return _mapper.ToDomain(dto);
        }

        public IEnumerable<Driver> GetAll()
        {
            var dtos = LoadAllDtos();
            return dtos.Select(_mapper.ToDomain);
        }

        public bool Exists(object id)
        {
            if (id is not string personnelNumber)
                throw new ArgumentException("Идентификатор должен быть строкой", nameof(id));

            var dtos = LoadAllDtos();
            return dtos.Any(d => CompareKeys(GetKey(d), personnelNumber));
        }

        public IEnumerable<Driver> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Категория не может быть пустой", nameof(category));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.LicenseCategory == category)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Driver> GetByExperience(int minExperience)
        {
            if (minExperience < 0)
                throw new ArgumentException("Минимальный стаж не может быть отрицательным", nameof(minExperience));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.ExperienceYears >= minExperience)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Driver> GetByClass(int driverClass)
        {
            if (driverClass < 1 || driverClass > 3)
                throw new ArgumentException("Классность должна быть от 1 до 3", nameof(driverClass));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.DriverClass == driverClass)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Driver> GetByBirthYearRange(int startYear, int endYear)
        {
            if (startYear < 1900) throw new ArgumentException("Начальный год не может быть меньше 1900", nameof(startYear));
            if (endYear < startYear) throw new ArgumentException("Конечный год не может быть меньше начального", nameof(endYear));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.BirthYear >= startYear && d.BirthYear <= endYear)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Driver> GetByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("ФИО не может быть пустым", nameof(fullName));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.FullName.Contains(fullName, StringComparison.OrdinalIgnoreCase))
                .Select(_mapper.ToDomain);
        }

        public double GetAverageExperience()
        {
            var dtos = LoadAllDtos();
            if (dtos.Count == 0) return 0;
            return dtos.Average(d => d.ExperienceYears);
        }

        public Dictionary<string, int> GetCategoryStatistics()
        {
            var dtos = LoadAllDtos();
            return dtos
                .GroupBy(d => d.LicenseCategory)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public Dictionary<int, int> GetClassStatistics()
        {
            var dtos = LoadAllDtos();
            return dtos
                .GroupBy(d => d.DriverClass)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        protected override string GetKey(Driver domain)
        {
            return domain.PersonnelNumber;
        }

        protected override string GetKey(DriverDto dto)
        {
            return dto.PersonnelNumber;
        }
    }
}