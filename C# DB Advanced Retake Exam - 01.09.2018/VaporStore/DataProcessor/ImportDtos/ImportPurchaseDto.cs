using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ImportDtos
{
    [XmlType("Purchase")]
    public class ImportPurchaseDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("Key")]
        [Required]
        [RegularExpression("^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string Key { get; set; }

        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        //<Purchase title = "Yu-Gi-Oh! Duel Links" >
        //< Type > Digital </ Type >
        //< Key > MMIB - 6IA6-L2WU</Key>
        //<Card>5208 8381 5687 8508</Card>
        //<Date>10/04/2016 17:40</Date>
        //</Purchase>
    }
}
