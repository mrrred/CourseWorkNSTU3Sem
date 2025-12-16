using CourseWork.Domain.Constants;
using CourseWork.Domain.Services;

namespace CourseWork.Domain.Models
{
    public class Trip
    {
        private readonly ITimeService _timeService;

        private DateTime _tripDate;
        private int _ticketsSold;
        private decimal _totalRevenue;

        public Trip(
            ITimeService timeService,
            DateTime tripDate,
            string routeCode,
            string driverPersonnelNumber,
            int ticketsSold,
            decimal totalRevenue)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));

            TripDate = tripDate;
            RouteCode = routeCode;
            DriverPersonnelNumber = driverPersonnelNumber;
            TicketsSold = ticketsSold;
            TotalRevenue = totalRevenue;
        }

        public DateTime TripDate
        {
            get => _tripDate;
            private set
            {
                ValidateTripDate(value);
                _tripDate = value;
            }
        }

        public string RouteCode { get; private set; }

        public string DriverPersonnelNumber { get; private set; }

        public int TicketsSold
        {
            get => _ticketsSold;
            set
            {
                ValidateTicketsSold(value);
                _ticketsSold = value;
            }
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                ValidateTotalRevenue(value);
                _totalRevenue = value;
            }
        }

        private void ValidateTripDate(DateTime date)
        {
            DateTime currentDate = _timeService.GetCurrentDate();

            if (date > currentDate)
                throw new ArgumentException("Дата рейса не может быть в будущем");

            if (date.Year < TripConstants.MinimumValidYear)
                throw new ArgumentException($"Дата рейса не может быть ранее {TripConstants.MinimumValidYear} года");
        }

        private void ValidateTicketsSold(int tickets)
        {
            if (tickets < 0)
                throw new ArgumentException("Количество проданных билетов не может быть отрицательным");

            if (tickets > TripConstants.MaximumTicketsSold)
                throw new ArgumentException($"Количество проданных билетов не может превышать {TripConstants.MaximumTicketsSold}");
        }

        private void ValidateTotalRevenue(decimal revenue)
        {
            if (revenue < 0)
                throw new ArgumentException("Выручка не может быть отрицательной");

            if (revenue > decimal.MaxValue / 2)
                throw new ArgumentException("Слишком большая выручка");
        }
    }
}