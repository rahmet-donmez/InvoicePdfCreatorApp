<%@ Page Title="Satış Listesi"
    Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true" CodeBehind="Default.aspx.cs" Inherits="InvoicePdfCreatorApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <h1>Satış Listesi</h1>

        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered"
            OnRowDataBound="GridView1_RowDataBound" OnRowCommand="GridView1_RowCommand"
            BackColor="#f2f2f2" BorderColor="Gray"
            HeaderStyle-BackColor="#aed8a7">
            <Columns>



                <asp:BoundField DataField="CustomerName" HeaderText="Müşteri Adı" />
                <asp:BoundField DataField="CustomerPhone" HeaderText="Telefon" />
                <asp:BoundField DataField="CustomerAddress" HeaderText="Adres" />
                <asp:BoundField DataField="CustomerCity" HeaderText="Şehir" />
                <asp:BoundField DataField="CustomerTaxNumber" HeaderText="Vergi No" />
                <asp:BoundField DataField="CustomerTaxAdministration" HeaderText="Vergi Dairesi" />
                <asp:TemplateField HeaderText="Ürünler">
                    <ItemTemplate>
                        <asp:GridView ID="NestedGridView" runat="server" AutoGenerateColumns="false" CssClass="table table-striped">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Ürün Adı" />
                                <asp:BoundField DataField="Code" HeaderText="Stok Kodu" />
                                <asp:BoundField DataField="Vat" HeaderText="KDV Oranı" />
                                <asp:BoundField DataField="UnitPrice" HeaderText="Birim Fiyat" />
                            </Columns>
                        </asp:GridView>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="İşlem">
                    <ItemTemplate>
                        <asp:Button ID="btnGeneratePdf" runat="server" Text="Faturalandır"
                            CommandName="GeneratePdf" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-success" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </main>
</asp:Content>
