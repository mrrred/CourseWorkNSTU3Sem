using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CourseWork.Presentation.Converters
{
    public class NullImageConverter : IValueConverter
    {
        private static readonly string PlaceholderImagePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "BusPlaceholder.png");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = value as string;

            // Если путь пустой или файл не существует - возвращаем заглушку
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                // Проверяем, существует ли файл заглушки
                if (File.Exists(PlaceholderImagePath))
                {
                    return new BitmapImage(new Uri(PlaceholderImagePath));
                }
                return null;
            }

            // Загружаем изображение по пути
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}