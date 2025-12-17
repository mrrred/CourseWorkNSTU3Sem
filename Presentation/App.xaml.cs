using Microsoft.Extensions.DependencyInjection;
using CourseWork.Presentation.Services;
using CourseWork.Presentation.ViewModels;
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
            // Создаем необходимые папки при запуске
            CreateApplicationFolders();

            // Конфигурация сервисов
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Создаем главное окно и устанавливаем DataContext через ServiceProvider
            var mainWindow = new MainWindow();
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.DataContext = mainViewModel;
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
            services.AddSingleton<MainViewModel>(); // Измените на Singleton, если нужно одно окно
            services.AddTransient<ViewModels.Bus.BusListViewModel>();
            services.AddTransient<ViewModels.Bus.BusEditViewModel>();
            services.AddTransient<ViewModels.Bus.BusDetailsViewModel>();

            // Другие ViewModels (заглушки пока)
            services.AddTransient<PlaceholderViewModel>();
        }

        private void CreateApplicationFolders()
        {
            try
            {
                // Папка для данных приложения
                var appDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BusTransportSystem"
                );

                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                // Папка для изображений
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