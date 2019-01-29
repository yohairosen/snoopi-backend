<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NotificationsPanel.aspx.cs" MasterPageFile="Template.master" Inherits="NotificationPanel" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.web.Localization.Strings.Accessors" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgNotifications" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Notifications" LocalizationId="AdsCountLabel" />&nbsp;
        <asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <asp:DataGrid ID="dgNotifications" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="Id">
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
               <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="Name" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "Name")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="FilteringGroup" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <span runat="server" id="lblSection">
                            <%# DataBinder.Eval(Container.DataItem, "Id") != null ? ResourceManagerAccessor.GetText("Notifications",System.Enum.GetName(typeof(Snoopi.core.DAL.Entities.NotificationGroupsEnum),DataBinder.Eval(Container.DataItem, "Group")) ) : ""%>
                        </span>
                    </ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="FromDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "FromDate")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="ToDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "ToDate")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="AnimalType" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "AnimalTypeId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="MinFrequency" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "MinFrequency")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="MaxFrequency" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "MaxFrequency")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="Area" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "AreaId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="Priority" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "Priority")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                
                <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="CreateDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "Created")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="File" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Image ID="ProductImage" CssClass="image-small" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("Banners",(string)DataBinder.Eval(Container.DataItem, "AdImageUrl"), 0,64, 64) %>' />
                    </ItemTemplate>
                </Localized:TemplateColumn>
                
                <Localized:TemplateColumn LocalizationClass="Notifications" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:HyperLink runat="server" ID="hlEdit"
                            ButtonStyle="ButtonStyle2"
                            NavigateUrl='<%# FormatEditUrl(Container.DataItem) %>'
                            LocalizationClass="Global" LocalizationId="ActionEdit" />
                        <Localized:Button runat="server" ID="hlDelete"
                            ButtonStyle="ButtonStyle2" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id").ToString()%>' OnClick="hlDelete_Click" OnClientClick="return confirmDelete()" LocalizationClass="Global" LocalizationId="ActionDelete" />
                         <Localized:Button runat="server" ID="hlSendPush"
                            ButtonStyle="ButtonStyle2" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id").ToString()%>' OnClick="hlSendPush_Click" OnClientClick="return confirmSend()" LocalizationClass="Notifications" LocalizationId="SendPush" />
                  
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
            return confirm('<%= NotificationStrings.GetText("ConfirmDelete")%>');
        }

        function confirmSend() {
            return confirm('<%= NotificationStrings.GetText("ConfirmSend")%>');
         }
    </script>

</asp:Content>
