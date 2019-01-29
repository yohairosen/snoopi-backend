<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SubCategories.aspx.cs" Inherits="Snoopi.web.SubCategories" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="ContentCategories" ContentPlaceHolderID="cphContent" runat="Server">
  
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgSubCategories" Value="0" />

        <Localized:Label ID="lblSearchCategory" runat="server" BlockMode="false" LocalizationClass="Categories" LocalizationId="SearchCategory" />&nbsp;
         <asp:DropDownList ID="ddlCategory"  runat="server"  OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true" EnableViewState ="true" ViewStateMode="Enabled" CssClass="input-text"></asp:DropDownList>&nbsp;&nbsp;
        <br />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="Categories" LocalizationId="SubCategoriesCountLabel" ></Localized:Label>&nbsp;<Localized:Label ID="lblTotal" runat="server" BlockMode="false"></Localized:Label>
        <br />
         
        <asp:DataGrid CssClass="items-list" ID="dgSubCategories" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            OnItemCommand="dgSubCategories_ItemCommand" ClientIDMode="Static"
            AllowPaging="true" PageSize="30" EnableViewState="false"
            DataKeyField="SubCategoryId"
            AllowSorting="True"
            >

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="SubCategoryId">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "SubCategoryId")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="SubCategoryName">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "SubCategoryName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>
<%--                 <Localized:TemplateColumn LocalizationClass="Products" HeaderTextLocalizationId="ProductImage" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><asp:Image ID="ProductImage" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("SubCategory",(string)DataBinder.Eval(Container.DataItem, "SubCategoryImage"), 0,64, 64) %>' AlternateText='<%# ((string)DataBinder.Eval(Container.DataItem, "SubCategoryName")).ToHtml() %>' /></ItemTemplate>
                </Localized:TemplateColumn>--%>
                 <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="SubCategoryRate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                     <ItemTemplate>
                            <Localized:TextBox ID="txtSubCategoryRate" TextBoxLength="Short" CssClass="rate" runat="server" Text='<%# (DataBinder.Eval(Container.DataItem, "SubCategoryRate")).ToString()  == "0" ? "0" : (DataBinder.Eval(Container.DataItem, "SubCategoryRate")).ToString()%>'></Localized:TextBox>
                        </ItemTemplate>
                </Localized:TemplateColumn>
                 <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="SubCategoryFilters" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                     <ItemTemplate>
                          <asp:ListBox ID="ddlFilters" runat="server" SelectionMode="Multiple"></asp:ListBox>
                        </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Categories" HeaderTextLocalizationId="MainCategoryName">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CategoryName")).ToHtml()%></ItemTemplate>
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
