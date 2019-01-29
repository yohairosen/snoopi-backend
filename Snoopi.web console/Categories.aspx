<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Categories.aspx.cs" Inherits="Snoopi.web.Categories" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="ContentCategories" ContentPlaceHolderID="cphContent" runat="Server">
  
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
         <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="Categories" LocalizationId="ExportButton"></Localized:LinkButton><br />
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgCategories" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Categories" LocalizationId="CategoriesCountLabel" ></Localized:Label>&nbsp;<Localized:Label ID="lblTotal" runat="server" BlockMode="false"></Localized:Label>
        <br />
        <asp:DataGrid CssClass="items-list" ID="dgCategories" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" 
            AllowCustomPaging="true" AllowPaging="true" PageSize="30"
            OnItemCommand="dgCategories_ItemCommand" ClientIDMode="Static"
            EnableViewState="false"
            DataKeyField="CategoryId"
            AllowSorting="True">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="CategoryId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "CategoryId")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="CategoryName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CategoryName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="CategoryRate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                     <ItemTemplate>
                            <Localized:TextBox ID="txtCategoryRate" TextBoxLength="Short" CssClass="rate" runat="server" Text='<%# (DataBinder.Eval(Container.DataItem, "CategoryRate")).ToString()  == "0" ? "0" : (DataBinder.Eval(Container.DataItem, "CategoryRate")).ToString()%>'></Localized:TextBox>
                        </ItemTemplate>
                </Localized:TemplateColumn>
<%--                <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductImage" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><asp:Image ID="ProductImage" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("Category",(string)DataBinder.Eval(Container.DataItem, "CategoryImage"), 0,64, 64) %>' AlternateText='<%# ((string)DataBinder.Eval(Container.DataItem, "CategoryName")).ToHtml() %>' /></ItemTemplate>
                </Localized:TemplateColumn>--%>
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
     <Localized:LinkButton runat="server" ID="SaveButton" CssClass="button-02" OnClick="btnSave_Click" LocalizationClass="Categories" LocalizationId="SaveButton"></Localized:LinkButton><br />
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>
     <script type="text/javascript">
         $(document).ready(function () {

             $('.rate').keypress(validateNumber);

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
