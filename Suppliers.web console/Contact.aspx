<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="Contact" MasterPageFile="~/SuppliersTemplate.master" %>


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
        <Localized:Label ID="Label1" runat="server" LocalizationClass="Contact" LocalizationId="ContactTitle" ></Localized:Label>
    </div>
    <div class="sub-title">
         <Localized:Label ID="Label4" runat="server" LocalizationClass="Contact" LocalizationId="ContactDescription" ></Localized:Label>
    </div>


    <div class="contact-wrapper">
            <div class="wrapper-in">
            <div class="wrapper-field">
                <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="FirstName" CssClass="required"></Localized:Label>
                <Localized:TextBox ID="txtFirstName" runat="server" ></Localized:TextBox>
                <asp:CustomValidator ID="revEml" runat="Server" Display="None" ControlToValidate="txtEmail" LocalizationClass="SupplierProfile" OnServerValidate="revEml_ServerValidate" LocalizationId="EmailWrong" ValidationExpression="/^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i"></asp:CustomValidator>
                <Localized:RequiredFieldValidator runat="server" LocalizationClass="Contact" LocalizationId="FirstNameReq"
                        ControlToValidate="txtFirstName" Display="None"
                        >*</Localized:RequiredFieldValidator>
            </div>
            <div class="wrapper-field">
                <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="Email" CssClass="required"></Localized:Label>
                <Localized:TextBox ID="txtEmail" runat="server" ></Localized:TextBox>
                <Localized:RequiredFieldValidator runat="server" LocalizationClass="Contact" LocalizationId="EmailReq"
                        ControlToValidate="txtEmail" Display="None"
                        >*</Localized:RequiredFieldValidator>
            </div>
            <div class="wrapper-field">
                <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="Phone"></Localized:Label>
                <Localized:TextBox ID="txtPhone" runat="server" ></Localized:TextBox>
            </div>
        </div>
        <div class="wrapper-in">
            <div class="wrapper-field">
                <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="UploadImage"></Localized:Label>                
                <div class="fileinputs">
                    <asp:FileUpload ID="FUploadImage" runat="server" CssClass="file" accept="image/*"></asp:FileUpload>
	               
	                <div class="fakefile">
		                <input type="text" readonly/>
		                <input type="button" />
	                </div>
                </div>
            </div>

            <div class="wrapper-field">
                <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="ContactType"></Localized:Label>
                <asp:DropDownList runat="server" ID="ddlContactType"></asp:DropDownList>
            </div>

            <div class="wrapper-field">
                <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="ContactDetails"></Localized:Label>
                <Localized:TextBox ID="txtContactDetails" runat="server" TextMode="MultiLine"></Localized:TextBox>
            </div>


        </div>
        <div class="wrapper-in last">
            <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="Problem"></Localized:Label>
            <Localized:Label runat="server" LocalizationClass="Contact" LocalizationId="ContactUs"></Localized:Label>
            <Localized:Label ID="lblPhone" runat="server"></Localized:Label>
        </div>
        <div class="wrapper-down">
            <Localized:Button runat="server" ID="btnSend" LocalizationClass="Contact" LocalizationId="Send" OnClick="btnSend_Click"/>

        </div>

    </div>

    </asp:Content>