namespace CourseWork.Data.Xml
{
    public class XmlFileSettings
    {
        public string DataDirectory { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BusTransportSystem"
        );

        public string BackupDirectory { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BusTransportSystem",
            "Backups"
        );

        public string BusFileName { get; } = "Buses.xml";
        public string DriverFileName { get; } = "Drivers.xml";
        public string RouteFileName { get; } = "Routes.xml";
        public string TripFileName { get; } = "Trips.xml";

        public string GetBusFilePath() => Path.Combine(DataDirectory, BusFileName);
        public string GetDriverFilePath() => Path.Combine(DataDirectory, DriverFileName);
        public string GetRouteFilePath() => Path.Combine(DataDirectory, RouteFileName);
        public string GetTripFilePath() => Path.Combine(DataDirectory, TripFileName);
        public string GetBackupDirectoryPath() => BackupDirectory;

        public void EnsureDataDirectoryExists()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }
        }

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