using Newtonsoft.Json;
using System.Collections.Generic;

namespace InvoicePdfCreatorApp.Models
{
    public class Invoice
    {
        [JsonProperty("FaturaID")]
        public int Id { get; set; }

        [JsonProperty("MusteriAdi")]
        public string CustomerName { get; set; }

        [JsonProperty("MusteriTel")]
        public string CustomerPhone { get; set; }

        [JsonProperty("MusteriAdresi")]
        public string CustomerAddress { get; set; }

        [JsonProperty("MusteriSehir")]
        public string CustomerCity { get; set; }

        [JsonProperty("MusteriTCVKN")]
        public string CustomerTaxNumber { get; set; }

        [JsonProperty("MusteriVergiDairesi")]
        public string CustomerTaxAdministration { get; set; }

        [JsonProperty("SatilanUrunler")]
        public List<InvoiceProduct> Products { get; set; }
    }
}
