<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Customers.aspx.cs" Inherits="Snoopi.web.Customers" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentCustomers" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgCustomers" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Bid" LocalizationId="bidsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp; 

        
        <br />
        <asp:DataGrid CssClass="items-list" ID="dgCustomers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" 
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="CustomerId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="CustomerId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate> <%# ((string)DataBinder.Eval(Container.DataItem, "CustomerId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="CustomerName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (Snoopi.core.BL.CustomerType)int.Parse(Request.QueryString["Type"]) == Snoopi.core.BL.CustomerType.Temp ? BidString.GetText("Temp") :  ((string)DataBinder.Eval(Container.DataItem, "CustomerName")).ToHtml()%></ItemTemplate>
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

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="BidCount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><a href="<%# ((string)DataBinder.Eval(Container.DataItem, "CustomerLink")).ToString()%>&BidType=<%# (int)Snoopi.core.BL.BidType.BidCount %>"  ><%# ((Int64)DataBinder.Eval(Container.DataItem, "BidCount")).ToString().ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="BidAbandoned" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><a href="<%# ((string)DataBinder.Eval(Container.DataItem, "CustomerLink")).ToString()%>&BidType=<%# (int)Snoopi.core.BL.BidType.BidAbandoned %>"  ><%# ((Int64)DataBinder.Eval(Container.DataItem, "BidAbandoned")).ToString().ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="BidPurchase" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><a href="<%# ((string)DataBinder.Eval(Container.DataItem, "CustomerLink")).ToString()%>&BidType=<%# (int)Snoopi.core.BL.BidType.BidPurchase %>"  ><%# ((Int64)DataBinder.Eval(Container.DataItem, "BidPurchase")).ToString().ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Bid" HeaderTextLocalizationId="BidActive" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><a href="<%# ((string)DataBinder.Eval(Container.DataItem, "CustomerLink")).ToString()%>&BidType=<%# (int)Snoopi.core.BL.BidType.BidActive %>" ><%# ((Int64)DataBinder.Eval(Container.DataItem, "BidActive")).ToString().ToHtml()%></a></ItemTemplate>
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
