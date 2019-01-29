<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AppUsers.aspx.cs" Inherits="Snoopi.web.AppUsers" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentAppUsers" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel runat="server" DefaultButton="btnSearch">
        <asp:HiddenField runat="server" ID="hfIsSearchActive" Value="false" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="SearchName" />&nbsp;
        <asp:TextBox ID="txtSearchName" runat="server" />
        <Localized:Label ID="Label1" runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="SearchPhone" />&nbsp;
        <asp:TextBox ID="txtSearchPhone" runat="server" />
         <Localized:Label ID="Label2" runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="SearchCreateDate" />&nbsp;
        <input type="text" id="dpSearchCreateDateFrom" runat="server" clientidmode="Static" readonly="readonly" />
         <Localized:Label ID="Label3" runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="To" />&nbsp;
        <input type="text" id="dpSearchCreateDateTo" runat="server" clientidmode="Static" readonly="readonly" />
        <Localized:LinkButton runat="server" ID="btnSearch"  OnClick="btnSearch_Click" CssClass="button-02" LocalizationClass="AppUsers" LocalizationId="SearchButton"/>
        <Localized:LinkButton runat="server" ID="btnExport" CssClass="button-02" OnClick="btnExport_Click" LocalizationClass="AppUsers" LocalizationId="ExportButton"></Localized:LinkButton>
    </asp:Panel>
    <br />
    <asp:PlaceHolder runat="server" ID="phHasItems" Visible="true">
        <asp:HiddenField runat="server" ID="hfCurrentPageIndex_dgAppUsers" Value="0" />
        <Localized:Label runat="server" BlockMode="false" LocalizationClass="AppUsers" LocalizationId="AppUsersCountLabel" />&nbsp;<asp:Label ID="lblTotal" runat="server" />&nbsp; 
        
        <br />
        <asp:DataGrid CssClass="items-list" ID="dgAppUsers" runat="server" UseAccessibleHeader="true"
            AutoGenerateColumns="false" AllowCustomPaging="true" OnItemCommand="dgAppUsers_ItemCommand"
            AllowPaging="true" PageSize="30" EnableViewState="false" ClientIDMode="Static" DataKeyField="AppUserId">

            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                 <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="UserId" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural  va-middle" />
                    <ItemTemplate><%# (DataBinder.Eval(Container.DataItem, "AppUserId")).ToString()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Email" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural  va-middle" />
                    <ItemTemplate><a href="mailto:<%# ((string)DataBinder.Eval(Container.DataItem, "Email")).ToHtml()%>"><%# ((string)DataBinder.Eval(Container.DataItem, "Email")).ToHtml()%></a></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="FirstName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "FirstName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="LastName" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "LastName")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="IsLocked" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsLocked"))%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Phone" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((string)DataBinder.Eval(Container.DataItem, "Phone")).ToHtml()%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="Address" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate>
                        <%# ((string)DataBinder.Eval(Container.DataItem, "Street")).ToHtml()%> <%# ((string)DataBinder.Eval(Container.DataItem, "HouseNum")).ToHtml() %> <br />
                        <%# AppUsersStrings.GetText(@"Floor")%> <%# ((string)DataBinder.Eval(Container.DataItem, "Floor")).ToHtml()%> 
                        <%# AppUsersStrings.GetText(@"ApartmentNumber")%> <%# ((string)DataBinder.Eval(Container.DataItem, "ApartmentNumber")).ToHtml()%> <br />
                        <%# ((string)DataBinder.Eval(Container.DataItem, "CityName")).ToHtml() %>
                    </ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="IsAdv" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# GlobalStrings.GetYesNo((bool)DataBinder.Eval(Container.DataItem, "IsAdv"))%></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="LastLogin" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "LastLogin")%></span></ItemTemplate>
                </Localized:TemplateColumn>

               <Localized:TemplateColumn LocalizationClass="AppUsers" HeaderTextLocalizationId="CreateDate" ItemStyle-CssClass="ltr">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center va-middle" />
                    <ItemTemplate><span class="dgDateManager"><%# DataBinder.Eval(Container.DataItem, "CreateDate")%></span></ItemTemplate>
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
                        <Localized:LinkButton ID="lbOrder" runat="server"
                            CausesValidation="False"
                            CommandName="Order"
                            LocalizationClass="AppUsers" LocalizationId="ActionOrder"
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

            $(function () {
                $("#dpSearchCreateDateFrom,#dpSearchCreateDateTo").datepicker({ dateFormat: 'dd/mm/yy' }).val();

            });

    </script>

</asp:Content>
