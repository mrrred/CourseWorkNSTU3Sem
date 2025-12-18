using CourseWork.Core.Config;
using CourseWork.Presentation.Services;
using CourseWork.Presentation.ViewModels;
using CourseWork.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CourseWork.Presentation
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Конфигурация сервисов
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                _serviceProvider = serviceCollection.BuildServiceProvider();

                // Создаем директории через конфигурацию
                var appConfig = _serviceProvider.GetRequiredService<IAppConfig>();
                appConfig.EnsureDirectoriesExist();

                // Создаем главное окно
                var mainWindow = new MainWindow();
                mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
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
            services.AddTransient<ViewModels.Bus.BusListViewModel>();
            services.AddTransient<ViewModels.Bus.BusEditViewModel>();
            services.AddTransient<ViewModels.Bus.BusDetailsViewModel>();
            services.AddTransient<ViewModels.Driver.DriverListViewModel>();
            services.AddTransient<ViewModels.Driver.DriverEditViewModel>();
            services.AddTransient<ViewModels.Driver.DriverDetailsViewModel>();
            services.AddTransient<ViewModels.Route.RouteListViewModel>();
            services.AddTransient<ViewModels.Route.RouteEditViewModel>();
            services.AddTransient<ViewModels.Route.RouteDetailsViewModel>();
            services.AddTransient<ViewModels.Trip.TripListViewModel>();
            services.AddTransient<ViewModels.Trip.TripEditViewModel>();
            services.AddTransient<ViewModels.Trip.TripDetailsViewModel>();
        }
    }
}