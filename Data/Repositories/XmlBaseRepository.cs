using CourseWork.Core.Config;
using CourseWork.Data.Mappings;
using CourseWork.Data.Xml;

namespace CourseWork.Data.Repositories
{
    public abstract class XmlBaseRepository<TDomain, TDto>
    {
        protected readonly IXmlDataManager<TDto> _xmlDataManager;
        protected readonly IMapper<TDomain, TDto> _mapper;
        protected readonly XmlFileSettings _settings;

        protected XmlBaseRepository(
            IXmlDataManager<TDto> xmlDataManager,
            IMapper<TDomain, TDto> mapper)
        {
            _xmlDataManager = xmlDataManager ?? throw new ArgumentNullException(nameof(xmlDataManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            // Initialize settings from xmlDataManager if possible
            _settings = new XmlFileSettings(new AppConfig());
            XmlValidationHelper.EnsureFileExists(_xmlDataManager.GetFilePath(), typeof(TDto));
        }

        protected List<TDto> LoadAllDtos()
        {
            return _xmlDataManager.LoadData();
        }

        protected void SaveAllDtos(List<TDto> dtos)
        {
            _xmlDataManager.SaveData(dtos);
        }

        protected virtual string GetKey(TDomain domain)
        {
            throw new NotImplementedException("Метод GetKey должен быть переопределен в производном классе");
        }

        protected virtual string GetKey(TDto dto)
        {
            throw new NotImplementedException("Метод GetKey должен быть переопределен в производном классе");
        }

        protected virtual bool CompareKeys(string key1, string key2)
        {
            return string.Equals(key1, key2, StringComparison.OrdinalIgnoreCase);
        }

        protected int GetCount()
        {
            var dtos = LoadAllDtos();
            return dtos.Count;
        }

        protected bool IsEmpty()
        {
            return GetCount() == 0;
        }

        protected void ClearAllData()
        {
            _xmlDataManager.ClearData();
        }

        protected void BackupRepositoryData()
        {
            var backupPath = Path.Combine(_settings.BackupDirectory, GetType().Name);
            _xmlDataManager.BackupData(backupPath);
        }

        protected long GetRepositoryFileSize()
        {
            return _xmlDataManager.GetFileSize();
        }

        protected bool RepositoryFileExists()
        {
            return _xmlDataManager.FileExists();
        }
    }
}