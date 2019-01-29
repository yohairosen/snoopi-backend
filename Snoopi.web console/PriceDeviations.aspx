    <%@ Page Language="C#" AutoEventWireup="true" CodeFile="PriceDeviations.aspx.cs" Inherits="Snoopi.web.PriceDeviations"  MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentPriceDeviations" ContentPlaceHolderID="cphContent" runat="Server">
      
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgPriceDeviations" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Prices" LocalizationId="PriceDeviationCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp;       
       <br /><br /> 
        <Localized:Label LocalizationClass="Bid" LocalizationId="NoAutoRefresh" runat="server"></Localized:Label>
        <br /><br />
        <asp:DataGrid CssClass="items-list" ID="dgPriceDeviations" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" 
            AllowPaging="false" EnableViewState="false" ClientIDMode="Static"  DataKeyNames="SupplierId, ProductId">

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

                 <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Prices" HeaderTextLocalizationId="RecommendedPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "RecommendedPrice")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                 <Localized:TemplateColumn LocalizationClass="Prices" HeaderTextLocalizationId="ActualPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "ActualPrice")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                  <Localized:TemplateColumn LocalizationClass="Prices" HeaderTextLocalizationId="DeviationPercentage" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "DeviationPercentage")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                 <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:LinkButton runat="server" ID="hlEdit"
                            ButtonStyle="ButtonStyle2"
                            OnClick = "approveDeviation"
                            CommandArgument='<%# Eval("SupplierId") + "," + Eval("ProductId")%>'
                            LocalizationClass="Prices" LocalizationId="ApproveDeviation" />
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
