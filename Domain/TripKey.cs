using System;

namespace CourseWork.Domain
{
    public class TripKey : IEquatable<TripKey>
    {
        public DateTime TripDate { get; }
        public string RouteCode { get; }
        public string DriverPersonnelNumber { get; }

        public TripKey(DateTime tripDate, string routeCode, string driverPersonnelNumber)
        {
            TripDate = tripDate.Date; // Важно: храним только дату без времени
            RouteCode = routeCode ?? throw new ArgumentNullException(nameof(routeCode));
            DriverPersonnelNumber = driverPersonnelNumber ?? throw new ArgumentNullException(nameof(driverPersonnelNumber));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TripKey);
        }

        public bool Equals(TripKey other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return TripDate == other.TripDate &&
                   RouteCode == other.RouteCode &&
                   DriverPersonnelNumber == other.DriverPersonnelNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TripDate, RouteCode, DriverPersonnelNumber);
        }

        public static bool operator ==(TripKey left, TripKey right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(TripKey left, TripKey right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{TripDate:yyyyMMdd}|{RouteCode}|{DriverPersonnelNumber}";
        }
    }
}