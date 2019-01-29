<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PromotedProducts.aspx.cs" Inherits="PromotedProducts" MasterPageFile="~/Template.master" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="CartBids" ContentPlaceHolderID="cphContent" runat="Server">

    <script type="text/javascript">
        function ddlProductsChange(ddlSupp) {
            var hf = document.getElementById("<%= hfProductSelectedValue.ClientID %>");
            hf.value = ddlSupp.value;
        }
        function ddlSectionChange(ddlSupp) {
            var hf = document.getElementById("<%= hfSectionSelectedValue.ClientID %>");
            hf.value = ddlSupp.value;
        }
        function ddlWeightChange(ddlSupp) {
            var hf = document.getElementById("<%= hfWeightSelectedValue.ClientID %>");
            hf.value = ddlSupp.value;
        }
    </script>

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgPromtedProducts" Value="0" />
                <asp:HiddenField runat="server" ID="hfWeightSelectedValue" Value="0" />
                <asp:HiddenField runat="server" ID="hfProductSelectedValue" Value="0" />
                <asp:HiddenField runat="server" ID="hfSectionSelectedValue" Value="0" />
                <asp:DataGrid CssClass="items-list" ID="dgPromtedProducts" runat="server" UseAccessibleHeader="true"
                    AutoGenerateColumns="false" AllowCustomPaging="true"
                    AllowPaging="true" PageSize="20" EnableViewState="true" DataKeyField="PromotedAreaId">

                    <HeaderStyle CssClass="header" />
                    <AlternatingItemStyle CssClass="alt" />
                    <ItemStyle CssClass="row" />
                    <PagerStyle Mode="NumericPages" CssClass="paging" />
                    <Columns>

                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="AreaId" ItemStyle-CssClass="ltr">
                            <HeaderStyle CssClass="t-center" />
                            <ItemStyle CssClass="t-natural va-middle" />
                            <ItemTemplate>
                                <span id="PromotedAreaId" runat="server">
                                    <%# DataBinder.Eval(Container.DataItem, "PromotedAreaId")%>
                                </span>
                            </ItemTemplate>
                        </Localized:TemplateColumn>

                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="AreaName" ItemStyle-CssClass="ltr">
                            <HeaderStyle CssClass="t-center" />
                            <ItemStyle CssClass="t-center va-middle" />
                            <ItemTemplate>
                                <span class="dgDateManager">
                                    <%# DataBinder.Eval(Container.DataItem, "PromotedAreaName") != null ? DataBinder.Eval(Container.DataItem, "PromotedAreaName") : ""%>
                                </span>
                            </ItemTemplate>
                        </Localized:TemplateColumn>

                        <asp:TemplateColumn HeaderText="מוצרים">
                            <ItemTemplate>

                                <asp:DataGrid ID="dgProducts" runat="server"
                                    BorderStyle="None" BorderWidth="0"
                                    CellPadding="3" CellSpacing="0" Width="100%"
                                    AutoGenerateColumns="False"
                                    DataSource='<%#DataBinder.Eval(Container.DataItem, "ProductPromoted") %>' OnItemDataBound="dgProducts_ItemDataBound">
                                    <HeaderStyle BackColor="#c0c0c0" />
                                    <Columns>
                                        <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Image" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-natural va-middle" />
                                            <ItemTemplate>
                                                <asp:Image ID="ProductImage" CssClass="image-small" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("Product", (string)DataBinder.Eval(Container.DataItem, "ProductImage"), 0, 64, 64) %>' AlternateText='<%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")) %>' />
                                                <span id="spnAreaId" runat="server" style="display: none">
                                                    <%# DataBinder.Eval(Container.DataItem, "AreaId")%>
                                                </span>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="ProductId" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <span id="spnPromotedProduct" runat="server" class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "ProductCode") != null ? DataBinder.Eval(Container.DataItem, "ProductCode") : ""%></span>
                                                <asp:DropDownList runat="server" ID="ddlPromtedProducts" onchange="ddlProductsChange(this)" Style="display: none" Width="70px">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <span class="dgDateManager" id="spnProductName">
                                                    <%# DataBinder.Eval(Container.DataItem, "ProductName") != null ? DataBinder.Eval(Container.DataItem, "ProductName") : ""%>
                                                </span>
                                                <span class="dgDateManager" id="spnID" runat="server" style="display: none">
                                                    <%# DataBinder.Eval(Container.DataItem, "ID") %>
                                                </span>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="Section" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <span runat="server" id="lblSection">
                                                    <%# DataBinder.Eval(Container.DataItem, "Section") != null ? ResourceManagerAccessor.GetText("PromotedArea", DataBinder.Eval(Container.DataItem, "Section").ToString()) : ""%>
                                                </span>
                                                <span runat="server" id="spnSection" style="display: none">
                                                    <%# DataBinder.Eval(Container.DataItem, "Section") %>
                                                </span>
                                                <asp:DropDownList runat="server" ID="ddlSection" onchange="ddlSectionChange(this)" Style="display: none" Width="120px">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="Weight" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <span id="lblWeight" runat="server" class="dgDateManager">
                                                    <%# DataBinder.Eval(Container.DataItem, "Weight") == null || Convert.ToInt16( DataBinder.Eval(Container.DataItem, "ID")) ==0 ?"": DataBinder.Eval(Container.DataItem, "Weight") %>
                                                </span>
                                                <asp:DropDownList runat="server" ID="ddlWeight" onchange="ddlWeightChange(this)" Style="display: none" Width="40px">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>

                                        <Localized:TemplateColumn ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <Localized:Button ID="btnEdit" LocalizationClass="PromotedArea" CommandArgument='<%# Eval("ProductCode") +";"+DataBinder.Eval(Container.DataItem, "Section") %>' OnClick="Edit_Click" LocalizationId="Edit" runat="server" />
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <Localized:Button LocalizationClass="PromotedArea" OnClick="Save_Click" LocalizationId="Save" runat="server" />
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <Localized:Button ID="btnRemove" LocalizationClass="PromotedArea" OnClick="Remove_Click" LocalizationId="Remove" runat="server" />
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                    </Columns>
                                </asp:DataGrid>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phHasNoItems" Visible="false">
        <div class="message info">
            <span class="icon"></span>
            <span class="text">
                <asp:Label runat="Server" ID="lblNoItems"></asp:Label></span>
        </div>
    </asp:PlaceHolder>
</asp:Content>
