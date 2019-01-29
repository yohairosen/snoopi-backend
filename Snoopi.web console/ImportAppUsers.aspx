<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="ImportAppUsers.aspx.cs" Inherits="Snoopi.web.ImportAppUsers" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">

    <Localized:Label ID="lblSelectImportFile" runat="Server" BlockMode="true" AssociatedControlID="fuImportFile" LocalizationClass="AppUsers" LocalizationId="SelectImportFileLabel"></Localized:Label>
    <asp:FileUpload ID="fuImportFile" runat="server" /> <br />
    <br />
    <Localized:LinkButton runat="server" ID="btnAcceptFile" OnClick="btnAcceptFile_Click" LocalizationClass="AppUsers" LocalizationId="AcceptFileButton"></Localized:LinkButton>

    <asp:PlaceHolder runat="server" ID="phErrors" Visible="false">
        <div class="message error">
            <span class="icon"></span>
            <span class="text"><asp:Label ID="lblErrors" runat="Server" /></span>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phAppUsersList" Visible="false">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgAppUsers" Value="0" />
        <Localized:Label ID="lblAppUserCount" runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="AppUsersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <Localized:Label ID="lblAppUserToImportCount" runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="AppUsersToImportCountLabel" />&nbsp;<asp:Label ID="lblTotalToImport" runat="server" /><br />
        <asp:DataGrid CssClass="items-list" ID="dgAppUsers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="false">
        
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row"/>
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Line" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Line") %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Email" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Email") %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Password" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Password")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="IsLocked" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "IsLocked")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="FirstName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "FirstName") %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="LastName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "LastName")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Phone")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Comments">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Comments")%></ItemTemplate>
                </Localized:TemplateColumn>
          </Columns>
        </asp:DataGrid>
        <br />
        <Localized:LinkButton runat="server" ID="btnImport" OnClick="btnImport_Click" LocalizationClass="AppUsers" LocalizationId="ImportAppUserButton"></Localized:LinkButton><br />        
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phImportResult" Visible="false">
        <div class="message" ID="divImportResult" runat="server">
            <span class="icon"></span>
            <span class="text"><asp:Label ID="lblImportResult" runat="Server" /></span><br />
        </div>
        <Localized:Label ID="lblImportedCountLabel" runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="AppUsersImportedCountLabel" />&nbsp;<asp:Label ID="lblTotalImported" runat="server" /><br />
    </asp:PlaceHolder>
</asp:Content>
