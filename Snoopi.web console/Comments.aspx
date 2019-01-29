<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Comments.aspx.cs" Inherits="Snoopi.web.Comments" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.core.DAL" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="pnlSearch" runat="server">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label ID="lblSearchBusiness" runat="server" BlockMode="false" LocalizationClass="Comments" LocalizationId="SearchBusiness" />&nbsp;
         <asp:ListBox ID="ddlSuppliers" runat="server" SelectionMode="Multiple" ClientIDMode="Static" OnSelectedIndexChanged="btnSearch_Click" AutoPostBack="true"></asp:ListBox>&nbsp;&nbsp;
         <Localized:Label ID="lblSearchStatus" runat="server" BlockMode="false" LocalizationClass="Comments" LocalizationId="lblSearchStatus" />&nbsp;
         <asp:ListBox ID="ddlStatus" runat="server" SelectionMode="Multiple" ClientIDMode="Static" OnSelectedIndexChanged="btnSearch_Click" AutoPostBack="true"></asp:ListBox>
    </asp:Panel>
    <br />
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgComments" Value="0" />
               <Localized:Label runat="server" BlockMode="false" LocalizationClass="Comments" LocalizationId="CommentsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static"/><br />

        <asp:DataGrid CssClass="items-list" ID="dgComments" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" DataKeyField="CommentId"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" OnItemCommand="dgComments_ItemCommand">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
               <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="BidId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "BidId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="ApproveDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((DateTime)DataBinder.Eval(Container.DataItem, "ApproveDate") == DateTime.MinValue ? "" : ((DateTime)DataBinder.Eval(Container.DataItem, "ApproveDate")).ToShortDateString())%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="BusinessName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "BusinessName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="CreateDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((DateTime)DataBinder.Eval(Container.DataItem, "CreateDate")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="SenderEmail" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "SenderEmail")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="Name" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Name")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="Rate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Rate")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="Content" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Content")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Comments" HeaderTextLocalizationId="Status" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# CommentsStrings.GetStatus((CommentStatus)DataBinder.Eval(Container.DataItem, "Status"))%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:Button runat="server" ID="hlApprove"
                            CommandArgument='<%# ((Int64)DataBinder.Eval(Container.DataItem, "CommentId")) %>'
                            CommandName="approve" CssClass="btn-alert button-02"
                            LocalizationClass="Comments" LocalizationId="Approve" />
                        <Localized:Button runat="server" ID="hlCancel"
                            CssClass="btn-alert button-02"
                            CommandName="cancel" CommandArgument='<%# ((Int64)DataBinder.Eval(Container.DataItem, "CommentId")) %>'
                            LocalizationClass="Comments" LocalizationId="Cancel" />
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
    <script>
        $(document).ready(function () {
            $(".btn-alert").click(function () {
                if ($("body").hasClass("ltr")) { return confirm("האם אתה בטוח בפעולה זו?") } else { return confirm("Are you sure in this action?") }
            });
            $('.bxslider').bxSlider({
                mode: 'fade',
                captions: true
            });
        });
    </script>
</asp:Content>
