<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Offers.aspx.cs" Inherits="Snoopi.web.Offers" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentOffers" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgOffers" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="OfferCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp;       
        <br /><br />
          <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Bid" LocalizationId="ExportButton"></Localized:LinkButton>
        <br /><br /> 
        <Localized:Label LocalizationClass="Bid" LocalizationId="NoAutoRefresh" runat="server"></Localized:Label>
        <br /><br />
        <asp:DataGrid CssClass="items-list" ID="dgOffers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" 
            AllowPaging="false" EnableViewState="false" ClientIDMode="Static">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="SupplierName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate> <%# ((string)DataBinder.Eval(Container.DataItem, "SupplierName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Price" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "TotalPrice")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>


                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Gift" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Gift")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

               <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="IsOrder" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsOrder"))).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>



            </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>

        <script type="text/javascript">

            $(function () {
                $("#dpSearchCreateDateFrom,#dpSearchCreateDateTo").datepicker({ dateFormat: 'dd/mm/yy' }).val();

            });

    </script>

</asp:Content>
