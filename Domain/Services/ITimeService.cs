namespace CourseWork.Domain.Services
{
    public interface ITimeService
    {
        DateTime GetCurrentDate();
        int GetCurrentYear();
    }
}