using Newtonsoft.Json;

namespace InvoicePdfCreatorApp.Models
{
    public class InvoiceProduct
    {
        [JsonProperty("UrunID")]
        public int Id { get; set; }

        [JsonProperty("UrunAdi")]
        public string Name { get; set; }

        [JsonProperty("StokKodu")]
        public string Code { get; set; }

        [JsonProperty("KDVOrani")]
        public decimal Vat { get; set; }

        [JsonProperty("KDVDahilBirimFiyati")]
        public decimal UnitPrice { get; set; }
    }
}
