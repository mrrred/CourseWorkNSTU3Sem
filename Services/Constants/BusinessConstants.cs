namespace CourseWork.Services.Constants
{
    public static class BusinessConstants
    {
        // Константы для валидации бизнес-правил
        public static class Bus
        {
            public const int OverhaulYearsThreshold = 10; // Лет после выпуска/ремонта для капремонта
            public const int MinimumYearForStatistics = 2000;
        }

        public static class Driver
        {
            public const int MinimumExperienceForSenior = 10; // Минимальный стаж для "опытного" водителя
            public const int MaximumTripsPerDay = 2; // Максимальное количество рейсов в день на одного водителя
        }

        public static class Trip
        {
            public const int MinimumTicketsForProfitableTrip = 20; // Минимальное количество билетов для прибыльного рейса
            public const decimal MinimumRevenuePerTicket = 100; // Минимальная выручка с одного билета
            public const int TopPerformingTripsCount = 10; // Количество топ-рейсов для отчетов
        }

        public static class Route
        {
            public const int MaximumStopsPerRoute = 20; // Максимальное количество остановок на маршруте
            public const int MinimumTravelTimeMinutes = 30; // Минимальное время в пути в минутах
        }

        // Константы для отчетов и статистики
        public static class Statistics
        {
            public const int DefaultReportDays = 30; // Дней по умолчанию для отчетов
            public const int QuarterlyDays = 90; // Дней в квартале
            public const int YearlyDays = 365; // Дней в году
        }
    }
}