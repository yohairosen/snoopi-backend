<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Snoopi.web.Users" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">

    <asp:Panel runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label runat="server" BlockMode="false" AssociatedControlID="txtSearch" LocalizationClass="Users" LocalizationId="SearchLabel" />&nbsp;
        <asp:TextBox ID="txtSearch" runat="server" />
        <asp:Button runat="server" ID="btnSearch" style="display:none" OnClick="btnSearch_Click" />
    </asp:Panel><br />
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgUsers" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Users" LocalizationId="UsersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <asp:DataGrid CssClass="items-list" ID="dgUsers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="false">
        
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row"/>
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Users" HeaderTextLocalizationId="Email" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><a href="mailto:<%# ((string)DataBinder.Eval(Container.DataItem, "Email")).ToHtml()%>"><%# ((string)DataBinder.Eval(Container.DataItem, "Email")).ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Users" HeaderTextLocalizationId="IsLocked">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsLocked"))%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Users" HeaderTextLocalizationId="LastLogin">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center" />
                    <ItemTemplate><span class="dgDateManager"><%# ((DateTime)DataBinder.Eval(Container.DataItem, "LastLogin")).ToString(@"yyyy-MM-ddTHH:mm:ssZ")%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:HyperLink runat="server" ID="hlEdit" 
                                   ButtonStyle="ButtonStyle2"
                                   NavigateUrl='<%# FormatEditUrl(Container.DataItem) %>' 
                                   LocalizationClass="Global" LocalizationId="ActionEdit" />
                        <Localized:HyperLink runat="server" ID="hlDelete" 
                                   ButtonStyle="ButtonStyle2"
                                   NavigateUrl='<%# FormatDeleteUrl(Container.DataItem) %>' 
                                   LocalizationClass="Global" LocalizationId="ActionDelete" />
                    </ItemTemplate>
                </Localized:TemplateColumn>
          </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text"><asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>

</asp:Content>
