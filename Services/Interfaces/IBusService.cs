using CourseWork.Domain.Models;

namespace CourseWork.Services.Interfaces
{
    public interface IBusService
    {
        void AddBus(Bus bus);
        void UpdateBus(Bus bus);
        void RemoveBus(string governmentNumber);
        Bus GetBusByGovernmentNumber(string governmentNumber);
        IEnumerable<Bus> GetAllBuses();
        IEnumerable<Bus> SearchBusesByBrand(string brand);
        IEnumerable<Bus> GetBusesByCapacityRange(int minCapacity, int maxCapacity);
        IEnumerable<Bus> GetBusesByYearRange(int startYear, int endYear);
        IEnumerable<Bus> GetBusesRequiringOverhaul(int yearsThreshold);
        int GetTotalFleetCapacity();
        IEnumerable<string> GetAvailableBrands();
        bool BusExists(string governmentNumber);
    }
}