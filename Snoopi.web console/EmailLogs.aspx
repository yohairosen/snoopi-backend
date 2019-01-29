<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EmailLogs.aspx.cs" Inherits="Snoopi.web.EmailLogs" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgLogs" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="EmailTemplates" LocalizationId="EmailLogsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" /><br />
        <Localized:LinkButton CssClass="Local" runat="Server" id="hlExportCurrent" LocalizationClass="Global" LocalizationId="ExportCurrentViewToExcel" OnClick="hlExportCurrent_Click" />&nbsp;&nbsp;<Localized:LinkButton CssClass="Local" runat="Server" id="hlExportAll" LocalizationClass="Global" LocalizationId="ExportAllViewToExcel" OnClick="hlExportAll_Click" /><br /><br />
        <asp:DataGrid CssClass="items-list" ID="dgLogs" runat="server" UseAccessibleHeader="true" 
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="true">
        
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row"/>
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Date">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center" />
                    <ItemTemplate><span class="dgDateManager"><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DeliveryDate")).ToString(@"yyyy-MM-ddTHH:mm:ssZ") %></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Status">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center" />
                    <ItemTemplate><%# GetStatus(DataBinder.Eval(Container.DataItem, "Status")) %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="EmailTemplates" HeaderTextLocalizationId="Subject">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Subject")).ToHtml() %></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="EmailTemplates" HeaderTextLocalizationId="Brief">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Body")).StripHtml(70).ToHtml() %>&hellip;</ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>

                        <Localized:LinkButton runat="server" ID="lbDelete"
                                    ButtonStyle="ButtonStyle2"
                                    CommandName="doDelete"
                                    CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmailLogId")%>' 
                                    OnCommand="lbDelete_Click" 
                                    LocalizationClass="Global" LocalizationId="ActionDelete" />
                                 
                        <Localized:HyperLink runat="server" ID="hlPreview" 
                            ButtonStyle="ButtonStyle2"
                            Target="_blank"
                            LocalizationClass="Global" LocalizationId="ActionView"></Localized:HyperLink>

                    </ItemTemplate>
                </Localized:TemplateColumn>
          </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text"><Localized:Label ID="Label1" runat="Server" LocalizationClass="Global" LocalizationId="MessageNoDataHere"></Localized:Label></span>
        </div>
    </asp:PlaceHolder>

</asp:Content>
