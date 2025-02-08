using InvoicePdfCreatorApp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace InvoicePdfCreatorApp.Controllers
{
    //Fatura işlemlerini yerine getiren controller
    [Route("api/invoices")]
    public class InvoiceController : ApiController
    {
        private static readonly HttpClient client = new HttpClient();

        //Fatura listesini dönen endpoint
        [HttpGet]
        public async Task<IHttpActionResult> GetInvoices()
        {
            string apiUrl = "http://istest.birfatura.net/api/test/SatislarGetir";
            string token = "GJqMl5x-qU6aEmgzArtZ61tvgoapfM-LEp1qjqjAasxMOjJMgODFcwUrTvALayn853Gr4bb6L0riY44SrShRnLYly1UWR5ssoUzX7hDwsSYVDtWR_mAMbIlJPzkyybg4_o51k6hQ-5Y-F7R5J1FBIC87SlWbPEhqMoOIvmtIM8SJJuzo0KkWXRbR2842DIkylCSwbo5dUSMo5mDEQ-pukag7NDFTUDihgLjePXgXNUQ";

            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsync(apiUrl, null);//api ye body kısmı boş olan istek atıldı
                response.EnsureSuccessStatusCode();
                //api den dönen cevap result değişkenine aktarıldı
                string result = await response.Content.ReadAsStringAsync();
                var invoiceList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Invoice>>(result);

                return Ok(invoiceList);
            }
            //satışların getirilmesi sırasında hata oluşması durumunu kontrol edildi
            catch (Exception ex)
            {
                return BadRequest("API çağrısında hata oluştu: " + ex.Message);
            }
        }
    }
}