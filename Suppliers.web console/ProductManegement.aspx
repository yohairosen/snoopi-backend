<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SuppliersTemplate.master" CodeFile="ProductManegement.aspx.cs" Inherits="ProductManegement" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Import Namespace="Snoopi.core.BL" %>
<%@ Import Namespace="Snoopi.core.DAL" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>

<asp:Content ID="cHead" runat="server" ContentPlaceHolderID="head">

    <script type="text/javascript">

        function pageChange(obj) {
            if (document.getElementById('formChanged').value == "true") {
                if (confirm("האם אתה רוצה לשמור את השינויים שנעשו בדף זה?")) {
                    $(".save-product").click();
                }
                document.getElementById('formChanged').value = "false";
            }         
        };

        $(document).ready(function () {

            $(".save-product").click(function () {
                hiddenProduct();
                document.getElementById('formChanged').value = "false";
            });

            $('form').change(function () {
                document.getElementById('formChanged').value = "true";
               
            });

           
            $('.product-price').keypress(validateNumber);

            $('.product-price').keyup(function () {
                if (parseInt(this.value)==0) {
                    alert('חובה להכניס מחיר גדול מ 0 עבור מוצר במלאי');
                return false;
                }
                return true;
            });
    
        });


     
        function DeleteAllConfirm() {
            return confirm('<%= SuppliersStrings.GetText(@"DeleteAllProductsConfirmation") %>');
        };

        function SaveAllConfirm() {
            return confirm('<%= SuppliersStrings.GetText(@"SaveAllProductsConfirmation") %>');
        };

        function validateNumber(event) {
            var key = window.event ? event.keyCode : event.which;

            if (event.keyCode === 8 || event.keyCode === 46) {    
                return true;
            }
            else if (key > 31 &&(key < 48 || key > 57)) {
                alert('חובה להכניס ערך נומרי');
                return false;
            }
             return true;
        };

        function hiddenProduct() {
            var str = "";
            $(".items-list tr td:first-child").each(function () { str = str + "," + $(this).html().trim(); });
            if (str.length > 0) str = str.substr(1);
            $("#hfProductIds").val(str);
        }
    </script>

</asp:Content>

<asp:Content ID="cContent" runat="server" ContentPlaceHolderID="cphContent" >
    <div class="title">
        <Localized:Label runat="server" LocalizationClass="Products" LocalizationId="ManegmentProducts"></Localized:Label>
    </div>
    <div class="sub-title">
        <Localized:Label ID="Label4" runat="server" LocalizationClass="Products" LocalizationId="ProductSupplierDescription"></Localized:Label>
    </div>  
     <asp:UpdateProgress ID="updProgress"
    AssociatedUpdatePanelID="up"
    runat="server">
        <ProgressTemplate> 
            <div id="loader" > </div>                
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="wrapper-category">
                <Localized:Label ID="Label3" runat="server" LocalizationClass="Products" LocalizationId="SearchLable" CssClass="right"></Localized:Label>

                <div>
                    <Localized:Label ID="Label1" runat="server" LocalizationClass="Products" LocalizationId="AnimalTypeLable"></Localized:Label>
                    <asp:DropDownList runat="server" ID="ddlAnimalType" AutoPostBack="true" OnSelectedIndexChanged="ddlAnimalType_SelectedIndexChanged" EnableViewState="true">
                    </asp:DropDownList>
                </div>

                <div>
                    <Localized:Label runat="server" LocalizationClass="Products" LocalizationId="CategoryLable"></Localized:Label>
                    <asp:DropDownList runat="server" ID="ddlCategory" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" EnableViewState="true">
                    </asp:DropDownList>
                </div>

                <div>
                    <Localized:Label ID="Label2" runat="server" LocalizationClass="Products" LocalizationId="SubCategoryLable" CssClass="label frst"></Localized:Label>
                    <asp:DropDownList runat="server" ID="ddlSubCategory" AutoPostBack="true" DataValueField="SubCategoryId" DataTextField="SubCategoryName"
                        OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" Enabled="false" EnableViewState="true">
                    </asp:DropDownList>
                </div>
            </div>
             

            <div class="wrraper-search">
                <div>
                    <Localized:Label runat="server" LocalizationClass="Products" LocalizationId="SearchByLable"></Localized:Label>
                    <Localized:TextBox ID="txtSearch" runat="server"></Localized:TextBox>
                    <Localized:Button runat="server" ID="btnSearchByCode" CssClass="search-button" OnClick="btnSearchByCode_Click" />
                    <asp:HiddenField runat="server" ID="hfProductIds" ClientIDMode="Static" />
                </div>
                <div>
                    <Localized:Label ID="Label5" runat="server" LocalizationClass="Products" LocalizationId="SearchIsExists" CssClass="search-is-exist"></Localized:Label>
                    <Localized:CheckBox ID="cbIsExists" runat="server" CssClass="cb-is-exist" OnCheckedChanged="cbIsExists_CheckedChanged" AutoPostBack="true"></Localized:CheckBox>
                    <div class="import">
                        <Localized:Button ID="btnImport" LocalizationClass="Products" LocalizationId="SupplierImportProducts" runat="server" CssClass="button" OnClick="btnImport_Click" />
                        <Localized:Button ID="btnExport" LocalizationClass="Products" LocalizationId="SupplierExportProducts" runat="server" CssClass="button" OnClick="btnExport_Click" />
                    </div>
                    
                </div>
                 <div class="select-all-product">  
                     <Localized:Button ID="btnResetAll" LocalizationClass="Products" LocalizationId="ResetAll" runat="server" CssClass="button" OnClick="btnResetAll_Click" OnClientClick="return DeleteAllConfirm();" />  
                     <Localized:Button ID="btnSaveAll" LocalizationClass="Products" LocalizationId="SaveAll" runat="server" CssClass="button" OnClick="btnSaveAll_Click" OnClientClick="return SaveAllConfirm();" />
                </div>

            </div>
            <asp:HiddenField runat="server" ID="formChanged" ClientIDMode="Static" Value="false" /> 
            <asp:DataGrid CssClass="items-list" ID="dgProducts" runat="server" UseAccessibleHeader="true" 
                AutoGenerateColumns="false" AllowCustomPaging="false"
                ClientIDMode="Static"
                AllowPaging="false" EnableViewState="true"
                DataKeyField="ProductId"
                AllowSorting="True">
                <HeaderStyle CssClass="header" />
                <AlternatingItemStyle CssClass="alt" />
                <ItemStyle CssClass="row" />
                <PagerStyle Mode="NumericPages" CssClass="paging" />
                <Columns>
                    <Localized:TemplateColumn ItemStyle-CssClass="invisible">
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="ProductId" ItemStyle-CssClass="ltr invisible" runat="server" CommandName="Sort"
                                CommandArgument="ProductId" CssClass="sortLnk invisible"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center short-header invisible" />
                        <ItemStyle CssClass="t-natural va-middle invisible" />
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "ProductId")%>
                        </ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="ProductNum" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="ProductNum" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center short-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate>
                            <%# DataBinder.Eval(Container.DataItem, "ProductNum")%>
                        </ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="ProductCode" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="ProductCode" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center short-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductCode")).ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="AnimalType" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="AnimalName" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center" Width="90px" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# (ProductController.ConvertListToString((AnimalCollection)DataBinder.Eval(Container.DataItem, "AnimalLst"))).ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>

                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="Category" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="CategoryName" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center" Width="100px" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CategoryName")).ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="SubCategory" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="SubCategoryName" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center" Width="95px" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "SubCategoryName")).ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="ProductName" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="ProductName" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center" Width="105px" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")).ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="ProductAmount" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="ProductAmount" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center long-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Amount")).ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton ID="LinkButton2" LocalizationClass="Products" LocalizationId="RecomendedPrice" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="RecomendedPrice" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center long-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate><%# ((decimal)DataBinder.Eval(Container.DataItem, "RecomendedPrice")).ToString().ToHtml()%></ItemTemplate>
                    </Localized:TemplateColumn>
                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton LocalizationClass="Products" LocalizationId="IsExist" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="IsExist" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center short-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate>
                            <center><asp:Checkbox runat="server" ID="cbIsExist" checked='<%#((bool)DataBinder.Eval(Container.DataItem, "IsExist")) == true ? true : false%>' /></center>
                        </ItemTemplate>
                    </Localized:TemplateColumn>

                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton ID="LinkButton1" LocalizationClass="Products" LocalizationId="ProductPrice" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="ProductPrice" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center short-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate>
                            <Localized:TextBox ID="txtPrice" runat="server" CssClass="product-price" Text='<%# ((decimal)DataBinder.Eval(Container.DataItem, "ProductPrice")).ToString()  == "0" ? "" : ((decimal)DataBinder.Eval(Container.DataItem, "ProductPrice")).ToString()%>'></Localized:TextBox>
                        </ItemTemplate>
                    </Localized:TemplateColumn>

                    <Localized:TemplateColumn>
                        <HeaderTemplate>
                            <Localized:LinkButton ID="LinkButton1" LocalizationClass="Products" LocalizationId="Gift" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort"
                                CommandArgument="Gift" CssClass="sortLnk"></Localized:LinkButton>
                        </HeaderTemplate>
                        <HeaderStyle CssClass="t-center short-header" />
                        <ItemStyle CssClass="t-natural va-middle" />
                        <ItemTemplate>
                            <Localized:TextBox ID="txtGift" runat="server" CssClass="product-gift" Text='<%# ((string)DataBinder.Eval(Container.DataItem, "Gift")).ToString()%>'></Localized:TextBox>
                        </ItemTemplate>
                    </Localized:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </ContentTemplate>
    </asp:UpdatePanel>
   
    <div class="wrapper-product-save">
        
        <Localized:Button
            ID="btnSave" LocalizationClass="Products" LocalizationId="Save" runat="server" CssClass="save-product button" OnClick="btnSave_Click" />
    </div>
    <div class="wrapper-recomment">
        <div>
            <Localized:Label runat="server" LocalizationClass="Products" LocalizationId="SupplierRecommend"></Localized:Label>
            <Localized:Button ID="btnRecommend" LocalizationClass="Products" LocalizationId="Recommend" runat="server" CssClass="btn-recommend button" OnClick="btnRecommend_Click" />
        </div>
    </div>
  
</asp:Content>
