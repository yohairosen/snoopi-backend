<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SupplierProducts.aspx.cs" Inherits="Snoopi.web.SupplierProducts" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.core.DAL" %>

<asp:Content ID="ContentSupplierProducts" ContentPlaceHolderID="cphContent" runat="Server">



    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgSupplierProducts" Value="0" />
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Products" LocalizationId="ExportButton"></Localized:LinkButton><br />
        <Localized:Label ID="Label2" runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="ProductsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <asp:DataGrid CssClass="items-list" ID="dgSupplierProducts" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            ClientIDMode="Static"
            AllowPaging="true" PageSize="30" EnableViewState="false"
            DataKeyField="ProductId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>

                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductCode" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ProductCode") %></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Amount" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Amount")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductPrice" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ProductPrice")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Gift" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Gift")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

            </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <Localized:Label ID="Label1" runat="Server" LocalizationClass="Products" LocalizationId="MessageNoDataHere"></Localized:Label></span>
        </div>
    </asp:PlaceHolder>

</asp:Content>
