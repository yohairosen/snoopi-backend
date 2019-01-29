<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SuppliersTemplate.master" CodeFile="DealsHistory.aspx.cs" Inherits="DealsHistory" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Import Namespace="Snoopi.core.BL" %>
<%@ Import Namespace="Snoopi.core.DAL" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>

<asp:Content ID="cHead" runat="server" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content ID="cContent" runat="server" ContentPlaceHolderID="cphContent">
    <div class="title">
        <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="OrderTitle" ></Localized:Label>
    </div>
    <div class="sub-title">
         <Localized:Label ID="Label4" runat="server" LocalizationClass="SupplierProfile" LocalizationId="OrderDescription" ></Localized:Label>
    </div>

     <div class="wrapper-order-search">

          <Localized:Label ID="Label6" runat="server" LocalizationClass="SupplierProfile" LocalizationId="SortLabel" CssClass="label frst"></Localized:Label>
      
         <Localized:Label ID="Label2" runat="server" LocalizationClass="SupplierProfile" LocalizationId="YearLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlyear" AutoPostBack="true"  EnableViewState="true">
        </asp:DropDownList>

         <Localized:Label ID="Label1" runat="server" LocalizationClass="SupplierProfile" LocalizationId="MonthLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlMonth" AutoPostBack="true"  OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged" EnableViewState="true">
        </asp:DropDownList>

         <Localized:Label ID="Label3" runat="server" LocalizationClass="SupplierProfile" LocalizationId="DayFromLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlDayFrom" AutoPostBack="true" Enabled="false" EnableViewState="true" CssClass="ddl-day">
        </asp:DropDownList>
    
         <Localized:Label ID="Label5" runat="server" LocalizationClass="SupplierProfile" LocalizationId="DayToLabel" CssClass="label"></Localized:Label>
        <asp:DropDownList runat="server" ID="ddlDayTo" AutoPostBack="true"  Enabled="false" EnableViewState="true" CssClass="ddl-day">
        </asp:DropDownList>
 
  </div>
 
   <asp:DataGrid CssClass="items-list" ID="dgOrders" runat="server" UseAccessibleHeader="true"  
            AutoGenerateColumns="false" AllowCustomPaging="false"            
            ClientIDMode="Static"
            AllowPaging="false"  EnableViewState="true"
            DataKeyField="OrderId"
            AllowSorting="True" ShowFooter="true" OnItemDataBound="dgOrders_ItemDataBound">
            <HeaderStyle CssClass="header" />
            <AlternatingItemStyle CssClass="alt" />
            <ItemStyle CssClass="row" />
            <FooterStyle CssClass="footer-table" />     
                <PagerStyle Mode="NumericPages" CssClass="paging" />
            <Columns>
                <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:LinkButton LocalizationClass="SupplierProfile" LocalizationId="OrderId" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort" 
                            CommandArgument="OrderId" CssClass="sortLnk"></Localized:LinkButton>    
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "OrderId")%></ItemTemplate>
                </Localized:TemplateColumn>
                     <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:LinkButton LocalizationClass="SupplierProfile" LocalizationId="OrderDate" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort" 
                            CommandArgument="OrderDate" CssClass="sortLnk"></Localized:LinkButton>
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# ((DateTime)DataBinder.Eval(Container.DataItem, "OrderDate")).ToShortDateString().ToHtml()%></ItemTemplate>
                     </Localized:TemplateColumn>
                 <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:LinkButton LocalizationClass="SupplierProfile" LocalizationId="BidId" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort" 
                            CommandArgument="AnimalName" CssClass="sortLnk"></Localized:LinkButton>
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "BidId")%></ItemTemplate>
                </Localized:TemplateColumn>
                
                  <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:LinkButton  LocalizationClass="SupplierProfile" LocalizationId="TotalPrice" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort" 
                            CommandArgument="TotalPrice" CssClass="sortLnk"></Localized:LinkButton>
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><%# (((decimal)DataBinder.Eval(Container.DataItem, "Price")).ToString() +" "+ SupplierProfileStrings.GetText("Shekel")).ToHtml()%> </ItemTemplate>
                       </Localized:TemplateColumn>
                  <Localized:TemplateColumn>
                    <HeaderTemplate>
                        <Localized:LinkButton LocalizationClass="SupplierProfile" LocalizationId="ApprovedDeal" ItemStyle-CssClass="ltr" runat="server" CommandName="Sort" 
                            CommandArgument="ApprovedDeal" CssClass="sortLnk"></Localized:LinkButton>
                    </HeaderTemplate>
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-natural va-middle" />
                    <ItemTemplate><div class='<%# (((bool)DataBinder.Eval(Container.DataItem, "IsPayed")) == true ? "vi-class" : "no-class")%>'></div></ItemTemplate>
                </Localized:TemplateColumn>

                <Localized:TemplateColumn LocalizationClass="SupplierProfile" HeaderTextLocalizationId="OrderDetails">
                    <HeaderStyle CssClass="t-center" />
                    <ItemStyle CssClass="t-center nowrap va-middle" />
                    <ItemTemplate>
                        <Localized:LinkButton ID="lbMore" runat="server"
                            CausesValidation="False" href="#"
                            LocalizationClass="SupplierProfile" LocalizationId="ClickDetails" CssClass="btn-more"  OnClientClick="OrderDetails(this)" OrderId='<%# DataBinder.Eval(Container.DataItem, "OrderId")%>'/>
                    </ItemTemplate>
                </Localized:TemplateColumn>
                      
            </Columns>
      
        </asp:DataGrid>
    <div class="wrapper-down-order">
        <div>
             <Localized:Button ID="btnExcel" LocalizationClass="SupplierProfile" LocalizationId="ExportExcel" runat="server" CssClass="button btn-excel" OnClick="btnExport_Click"/>
             <Localized:Button ID="btnPrint" runat="server" LocalizationClass="SupplierProfile" LocalizationId="Print" CssClass="button btn-print" OnClientClick="printDiv()"/>          
        </div>
     </div>
               
    <asp:Panel runat="server"  CssClass="popup-black" ID="pnlWrapperPopup"></asp:Panel>
    <asp:Panel runat="server"  CssClass="inside-popup" ID="Popup" ClientIDMode="Static">
    </asp:Panel>

</asp:Content>
