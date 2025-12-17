namespace CourseWork.Data.Xml
{
    public interface IXmlDataManager<T>
    {
        List<T> LoadData();
        void SaveData(List<T> data);
        string GetFilePath();
        bool FileExists();
        void DeleteFile();
        void ClearData();
        void BackupData(string backupPath);
        long GetFileSize();
    }
}