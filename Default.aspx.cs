using InvoicePdfCreatorApp.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InvoicePdfCreatorApp
{
    public partial class _Default : Page
    {
        private static readonly HttpClient client = new HttpClient();
        protected List<Invoice> Invoices = new List<Invoice>();
        protected List<InvoiceProduct> Products = new List<InvoiceProduct>();

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await GetInvoices();
            }
        }

        public async Task GetInvoices()
        {
            string apiUrl = "https://localhost:44351/api/invoices"; // kendi controllerımıza istek atıyoruz. derlendiği portu yazdık

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string jsonData = await response.Content.ReadAsStringAsync();

            Invoices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Invoice>>(jsonData);//dönen datayı kendi invoice datamıza dönüştürüyoruz

            GridView1.DataSource = Invoices;
            GridView1.DataBind();
        }

        //ürünler
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Invoice invoice = (Invoice)e.Row.DataItem;
                GridView nestedGridView = (GridView)e.Row.FindControl("NestedGridView");

                if (nestedGridView != null && invoice.Products != null)
                {

                    nestedGridView.DataSource = invoice.Products;
                    nestedGridView.DataBind();
                }
            }
        }


        public async Task<Invoice> GetInvoice(int id)
        {
            string apiUrl = "https://localhost:44351/api/invoices";

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string jsonData = await response.Content.ReadAsStringAsync();
            Invoices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Invoice>>(jsonData);
            Invoice selectedInvoice = Invoices.FirstOrDefault(inv => inv.Id == id);

            return selectedInvoice;
        }
        protected async void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GeneratePdf")
            {
                int invoiceId = Convert.ToInt32(e.CommandArgument);

                Invoice selectedInvoice = await GetInvoice(invoiceId);

                if (selectedInvoice != null)
                {

                    GeneratePdfForInvoice(selectedInvoice);
                }
                else
                {
                    Response.Write("<script>alert('Fatura bulunamadı!');</script>");
                }
            }
        }

        private void GeneratePdfForInvoice(Invoice invoice)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //belgeyi oluştur
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                //  Türkçe karakterleri desteklemesi amacıyla eklenmiştir
                BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(baseFont, 16);

                document.Add(new Paragraph("#Fatura Numarası: " + invoice.Id, titleFont));
                document.Add(new Paragraph("\n"));

                // Müşteri Bilgileri
                PdfPTable customerInfoTable = new PdfPTable(1);
                customerInfoTable.WidthPercentage = 100;
                customerInfoTable.AddCell(new PdfPCell(new Phrase("Müşteri Bilgileri\n", titleFont))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = Rectangle.NO_BORDER
                });

                Font customerFont = new Font(baseFont, 12);
                PdfPCell customerCell = new PdfPCell();
                customerCell.Border = Rectangle.BOX;
                customerCell.Padding = 10;
                customerCell.Phrase = new Phrase(
                    "Adı: " + invoice.CustomerName + "\n" +
                    "Telefon: " + invoice.CustomerPhone + "\n" +
                    "Adres: " + invoice.CustomerAddress + "\n" +
                    "Şehir: " + invoice.CustomerCity + "\n" +
                    "Vergi: " + invoice.CustomerTaxNumber + " - " + invoice.CustomerTaxAdministration,
                    customerFont);

                customerInfoTable.AddCell(customerCell);
                document.Add(customerInfoTable);
                document.Add(new Paragraph("\n"));

                // Ürün bilgileri
                if (invoice.Products != null && invoice.Products.Count > 0)
                {
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 3, 2, 2, 2 });

                    Font headerFont = new Font(baseFont, 12);
                    table.AddCell(new PdfPCell(new Phrase("Ürün Adı", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Stok Kodu", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("KDV Oranı", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Birim Fiyat", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    Font rowFont = new Font(baseFont, 10);
                    foreach (var product in invoice.Products)
                    {
                        table.AddCell(new PdfPCell(new Phrase(product.Name, rowFont)));
                        table.AddCell(new PdfPCell(new Phrase(product.Code, rowFont)));
                        table.AddCell(new PdfPCell(new Phrase(product.Vat.ToString() + "%", rowFont)));
                        table.AddCell(new PdfPCell(new Phrase(product.UnitPrice.ToString("C"), rowFont)));
                    }

                    document.Add(new Paragraph("Ürünler", titleFont));
                    document.Add(new Paragraph("\n"));
                    document.Add(table);

                    // Toplam tutar
                    decimal totalAmount = invoice.Products.Sum(p => p.UnitPrice); 
                    Font totalFont = new Font(baseFont, 12);
                    PdfPTable totalTable = new PdfPTable(1);
                    totalTable.WidthPercentage = 100;

                    PdfPCell totalCell = new PdfPCell(new Phrase("Toplam Tutar: " + totalAmount.ToString("C"), totalFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    totalTable.AddCell(totalCell);
                    document.Add(totalTable);
                }
                else
                {
                    document.Add(new Paragraph("Bu faturaya ait ürün bulunmamaktadır.", new Font(baseFont, 12, Font.NORMAL, BaseColor.RED)));
                }

                document.Close();
                writer.Close();

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=Fatura.pdf");
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            }
        }

    }
}