<%@ Page Language="C#" MasterPageFile="Template.master" CodeFile="EditAd.aspx.cs" Inherits="EditAd" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.web.Localization" %>


<asp:Content ContentPlaceHolderID="cphHead" ID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="Server">
    <script>
        $(document).ready(function () {
            $('.file-upload').on('change', function (evt) {
                if (this.files.length > 0 && (this.files[0].size / 1024 / 1024) > 4) {
                    $("#IsValid").val("false");
                } else {
                    $("#IsValid").val("true");
                }
                console.log(this.files[0].size);
            });
        });

        function Validate() {
            if ($("#IsValid").val() == "true") return true;
            alert('<%= SystemSettingsStrings.GetText("FileSize")%>');
        return false;
    }
    </script>
    <input type="hidden" id="IsValid" value="true" />
    <asp:Panel ID="pnlEditAd" runat="server" DefaultButton="btnSave">
        <asp:HiddenField runat="server" ID="hfOriginalAdId" />
        <table>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblBusinessName" AssociatedControlID="ddlBusinessName" LocalizationClass="Suppliers" LocalizationId="BusinessName"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList ID="ddlBusinessName" CssClass="input-text" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFromDate" AssociatedControlID="txtFromDate" LocalizationClass="Advertisements" LocalizationId="FromDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtFromDate" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblToDate" AssociatedControlID="txtToDate" LocalizationClass="Advertisements" LocalizationId="ToDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtToDate" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblBunner" AssociatedControlID="ddlBunner" LocalizationClass="Advertisements" LocalizationId="Bunner">
                    </Localized:Label>
                </th>
                <td class="nowrap">
                    <asp:DropDownList CssClass="input-text" ID="ddlBunner" runat="server"></asp:DropDownList>
                </td>
            </tr>
             <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label1" AssociatedControlID="href" LocalizationClass="Advertisements" LocalizationId="href"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="href" runat="server"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFile" AssociatedControlID="fuImage" LocalizationClass="Advertisements" LocalizationId="File"></Localized:Label></th>
                <td class="nowrap">
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="file-upload" />
                    <asp:Image ID="HomeImage" runat="server" CssClass="image-small" />
                    <Localized:Button ID="btnDeleteImage" FileUploadName="fuImage" ImageName="HomeImage" runat="server" OnClientClick="return Validate();" CssClass="button-02" LocalizationClass="Advertisements" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" CausesValidation="false" />
                </td>
            </tr>
            <tr>
                <td colspan="3" class="t-center">
                    <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="Advertisements" LocalizationId="SaveAd"></Localized:LinkButton>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfBeforeRegisterAdName" runat="server" Visible="false" />
    </asp:Panel>
    <script type="text/javascript">
        $(function () {
            $('#<%= txtFromDate.ClientID%>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
            $('#<%= txtToDate.ClientID%>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
        });

    </script>
</asp:Content>
