<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdCompanies.aspx.cs" Inherits="AdCompanies" MasterPageFile="Template.master" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label runat="server" BlockMode="false" AssociatedControlID="txtSearchName" LocalizationClass="Companies" LocalizationId="SearchName" />&nbsp;
        <asp:TextBox ID="txtSearchName" runat="server" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" AssociatedControlID="txtSearchPhone" LocalizationClass="Companies" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />
        <Localized:LinkButton runat="server" ID="btnSearch" CssClass="button-02" OnClick="btnSearch_Click" LocalizationClass="Global" LocalizationId="Search" />
    </asp:Panel>
    <br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgCompanies" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Companies" LocalizationId="CompaniesCountLabel" />&nbsp;
        <asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <asp:DataGrid ID="dgCompanies" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="CompanyId">
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Companies" HeaderTextLocalizationId="BusinessName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "BusinessName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Companies" HeaderTextLocalizationId="Email" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Email")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Companies" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Phone")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Companies" HeaderTextLocalizationId="ContactName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ContactName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Companies" HeaderTextLocalizationId="ContactPhone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ContactPhone")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Companies" HeaderTextLocalizationId="CreateDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "CreatedDate")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:HyperLink runat="server" ID="hlEdit"
                            ButtonStyle="ButtonStyle2"
                            NavigateUrl='<%# FormatEditUrl(Container.DataItem) %>'
                            LocalizationClass="Global" LocalizationId="ActionEdit" />
                        <Localized:Button runat="server" ID="hlDelete"
                            ButtonStyle="ButtonStyle2" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompanyId").ToString()%>' OnClick="hlDelete_Click" OnClientClick="return confirmDelete()" LocalizationClass="Global" LocalizationId="ActionDelete" />
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
        function confirmDelete() {
            return confirm('<%= AdvertisementsStrings.GetText("ConfirmDelete")%>');
        }
    </script>

</asp:Content>
