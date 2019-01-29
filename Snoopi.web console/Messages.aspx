<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Messages.aspx.cs" Inherits="Snoopi.web.Messages" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">

    <%--<asp:Panel runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label runat="server" BlockMode="false" AssociatedControlID="txtSearch" LocalizationClass="Messages" LocalizationId="SearchLabel" />&nbsp;
        <asp:TextBox ID="txtSearch" runat="server" />
        <asp:Button runat="server" ID="btnSearch" style="display:none" OnClick="btnSearch_Click" />
    </asp:Panel>--%><br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgMessages" Value="0" />
<%--       <Localized:Label runat="server" BlockMode="false" LocalizationClass="Messages" LocalizationId="MessagesCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static"/><br />--%>
         <asp:DataGrid CssClass="items-list" ID="dgMessages" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="false"
            AllowPaging="false" PageSize="30" EnableViewState="false" ClientIDMode="Static">
        
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row"/>
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Messages" HeaderTextLocalizationId="MessageId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "MessageId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Messages" HeaderTextLocalizationId="Description" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Description")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>

         
        
                <Localized:TemplateColumn LocalizationClass="Messages" HeaderTextLocalizationId="SendingDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((DateTime)DataBinder.Eval(Container.DataItem, "SendingDate")).ToString()%></ItemTemplate>
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
