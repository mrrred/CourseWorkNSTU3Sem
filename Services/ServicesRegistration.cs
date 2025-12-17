using CourseWork.Data.Dtos;
using CourseWork.Data.Mappings;
using CourseWork.Data.Repositories;
using CourseWork.Data.Xml;
using CourseWork.Domain.Interfaces;
using CourseWork.Domain.Services;
using CourseWork.Services.Interfaces;
using CourseWork.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CourseWork.Services
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Domain Services
            services.AddSingleton<ITimeService, SystemTimeService>();

            // XML Settings and Managers
            services.AddSingleton<XmlFileSettings>();

            services.AddSingleton<IXmlDataManager<BusDto>>(provider =>
                new XmlDataManager<BusDto>(
                    provider.GetRequiredService<XmlFileSettings>(),
                    provider.GetRequiredService<XmlFileSettings>().BusFileName
                ));

            services.AddSingleton<IXmlDataManager<DriverDto>>(provider =>
                new XmlDataManager<DriverDto>(
                    provider.GetRequiredService<XmlFileSettings>(),
                    provider.GetRequiredService<XmlFileSettings>().DriverFileName
                ));

            services.AddSingleton<IXmlDataManager<RouteDto>>(provider =>
                new XmlDataManager<RouteDto>(
                    provider.GetRequiredService<XmlFileSettings>(),
                    provider.GetRequiredService<XmlFileSettings>().RouteFileName
                ));

            services.AddSingleton<IXmlDataManager<TripDto>>(provider =>
                new XmlDataManager<TripDto>(
                    provider.GetRequiredService<XmlFileSettings>(),
                    provider.GetRequiredService<XmlFileSettings>().TripFileName
                ));

            // Mappers
            services.AddSingleton<IMapper<CourseWork.Domain.Models.Bus, BusDto>, BusMapper>();
            services.AddSingleton<IMapper<CourseWork.Domain.Models.Driver, DriverDto>, DriverMapper>();
            services.AddSingleton<IMapper<CourseWork.Domain.Models.Route, RouteDto>, RouteMapper>();
            services.AddSingleton<IMapper<CourseWork.Domain.Models.Trip, TripDto>, TripMapper>();

            // Repositories
            services.AddSingleton<IBusRepository, BusRepository>();
            services.AddSingleton<IDriverRepository, DriverRepository>();
            services.AddSingleton<IRouteRepository, RouteRepository>();
            services.AddSingleton<ITripRepository, TripRepository>();

            // Services
            services.AddSingleton<IBusService, BusService>();
            services.AddSingleton<IDriverService, DriverService>();
            services.AddSingleton<IRouteService, RouteService>();
            services.AddSingleton<ITripService, TripService>();

            return services;
        }
    }
}
