<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Suppliers.aspx.cs" Inherits="Snoopi.web.Suppliers" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label runat="server" BlockMode="false" AssociatedControlID="txtSearchName" LocalizationClass="Suppliers" LocalizationId="SearchName" />&nbsp;
        <asp:TextBox ID="txtSearchName" runat="server" />
         <Localized:Label ID="Label1" runat="server" BlockMode="false" AssociatedControlID="txtSearchPhone" LocalizationClass="Suppliers" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />

        <Localized:LinkButton runat="server" ID="btnSearch" CssClass="button-02" OnClick="btnSearch_Click" LocalizationClass="Global" LocalizationId="Search" />
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="AppUsers" LocalizationId="ExportButton"></Localized:LinkButton>
        <Localized:LinkButton runat="server" ID="btnExportForCRM" CssClass="button-02" OnClick="btnExportForCRM_Click" LocalizationClass="AppUsers" LocalizationId="ExportForCRMButton"></Localized:LinkButton><br />

    </asp:Panel><br />

    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgSuppliers" Value="0" />
             <Localized:Label runat="server" BlockMode="false" LocalizationClass="Suppliers" LocalizationId="SuppliersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" ClientIDMode="Static"/><br />  
        <asp:DataGrid CssClass="items-list suppliersList" ID="dgSuppliers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" OnItemCommand="dgSuppliers_ItemCommand"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="SupplierId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
<%--                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="SupplierId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((Int64)DataBinder.Eval(Container.DataItem, "SupplierId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>--%>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="BusinessName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "BusinessName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Email" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Email")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Phone")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="ContactName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ContactName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="ContactPhone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "ContactPhone")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="CityName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "CityName")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Street" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Street")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="HouseNum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "HouseNum")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Precent" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Precent")%>%</ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="SumPerMonth" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "SumPerMonth")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="CreateDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# (DateTime)DataBinder.Eval(Container.DataItem, "CreateDate")%></ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="citiesSupplied" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# ((List<Snoopi.core.DAL.City>)DataBinder.Eval(Container.DataItem, "citiesSupplied"))%>'>
                            <ItemTemplate>
                                <%#Eval("CityName")%>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="citiesHomeService" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate>
                        <asp:Repeater ID="Repeater2" runat="server" DataSource='<%# ((List<Snoopi.core.DAL.City>)DataBinder.Eval(Container.DataItem, "citiesHomeService"))%>'>
                            <ItemTemplate>
                                <%#Eval("CityName")%>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>  
                </Localized:TemplateColumn><%-- <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="MaxWinningsNum" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "MaxWinningsNum")%></ItemTemplate>
                </Localized:TemplateColumn>--%>
               
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="AvgRate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "AvgRate")%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="Global" HeaderTextLocalizationId="Actions">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:HyperLink runat="server" ID="hlEdit"
                            ButtonStyle="ButtonStyle2"
                            NavigateUrl='<%# FormatEditUrl(Container.DataItem) %>'
                            LocalizationClass="Global" LocalizationId="ActionEdit" />
                        <Localized:HyperLink runat="server" ID="hlDelete"
                            ButtonStyle="ButtonStyle2"
                            NavigateUrl='<%# FormatDeleteUrl(Container.DataItem) %>'
                            LocalizationClass="Global" LocalizationId="ActionDelete" />
                    </ItemTemplate>
                </Localized:TemplateColumn>
                <Localized:TemplateColumn LocalizationClass="Suppliers" HeaderTextLocalizationId="Details">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap" />
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbComments" runat="server"
                            CausesValidation="False"
                            CommandName="Comments"
                            LocalizationClass="Suppliers" LocalizationId="Comments"
                            ButtonStyle="ButtonStyle2" />
                        <Localized:LinkButton ID="lbProducts" runat="server"
                            CausesValidation="False"
                            CommandName="Products"
                            LocalizationClass="Suppliers" LocalizationId="Products"
                            ButtonStyle="ButtonStyle2" />
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
    <script type="text/javascript">
        $(window).bind('load resize', function () {
            var vWidth = $(window).width();
            var vHeight = (($(window).height() - $('.items-list').offset().top)) + 100 + 'px';
            $("#dvBodyContent").css({ 'height': vHeight, 'min-height': 'initial' });
        });
 
      
        
    </script>

</asp:Content>
