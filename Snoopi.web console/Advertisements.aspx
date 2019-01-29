<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Advertisements.aspx.cs" MasterPageFile="Template.master" Inherits="Advertisements" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgCompanies" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Advertisements" LocalizationId="AdsCountLabel" />&nbsp;
        <asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <asp:DataGrid ID="dgAds" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="CompanyId">
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Advertisements" HeaderTextLocalizationId="BusinessName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "CompanyName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Advertisements" HeaderTextLocalizationId="FromDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "FromDate")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Advertisements" HeaderTextLocalizationId="ToDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "ToDate")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Advertisements" HeaderTextLocalizationId="Bunner" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <span runat="server" id="lblSection">
                            <%# DataBinder.Eval(Container.DataItem, "BunnerId") != null ? ResourceManagerAccessor.GetText("Advertisements",System.Enum.GetName(typeof(Snoopi.core.DAL.Entities.BunnerType),DataBinder.Eval(Container.DataItem, "BunnerId")) ) : ""%>
                        </span>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Advertisements" HeaderTextLocalizationId="AdPic" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Image ID="ProductImage" CssClass="image-small" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("Banners",(string)DataBinder.Eval(Container.DataItem, "FilePath"), 0,64, 64) %>' />
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Advertisements" HeaderTextLocalizationId="CreateDate" ItemStyle-CssClass="ltr">
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
                            ButtonStyle="ButtonStyle2" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id").ToString()%>' OnClick="hlDelete_Click" OnClientClick="return confirmDelete()" LocalizationClass="Global" LocalizationId="ActionDelete" />
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
