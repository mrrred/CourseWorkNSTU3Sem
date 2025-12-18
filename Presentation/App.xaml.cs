using Microsoft.Extensions.DependencyInjection;
using CourseWork.Presentation.Services;
using CourseWork.Presentation.ViewModels;
using CourseWork.Presentation.ViewModels.Bus;
using CourseWork.Presentation.ViewModels.Driver;
using CourseWork.Presentation.ViewModels.Route;
using CourseWork.Presentation.ViewModels.Trip;
using CourseWork.Services;
using System;
using System.IO;
using System.Windows;

namespace CourseWork.Presentation
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CreateApplicationFolders();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = new MainWindow();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Сервисы Presentation
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Сервисы из CourseWork.Services
            services.AddApplicationServices();

            // ViewModels
            services.AddSingleton<MainViewModel>();

            // Bus
            services.AddTransient<BusListViewModel>();
            services.AddTransient<BusEditViewModel>();
            services.AddTransient<BusDetailsViewModel>();

            // Driver
            services.AddTransient<DriverListViewModel>();
            services.AddTransient<DriverEditViewModel>();
            services.AddTransient<DriverDetailsViewModel>();

            // Route
            services.AddTransient<RouteListViewModel>();
            services.AddTransient<RouteEditViewModel>();
            services.AddTransient<RouteDetailsViewModel>();

            // Trip
            services.AddTransient<TripListViewModel>();
            services.AddTransient<TripEditViewModel>();
            services.AddTransient<TripDetailsViewModel>();
        }

        private void CreateApplicationFolders()
        {
            try
            {
                var appDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BusTransportSystem"
                );

                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                var imagesFolder = Path.Combine(appDataFolder, "Images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании папок приложения: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}