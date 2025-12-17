using System.Xml.Serialization;

namespace CourseWork.Data.Dtos
{
    [XmlRoot("Trip")]
    public class TripDto
    {
        [XmlElement("TripDate")]
        public DateTime TripDate { get; set; }

        [XmlElement("RouteCode")]
        public string RouteCode { get; set; } = string.Empty;

        [XmlElement("DriverPersonnelNumber")]
        public string DriverPersonnelNumber { get; set; } = string.Empty;

        [XmlElement("TicketsSold")]
        public int TicketsSold { get; set; }

        [XmlElement("TotalRevenue")]
        public decimal TotalRevenue { get; set; }

        [XmlElement("TripId")]
        public Guid TripId { get; set; } = Guid.NewGuid();
    }
}