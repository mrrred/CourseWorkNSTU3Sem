using CourseWork.Data.Dtos;
using CourseWork.Data.Mappings;
using CourseWork.Data.Xml;
using CourseWork.Data.Exceptions;
using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Models;

namespace CourseWork.Data.Repositories
{
    public class BusRepository : XmlBaseRepository<Bus, BusDto>, IBusRepository
    {
        public BusRepository(
            IXmlDataManager<BusDto> xmlDataManager,
            IMapper<Bus, BusDto> mapper)
            : base(xmlDataManager, mapper)
        {
        }

        public void Add(Bus item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();

            if (dtos.Any(d => CompareKeys(GetKey(d), item.GovernmentNumber)))
                throw new DataException($"Автобус с государственным номером {item.GovernmentNumber} уже существует");

            dtos.Add(_mapper.ToDto(item));
            SaveAllDtos(dtos);
        }

        public void Update(Bus item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var existingDtoIndex = dtos.FindIndex(d => CompareKeys(GetKey(d), item.GovernmentNumber));

            if (existingDtoIndex == -1)
                throw new DataException($"Автобус с государственным номером {item.GovernmentNumber} не найден");

            dtos[existingDtoIndex] = _mapper.ToDto(item);
            SaveAllDtos(dtos);
        }

        public void Remove(Bus item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var dtos = LoadAllDtos();
            var count = dtos.RemoveAll(d => CompareKeys(GetKey(d), item.GovernmentNumber));

            if (count == 0)
                throw new DataException($"Автобус с государственным номером {item.GovernmentNumber} не найден");

            SaveAllDtos(dtos);
        }

        public Bus GetById(object id)
        {
            if (id is not string governmentNumber)
                throw new ArgumentException("Идентификатор должен быть строкой", nameof(id));

            var dtos = LoadAllDtos();
            var dto = dtos.FirstOrDefault(d => CompareKeys(GetKey(d), governmentNumber));

            if (dto == null)
                throw new DataException($"Автобус с государственным номером {governmentNumber} не найден");

            return _mapper.ToDomain(dto);
        }

        public IEnumerable<Bus> GetAll()
        {
            var dtos = LoadAllDtos();
            return dtos.Select(_mapper.ToDomain);
        }

        public bool Exists(object id)
        {
            if (id is not string governmentNumber)
                throw new ArgumentException("Идентификатор должен быть строкой", nameof(id));

            var dtos = LoadAllDtos();
            return dtos.Any(d => CompareKeys(GetKey(d), governmentNumber));
        }

        public IEnumerable<Bus> GetByBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Марка не может быть пустой", nameof(brand));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.BrandModel.Contains(brand, StringComparison.OrdinalIgnoreCase))
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Bus> GetByCapacityRange(int minCapacity, int maxCapacity)
        {
            if (minCapacity < 0) throw new ArgumentException("Минимальная вместимость не может быть отрицательной", nameof(minCapacity));
            if (maxCapacity < minCapacity) throw new ArgumentException("Максимальная вместимость не может быть меньше минимальной", nameof(maxCapacity));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.Capacity >= minCapacity && d.Capacity <= maxCapacity)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Bus> GetByYearRange(int startYear, int endYear)
        {
            if (startYear < 1900) throw new ArgumentException("Начальный год не может быть меньше 1900", nameof(startYear));
            if (endYear < startYear) throw new ArgumentException("Конечный год не может быть меньше начального", nameof(endYear));

            var dtos = LoadAllDtos();
            return dtos
                .Where(d => d.YearOfManufacture >= startYear && d.YearOfManufacture <= endYear)
                .Select(_mapper.ToDomain);
        }

        public IEnumerable<Bus> GetByOverhaulStatus(bool hasOverhaul)
        {
            var dtos = LoadAllDtos();

            if (hasOverhaul)
            {
                return dtos
                    .Where(d => d.YearOfOverhaul.HasValue)
                    .Select(_mapper.ToDomain);
            }
            else
            {
                return dtos
                    .Where(d => !d.YearOfOverhaul.HasValue)
                    .Select(_mapper.ToDomain);
            }
        }

        public int GetTotalCapacity()
        {
            var dtos = LoadAllDtos();
            return dtos.Sum(d => d.Capacity);
        }

        public double GetAverageCapacity()
        {
            var dtos = LoadAllDtos();
            if (dtos.Count == 0) return 0;
            return dtos.Average(d => d.Capacity);
        }

        public IEnumerable<string> GetAllBrands()
        {
            var dtos = LoadAllDtos();
            return dtos
                .Select(d => d.BrandModel)
                .Distinct()
                .OrderBy(b => b);
        }

        protected override string GetKey(Bus domain)
        {
            return domain.GovernmentNumber;
        }

        protected override string GetKey(BusDto dto)
        {
            return dto.GovernmentNumber;
        }
    }
}