<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditSupplier.aspx.cs" Inherits="Snoopi.web.EditSupplier" MasterPageFile="Template.master" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">

    <asp:Panel ID="pnlEditSupplier" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalSupplierId" />
        <table>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblBusinessName" AssociatedControlID="txtBusinessName" LocalizationClass="Suppliers" LocalizationId="BusinessName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtBusinessName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblEmail" AssociatedControlID="txtEmail" LocalizationClass="Suppliers" LocalizationId="Email"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtEmail" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvEmail" runat="server"
                        ControlToValidate="txtEmail" LocalizationClass="Suppliers" LocalizationId="SupplierEmailRequired" Display="None"></Localized:RequiredFieldValidator>
                    <Localized:RegularExpressionValidator ID="revEml" runat="Server"
                        LocalizationClass="Suppliers" LocalizationId="SupplierEmailInvalid" Display="None" ControlToValidate="txtEmail"
                        ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$">*</Localized:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblIsService" AssociatedControlID="ddlIsProduct" LocalizationClass="Suppliers" LocalizationId="SupplierType"></Localized:Label></th>
                <td class="nowrap">
                     <asp:DropDownList runat="Server" id="ddlIsProduct" OnSelectedIndexChanged="ddlIsProduct_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList><br /><br />
                </td>
            </tr>

           <%-- <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblIsProduct" AssociatedControlID="chkIsProduct" LocalizationClass="Suppliers" LocalizationId="IsProduct"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsProduct" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblIsService" AssociatedControlID="chkIsService" LocalizationClass="Suppliers" LocalizationId="IsService"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsService" CssClass="checkbox-wrapper" runat="server" OnCheckedChanged="chkIsService_CheckedChanged" AutoPostBack="true" />
                </td>
            </tr>--%>
             <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblIsPremium" AssociatedControlID="chkIsPremium" LocalizationClass="Suppliers" LocalizationId="IsPremium"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsPremium" CssClass="checkbox-wrapper" runat="server"  AutoPostBack="true" />
                </td>
            </tr>
            <tr id="services" runat="server" visible="false">
                <th class="t-natural">
                    <Localized:Label ID="lblServices" runat="Server" LocalizationClass="Suppliers" LocalizationId="Services"></Localized:Label></th>
                <td class="nowrap">
                    <asp:ListBox ID="ddlServices" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblPhone" AssociatedControlID="txtPhone" LocalizationClass="Suppliers" LocalizationId="Phone"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblContactName" AssociatedControlID="txtContactName" LocalizationClass="Suppliers" LocalizationId="ContactName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtContactName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblContactPhone" AssociatedControlID="txtContactPhone" LocalizationClass="Suppliers" LocalizationId="ContactPhone"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtContactPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblChkIsLocked" AssociatedControlID="chkIsLocked" LocalizationClass="Suppliers" LocalizationId="IsLocked"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsLocked" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>
            <%--            <tr runat="server" id="trCurrentPassword" visible="false">
                <th class="t-natural"><Localized:Label runat="Server" ID="lblCurrentPassword" AssociatedControlID="txtCurrentPassword" LocalizationClass="Suppliers" LocalizationId="SupplierCurrentPasswordLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtCurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                </td>
            </tr>--%>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblPassword" AssociatedControlID="txtPassword" LocalizationClass="Suppliers" LocalizationId="SupplierNewPasswordLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPassword" runat="server" Placeholder="****" TextMode="Password"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvPasswordRequired" runat="server"
                        ControlToValidate="txtPassword" Display="None" LocalizationClass="Suppliers" LocalizationId="SupplierNewPasswordRequired">*</Localized:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblConfirmPassword" AssociatedControlID="txtConfirmPassword" LocalizationClass="Suppliers" LocalizationId="SupplierNewPasswordConfirmLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtConfirmPassword" runat="server" Placeholder="****" TextMode="Password"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvConfirmPasswordRequired" runat="server"
                        ControlToValidate="txtConfirmPassword" Display="None" LocalizationClass="Suppliers" LocalizationId="SupplierNewPasswordConfirmRequired">*</Localized:RequiredFieldValidator>
                    <Localized:CompareValidator ID="cvPasswordCompare" runat="server"
                        ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword"
                        Display="None" LocalizationClass="Suppliers" LocalizationId="SupplierNewPasswordConfirmInvalid">*</Localized:CompareValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label ID="lblCity" runat="Server" AssociatedControlID="ddlCity" LocalizationClass="Suppliers" LocalizationId="City"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList CssClass="input-text" ID="ddlCity" runat="server" />
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblStreet" AssociatedControlID="txtStreet" LocalizationClass="Suppliers" LocalizationId="Street"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtStreet" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblHouseNum" AssociatedControlID="txtContactName" LocalizationClass="Suppliers" LocalizationId="HouseNum"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtHouseNum" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblPrecent" AssociatedControlID="txtPrecent" LocalizationClass="Suppliers" LocalizationId="Precent"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPrecent" runat="server"></asp:TextBox>
                    <asp:RangeValidator ID="rangeValidatorTxtPrecent" ControlToValidate="txtPrecent" Type="Integer" MaximumValue="100" MinimumValue="0" ForeColor="Red" runat="server" Display="None"></asp:RangeValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblSumPerMonth" AssociatedControlID="txtSumPerMonth" LocalizationClass="Suppliers" LocalizationId="SumPerMonth"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtSumPerMonth" runat="server"></asp:TextBox>
                    <Localized:RegularExpressionValidator ID="RegularExpressionValidatorSumPerMonth" runat="Server"
                        LocalizationClass="Suppliers" LocalizationId="SupplierSumPerMonthInvalid" Display="None" ControlToValidate="txtSumPerMonth"
                        ValidationExpression="^\d+$">*</Localized:RegularExpressionValidator>
                </td>
            </tr>
          <%--  <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblAllowChangeStatusJoinBid" AssociatedControlID="chkAllowChangeStatusJoinBid" LocalizationClass="Suppliers" LocalizationId="AllowChangeStatusJoinBid"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkAllowChangeStatusJoinBid" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>--%>
           <%-- <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblStatusJoinBid" AssociatedControlID="chkIsStatusJoinBid" LocalizationClass="Suppliers" LocalizationId="StatusJoinBid"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsStatusJoinBid" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>--%>
           <%-- <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblMaxWinningsNum" AssociatedControlID="txtMaxWinningsNum" LocalizationClass="Suppliers" LocalizationId="MaxWinningsNum"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtMaxWinningsNum" runat="server"></asp:TextBox>
                    <Localized:RegularExpressionValidator ID="RegularExpressionValidatortxtMaxWinningsNum" runat="Server"
                        LocalizationClass="Suppliers" LocalizationId="SupplierTxtMaxWinningsNumInvalid" Display="None" ControlToValidate="txtMaxWinningsNum"
                        ValidationExpression="^\d+$">*</Localized:RegularExpressionValidator>
                </td>
            </tr>--%>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label1" AssociatedControlID="txtMastercardCode" LocalizationClass="Suppliers" LocalizationId="MastercardCode"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtMastercardCode" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                        ControlToValidate="txtMastercardCode" LocalizationClass="Suppliers" LocalizationId="SupplierMastercardCodeRequired" Display="None"></Localized:RequiredFieldValidator>
                    <Localized:RegularExpressionValidator ID="RegularExpressionValidatortxtMastercardCode" runat="Server"
                        LocalizationClass="Suppliers" LocalizationId="SupplierMastercardCodeInvalid" Display="None" ControlToValidate="txtMastercardCode"
                         ValidationExpression="^\d{7}">*</Localized:RegularExpressionValidator>
                </td>
            </tr>

            <tr>
                <td colspan="3" class="t-center">
                    <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="Suppliers" LocalizationId="SaveButton"></Localized:LinkButton>
                </td>
            </tr>

        </table>
        <asp:HiddenField ID="hfBeforeRegisterSupplierName" runat="server" Visible="false" />

    </asp:Panel>

</asp:Content>

