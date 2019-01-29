<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PromotedSuppliers.aspx.cs" Inherits="PromotedSuppliers" MasterPageFile="~/Template.master" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="CartBids" ContentPlaceHolderID="cphContent" runat="Server">
    <table>
        <tr>
            <td>
                <div>
                    <Localized:Label runat="server" LocalizationClass="PromotedArea" LocalizationId="AreaName"></Localized:Label>
                    <asp:DropDownList runat="server" ID="ddlAreas" DataTextField="name" DataValueField="id"></asp:DropDownList>
                </div>

            </td>
            <td>
                <div>
                    <Localized:Label runat="server" LocalizationClass="Suppliers" LocalizationId="ServiceType"></Localized:Label>
                    <asp:ListBox ID="ddlServices" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox>
                </div>
            </td>
            <td>
                <Localized:Button LocalizationClass="suppliers" LocalizationId="Search" OnClick="btnSearch_Click" ID="btnSearch" runat="server" />
            </td>
        </tr>
    </table>
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgPromtedSuppliers" Value="0" />
                <asp:HiddenField runat="server" ID="hfSupplierOriginalId" Value="0" />
                <asp:HiddenField runat="server" ID="hfSupplierSelectedValue" Value="0" />


                <asp:DataGrid CssClass="items-list" ID="dgPromtedSuppliers" runat="server" UseAccessibleHeader="true"
                    AutoGenerateColumns="false" AllowCustomPaging="true"
                    AllowPaging="true" PageSize="20" EnableViewState="true" DataKeyField="ServiceId">

                    <HeaderStyle CssClass="header" />
                    <AlternatingItemStyle CssClass="alt" />
                    <ItemStyle CssClass="row" />
                    <PagerStyle Mode="NumericPages" CssClass="paging" />
                    <Columns>

                        <%--                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="AreaId" ItemStyle-CssClass="ltr">
                            <HeaderStyle CssClass="t-center" />
                            <ItemStyle CssClass="t-natural va-middle" />
                            <ItemTemplate><%# ((int)DataBinder.Eval(Container.DataItem, "PromotedAreaId")).ToString().ToHtml()%></ItemTemplate>
                        </Localized:TemplateColumn>

                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="AreaName" ItemStyle-CssClass="ltr">
                            <HeaderStyle CssClass="t-center" />
                            <ItemStyle CssClass="t-center va-middle" />
                            <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "PromotedAreaName") != null ? DataBinder.Eval(Container.DataItem,  "PromotedAreaName") : ""%></span></ItemTemplate>
                        </Localized:TemplateColumn>--%>
                        <Localized:TemplateColumn LocalizationClass="Suppliers" ItemStyle-Width="350px" HeaderTextLocalizationId="ServiceType" ItemStyle-CssClass="ltr">
                            <HeaderStyle CssClass="t-center" />
                            <ItemStyle CssClass="t-natural va-middle" />
                            <ItemTemplate>
                                <asp:Label ID="spnPromotedSupplier" runat="server"> <%# DataBinder.Eval(Container.DataItem, "ServiceName") != null ? DataBinder.Eval(Container.DataItem, "ServiceName") : ""%></asp:Label>

                            </ItemTemplate>
                        </Localized:TemplateColumn>
                        <asp:TemplateColumn HeaderText="ספקים">
                            <ItemTemplate>

                                <asp:DataGrid ID="dgSuppliers" runat="server"
                                    BorderStyle="None" BorderWidth="0"
                                    CellPadding="3" CellSpacing="0" Width="100%"
                                    AutoGenerateColumns="False" ClientIDMode="Predictable"
                                    DataSource='<%#DataBinder.Eval(Container.DataItem, "SupplierPromoted") %>'>
                                    <HeaderStyle BackColor="#c0c0c0" />
                                    <Columns>
                                        <Localized:TemplateColumn LocalizationClass="Suppliers" ItemStyle-Width="350px" HeaderTextLocalizationId="BusinessName" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-natural va-middle" />
                                            <ItemTemplate>

                                                <asp:DropDownList runat="server" ID="ddlPromtedSuppliers" onchange="ddlSuppliersChange(this)" Style="display: none" Width="320px">
                                                </asp:DropDownList>
                                                <asp:Label ID="spnPromotedSupplier" runat="server"> <%# DataBinder.Eval(Container.DataItem, "SupplierBuisnesName") != null ? DataBinder.Eval(Container.DataItem, "SupplierBuisnesName") : ""%></asp:Label>

                                            </ItemTemplate>
                                        </Localized:TemplateColumn>

                                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="FromDate" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <asp:Panel runat="server" ID="datepickerFromDate">
                                                    <asp:TextBox CssClass="fromDatePicker" ClientIDMode="Static" Enabled="false" Text='<%# DataBinder.Eval(Container.DataItem, "StartTime") != null ? Convert.ToDateTime( DataBinder.Eval(Container.DataItem, "StartTime")).ToShortDateString() : ""%>' runat="server" />
                                                    </span>
                                                </asp:Panel>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn LocalizationClass="PromotedArea" HeaderTextLocalizationId="ToDate" ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                <asp:Panel runat="server" ID="datepickerToDate">
                                                    <asp:TextBox CssClass='<%# "toDatePicker-" + ((int)DataBinder.
                                                Eval(Container.DataItem, "PromotedAreaId")).ToString().ToHtml()+ "-" + Container.ItemIndex%>'
                                                        ClientIDMode="Static" Enabled="false" Text='<%# DataBinder.Eval(Container.DataItem, "EndTime") != null ? Convert.ToDateTime( DataBinder.Eval(Container.DataItem, "EndTime")).ToShortDateString() : ""%>' runat="server" />
                                                </asp:Panel>
                                                </span>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>

                                        <Localized:TemplateColumn ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center nowrap" />
                                            <ItemTemplate>
                                                <span>
                                                <Localized:HyperLink runat="server" ID="hlEdit"
                                                    ButtonStyle="ButtonStyle2"
                                                    NavigateUrl='<%# FormatEditUrl(Container.DataItem) %>'
                                                    LocalizationClass="Global" LocalizationId="ActionEdit" />
                                                                       <Localized:Button runat="server" ID="hlDelete"
                            ButtonStyle="ButtonStyle2" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id").ToString()%>' OnClick="hlDelete_Click" OnClientClick="return confirmDelete()" LocalizationClass="Global" LocalizationId="ActionDelete" />

                                                     </span>
                                            </ItemTemplate>
                                        </Localized:TemplateColumn>
                                        <Localized:TemplateColumn ItemStyle-CssClass="ltr">
                                            <HeaderStyle CssClass="t-center" />
                                            <ItemStyle CssClass="t-center va-middle" />
                                            <ItemTemplate>
                                                                                        
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
    <script type="text/javascript">
        function confirmDelete() {
            return confirm('<%= Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("ConfirmDelete")%>');
        }
    </script>
</asp:Content>
