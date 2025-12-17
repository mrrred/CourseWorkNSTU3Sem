using CourseWork.Data.Dtos;
using CourseWork.Domain.Models;
using CourseWork.Domain.Services;

namespace CourseWork.Data.Mappings
{
    public class BusMapper : IMapper<Bus, BusDto>
    {
        private readonly ITimeService _timeService;

        public BusMapper(ITimeService timeService)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
        }

        public BusDto ToDto(Bus bus)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            return new BusDto
            {
                GovernmentNumber = bus.GovernmentNumber,
                BrandModel = bus.BrandModel,
                Capacity = bus.Capacity,
                YearOfManufacture = bus.YearOfManufacture,
                YearOfOverhaul = bus.YearOfOverhaul,
                MileageAtYearStart = bus.MileageAtYearStart,
                PhotoPath = bus.PhotoPath
            };
        }

        public Bus ToDomain(BusDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var bus = new Bus(
                _timeService,
                dto.GovernmentNumber,
                dto.BrandModel,
                dto.Capacity,
                dto.YearOfManufacture,
                dto.MileageAtYearStart,
                dto.YearOfOverhaul,
                dto.PhotoPath
            );

            return bus;
        }
    }
}