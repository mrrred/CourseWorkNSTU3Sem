using CourseWork.Core.Config;
using System.Collections.Generic;
using System.IO;

namespace CourseWork.Data.Xml
{
    public class XmlFileSettings
    {
        private readonly IAppConfig _appConfig;

        public XmlFileSettings(IAppConfig appConfig)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public string DataDirectory => _appConfig.DataDirectory;
        public string BackupDirectory => _appConfig.BackupsDirectory;

        public string BusFileName => _appConfig.BusFileName;
        public string DriverFileName => _appConfig.DriverFileName;
        public string RouteFileName => _appConfig.RouteFileName;
        public string TripFileName => _appConfig.TripFileName;

        public string GetBusFilePath() => _appConfig.GetBusFilePath();
        public string GetDriverFilePath() => _appConfig.GetDriverFilePath();
        public string GetRouteFilePath() => _appConfig.GetRouteFilePath();
        public string GetTripFilePath() => _appConfig.GetTripFilePath();
        public string GetBackupDirectoryPath() => _appConfig.GetBackupDirectoryPath();

        public void EnsureDataDirectoryExists() => _appConfig.EnsureDirectoriesExist();

        public IEnumerable<string> GetAllFilePaths()
        {
            return new[]
            {
                GetBusFilePath(),
                GetDriverFilePath(),
                GetRouteFilePath(),
                GetTripFilePath()
            };
        }
    }
}