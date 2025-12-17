using System.Xml.Serialization;

namespace CourseWork.Data.Dtos
{
    [XmlRoot("Driver")]
    public class DriverDto
    {
        [XmlElement("FullName")]
        public string FullName { get; set; } = string.Empty;

        [XmlElement("PersonnelNumber")]
        public string PersonnelNumber { get; set; } = string.Empty;

        [XmlElement("BirthYear")]
        public int BirthYear { get; set; }

        [XmlElement("ExperienceYears")]
        public int ExperienceYears { get; set; }

        [XmlElement("LicenseCategory")]
        public string LicenseCategory { get; set; } = string.Empty;

        [XmlElement("DriverClass")]
        public int DriverClass { get; set; }
    }
}