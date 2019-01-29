<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditCampaign.aspx.cs" Inherits="Snoopi.web.EditCampaign" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ContentPlaceHolderID="cphContent" Runat="Server">
          
    <asp:Panel id="pnlEditCampaign" runat="server" DefaultButton="btnSave">

         <asp:CustomValidator runat="server" LocalizationClass="Campaign" LocalizationId="NumericOnly" OnServerValidate="NumericOnly_ServerValidate" Display="none"></asp:CustomValidator>
        <asp:CustomValidator ID="dateCustVal" OnServerValidate="Date_ServerValidate" runat="server" Display="none"></asp:CustomValidator>
        <asp:CustomValidator runat="server" ID="ValidateCheckbox" OnServerValidate="ValidateCheckbox_ServerValidate" Display="none"></asp:CustomValidator>
        <asp:CustomValidator runat="server" ID="ValidateCountOrSum" OnServerValidate="ValidateCountOrSum_ServerValidate" Display="none"></asp:CustomValidator>
        <asp:HiddenField runat="server" ID="hfOriginalCampaignId" />
        <table>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblEmail" AssociatedControlID="txtCampaignName" LocalizationClass="Campaign" LocalizationId="LblCampaignName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtCampaignName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblChkIsLocked" AssociatedControlID="cbIsGift" LocalizationClass="Campaign" LocalizationId="LblIsGift"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="cbIsGift" CssClass="checkbox-wrapper" runat="server" />
                    
                </td>
            </tr>
              <tr>
                <th class="t-natural"><Localized:Label runat="Server" AssociatedControlID="txtRemarks" LocalizationClass="Campaign" LocalizationId="LblRemarks"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtRemarks" runat="server"></asp:TextBox>
                </td>
            </tr>
      
              <tr>
                <th class="t-natural"><Localized:Label runat="Server" AssociatedControlID="cbIsDiscount" LocalizationClass="Campaign" LocalizationId="LblIsDiscount"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox ID="cbIsDiscount" CssClass="checkbox-wrapper" runat="server" />
                </td>
            </tr>
             <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="Label1" AssociatedControlID="txtPrecentDiscount" LocalizationClass="Campaign" LocalizationId="LblPrecentDiscount"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPrecentDiscount" runat="server"></asp:TextBox>
                     </td>
            </tr>
             <tr>
                <th class="t-natural"><Localized:Label runat="Server" AssociatedControlID="cStartDate" LocalizationClass="Campaign" LocalizationId="LblStartDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:Calendar CssClass="input-text" ID="cStartDate" runat="server"></asp:Calendar>                    
                </td>
            </tr>

             <tr>
                <th class="t-natural"><Localized:Label runat="Server"  AssociatedControlID="cEndDate" LocalizationClass="Campaign" LocalizationId="LblEndDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:Calendar CssClass="input-text" ID="cEndDate" runat="server"></asp:Calendar>
                </td>
            </tr>
             <tr>
                <th class="t-natural"><Localized:Label runat="Server" AssociatedControlID="txtDestinationCount" LocalizationClass="Campaign" LocalizationId="LblDestinationCount"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtDestinationCount" runat="server"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <th class="t-natural"><Localized:Label runat="Server" AssociatedControlID="txtDestinationSum" LocalizationClass="Campaign" LocalizationId="LblDestinationSum"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtDestinationSum" runat="server"></asp:TextBox>                   
                </td>
            </tr>

             <tr>
                <th class="t-natural"><Localized:Label runat="Server"  AssociatedControlID="txtImplemationCount" LocalizationClass="Campaign" LocalizationId="LblImplemationCount"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtImplemationCount" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr><td colspan="3" class="t-center">
            
                <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="Campaign" LocalizationId="Save"></Localized:LinkButton>

            </td></tr>

        </table>
        <asp:HiddenField ID="hfBeforeRegisterUserName" runat="server" Visible="false" />
        
    </asp:Panel>

</asp:Content>

