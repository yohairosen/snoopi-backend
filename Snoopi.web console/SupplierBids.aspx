<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SupplierBids.aspx.cs" Inherits="Snoopi.web.SupplierBids" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentBids" ContentPlaceHolderID="cphContent" runat="Server">
   <%-- <asp:Panel runat="server" DefaultButton="">
         <input type="text" id="datepickerFrom" readonly="readonly" runat="server" clientidmode="Static" />
        <input type="text" id="datepickerTo" runat="server" clientidmode="Static" readonly="readonly" />

    </asp:Panel>--%>
    <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Bid" LocalizationId="ExportButton"></Localized:LinkButton>
    <br />
    <br />
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgBids" Value="0" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="BidsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp; 
        <br />

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
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "BidId")%></ItemTemplate>
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
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="TotalPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "Price")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="Products" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# ((List<Snoopi.core.BL.BidProductUI>)DataBinder.Eval(Container.DataItem, "LstProduct"))%>'>
                            <ItemTemplate>
                                <%#Eval("ProductName") +","%>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="OrderDate1" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (DateTime?)DataBinder.Eval(Container.DataItem, "OrderDate")%></ItemTemplate>
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
