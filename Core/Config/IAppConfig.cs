using System.Collections.Generic;

namespace CourseWork.Core.Config
{
    /// <summary>
    /// Интерфейс конфигурации приложения
    /// </summary>
    public interface IAppConfig
    {
        /// <summary>
        /// Название приложения
        /// </summary>
        string AppName { get; }

        /// <summary>
        /// Версия приложения
        /// </summary>
        string AppVersion { get; }

        /// <summary>
        /// Базовая директория приложения
        /// </summary>
        string BaseDirectory { get; }

        /// <summary>
        /// Директория для хранения данных
        /// </summary>
        string DataDirectory { get; }

        /// <summary>
        /// Директория для изображений
        /// </summary>
        string ImagesDirectory { get; }

        /// <summary>
        /// Директория для резервных копий
        /// </summary>
        string BackupsDirectory { get; }

        /// <summary>
        /// Директория для ресурсов
        /// </summary>
        string ResourcesDirectory { get; }

        /// <summary>
        /// Имя файла для автобусов
        /// </summary>
        string BusFileName { get; }

        /// <summary>
        /// Имя файла для водителей
        /// </summary>
        string DriverFileName { get; }

        /// <summary>
        /// Имя файла для маршрутов
        /// </summary>
        string RouteFileName { get; }

        /// <summary>
        /// Имя файла для рейсов
        /// </summary>
        string TripFileName { get; }

        /// <summary>
        /// Имя файла-заглушки для изображений
        /// </summary>
        string PlaceholderImageName { get; }

        /// <summary>
        /// Полный путь к файлу автобусов
        /// </summary>
        string GetBusFilePath();

        /// <summary>
        /// Полный путь к файлу водителей
        /// </summary>
        string GetDriverFilePath();

        /// <summary>
        /// Полный путь к файлу маршрутов
        /// </summary>
        string GetRouteFilePath();

        /// <summary>
        /// Полный путь к файлу рейсов
        /// </summary>
        string GetTripFilePath();

        /// <summary>
        /// Полный путь к файлу-заглушке
        /// </summary>
        string GetPlaceholderImagePath();

        /// <summary>
        /// Путь к директории резервных копий
        /// </summary>
        string GetBackupDirectoryPath();

        /// <summary>
        /// Получает все пути к файлам данных
        /// </summary>
        IEnumerable<string> GetAllDataFilePaths();

        /// <summary>
        /// Создает необходимые директории, если они не существуют
        /// </summary>
        void EnsureDirectoriesExist();
    }
}