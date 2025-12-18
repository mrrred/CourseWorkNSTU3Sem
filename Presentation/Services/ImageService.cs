using CourseWork.Core.Config;
using System;
using System.IO;
using System.Linq;

namespace CourseWork.Presentation.Services
{
    public class ImageService : IImageService
    {
        private readonly IAppConfig _appConfig;

        public ImageService(IAppConfig appConfig)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            EnsureDirectoriesExist();
            CreatePlaceholderImageIfNotExists();
        }

        private void EnsureDirectoriesExist()
        {
            _appConfig.EnsureDirectoriesExist();
        }

        private void CreatePlaceholderImageIfNotExists()
        {
            var placeholderPath = _appConfig.GetPlaceholderImagePath();
            if (!File.Exists(placeholderPath))
            {
                // Создаем пустой файл как заглушку
                // На практике лучше иметь реальное изображение в Resources
                File.WriteAllText(placeholderPath, "");
            }
        }

        public string CopyImageToAppData(string sourceImagePath)
        {
            if (string.IsNullOrWhiteSpace(sourceImagePath) || !File.Exists(sourceImagePath))
                return null;

            try
            {
                var extension = Path.GetExtension(sourceImagePath);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var destinationPath = Path.Combine(_appConfig.ImagesDirectory, uniqueFileName);

                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch
            {
                return null;
            }
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                return;

            try
            {
                // Удаляем только если файл находится в нашей папке с изображениями
                if (imagePath.StartsWith(_appConfig.ImagesDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(imagePath);
                }
            }
            catch
            {
                // Игнорируем ошибки удаления
            }
        }

        public string GetPlaceholderImagePath()
        {
            return _appConfig.GetPlaceholderImagePath();
        }

        public bool IsValidImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            var extension = Path.GetExtension(filePath)?.ToLower();
            return validExtensions.Contains(extension);
        }
    }
}