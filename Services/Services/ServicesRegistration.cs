using CourseWork.Core.Config;
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

            // Конфигурация приложения
            services.AddSingleton<IAppConfig, AppConfig>();

            // XML Settings and Managers
            services.AddSingleton<XmlFileSettings>();

            // Data Managers
            services.AddSingleton<IXmlDataManager<BusDto>>(provider =>
            {
                var settings = provider.GetRequiredService<XmlFileSettings>();
                return new XmlDataManager<BusDto>(settings, settings.BusFileName);
            });

            services.AddSingleton<IXmlDataManager<DriverDto>>(provider =>
            {
                var settings = provider.GetRequiredService<XmlFileSettings>();
                return new XmlDataManager<DriverDto>(settings, settings.DriverFileName);
            });

            services.AddSingleton<IXmlDataManager<RouteDto>>(provider =>
            {
                var settings = provider.GetRequiredService<XmlFileSettings>();
                return new XmlDataManager<RouteDto>(settings, settings.RouteFileName);
            });

            services.AddSingleton<IXmlDataManager<TripDto>>(provider =>
            {
                var settings = provider.GetRequiredService<XmlFileSettings>();
                return new XmlDataManager<TripDto>(settings, settings.TripFileName);
            });

            // Mappers
            services.AddSingleton<IMapper<Domain.Models.Bus, BusDto>, BusMapper>();
            services.AddSingleton<IMapper<Domain.Models.Driver, DriverDto>, DriverMapper>();
            services.AddSingleton<IMapper<Domain.Models.Route, RouteDto>, RouteMapper>();
            services.AddSingleton<IMapper<Domain.Models.Trip, TripDto>, TripMapper>();

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