<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Filters.aspx.cs" Inherits="Snoopi.web.Filters" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.core.DAL" %>
<%@ Import Namespace="Snoopi.core.BL" %>

<asp:Content ID="ContentFilters" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgFilters" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Filters" LocalizationId="FiltersCountLabel"></Localized:Label>&nbsp;<Localized:Label ID="lblTotal" runat="server" BlockMode="false"></Localized:Label>
        <br /><br />
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Filters" LocalizationId="ExportButton"></Localized:LinkButton>
        <br /><br />
        <asp:DataGrid CssClass="items-list" ID="dgFilters" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="false"
            OnItemCommand="dgFilters_ItemCommand" ClientIDMode="Static"
            AllowPaging="false" PageSize="30" EnableViewState="false"
            DataKeyField="FilterId"
            AllowSorting="True">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Filters" HeaderTextLocalizationId="FilterName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "FilterName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Filters" HeaderTextLocalizationId="SubFilterName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (ProductController.ConvertSubFilterListToString((List<SubFilterUI>)DataBinder.Eval(Container.DataItem, "LstSubFilter"))).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
              

                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap va-middle" />
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbEdit" runat="server"
                            CausesValidation="False"
                            CommandName="Edit"
                            LocalizationClass="Global" LocalizationId="ActionEdit"
                            ButtonStyle="ButtonStyle2" />
                        <Localized:LinkButton ID="lbDelete" runat="server"
                            CausesValidation="False"
                            CommandName="Delete"
                            LocalizationClass="Global" LocalizationId="ActionDelete"
                            ButtonStyle="ButtonStyle2" />
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

</asp:Content>
