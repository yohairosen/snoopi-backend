<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EmailTemplates.aspx.cs" Inherits="Snoopi.web.EditEmailTemplates" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgTemplates" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="EmailTemplates" LocalizationId="EmailTemplatesCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <asp:DataGrid CssClass="items-list" ID="dgTemplates" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="true">
        
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row"/>
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Name">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Name")).ToHtml() %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:HyperLink runat="server" ID="hlEdit" 
                                   ButtonStyle="ButtonStyle2"
                                   NavigateUrl='<%# FormatEditUrl(Container.DataItem) %>' 
                                   LocalizationClass="Global" LocalizationId="ActionEdit" />

                        <Localized:LinkButton runat="server" ID="lbDelete"
                                    ButtonStyle="ButtonStyle2"
                                    CommandName="doDelete"
                                    CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailTemplateId")%>' 
                                    OnCommand="lbDelete_Click" 
                                    LocalizationClass="Global" LocalizationId="ActionDelete" />
                    </ItemTemplate>
                </Localized:TemplateColumn>
          </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text"><Localized:Label runat="Server" LocalizationClass="Global" LocalizationId="MessageNoDataHere"></Localized:Label></span>
        </div>
    </asp:PlaceHolder>

</asp:Content>
