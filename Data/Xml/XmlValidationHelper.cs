using System.Xml.Schema;
using System.Xml.Serialization;

namespace CourseWork.Data.Xml
{
    public static class XmlValidationHelper
    {
        public static void ValidateFileAccess(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Директория не найдена: {directory}");
        }

        public static bool IsFileEmpty(string filePath)
        {
            if (!File.Exists(filePath)) return true;

            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length == 0;
        }

        public static bool IsValidXmlStructure(string filePath, Type expectedType)
        {
            if (!File.Exists(filePath)) return true;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(expectedType);
                    var result = serializer.Deserialize(stream);
                    return result != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void CreateEmptyXmlFile(string filePath, Type dataType)
        {
            if (File.Exists(filePath)) return;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(List<>).MakeGenericType(dataType));
                var emptyList = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));
                serializer.Serialize(stream, emptyList);
            }
        }

        public static void EnsureFileExists(string filePath, Type dataType)
        {
            if (!File.Exists(filePath))
            {
                CreateEmptyXmlFile(filePath, dataType);
            }
        }
    }
}