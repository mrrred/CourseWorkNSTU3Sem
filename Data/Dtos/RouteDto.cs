using System.Xml.Serialization;

namespace CourseWork.Data.Dtos
{
    [XmlRoot("Route")]
    public class RouteDto
    {
        [XmlElement("RouteCode")]
        public string RouteCode { get; set; } = string.Empty;

        [XmlElement("StartPoint")]
        public string StartPoint { get; set; } = string.Empty;

        [XmlElement("EndPoint")]
        public string EndPoint { get; set; } = string.Empty;

        [XmlArray("IntermediatePoints")]
        [XmlArrayItem("Point")]
        public List<string> IntermediatePoints { get; set; } = new List<string>();

        [XmlElement("DepartureTime")]
        public string DepartureTimeString { get; set; } = string.Empty;

        [XmlArray("DepartureDays")]
        [XmlArrayItem("Day")]
        public List<string> DepartureDaysStrings { get; set; } = new List<string>();

        [XmlElement("TravelTime")]
        public string TravelTimeString { get; set; } = string.Empty;

        [XmlIgnore]
        public TimeSpan DepartureTime => TimeSpan.Parse(DepartureTimeString);

        [XmlIgnore]
        public TimeSpan TravelTime => TimeSpan.Parse(TravelTimeString);

        [XmlIgnore]
        public List<DayOfWeek> DepartureDays => DepartureDaysStrings.Select(d => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), d)).ToList();
    }
}