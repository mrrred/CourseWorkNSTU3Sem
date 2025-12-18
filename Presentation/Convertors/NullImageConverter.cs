using CourseWork.Core.Config;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CourseWork.Presentation.Converters
{
    public class NullImageConverter : IValueConverter
    {
        private static string _placeholderPath;

        private static string GetPlaceholderPath()
        {
            if (_placeholderPath == null)
            {
                // Пытаемся найти файл в нескольких местах
                var paths = new[]
                {
                    new AppConfig().GetPlaceholderImagePath()
                };

                _placeholderPath = paths.FirstOrDefault(File.Exists);
            }

            return _placeholderPath;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = value as string;

            // Если путь пустой или файл не существует - пытаемся вернуть заглушку
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                var placeholderPath = GetPlaceholderPath();
                if (!string.IsNullOrEmpty(placeholderPath) && File.Exists(placeholderPath))
                {
                    return LoadImage(placeholderPath);
                }
                return null;
            }

            // Загружаем изображение по указанному пути
            return LoadImage(imagePath);
        }

        private BitmapImage LoadImage(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}