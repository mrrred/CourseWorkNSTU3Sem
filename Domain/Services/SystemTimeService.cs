namespace CourseWork.Domain.Services
{
    public class SystemTimeService : ITimeService
    {
        public DateTime GetCurrentDate() => DateTime.Now;
        public int GetCurrentYear() => DateTime.Now.Year;
    }
}