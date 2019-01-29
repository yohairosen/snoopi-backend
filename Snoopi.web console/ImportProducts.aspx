<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="ImportProducts.aspx.cs" Inherits="Snoopi.web.ImportProducts" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.core.DAL" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">
    <Localized:Label runat="server" LocalizationClass="Products" LocalizationId="ImportProductsDesc"></Localized:Label>
    <Localized:Label ID="lblSelectImportFile" runat="Server" BlockMode="true" AssociatedControlID="fuImportFile" LocalizationClass="Products" LocalizationId="SelectImportFileLabel"></Localized:Label>
    <asp:FileUpload ID="fuImportFile" runat="server" /> <br />
    <br />
    <Localized:LinkButton runat="server" ID="btnAcceptFile" OnClick="btnAcceptFile_Click" LocalizationClass="Products" LocalizationId="AcceptFileButton"></Localized:LinkButton>

    <asp:PlaceHolder runat="server" ID="phErrors" Visible="false">
        <div class="message error">
            <span class="icon"></span>
            <span class="text"><asp:Label ID="lblErrors" runat="Server" /></span>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phProductsList" Visible="false">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_Products" Value="0" />
        <Localized:Label ID="lblProductsCount" runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="ProductsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <Localized:Label ID="lblProductsToImportCount" runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="ProductsToImportCountLabel" />&nbsp;<asp:Label ID="lblTotalToImport" runat="server" /><br />
        <asp:DataGrid CssClass="items-list" ID="dgProducts" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="false"
            AllowPaging="false" PageSize="30" EnableViewState="false">
        
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row"/>
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Line" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Line") %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductCode" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "ProductCode") is DBNull ? "" :(string)DataBinder.Eval(Container.DataItem, "ProductCode")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductNum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ProductNum") %></ItemTemplate>
                </Localized:TemplateColumn>  
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Amount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Amount")%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="RecomendedPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "RecomendedPrice") %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Description" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Description")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="AnimalType" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "AnimalType")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Category" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "CategoryId")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="SubCategory" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "SubCategoryId")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Comments">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Comments")%></ItemTemplate>
                </Localized:TemplateColumn>
          </Columns>
        </asp:DataGrid>
        <br />
        <Localized:LinkButton runat="server" ID="btnImport" OnClick="btnImport_Click" LocalizationClass="Products" LocalizationId="ImportProductsButton"></Localized:LinkButton><br />        
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phImportResult" Visible="false">
        <div class="message" ID="divImportResult" runat="server">
            <span class="icon"></span>
            <span class="text"><asp:Label ID="lblImportResult" runat="Server" /></span><br />
        </div>
        <Localized:Label ID="lblImportedCountLabel" runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="ProductsImportedCountLabel" />&nbsp;<asp:Label ID="lblTotalImported" runat="server" /><br />
    </asp:PlaceHolder>
</asp:Content>
