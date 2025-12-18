using CourseWork.Presentation.Common;
using System;

namespace CourseWork.Presentation.ViewModels.Trip
{
    public class TripItemViewModel : ObservableObject
    {
        private DateTime _tripDate;
        private string _routeCode;
        private string _driverPersonnelNumber;
        private int _ticketsSold;
        private decimal _totalRevenue;

        public DateTime TripDate
        {
            get => _tripDate;
            set => SetProperty(ref _tripDate, value);
        }

        public string RouteCode
        {
            get => _routeCode;
            set => SetProperty(ref _routeCode, value);
        }

        public string DriverPersonnelNumber
        {
            get => _driverPersonnelNumber;
            set => SetProperty(ref _driverPersonnelNumber, value);
        }

        public int TicketsSold
        {
            get => _ticketsSold;
            set => SetProperty(ref _ticketsSold, value);
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public string TripDateDisplay => TripDate.ToString("dd.MM.yyyy");
        public decimal RevenuePerTicket => TicketsSold > 0 ? TotalRevenue / TicketsSold : 0;
        public string RevenuePerTicketDisplay => RevenuePerTicket.ToString("F2");

        public TripItemViewModel Clone()
        {
            return new TripItemViewModel
            {
                TripDate = this.TripDate,
                RouteCode = this.RouteCode,
                DriverPersonnelNumber = this.DriverPersonnelNumber,
                TicketsSold = this.TicketsSold,
                TotalRevenue = this.TotalRevenue
            };
        }
    }
}