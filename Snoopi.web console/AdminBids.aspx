<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminBids.aspx.cs" Inherits=" Snoopi.web.AdminBids"  MasterPageFile="Template.master" ValidateRequest="false"%>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentBids" ContentPlaceHolderID="cphContent" runat="Server">
           <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label ID="Label4" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="BidNumber" />&nbsp;
        <asp:TextBox ID="txtBidNumber" runat="server" />
        <Localized:Label ID="Label5" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SerchCustomerId" />&nbsp;
        <asp:TextBox ID="txtCustomerId" runat="server" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="CitySearch" />&nbsp;
        <asp:TextBox ID="txtCityName" runat="server" />
        <br />
        <br />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />
        <Localized:Label ID="Label2" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="FromDate" />&nbsp;
        <input type="text" id="dpSearchCreateDateFrom" runat="server" clientidmode="Static" />
        <Localized:Label ID="Label3" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="ToDate" />&nbsp;
        <input type="text" id="dpSearchCreateDateTo" runat="server" clientidmode="Static" />
        <br />
        <br />
        <Localized:LinkButton runat="server" ID="btnSearch" OnClick="btnSearch_Click" CssClass="button-02" LocalizationClass="Bid" LocalizationId="SearchButton" />
        <%--Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Bid" LocalizationId="ExportButton"></Localized:LinkButton>--%>
    </asp:Panel>

    <br />
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgBids" Value="0" />
        <Localized:Label runat="server" BlockMode="false" ID="BidsCountLabel" LocalizationClass="Bid" LocalizationId="BidsCountLabel" />&nbsp;<asp:Label ID="lblTotal1" runat="server" />&nbsp; 
       <div runat="server" id="linksSearch">
           <a href='/Bids.aspx'>
               <Localized:Label runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumCountBids" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp;</a>
           <br />
           <%--<a href='/Bids.aspx?Filter=ActiveBids'>
               <Localized:Label ID="Label6" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumActiveBids" />&nbsp;<asp:Label ID="lblSumActiveBids" runat="server" />&nbsp; </a>
           <br />
           <a href='/Bids.aspx?Filter=BidsWithOffers'>
               <Localized:Label ID="Label8" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumAbandonedBidsWithOffers" />&nbsp;<asp:Label ID="lblSumAbandonedBidsWithOffers" runat="server" />&nbsp; </a>
           <br />
           <a href='/Bids.aspx?Filter=BidsWithOutOffers'>
               <Localized:Label ID="Label7" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumAbandonedBidsWithOutOffers" />&nbsp;<asp:Label ID="lblSumAbandonedBidsWithOutOffers" runat="server" />&nbsp; </a>
           <br />
           <a href='/Bids.aspx?Filter=PurchaseBids'>
               <Localized:Label ID="Label10" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumPurchaseBids" />&nbsp;<asp:Label ID="lblSumPurchaseBids" runat="server" />&nbsp; </a>
           <br />--%>
       </div>
        <asp:DataGrid CssClass="items-list" ID="dgBids" runat="server" UseAccessibleHeader="true"
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
                <%--<Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="EndDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "EndDate") != null ? ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToShortDateString() : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="EndTime" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "EndDate") != null ? ((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")).ToShortTimeString() : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>--%>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="CustomerId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate>
                        <a href='Customers.aspx?Id=<%# ((Int64)DataBinder.Eval(Container.DataItem, "CustomerId")).ToString()%>&Type=<%# ((int)((Snoopi.core.BL.CustomerType)DataBinder.Eval(Container.DataItem, "CustomerType"))).ToString()  %>'>
                            <%# (((Snoopi.core.BL.CustomerType)DataBinder.Eval(Container.DataItem, "CustomerType") == Snoopi.core.BL.CustomerType.Temp) ? "000" : "" ) +
                        ((Int64)DataBinder.Eval(Container.DataItem, "CustomerId")).ToString().ToHtml() %></a>
                    </ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="CustomerName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%#(Snoopi.core.BL.CustomerType)(DataBinder.Eval(Container.DataItem, "CustomerType")) == Snoopi.core.BL.CustomerType.Temp ? BidString.GetText("Temp") :  ((string)DataBinder.Eval(Container.DataItem, "CustomerName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>


                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Phone")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="CityName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "City")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <%--<Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="OffersCount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><a href='/Offers.aspx?id=<%#((Int64)DataBinder.Eval(Container.DataItem, "BidId")).ToString()%>'><%# ((Int32)DataBinder.Eval(Container.DataItem, "OfferNum")).ToString().ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>


                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="SupplierName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "SupplierName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>--%>


                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Price" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "Price")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Products" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Products"))%></ItemTemplate>
                </Localized:TemplateColumn>

              <%--  <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="IsActive" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" /> 
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsActive"))%></ItemTemplate>
                </Localized:TemplateColumn>--%>

                  <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:Button runat="server" ID="hlEdit"
                            ButtonStyle="ButtonStyle2"
                            OnClick = "approveDeal"
                            CommandArgument='<%# Eval("BidId")%>'
                            LocalizationClass="Bid" LocalizationId="ApproveDeal" />
                        <Localized:Button runat="server" ID="bt_reject"
                            ButtonStyle="ButtonStyle2"
                            OnClick = "rejectDeal"
                            CommandArgument='<%# Eval("BidId")%>'
                            LocalizationClass="Bid" LocalizationId="RejectDeal" />
                        <br />
                          <Localized:Button runat="server" ID="bt_resend"
                            ButtonStyle="ButtonStyle2"
                            OnClick = "resendDeal"
                            CommandArgument='<%# Eval("BidId")%>'
                            LocalizationClass="Bid" LocalizationId="ResendDeal" />

                       </ItemTemplate>
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