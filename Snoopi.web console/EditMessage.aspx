<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditMessage.aspx.cs" Inherits="Snoopi.web.EditMessage" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        $(document).ready(function () { 
        $("#btnSave").click(function () {
            return confirm('<%= MessagesStrings.GetText(@"MessageConfirmSend") %>');
        });
});
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" Runat="Server">
          
    <asp:Panel id="pnlEditMessage" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalMessageId" />
                 <Localized:Label ID="Label6" runat="server" BlockMode="false" LocalizationClass="Orders" LocalizationId="SendTo" />&nbsp;
         <asp:ListBox ID="ddlSendTo" runat="server" SelectionMode="Multiple" ClientIDMode="Static"></asp:ListBox><br /><br />
        <table>
            <tr>
                <th class="t-natural"><Localized:Label runat="Server" ID="lblDescription" AssociatedControlID="txtDescription" LocalizationClass="Messages" LocalizationId="MessageDescriptionLabel"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtDescription" runat="server"></asp:TextBox> <br />                  
                </td>
            </tr>
            <tr><td colspan="3" class="t-center">
            
                <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" ClientIDMode="Static" LocalizationClass="Messages" LocalizationId="SendButton"></Localized:LinkButton>

            </td></tr>

        </table>
        
    </asp:Panel>

</asp:Content>

