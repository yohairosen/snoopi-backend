<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditAppUser.aspx.cs" Inherits="Snoopi.web.EditAppUser" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphContent" Runat="Server">
          
    <asp:Panel id="pnlEditAppUser" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalAppUserId" />
        <table>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblEmail" AssociatedControlID="txtEmail" LocalizationClass="AppUsers" LocalizationId="AppUserEmailLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtEmail" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail" LocalizationClass="AppUsers" LocalizationId="AppUserEmailRequired" Display="None" 
                        ></Localized:RequiredFieldValidator><Localized:RegularExpressionValidator ID="revEml" runat="Server" 
                        LocalizationClass="AppUsers" LocalizationId="AppUserEmailInvalid" Display="None" ControlToValidate="txtEmail" 
                        ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$">*</Localized:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblChkIsLocked" AssociatedControlID="chkIsLocked" LocalizationClass="AppUsers" LocalizationId="AppUserLockedLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsLocked" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>
<%--            <tr runat="server" id="trCurrentPassword" visible="false">
                <th class="t-natural"><Localized:Label runat="Server" ID="lblCurrentPassword" AssociatedControlID="txtCurrentPassword" LocalizationClass="AppUsers" LocalizationId="AppUserCurrentPasswordLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtCurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                </td>
            </tr>--%>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblPassword" AssociatedControlID="txtPassword" LocalizationClass="AppUsers" LocalizationId="AppUserNewPasswordLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvPasswordRequired" runat="server" 
                        ControlToValidate="txtPassword" Display="None" LocalizationClass="AppUsers" LocalizationId="AppUserNewPasswordRequired"
                        >*</Localized:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblConfirmPassword" AssociatedControlID="txtConfirmPassword" LocalizationClass="AppUsers" LocalizationId="AppUserNewPasswordConfirmLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvConfirmPasswordRequired" runat="server" 
                        ControlToValidate="txtConfirmPassword" Display="None" LocalizationClass="AppUsers" LocalizationId="AppUserNewPasswordConfirmRequired"
                        >*</Localized:RequiredFieldValidator><Localized:CompareValidator ID="cvPasswordCompare" runat="server" 
                        ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword" 
                        Display="None" LocalizationClass="AppUsers" LocalizationId="AppUserNewPasswordConfirmInvalid"
                        >*</Localized:CompareValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblFirstName" AssociatedControlID="txtFirstName" LocalizationClass="AppUsers" LocalizationId="AppUserFirstNameLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtFirstName" runat="server"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblLastName" AssociatedControlID="txtLastName" LocalizationClass="AppUsers" LocalizationId="AppUserLastNameLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtlastName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblPhone" AssociatedControlID="txtPhone" LocalizationClass="AppUsers" LocalizationId="AppUserPhoneLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label ID="lblCity" runat="Server" AssociatedControlID="ddlCity" LocalizationClass="AppUsers" LocalizationId="City"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList CssClass="input-text" ID="ddlCity" runat="server" />
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblStreet" AssociatedControlID="txtStreet" LocalizationClass="AppUsers" LocalizationId="Street"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtStreet" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblHouseNum" AssociatedControlID="txtHouseNum" LocalizationClass="AppUsers" LocalizationId="HouseNum"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtHouseNum" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblFloor" AssociatedControlID="txtFloor" LocalizationClass="AppUsers" LocalizationId="FloorLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtFloor" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="Label1" AssociatedControlID="txtAptNum" LocalizationClass="AppUsers" LocalizationId="AptNumLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtAptNum" runat="server"></asp:TextBox>
                </td>
            </tr>


            
            <tr><td colspan="3" class="t-center">
            
                <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="AppUsers" LocalizationId="SaveButton"></Localized:LinkButton>

            </td></tr>

        </table>
        <asp:HiddenField ID="hfBeforeRegisterAppUserName" runat="server" Visible="false" />
        
    </asp:Panel>

</asp:Content>

