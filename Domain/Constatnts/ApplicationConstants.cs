namespace CourseWork.Domain.Constants
{
    public static class BusConstants
    {
        public const int MinimumYear = 1900;
        public const int MaximumCapacity = 200;
        public const int MinimumCapacity = 5;
        public const int MaximumGovernmentNumberLength = 20;
        public const int MinimumGovernmentNumberLength = 3;
    }

    public static class DriverConstants
    {
        public const int MinimumNameLength = 5;
        public const int MaximumNameLength = 100;
        public const int MaximumExperienceYears = 50;
        public const int MinimumDrivingAge = 18;
        public const int MaximumDrivingAge = 70;
        public const int MinimumPersonnelNumberLength = 3;
        public const int MaximumPersonnelNumberLength = 20;
    }

    public static class TripConstants
    {
        public const int MinimumValidYear = 2000;
        public const int MaximumTicketsSold = 1000;
    }

    public static class RouteConstants
    {
        public const int MaximumRouteCodeLength = 10;
        public const int MinimumRouteCodeLength = 2;
        public const int MaximumPointNameLength = 100;
        public const int MinimumPointNameLength = 2;
        public const int MaximumIntermediatePoints = 50;
        public const int MinimumDepartureDays = 1;
        public const int MaximumDepartureDays = 7;
        public const int MaximumTravelTimeHours = 48;
    }

    public static class CategoryConstants
    {
        public static readonly string[] ValidDriverCategories = { "D", "E" };
        public static readonly int[] ValidDriverClasses = { 1, 2, 3 };
    }
}