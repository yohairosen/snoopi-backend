<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ServiceSuppliersReport.aspx.cs" Inherits="ServiceSuppliersReport" MasterPageFile="Template.master" ValidateRequest="false"%>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <Localized:Label ID="Label4" runat="server" BlockMode="false" AssociatedControlID="txtSearchSupplierId" LocalizationClass="Suppliers" LocalizationId="SearchSupplierId" />&nbsp;
        <asp:TextBox ID="txtSearchSupplierId" runat="server" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" AssociatedControlID="txtSearchName" LocalizationClass="Suppliers" LocalizationId="SearchNames" />&nbsp;
        <asp:TextBox ID="txtSearchName" runat="server" />
        <Localized:Label ID="Label2" runat="server" BlockMode="false" AssociatedControlID="txtSearchPhone" LocalizationClass="Suppliers" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />
        <Localized:Label ID="Label5" runat="server" BlockMode="false" AssociatedControlID="txtSearchCity" LocalizationClass="Suppliers" LocalizationId="SearchCity" />&nbsp;
        <asp:TextBox ID="txtSearchCity" runat="server" /><br />
        <br />
        <Localized:Label ID="Label10" runat="server" BlockMode="false" LocalizationClass="SupplierEvent" LocalizationId="FromDate" />
        <asp:TextBox ID="datepickerFrom" runat="server" ClientIDMode="Static"></asp:TextBox>
        <Localized:Label ID="Label11" runat="server" BlockMode="false" LocalizationClass="SupplierEvent" LocalizationId="ToDate" />
        <asp:TextBox ID="datepickerTo" runat="server" ClientIDMode="Static"></asp:TextBox>
        <Localized:LinkButton runat="server" ID="btnSearch" CssClass="button-02" OnClick="btnSearch_Click" LocalizationClass="Global" LocalizationId="Search" />
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="AppUsers" LocalizationId="ExportButton"></Localized:LinkButton><br />
    </asp:Panel>
    <br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgSuppliers" Value="0" />
        <Localized:Label ID="Label3" runat="server" BlockMode="false" LocalizationClass="Suppliers" LocalizationId="SuppliersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <div runat="server" id="filters">
<%--            <a href='/SuppliersReport.aspx'>
                <Localized:Label ID="Label7" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumOffers" />&nbsp;<asp:Label ID="lblSumOffers" runat="server" /></a><br />
            <a href='/SuppliersReport.aspx?Filter=Win'>
                <Localized:Label ID="Label8" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumWin" />&nbsp;<asp:Label ID="lblSumWin" runat="server" /></a><br />
            <a href='/SuppliersReport.aspx?Filter=NoWin'>
                <Localized:Label ID="Label9" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumNoWin" />&nbsp;<asp:Label ID="lblSumNoWin" runat="server" /></a><br />
            <a href='/SuppliersReport.aspx?Filter=ActiveBids'>
                <Localized:Label ID="Label6" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="SumActiveBids" />&nbsp;<asp:Label ID="lblSumActiveBids" runat="server" /></a><br />--%>
        </div>
        <asp:DataGrid CssClass="items-list" ID="dgSuppliers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" 
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="SupplierId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="SupplierId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "SupplierId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="BusinessName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "BusinessName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Phone")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="ContactName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ContactName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="ContactPhone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ContactPhone")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="CityName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CityName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="SupplierEvent" HeaderTextLocalizationId="ClickNum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "ClickNum")).ToString().ToHtml()%>
                        <%--<a href='/SupplierBids.aspx?Id=<%#((Int64)DataBinder.Eval(Container.DataItem, "SupplierId")).ToString()%>&Action=Offers&FromDate=<%#datepickerFrom.Text.Replace("/", "%2F")%>&ToDate=<%#datepickerTo.Text.Replace("/", "%2F")%>&BidId=<%#txtSearchBidId.Text%>'></a>--%>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="SupplierEvent" HeaderTextLocalizationId="ClickToCallNum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate>
                        <%# ((Int64)DataBinder.Eval(Container.DataItem, "ClickToCallNum")).ToString().ToHtml()%>
                        <%--<a href='/SupplierBids.aspx?Id=<%#((Int64)DataBinder.Eval(Container.DataItem, "SupplierId")).ToString()%>&Action=Win&FromDate=<%#datepickerFrom.Text.Replace("/", "%2F")%>&ToDate=<%#datepickerTo.Text.Replace("/", "%2F")%>&BidId=<%#txtSearchBidId.Text%>'></a>--%>

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
            $("#datepickerFrom,#datepickerTo").datepicker({ dateFormat: 'dd/mm/yy' }).val();

        });

    </script>
</asp:Content>
