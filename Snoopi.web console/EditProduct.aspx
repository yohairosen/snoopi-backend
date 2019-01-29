<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditProduct.aspx.cs" Inherits="Snoopi.web.EditProduct" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="Snoopi.core.DAL" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="pnlEditProduct" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalProductId" />
        <table>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblProductName" AssociatedControlID="txtProductName" LocalizationClass="Products" LocalizationId="ProductName"></Localized:Label>*</th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtProductName" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvProductName" runat="server"
                        ControlToValidate="txtProductName" Display="None" LocalizationClass="Products" LocalizationId="ProductNameRequired"></Localized:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblProductCode" AssociatedControlID="txtProductCode" LocalizationClass="Products" LocalizationId="ProductCode"></Localized:Label>*</th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtProductCode" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvProductCode" runat="server"
                        ControlToValidate="txtProductCode" Display="None" LocalizationClass="Products" LocalizationId="ProductCodeRequired"></Localized:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label3" AssociatedControlID="txtProductNum" LocalizationClass="Products" LocalizationId="ProductNum"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtProductNum" runat="server"></asp:TextBox>
                    <Localized:RegularExpressionValidator ID="revEml" runat="Server"
                        LocalizationClass="Products" LocalizationId="ProductNumInvalid" Display="None" ControlToValidate="txtProductNum"
                        ValidationExpression="^[1-9][0-9]*$"></Localized:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblImage" AssociatedControlID="fuImage" LocalizationClass="Products" LocalizationId="ProductImage"></Localized:Label></th>
                <td class="nowrap">
                    <asp:FileUpload ID="fuImage" runat="server" />
                    <asp:Image ID="imgImage" runat="server" CssClass="image-small" />
                    <Localized:Button ID="btnDeleteImage" runat="server" CssClass="button-02" LocalizationClass="Products" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" />
                </td>
            </tr>
               <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblRecomendedPrice" AssociatedControlID="txtRecomendedPrice" LocalizationClass="Products" LocalizationId="RecomendedPrice"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtRecomendedPrice" runat="server" ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblProductAmount" AssociatedControlID="txtProductAmount" LocalizationClass="Products" LocalizationId="ProductAmount"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtProductAmount" runat="server"></asp:TextBox>
<%--                    <Localized:RequiredFieldValidator ID="rfvProductAmount" runat="server"
                        ControlToValidate="txtProductCode" Display="None" LocalizationClass="Products" LocalizationId="ProductAmountRequired"></Localized:RequiredFieldValidator>--%>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblDescription" AssociatedControlID="txtProductDescription" LocalizationClass="Products" LocalizationId="Description"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtProductDescription" runat="server"></asp:TextBox>
<%--                    <Localized:RequiredFieldValidator ID="rfvProductDescription" runat="server"
                        ControlToValidate="txtProductDescription" Display="None" LocalizationClass="Products" LocalizationId="ProductDescriptionRequired"></Localized:RequiredFieldValidator>--%>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblAnimalType" AssociatedControlID="ddlAnimalType" LocalizationClass="Products" LocalizationId="AnimalType"></Localized:Label>*</th>
                <td class="nowrap">
                    <asp:ListBox ID="ddlAnimalType" runat="server" SelectionMode="Multiple" ClientIDMode="Static" CssClass="tdElementWidth"></asp:ListBox>
                    <Localized:RequiredFieldValidator ID="rfvAnimalType" runat="server"
                        ControlToValidate="ddlAnimalType" Display="None" LocalizationClass="Products" LocalizationId="AnimalTypeRequired"></Localized:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblCategory" AssociatedControlID="ddlCategory" LocalizationClass="Products" LocalizationId="Category"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList ID="ddlCategory" runat="server" SelectionMode="Multiple" ClientIDMode="Static" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true" CssClass="tdElementWidth"></asp:DropDownList>
                    <Localized:Label runat="Server" ID="Label1" LocalizationClass="Products" LocalizationId="CategoriesNotShown"></Localized:Label>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblSubCategory" AssociatedControlID="ddlSubCategory" LocalizationClass="Products" LocalizationId="SubCategory"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList ID="ddlSubCategory" runat="server" SelectionMode="Multiple" ClientIDMode="Static" CssClass="tdElementWidth"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFilter" LocalizationClass="Products" LocalizationId="Filter"></Localized:Label></th>
                <td class="nowrap">
                    <Localized:Label runat="Server" ID="Label2" LocalizationClass="Products" LocalizationId="ChooseFilter"></Localized:Label>
                    <asp:UpdatePanel runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:GridView runat="server" ID="gvFilters" ShowHeader="true" AutoGenerateColumns="false" EnableViewState="true"
                                AlternatingRowStyle-CssClass="alt" RowStyle-CssClass="row"
                                OnRowDataBound="gvFilters_RowDataBound"
                                DataKeyNames="FilterId">
                                <Columns>
                                    <Localized:TemplateField LocalizationClass="Products" HeaderTextLocalizationId="FilterBy" ItemStyle-CssClass="ltr">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFilterName" runat="server" Text='<%#((string)DataBinder.Eval(Container.DataItem, "FilterName")).ToHtml() %>' />
                                        </ItemTemplate>
                                    </Localized:TemplateField>
                                    <Localized:TemplateField LocalizationClass="Products" HeaderTextLocalizationId="SubFilter">
                                        <HeaderStyle CssClass="t-center" />
                                        <ItemStyle CssClass="t-natural" />
                                        <ItemTemplate>
                                            <asp:CheckBoxList ID="ddlSubFilter" runat="server" CssClass="chkChoice" SelectionMode="Multiple" DataValueField="SubFilterId" DataSource='<%# ((List<Snoopi.core.BL.SubFilterUI>)DataBinder.Eval(Container.DataItem, "LstSubFilter"))%>' DataTextField="SubFilterName" ClientIDMode="Static"></asp:CheckBoxList>
                                        </ItemTemplate>
                                    </Localized:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>

            </tr>

            <tr runat="server" id="isNew" visible="false">
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblIsSendSupplier" AssociatedControlID="cbxIsSendSupplier" LocalizationClass="Products" LocalizationId="IsSendSupplier"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="cbxIsSendSupplier" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td colspan="3" class="t-center">
                    <Localized:LinkButton runat="server" ID="btnSave" OnClick="btnSave_Click" OnClientClick="return validate();" CssClass="button" LocalizationClass="Products" LocalizationId="SaveProduct"></Localized:LinkButton>
                   <Localized:LinkButton runat="server" ID="btnBackToProductList" OnClick="btnBackToProductList_Click"  CssClass="button" LocalizationClass="Products" LocalizationId="BackToProductList"></Localized:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <script type="text/javascript">

        $(function () {
            $(".chkChoice label").attr("for", "");;
        });
        function validate()
        {
            var a = document.getElementById("txtRecomendedPrice").value;
            if (a == "0") {
                alert("חובה להכניס מחיר מומלץ גדול מ 0");
                return false;
            }
            else {
                return true;
            }
        }
    </script>
   
</asp:Content>

