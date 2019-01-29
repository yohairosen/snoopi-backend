<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AppUserOrders.aspx.cs" Inherits="Snoopi.web.AppUserOrders" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Assembly="TimePicker" Namespace="MKB.TimePicker" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <%--        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" AssociatedControlID="txtSearchName" LocalizationClass="Orders" LocalizationId="SearchName" />&nbsp;
        <asp:TextBox ID="txtSearchName" runat="server" />
         <Localized:Label ID="Label2" runat="server" BlockMode="false" AssociatedControlID="txtSearchPhone" LocalizationClass="Orders" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />--%>
        <Localized:Label ID="Label7" runat="server" BlockMode="false" AssociatedControlID="txtSearchBid" LocalizationClass="Orders" LocalizationId="SearchBid" />&nbsp;
        <asp:TextBox ID="txtSearchBid" runat="server" />
        <Localized:Label ID="Label4" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="FromCreateDate" />
        <input type="text" id="datepickerFrom" readonly="readonly" runat="server" clientidmode="Static" />
        <Localized:Label ID="Label5" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="ToDate" />
        <input type="text" id="datepickerTo" runat="server" clientidmode="Static" readonly="readonly" />
        <Localized:LinkButton runat="server" ID="btnSearch"  OnClick="btnSearch_Click" CssClass="button-02" LocalizationClass="Global" LocalizationId="Search"/>
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="AppUsers" LocalizationId="ExportButton"></Localized:LinkButton><br />
    </asp:Panel>
    <br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgOrders" Value="0" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="OrdersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp; 
        <asp:DataGrid CssClass="items-list" ID="dgOrders" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="true" ClientIDMode="Static" DataKeyField="OrderId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
<%--                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="OrderId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "OrderId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>--%>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="BidId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "BidId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="BidEndDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "BidEndDate")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="LstProducts" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Repeater ID="Repeater2" runat="server" DataSource='<%# ((List<Snoopi.core.BL.BidProductUI>)DataBinder.Eval(Container.DataItem, "LstProduct"))%>'>
                            <ItemTemplate>
                                <%# ((string)DataBinder.Eval(Container.DataItem, "ProductAmount")).ToHtml()%> <%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToHtml()%>
                                <br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="Gift" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "Gift")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="DonationPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "DonationPrice")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="TotalPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "TotalPrice")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="PrecentDiscount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "PrecentDiscount")%>%</ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="PriceAfterDiscount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "PriceAfterDiscount")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="CampaignName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CampaignName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="IsSendReceived" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsSendReceived"))%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="OrderDate1" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "OrderDate")%></ItemTemplate>
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
