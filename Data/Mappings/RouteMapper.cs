using CourseWork.Data.Dtos;
using CourseWork.Domain.Models;

namespace CourseWork.Data.Mappings
{
    public class RouteMapper : IMapper<Route, RouteDto>
    {
        public RouteDto ToDto(Route route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));

            return new RouteDto
            {
                RouteCode = route.RouteCode,
                StartPoint = route.StartPoint,
                EndPoint = route.EndPoint,
                IntermediatePoints = route.IntermediatePoints.ToList(),
                DepartureTimeString = route.DepartureTime.ToString(),
                TravelTimeString = route.TravelTime.ToString(),
                DepartureDaysStrings = route.DepartureDays.Select(d => d.ToString()).ToList()
            };
        }

        public Route ToDomain(RouteDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var route = new Route(
                dto.RouteCode,
                dto.StartPoint,
                dto.EndPoint,
                dto.IntermediatePoints,
                dto.DepartureTime,
                dto.DepartureDays,
                dto.TravelTime
            );

            return route;
        }
    }
}