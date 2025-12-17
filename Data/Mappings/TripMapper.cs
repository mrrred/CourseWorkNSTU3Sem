using CourseWork.Data.Dtos;
using CourseWork.Domain.Models;
using CourseWork.Domain.Services;

namespace CourseWork.Data.Mappings
{
    public class TripMapper : IMapper<Trip, TripDto>
    {
        private readonly ITimeService _timeService;

        public TripMapper(ITimeService timeService)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
        }

        public TripDto ToDto(Trip trip)
        {
            if (trip == null) throw new ArgumentNullException(nameof(trip));

            return new TripDto
            {
                TripDate = trip.TripDate,
                RouteCode = trip.RouteCode,
                DriverPersonnelNumber = trip.DriverPersonnelNumber,
                TicketsSold = trip.TicketsSold,
                TotalRevenue = trip.TotalRevenue
            };
        }

        public Trip ToDomain(TripDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var trip = new Trip(
                _timeService,
                dto.TripDate,
                dto.RouteCode,
                dto.DriverPersonnelNumber,
                dto.TicketsSold,
                dto.TotalRevenue
            );

            return trip;
        }
    }
}