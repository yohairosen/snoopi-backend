<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="Snoopi.web.Products" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.core.DAL" %>


<asp:Content ID="ContentProducts" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />

        <Localized:Label ID="Label2" runat="server" BlockMode="false" AssociatedControlID="txtSearchCode" LocalizationClass="Products" LocalizationId="SearchCode" />&nbsp;
        <asp:TextBox ID="txtSearchCode" runat="server" />

         <Localized:Label ID="lblSearchCategory" runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="SearchCategory" />&nbsp;
         <asp:DropDownList ID="ddlCategory"  runat="server"  OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true" EnableViewState ="true" ViewStateMode="Enabled" CssClass="input-text"></asp:DropDownList>&nbsp;&nbsp;

         <Localized:Label ID="Label1" runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="SearchSubCategory" />&nbsp;
         <asp:DropDownList ID="ddlSubCategory" CssClass="input-text" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" runat="server" AutoPostBack="true" EnableViewState ="true" ViewStateMode="Enabled" ></asp:DropDownList>&nbsp;&nbsp;

        <Localized:LinkButton runat="server" ID="btnSearch" CssClass="button-02" OnClick="btnSearch_Click" LocalizationClass="Global" LocalizationId="Search" />
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Products" LocalizationId="ExportButton"></Localized:LinkButton><br />
    </asp:Panel>
    <br />
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgProducts" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Products" LocalizationId="ProductsCountLabel"></Localized:Label>&nbsp;<Localized:Label ID="lblTotal" runat="server" BlockMode="false"></Localized:Label>
        <br />
        <br />
        <asp:DataGrid CssClass="items-list" ID="dgProducts" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            OnItemCommand="dgProducts_ItemCommand" ClientIDMode="Static"
            AllowPaging="true" PageSize="30" EnableViewState="false"
            DataKeyField="ProductId"
            AllowSorting="True">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductCode" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductCode")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductNum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "ProductNum")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductImage" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate>
                        <asp:Image ID="ProductImage" CssClass="image-small" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("Product",(string)DataBinder.Eval(Container.DataItem, "ProductImage"), 0,64, 64) %>' AlternateText='<%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")) %>' Visible='<%# (Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsImage")))%>'/></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Amount" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Amount")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="RecomendedPrice" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "RecomendedPrice"))%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Description" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Description")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="AnimalType" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (Snoopi.core.BL.ProductController.ConvertListToString((AnimalCollection)DataBinder.Eval(Container.DataItem, "AnimalLst"))).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Category" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CategoryName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="SubCategory" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "SubCategoryName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="Date" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "CreateDate")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductRate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate>
                        <Localized:TextBox ID="txtProductRate" TextBoxLength="Short" CssClass="product-rate" runat="server" Text='<%# (DataBinder.Eval(Container.DataItem, "ProductRate") ?? "0").ToString() %>'></Localized:TextBox>
                    </ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap va-middle" />
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbEdit" runat="server"
                            CausesValidation="False"
                            CommandName="Edit"
                            LocalizationClass="Global" LocalizationId="ActionEdit"
                            ButtonStyle="ButtonStyle2" />
                        <Localized:LinkButton ID="lbDelete" runat="server"
                            CausesValidation="False"
                            CommandName="Delete"
                            LocalizationClass="Global" LocalizationId="ActionDelete"
                            ButtonStyle="ButtonStyle2" />
                    </ItemTemplate>
                </Localized:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </asp:PlaceHolder>
    <br /><br />
    <Localized:LinkButton runat="server" ID="SaveButton" CssClass="button-02" OnClick="btnSave_Click" LocalizationClass="Products" LocalizationId="SaveButton"></Localized:LinkButton><br />
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>

     <%--<script type="text/javascript">
         //clear url page param for paging
         $(function () {
             var query = window.location.search.substring(1)

             if (query.length) {
                 if (window.history != undefined && window.history.pushState != undefined) {
                     window.history.pushState({}, document.title, window.location.pathname);
                 }
             }
         });

    </script>--%>
    <script type="text/javascript">
        $(document).ready(function () {

            $('.product-rate').keypress(validateNumber);

        });

        function validateNumber(event) {
            var key = window.event ? event.keyCode : event.which;

            if (event.keyCode === 8 || event.keyCode === 46) {
                return true;
            }
            else if (key > 31 && (key < 48 || key > 57)) {
                alert('חובה להכניס ערך נומרי');
                return false;
            }
            return true;
        };
    </script>


</asp:Content>
