using CourseWork.Domain.Models;

namespace CourseWork.Domain.Interfaces
{
    public interface IBusRepository : IRepository<Bus>
    {
        IEnumerable<Bus> GetByBrand(string brand);
        IEnumerable<Bus> GetByCapacityRange(int minCapacity, int maxCapacity);
        IEnumerable<Bus> GetByYearRange(int startYear, int endYear);
        IEnumerable<Bus> GetByOverhaulStatus(bool hasOverhaul);
        int GetTotalCapacity();
        double GetAverageCapacity();
        IEnumerable<string> GetAllBrands();
    }
}