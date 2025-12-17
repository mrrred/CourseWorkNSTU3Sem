using System.Xml.Serialization;

namespace CourseWork.Data.Dtos
{
    [XmlRoot("Bus")]
    public class BusDto
    {
        [XmlElement("GovernmentNumber")]
        public string GovernmentNumber { get; set; } = string.Empty;

        [XmlElement("BrandModel")]
        public string BrandModel { get; set; } = string.Empty;

        [XmlElement("Capacity")]
        public int Capacity { get; set; }

        [XmlElement("YearOfManufacture")]
        public int YearOfManufacture { get; set; }

        [XmlElement("YearOfOverhaul")]
        public int? YearOfOverhaul { get; set; }

        [XmlElement("MileageAtYearStart")]
        public int MileageAtYearStart { get; set; }

        [XmlElement("PhotoPath")]
        public string? PhotoPath { get; set; }
    }
}