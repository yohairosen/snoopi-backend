<%@ Page Language="C#" MasterPageFile="~/SuppliersTemplate.master" AutoEventWireup="true" CodeFile="MyProfile.aspx.cs" Inherits="Snoopi.web.MyProfile" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/SuppliersTemplate.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Import Namespace="dg.Utilities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <div class="title">
        <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="MyProfilePageTitle"></Localized:Label>
    </div>
    <div class="sub-title-profile">
        <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="MyProfileDesc"></Localized:Label>
    </div>
    <div class="body-wrapper">
        <div class="first-wrapper my-profile">
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="businessLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtbusiness" ReadOnly="true"></Localized:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="ContactNameLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtContactName" ReadOnly="true" Length="Long"></Localized:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PhoneContactLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtContactPhone" ReadOnly="true"></Localized:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="EmailLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtEmail" ReadOnly="true"></Localized:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="PhoneLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtPhone" ReadOnly="true"></Localized:TextBox>
            </div>
            <div id="dvDescription" runat="server" class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="Description"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtDescription" ReadOnly="true" TextMode="MultiLine" Height="74px" Width="200px"></Localized:TextBox>
            </div>
            <div id="dvDiscount" runat="server" class="field-wrapper">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="Discount"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtDiscount" ReadOnly="true" TextMode="MultiLine" Height="74px" Width="200px"></Localized:TextBox>
            </div>

        </div>
        <div class="second-wrapper my-profile">
            <div class="field-wrapper">
                <Localized:Label ID="Label1" runat="server" LocalizationClass="SupplierProfile" LocalizationId="CityLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtCity" ReadOnly="true" Length="Long"></Localized:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label ID="Label2" runat="server" LocalizationClass="SupplierProfile" LocalizationId="StreetLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtStreet" ReadOnly="true" Length="Long"></Localized:TextBox>
            </div>
            <div class="field-wrapper">
                <Localized:Label ID="Label3" runat="server" LocalizationClass="SupplierProfile" LocalizationId="NumberLabel"></Localized:Label>
                <Localized:TextBox runat="server" ID="txtNumber" ReadOnly="true"></Localized:TextBox>
            </div>
            <div id="dvProfileImage" runat="server" class="field-wrapper">
                <Localized:Label ID="Label5" runat="server" LocalizationClass="SupplierProfile" LocalizationId="ProfileImage"></Localized:Label>
                <asp:Image ID="imgImage" runat="server" CssClass="image-small" Height="180px" />
            </div>
        </div>
        <div class="third-wrapper my-profile-3">
             <div id="dvProductsSupplierCities" runat="server">
                <Localized:Label ID="Label4" runat="server" LocalizationClass="SupplierProfile" LocalizationId="CitySupplied"></Localized:Label>
                <div class="city-wrapper">
                    <asp:ListView ID="lvCity" runat="server">
                        <ItemTemplate>
                            <div class="city-name-wrapper">
                                <img src="resources/images/vi.png" /><span><%# Eval("CityName") %></span></div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
            <div id="dvHomeServiceCities" runat="server">
                <Localized:Label runat="server" LocalizationClass="SupplierProfile" LocalizationId="CityHomeService"></Localized:Label>
                <div class="city-wrapper">
                    <asp:ListView ID="lvHomeCity" runat="server">
                        <ItemTemplate>
                            <div class="city-name-wrapper">
                                <img src="resources/images/vi.png" /><span><%# Eval("CityName") %></span></div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>

        </div>
        <div class="wrapper-down">
            <Localized:LinkButton ID="btnSave" LocalizationClass="SupplierProfile" LocalizationId="Edit" runat="server" CssClass="edit-btn button" PostBackUrl="EditMyProfile.aspx" /></div>
    </div>



</asp:Content>
