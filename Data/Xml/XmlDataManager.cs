using CourseWork.Data.Exceptions;
using System.Xml.Serialization;

namespace CourseWork.Data.Xml
{
    public class XmlDataManager<T> : IXmlDataManager<T>
    {
        private readonly string _filePath;
        private readonly XmlFileSettings _settings;

        public XmlDataManager(XmlFileSettings settings, string fileName)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _settings.EnsureDataDirectoryExists();
            _filePath = Path.Combine(_settings.DataDirectory, fileName);
        }

        public List<T> LoadData()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            try
            {
                using (var stream = new FileStream(_filePath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(List<T>));
                    return (List<T>)serializer.Deserialize(stream) ?? new List<T>();
                }
            }
            catch (Exception ex)
            {
                throw new DataException($"Ошибка загрузки данных из файла {_filePath}", ex);
            }
        }

        public void SaveData(List<T> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            try
            {
                using (var stream = new FileStream(_filePath, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(stream, data);
                }
            }
            catch (Exception ex)
            {
                throw new DataException($"Ошибка сохранения данных в файл {_filePath}", ex);
            }
        }

        public string GetFilePath() => _filePath;

        public bool FileExists() => File.Exists(_filePath);

        public void DeleteFile()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        public void ClearData()
        {
            SaveData(new List<T>());
        }

        public void BackupData(string backupPath)
        {
            if (!File.Exists(_filePath)) return;

            var backupFilePath = Path.Combine(backupPath, $"{Path.GetFileNameWithoutExtension(_filePath)}_{DateTime.Now:yyyyMMdd_HHmmss}.xml");
            File.Copy(_filePath, backupFilePath, true);
        }

        public long GetFileSize()
        {
            if (!File.Exists(_filePath)) return 0;

            var fileInfo = new FileInfo(_filePath);
            return fileInfo.Length;
        }
    }
}