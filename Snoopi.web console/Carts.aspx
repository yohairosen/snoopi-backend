<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Carts.aspx.cs" Inherits="Snoopi.web.Carts" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="CartBids" ContentPlaceHolderID="cphContent" runat="Server">

   
    <br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgBids" Value="0" />
      
        <asp:DataGrid CssClass="items-list" ID="dgBids" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="CartId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>

               <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="CartId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((int)DataBinder.Eval(Container.DataItem, "CartId")).ToString().ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                 <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Email" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.UniqueEmail") != null ? DataBinder.Eval(Container.DataItem,  "User.UniqueEmail") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.Phone") != null ? DataBinder.Eval(Container.DataItem, "User.Phone") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Firstname" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.FirstName") != null ? DataBinder.Eval(Container.DataItem, "User.FirstName") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Lastname" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.LastName") != null ?DataBinder.Eval(Container.DataItem, "User.LastName") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="City" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.CityName") != null ? DataBinder.Eval(Container.DataItem, "User.CityName") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Street" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.Street") != null ? DataBinder.Eval(Container.DataItem, "User.Street") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="BuildingNumber" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.HouseNum") != null ? DataBinder.Eval(Container.DataItem, "User.HouseNum"): ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                
                  <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Apartment" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "User.ApartmentNumber") != null ? DataBinder.Eval(Container.DataItem, "User.ApartmentNumber") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="SupplierId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "SupplierId") != null ? DataBinder.Eval(Container.DataItem, "SupplierId") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="SupplierName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "SupplierBusinessName") != null ? DataBinder.Eval(Container.DataItem, "SupplierBusinessName") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>

                 <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="CartSum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "TotalSum") != null ? DataBinder.Eval(Container.DataItem, "TotalSum") : ""%></span></ItemTemplate>
                </Localized:TemplateColumn>


                 <asp:TemplateColumn HeaderText="מוצרים">
                    <ItemTemplate>
                        
                        <asp:DataGrid ID="dgProducts" runat="server"
                            BorderStyle="None" BorderWidth="0"
                            CellPadding="3" CellSpacing="0" Width="100%"
                            AutoGenerateColumns="False"
                            DataSource='<%#DataBinder.Eval(Container.DataItem, "Products") %>'>
                            <HeaderStyle BackColor="#c0c0c0" />
                            <Columns>
                                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Image" ItemStyle-CssClass="ltr">
                                    <HeaderStyle CssClass="t-center" />
                                    <ItemStyle CssClass="t-natural va-middle" />
                                    <ItemTemplate>
                                        <asp:Image ID="ProductImage" CssClass="image-small" runat="server" Width="100px" ImageUrl='<%# Snoopi.core.MediaUtility.GetImagePath("Product",(string)DataBinder.Eval(Container.DataItem, "ProductImage"), 0,64, 64) %>' AlternateText='<%# ((string)DataBinder.Eval(Container.DataItem, "ProductName")) %>' />
                                    </ItemTemplate>
                                </Localized:TemplateColumn>
                                 <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="ProductId" ItemStyle-CssClass="ltr">
                                    <HeaderStyle CssClass="t-center" />
                                    <ItemStyle CssClass="t-center va-middle" />
                                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "ProductId") != null ? DataBinder.Eval(Container.DataItem, "ProductId") : ""%></span></ItemTemplate>
                                </Localized:TemplateColumn>
                                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="ProductName" ItemStyle-CssClass="ltr">
                                    <HeaderStyle CssClass="t-center" />
                                    <ItemStyle CssClass="t-center va-middle" />
                                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "ProductName") != null ? DataBinder.Eval(Container.DataItem, "ProductName") : ""%></span></ItemTemplate>
                                </Localized:TemplateColumn>
                               <%--  <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="EndTime" ItemStyle-CssClass="ltr">
                                    <HeaderStyle CssClass="t-center" />
                                    <ItemStyle CssClass="t-center va-middle" />
                                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "RecomendedPrice") != null ? DataBinder.Eval(Container.DataItem, "RecomendedPrice") : ""%></span></ItemTemplate>
                                </Localized:TemplateColumn>
                        --%>
                                <Localized:TemplateColumn LocalizationClass="Cart" HeaderTextLocalizationId="Amount" ItemStyle-CssClass="ltr">
                                    <HeaderStyle CssClass="t-center" />
                                    <ItemStyle CssClass="t-center va-middle" />
                                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "ProductAmount") != null ? DataBinder.Eval(Container.DataItem, "ProductAmount") : ""%></span></ItemTemplate>
                                </Localized:TemplateColumn>
                            
                            </Columns>
                        </asp:DataGrid>

                    </ItemTemplate>
                </asp:TemplateColumn>

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

        $(function () {
            $("#dpSearchCreateDateFrom,#dpSearchCreateDateTo").datepicker({ dateFormat: 'dd/mm/yy' }).val();

        });

    </script>

</asp:Content>
