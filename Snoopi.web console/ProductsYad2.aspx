<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProductsYad2.aspx.cs" Inherits="Snoopi.web.ProductsYad2" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.core.DAL" %>

<asp:Content ID="ContentAppUsers" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="pnlSearch" runat="server">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
         <Localized:Label ID="lblSearchStatus" runat="server" BlockMode="false" LocalizationClass="Yad2" LocalizationId="lblSearchStatus" />&nbsp;
         <asp:ListBox ID="ddlStatus" runat="server" SelectionMode="Multiple" ClientIDMode="Static" OnSelectedIndexChanged="btnSearch_Click" AutoPostBack="true"></asp:ListBox>
        &nbsp;&nbsp;
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="AppUsers" LocalizationId="ExportButton"></Localized:LinkButton><br />
    </asp:Panel>

    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <Localized:Label ID="Label1" runat="Server" LocalizationClass="Yad2" LocalizationId="MessageNoDataHere"></Localized:Label></span>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgProductsYad2" Value="0" />
        <Localized:Label ID="Label3" runat="server" BlockMode="false" LocalizationClass="Yad2" LocalizationId="ProductsCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static" /><br />
        <br />
        <asp:DataGrid CssClass="items-list" ID="dgProductsYad2" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            OnItemCommand="dgProductsYad2_ItemCommand" ClientIDMode="Static"
            AllowPaging="true" PageSize="30" EnableViewState="false"
            DataKeyField="ProductId"
            AllowSorting="True">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ProductName") %></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="LstCategory" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Repeater ID="Repeater2" runat="server" DataSource='<%# ((List<Snoopi.core.DAL.CategoryYad2>)DataBinder.Eval(Container.DataItem, "LstCategory"))%>'>
                            <ItemTemplate>
                                <%#Eval("CategoryYad2Name")%>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="Price" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Price") %></ItemTemplate>
                </Localized:TemplateColumn>


                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="CityName" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "CityName")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="ContactName" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ContactName")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Phone")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="Details" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Details")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="Status" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# Yad2Strings.GetText(Enum.GetName(typeof(StatusType), (StatusType)DataBinder.Eval(Container.DataItem, "Status")))%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="StatusRemarksLabel" ItemStyle-CssClass="ltr" runat="server">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "StatusRemarks")%></ItemTemplate>
                </Localized:TemplateColumn>
                   <Localized:TemplateColumn LocalizationClass="Yad2" HeaderTextLocalizationId="ProductImage" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><asp:Image ID="ProductImage" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("ProductYad2",(string)DataBinder.Eval(Container.DataItem, "ProductImage"), 0,64, 64) %>' AlternateText='<%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToHtml() %>' /></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap va-middle" />
                    <ItemTemplate>
                        <%--                        <Localized:LinkButton ID="lbDetails" runat="server"
                            CausesValidation="False"
                            CommandName="Details"
                            LocalizationClass="Yad2" LocalizationId="Details"
                            ButtonStyle="ButtonStyle2" />--%>
                        <Localized:LinkButton runat="server" ID="hlApprove"
                            CommandArgument='<%# ((Int64)DataBinder.Eval(Container.DataItem, "ProductId")) %>'
                            CommandName="approve" CssClass="btn-alert button-02"
                            LocalizationClass="Yad2" LocalizationId="Approve" />
                        <Localized:LinkButton runat="server" ID="hlCancel"
                            CssClass="btn-alert button-02"
                            CommandName="cancel" CommandArgument='<%# ((Int64)DataBinder.Eval(Container.DataItem, "ProductId")) %>'
                            LocalizationClass="Yad2" LocalizationId="CancelDrive" />
                    </ItemTemplate>
                </Localized:TemplateColumn>

            </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <asp:Panel ID="pnlStatusRemarks" runat="server" Visible="false" Style="position: absolute; opacity: 0.5; background-color: black; height: 100%; width: 100%; display: block; top: 0px; left: 0px;"></asp:Panel>
    <asp:Panel ID="pnlRemark" runat="server" Visible="false" Style="position: absolute; top: 200px; background-color: aliceblue; padding: 50px; border: 10px solid; left: 50%;">
        <Localized:Label ID="lblInspectorNote" runat="Server" AssociatedControlID="txtStatusRemarks" LocalizationClass="Yad2" LocalizationId="StatusRemarks"></Localized:Label>
        <br />
        <br />
        <asp:TextBox runat="server" ID="txtStatusRemarks" TextMode="MultiLine" Height="100" Width="400"></asp:TextBox>
        <br />
        <br />
        <asp:HiddenField runat="server" ID="hiddenFieldProductId" />
        <Localized:Button runat="server" ID="btnReject" CssClass="button" OnClick="btnReject_Click" LocalizationClass="Yad2" LocalizationId="RejectButton" Visible="false"></Localized:Button>
        <Localized:Button runat="server" ID="btnApprove" CssClass="button" OnClick="btnApprove_Click" LocalizationClass="Yad2" LocalizationId="ApproveButton" Visible="false"></Localized:Button>
        <Localized:Button runat="server" ID="btnCancel" CssClass="button" OnClick="btnCancel_Click" LocalizationClass="Yad2" LocalizationId="CancelButton"></Localized:Button>
    </asp:Panel>
</asp:Content>
