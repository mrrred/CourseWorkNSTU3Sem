using System.IO;
using System.Windows.Media.Imaging;

namespace CourseWork.Presentation.Services
{
    public class ImageService : IImageService
    {
        private readonly string _appDataFolder;
        private readonly string _imagesFolder;
        private const string PlaceholderImageName = "BusPlaceholder.png";

        public ImageService()
        {
            _appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BusTransportSystem"
            );

            _imagesFolder = Path.Combine(_appDataFolder, "Images");

            EnsureDirectoriesExist();
            CreatePlaceholderImageIfNotExists();
        }

        private void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(_appDataFolder))
                Directory.CreateDirectory(_appDataFolder);

            if (!Directory.Exists(_imagesFolder))
                Directory.CreateDirectory(_imagesFolder);
        }

        private void CreatePlaceholderImageIfNotExists()
        {
            var placeholderPath = Path.Combine(_imagesFolder, PlaceholderImageName);
            if (!File.Exists(placeholderPath))
            {
                // Создаем простую заглушку (белый прямоугольник 300x200)
                var placeholder = new System.Windows.Media.Imaging.WriteableBitmap(300, 200, 96, 96,
                    System.Windows.Media.PixelFormats.Bgra32, null);

                // Можно было бы нарисовать что-то, но для простоты просто белый фон
                // В реальном проекте здесь была бы логика создания изображения-заглушки
                // Пока оставляем просто создание файла
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
                var destinationPath = Path.Combine(_imagesFolder, uniqueFileName);

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
                if (imagePath.StartsWith(_imagesFolder))
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
            return Path.Combine(_imagesFolder, PlaceholderImageName);
        }

        public bool IsValidImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            var extension = Path.GetExtension(filePath)?.ToLower();

            return validExtensions.Contains(extension);
        }

        public BitmapImage LoadImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }
}