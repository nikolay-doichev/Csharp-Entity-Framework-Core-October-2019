using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDtos
{
    [XmlType("Purchase")]
    public class ExportPurchaseDto
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        
        public ExportGameDto Game { get; set; }

        //< Purchases >
        //    < Purchase >
        //      < Card > 7991 7779 5123 9211</Card>
        //      <Cvc>340</Cvc>
        //      <Date>2017-08-31 17:09</Date>
        //      
        //    </ Purchase >
        //    < Purchase >
        //      < Card > 7790 7962 4262 5606</Card>
        //      <Cvc>966</Cvc>
        //      <Date>2018-02-28 08:38</Date>
        //      <Game title = "Tom Clancy's Ghost Recon Wildlands" >
        //        < Genre > Action </ Genre >
        //        < Price > 59.99 </ Price >
        //      </ Game >
        //    </ Purchase >
        //  </ Purchases >
    }
}