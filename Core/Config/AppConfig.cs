using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CourseWork.Core.Config
{
    /// <summary>
    /// Реализация конфигурации приложения
    /// </summary>
    public class AppConfig : IAppConfig
    {
        private readonly string _baseDirectory;

        /// <summary>
        /// Создает экземпляр конфигурации с использованием текущей директории приложения
        /// </summary>
        public AppConfig()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Создает экземпляр конфигурации с указанной базовой директорией
        /// </summary>
        /// <param name="baseDirectory">Базовая директория</param>
        public AppConfig(string baseDirectory)
        {
            _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
        }

        public string AppName => "Система учета пассажирских автоперевозок";
        public string AppVersion => "1.0.0";
        public string BaseDirectory => _baseDirectory;

        public string DataDirectory => Path.Combine(_baseDirectory, "Data");
        public string ImagesDirectory => Path.Combine(DataDirectory, "Images");
        public string BackupsDirectory => Path.Combine(DataDirectory, "Backups");
        public string ResourcesDirectory => Path.Combine(_baseDirectory, "Resources");

        public string BusFileName => "Buses.xml";
        public string DriverFileName => "Drivers.xml";
        public string RouteFileName => "Routes.xml";
        public string TripFileName => "Trips.xml";
        public string PlaceholderImageName => "BusPlaceholder.png";

        public string GetBusFilePath() => Path.Combine(DataDirectory, BusFileName);
        public string GetDriverFilePath() => Path.Combine(DataDirectory, DriverFileName);
        public string GetRouteFilePath() => Path.Combine(DataDirectory, RouteFileName);
        public string GetTripFilePath() => Path.Combine(DataDirectory, TripFileName);
        public string GetPlaceholderImagePath() => Path.Combine(ImagesDirectory, PlaceholderImageName);
        public string GetBackupDirectoryPath() => BackupsDirectory;

        public IEnumerable<string> GetAllDataFilePaths()
        {
            return new[]
            {
                GetBusFilePath(),
                GetDriverFilePath(),
                GetRouteFilePath(),
                GetTripFilePath()
            };
        }

        public void EnsureDirectoriesExist()
        {
            // Основные директории
            Directory.CreateDirectory(DataDirectory);
            Directory.CreateDirectory(ImagesDirectory);
            Directory.CreateDirectory(BackupsDirectory);
            Directory.CreateDirectory(ResourcesDirectory);

            // Поддиректории ресурсов
            var resourcesImages = Path.Combine(ResourcesDirectory, "Images");
            Directory.CreateDirectory(resourcesImages);
        }
    }
}