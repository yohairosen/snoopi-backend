<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditUser.aspx.cs" Inherits="Snoopi.web.EditUser" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphContent" Runat="Server">
          
    <asp:Panel id="pnlEditUser" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalUserId" />
        <table>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblEmail" AssociatedControlID="txtEmail" LocalizationClass="Users" LocalizationId="UserEmailLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtEmail" runat="server"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail" LocalizationClass="Users" LocalizationId="UserEmailRequired" Display="None" 
                        ></Localized:RequiredFieldValidator><Localized:RegularExpressionValidator ID="revEml" runat="Server" 
                        LocalizationClass="Users" LocalizationId="UserEmailInvalid" Display="None" ControlToValidate="txtEmail" 
                        ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$">*</Localized:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblChkIsLocked" AssociatedControlID="chkIsLocked" LocalizationClass="Users" LocalizationId="UserLockedLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="chkIsLocked" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="trCurrentPassword" visible="false">
                <th class="t-natural"><Localized:Label runat="Server" ID="lblCurrentPassword" AssociatedControlID="txtCurrentPassword" LocalizationClass="Users" LocalizationId="UserCurrentPasswordLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtCurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblPassword" AssociatedControlID="txtPassword" LocalizationClass="Users" LocalizationId="UserNewPasswordLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvPasswordRequired" runat="server" 
                        ControlToValidate="txtPassword" Display="None" LocalizationClass="Users" LocalizationId="UserNewPasswordRequired"
                        >*</Localized:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblConfirmPassword" AssociatedControlID="txtConfirmPassword" LocalizationClass="Users" LocalizationId="UserNewPasswordConfirmLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <Localized:RequiredFieldValidator ID="rfvConfirmPasswordRequired" runat="server" 
                        ControlToValidate="txtConfirmPassword" Display="None" LocalizationClass="Users" LocalizationId="UserNewPasswordConfirmRequired"
                        >*</Localized:RequiredFieldValidator><Localized:CompareValidator ID="cvPasswordCompare" runat="server" 
                        ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword" 
                        Display="None" LocalizationClass="Users" LocalizationId="UserNewPasswordConfirmInvalid"
                        >*</Localized:CompareValidator>
                </td>
            </tr>
<%--            <tr runat="server" id="trPermissions"><td colspan="3" class="t-natural">
            
                <Localized:Panel ID="pnlPermissions" runat="server" LocalizationClass="Users" LocalizationId="PermissionsPanel">
                    <asp:Repeater ID="rptrPermissions" runat="server">
                    <ItemTemplate>
                    <asp:CheckBox runat="server" CssClass="checkbox-wrapper" id="chkPermission" data-permission="<%# Container.DataItem %>" text="<%# PermissionDescription(Container.DataItem) %>" checked="<%# UserHasPermission(Container.DataItem)%>" />
                    <br/>
                    </ItemTemplate>
                    </asp:Repeater>
                </Localized:Panel>

            </td></tr>--%>
            <tr><td colspan="3" class="t-center">
            
                <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="Users" LocalizationId="SaveButton"></Localized:LinkButton>

            </td></tr>

        </table>
        <asp:HiddenField ID="hfBeforeRegisterUserName" runat="server" Visible="false" />
        
    </asp:Panel>

</asp:Content>

