<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrderDetails.aspx.cs" Inherits="Snoopi.web.OrderDetails" %>
<%@ Register Namespace="Snoopi.web.WebControls" Assembly="Snoopi.web" TagPrefix="Custom" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<!DOCTYPE html>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
        <title></title>
        <link href="resources/css/Main.css" rel="stylesheet" />
       <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>  
      
</head>
<body id="Body" runat="server">
    <input type="button" class="close" id="OrderClose" value="X" onclick="CloseOrder(this);" />
      <div class="title">
                        <Localized:Label ID="Label7"  runat="server" LocalizationClass="SupplierProfile" LocalizationId="OrderDetailsTitle" ></Localized:Label>
       </div>
            <form id="form1" runat="server">
                <div class="wrapper-span">
                    <Localized:Label ID="lblOrderIdlabel" runat="server" LocalizationClass="SupplierProfile" LocalizationId="OrderIdlabel" CssClass="black-span"></Localized:Label>
                    <Localized:Label runat="server" ID="lblOrderId" CssClass="gray-span"></Localized:Label>
                </div>
                <div class="wrapper-span">
                    <Localized:Label runat="server" ID="lblOrderDateLabel" LocalizationClass="SupplierProfile" LocalizationId="OrderDateLabel" CssClass="black-span"></Localized:Label>
                    <Localized:Label runat="server" ID="lblOrderDate" CssClass="gray-span"></Localized:Label>
                </div>
                <div class="wrapper-span">
                    <Localized:Label runat="server" ID="lblPriceLabel" LocalizationClass="SupplierProfile" LocalizationId="PriceLabel" CssClass="black-span"></Localized:Label>
                    <Localized:Label runat="server" ID="lblPrice" CssClass="gray-span"></Localized:Label>
                </div>
                <div class="wrapper-span">
                    <Localized:Label ID="lblBidIdLabel" runat="server" LocalizationClass="SupplierProfile" LocalizationId="BidIdLabel" CssClass="black-span"></Localized:Label>
                    <Localized:Label runat="server" ID="lblBidId" CssClass="gray-span"></Localized:Label>
                </div>
                 <div class="wrapper-span">
                    <Localized:Label ID="LblUserLabel" runat="server" LocalizationClass="SupplierProfile" LocalizationId="AppUserLabel" CssClass="black-span"></Localized:Label>
                    <Localized:Label runat="server" ID="LblUser" CssClass="gray-span"></Localized:Label>
                </div>
                 <div class="wrapper-span">
                    <Localized:Label ID="LblUserPhoneLabel" runat="server" LocalizationClass="SupplierProfile" LocalizationId="AppUserPhoneLabel" CssClass="black-span"></Localized:Label>
                    <Localized:Label runat="server" ID="LblUserPhone" CssClass="gray-span"></Localized:Label>
                </div>
                <div class="wrapper-span">
                <Localized:Label runat="server" ID="lblDealsIncludeLabel" LocalizationClass="SupplierProfile" LocalizationId="DealsIncludeLabel" CssClass="black-span"></Localized:Label>
                    <div class="product-wrapper">
                        <asp:ListView runat="server" ID="lvDealsInclude">
                            <ItemTemplate>
                                
                                <span><%# ProductsStrings.GetText("OrderAmount", new System.Globalization.CultureInfo("He-IL")) +":"%><%#Eval("Amount")%></span>&nbsp;	&nbsp;	                              
                                <span><%# ProductsStrings.GetText("ProductName", new System.Globalization.CultureInfo("He-IL")) +":"%><%# Eval("ProductName")%></span>&nbsp;	&nbsp;	
                              <%--  <span><%# Eval("ProductAmount") %></span>&nbsp;	&nbsp;	--%>
                                <span><%# ProductsStrings.GetText("ProductCode", new System.Globalization.CultureInfo("He-IL")) +":"%><%#Eval("ProductCode")%></span></br>
                            </ItemTemplate>
                        </asp:ListView>
                   </div>
               </div>

           </form>
</body>
</html>
