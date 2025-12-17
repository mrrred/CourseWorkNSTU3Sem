using CourseWork.Domain.Models;

namespace CourseWork.Services.Interfaces
{
    public interface IDriverService
    {
        void AddDriver(Driver driver);
        void UpdateDriver(Driver driver);
        void RemoveDriver(string personnelNumber);
        Driver GetDriverByPersonnelNumber(string personnelNumber);
        IEnumerable<Driver> GetAllDrivers();
        IEnumerable<Driver> SearchDriversByName(string name);
        IEnumerable<Driver> GetDriversByCategory(string category);
        IEnumerable<Driver> GetDriversByExperienceRange(int minExperience, int maxExperience);
        IEnumerable<Driver> GetDriversByClass(int driverClass);
        IEnumerable<Driver> GetAvailableDriversForDate(DateTime date);
        Dictionary<string, int> GetDriverStatisticsByCategory();
        Dictionary<int, int> GetDriverStatisticsByClass();
        bool DriverExists(string personnelNumber);
    }
}