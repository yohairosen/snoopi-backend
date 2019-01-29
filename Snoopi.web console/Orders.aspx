<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Orders.aspx.cs" Inherits="Snoopi.web.Orders" MasterPageFile="Template.master" ValidateRequest="false" %>

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
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">

        <%--        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" AssociatedControlID="txtSearchName" LocalizationClass="Orders" LocalizationId="SearchName" />&nbsp;
        <asp:TextBox ID="txtSearchName" runat="server" />
         <Localized:Label ID="Label2" runat="server" BlockMode="false" AssociatedControlID="txtSearchPhone" LocalizationClass="Orders" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />--%>
        <Localized:Label ID="lblSearchBusiness" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="SearchBusiness" />&nbsp;
         <asp:ListBox ID="ddlSuppliers" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox>&nbsp;&nbsp;
         <Localized:Label ID="Label6" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="IsSendReceived" />&nbsp;
         <asp:ListBox ID="ddlIsSendReceived" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox>&nbsp;&nbsp;
        <Localized:Label ID="Label8" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="SearchStatus" />&nbsp;
         <asp:ListBox ID="ddlStatus" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox>&nbsp;&nbsp;
        <Localized:Label ID="Label9" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="SearchPaymentStatus" />&nbsp;
         <asp:ListBox ID="ddlPaymentStatus" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox>&nbsp;&nbsp;
        <br /><br />
         <Localized:Label ID="Label7" runat="server" BlockMode="false" AssociatedControlID="txtSearchBid" LocalizationClass="Orders" LocalizationId="SearchBid" />&nbsp;
        <asp:TextBox ID="txtSearchBid" runat="server" />      
        <Localized:Label ID="Label4" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="FromDate" />
        <input type="text" id="datepickerFrom" readonly="readonly" runat="server" clientidmode="Static" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="From" />
        <cc1:TimeSelector ID="TimeSelectorFrom" runat="server" SelectedTimeFormat="TwentyFour" DisplaySeconds="false" DisplayButtons="false" CssClass="timePicker"></cc1:TimeSelector>
        <Localized:Label ID="Label5" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="ToDate" />
        <input type="text" id="datepickerTo" runat="server" clientidmode="Static" readonly="readonly" />
        <Localized:Label ID="Label2" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="To" />
        <cc1:TimeSelector ID="TimeSelectorTo" runat="server" SelectedTimeFormat="TwentyFour" DisplaySeconds="false" DisplayButtons="false" CssClass="timePicker"></cc1:TimeSelector>
        <Localized:LinkButton runat="server" ID="btnSearch" CssClass="button-02" OnClick="btnSearch_Click" LocalizationClass="Global" LocalizationId="Search" />
       <div style="float:left;margin:10px"> <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click"  LocalizationClass="AppUsers" LocalizationId="ExportButton"></Localized:LinkButton></div>
       <div style="float:left;margin:10px"> <Localized:LinkButton runat="server" ID="btnExportForCRM" CssClass="button-02" OnClick="btnExportForCRM_Click"  LocalizationClass="AppUsers" LocalizationId="ExportForCRMButton"></Localized:LinkButton></div>

    </asp:Panel>
    <br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgOrders" Value="0" />
        <Localized:Label ID="Label3" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="OrdersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <asp:DataGrid CssClass="items-list" ID="dgOrders" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" OnItemCommand="dgOrders_ItemCommand"
            AllowPaging="true" PageSize="30" EnableViewState="true" ClientIDMode="Static" DataKeyField="OrderId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="OrderId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "OrderId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="SupplierId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "SupplierId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="SupplierName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "SupplierName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
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
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Precent" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Precent")%>%</ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="PaymentForSupplier" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "PaymentForSupplier")%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="SuppliedDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "SuppliedDate") == DateTime.MinValue ? "" : DataBinder.Eval(Container.DataItem, "SuppliedDate")  %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="IsSendReceived" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsSendReceived"))%></ItemTemplate>
                </Localized:TemplateColumn>
<%--                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="TransactionResponseCode" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%#DataBinder.Eval(Container.DataItem, "TransactionResponseCode")%></ItemTemplate>
                </Localized:TemplateColumn>--%>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="OrderDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "OrderDate")%></ItemTemplate>
                </Localized:TemplateColumn>
<%--                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="TransactionStatus" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:DropDownList runat="server" ID="ddlTransactionStatus" OnSelectedIndexChanged="ddlTransactionStatus_SelectedIndexChanged" AutoPostBack="true" EnableViewState="true"></asp:DropDownList>
                    </ItemTemplate>
                </Localized:TemplateColumn>--%>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="PaySupplierStatus" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:DropDownList runat="server" ID="ddlPaySupplierStatus" OnSelectedIndexChanged="ddlPaySupplierStatus_SelectedIndexChanged" AutoPostBack="true" EnableViewState="true"></asp:DropDownList>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="ChangeStatus" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:CheckBox runat="server" ID="chkStatusPayed" OnCheckedChanged="chkStatusPayed_CheckedChanged"></asp:CheckBox>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Orders" HeaderTextLocalizationId="Remarks" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:TextBox runat="server"  ID="txtRemarks" Text='<%#Eval("Remarks")%>' OnTextChanged="txtRemarks_TextChanged"></asp:TextBox>
                    </ItemTemplate>
                </Localized:TemplateColumn>

            </Columns>
        </asp:DataGrid>
        <br />
        <Localized:Label ID="Label10" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="PaySupplierLabel" />&nbsp;<asp:Label ID="lblSum" runat="server" ClientIDMode="Static" /><br />
        <br />
        <Localized:LinkButton runat="server" ID="lbUpdate" CssClass="button-02" OnClick="lbUpdate_Click" LocalizationClass="Orders" LocalizationId="UpdateButton"></Localized:LinkButton>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>

     <asp:Panel ID="pnlSaveStatus" runat="server" Visible="false" Style="position: absolute; opacity: 0.5; background-color: black; height: 100%; width: 100%; display: block; top: 0px; left: 0px;"></asp:Panel>
    <asp:Panel ID="pnlSave" runat="server" Visible="false" Style="position: absolute; top: 200px; background-color: aliceblue; padding: 50px; border: 10px solid; left: 50%;">
        <Localized:Label ID="lblSaveNote" runat="Server" LocalizationClass="Orders" LocalizationId="lblSaveNote"></Localized:Label>
        <Localized:Button runat="server" ID="btnSave" CssClass="button-02" OnClick="lbUpdate_Click" LocalizationClass="Orders" LocalizationId="SaveButton"></Localized:Button>
        <Localized:Button runat="server" ID="btnDontSave" CssClass="button-02" OnClick="btnDontSave_Click" LocalizationClass="Orders" LocalizationId="DontSaveButton"></Localized:Button>
    </asp:Panel>

    <script type="text/javascript">

        $(function () {
            $("#datepickerFrom,#datepickerTo").datepicker({ dateFormat: 'dd/mm/yy' }).val();

        });

    </script>
</asp:Content>
