using CourseWork.Domain.Models;

namespace CourseWork.Domain.Interfaces
{
    public interface IDriverRepository : IRepository<Driver>
    {
        IEnumerable<Driver> GetByCategory(string category);
        IEnumerable<Driver> GetByExperience(int minExperience);
        IEnumerable<Driver> GetByClass(int driverClass);
        IEnumerable<Driver> GetByBirthYearRange(int startYear, int endYear);
        IEnumerable<Driver> GetByFullName(string fullName);
        double GetAverageExperience();
        Dictionary<string, int> GetCategoryStatistics();
        Dictionary<int, int> GetClassStatistics();
    }
}