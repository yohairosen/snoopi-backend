<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyBids.aspx.cs" Inherits="Snoopi.web.MyBids" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentBids" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgMyBids" Value="0" />
        <Localized:Label runat="server" BlockMode="false" ID="BidsCountLabel" LocalizationClass="Bid"  LocalizationId="BidsCountLabel"/>&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp; 
        
        <br />
          <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Bid" LocalizationId="ExportButton"></Localized:LinkButton>
        <br />
        <asp:DataGrid CssClass="items-list" ID="dgMyBids" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" 
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="BidId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>

                 <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="BidId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "BidId")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="StartDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "StartDate") != null ? ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToShortDateString() : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="StartTime" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "StartDate") != null ? ((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")).ToShortTimeString() : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="EndDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "EndDate") != null ? ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToShortDateString() : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="EndTime" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "EndDate") != null ? ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToShortTimeString() : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="OffersCount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><a href='/Offers.aspx?id=<%#((Int64)DataBinder.Eval(Container.DataItem, "BidId")).ToString()%>' ><%# ((Int32)DataBinder.Eval(Container.DataItem, "OfferNum")).ToString().ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>

                   <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Products" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Products"))%></ItemTemplate>
                </Localized:TemplateColumn>     
               
                 <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="OrderDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center " />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "OrderDate")%></span></ItemTemplate>
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
