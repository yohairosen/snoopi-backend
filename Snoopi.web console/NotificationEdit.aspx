<%@ Page Language="C#" MasterPageFile="Template.master" CodeFile="NotificationEdit.aspx.cs" Inherits="NotificationEdit" %>

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
                    <Localized:Label runat="Server" ID="lblName" AssociatedControlID="txtName" LocalizationClass="Notifications" LocalizationId="Name"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFilteringGroup" AssociatedControlID="ddlFilteringGroup" LocalizationClass="Notifications" LocalizationId="FilteringGroup"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList ID="ddlFilteringGroup" CssClass="input-text" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFromDate" AssociatedControlID="txtFromDate" LocalizationClass="Notifications" LocalizationId="FromDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtFromDate" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblToDate" AssociatedControlID="txtToDate" LocalizationClass="Notifications" LocalizationId="ToDate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtToDate" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblArea" AssociatedControlID="ddlArea" LocalizationClass="Notifications" LocalizationId="Area">
                    </Localized:Label>
                </th>
                <td class="nowrap">
                    <asp:DropDownList CssClass="input-text" ID="ddlArea" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblMinFrequency" AssociatedControlID="txtMinFrequency" LocalizationClass="Notifications" LocalizationId="MinFrequency"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtMinFrequency" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblMaxFrequency" AssociatedControlID="txtMaxFrequency" LocalizationClass="Notifications" LocalizationId="MaxFrequency"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtMaxFrequency" runat="server"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblAnimalType" AssociatedControlID="ddlAnimalType" LocalizationClass="Notifications" LocalizationId="AnimalType"></Localized:Label></th>
                <td class="nowrap">
                    <asp:DropDownList CssClass="input-text" ID="ddlAnimalType" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblPriority" AssociatedControlID="txtPriority" LocalizationClass="Notifications" LocalizationId="Priority"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtPriority" runat="server"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblMessage" AssociatedControlID="txtMessage" LocalizationClass="Notifications" LocalizationId="MessageTemplate"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtMessage" runat="server" TextMode="multiline" Columns="10"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblIsAuto" AssociatedControlID="cbIsAuto" LocalizationClass="Notifications" LocalizationId="IsAuto"></Localized:Label></th>
                <td class="nowrap">
                    <asp:CheckBox CssClass="input-text" ID="cbIsAuto" runat="server"></asp:CheckBox>
                </td>
            </tr>

            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblRunEvery" AssociatedControlID="txtRunEvery" LocalizationClass="Notifications" LocalizationId="RunEvery"></Localized:Label></th>
                <td class="nowrap">
                    <asp:TextBox CssClass="input-text" ID="txtRunEvery" runat="server"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="lblFile" AssociatedControlID="fuImage" LocalizationClass="Notifications" LocalizationId="File"></Localized:Label></th>
                <td class="nowrap">
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="file-upload" />
                    <asp:Image ID="HomeImage" runat="server" CssClass="image-small" />
                    <Localized:Button ID="btnDeleteImage" FileUploadName="fuImage" ImageName="HomeImage" runat="server" OnClientClick="return Validate();" CssClass="button-02" LocalizationClass="Advertisements" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" CausesValidation="false" />
                </td>
            </tr>
            <tr>
                <td class="t-center">
                    <Localized:LinkButton runat="server" ID="btnCheckUsersNumber" CssClass="button" OnClick="checkUserNumber_Click" LocalizationClass="Notifications" LocalizationId="CheckHowManyUsers"></Localized:LinkButton>
                </td>
                <td class="nowrap">
                    <asp:Label ID="lblNumOfUsers" runat="server"></asp:Label>
                </td>

            </tr>
            <tr>

                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label1" AssociatedControlID="fuImage" LocalizationClass="Notifications" LocalizationId="webUsers"></Localized:Label></th>

                <td class="nowrap">
                    <asp:Label ID="lblwebUsers" runat="server"></asp:Label>
                </td>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label4" AssociatedControlID="fuImage" LocalizationClass="Notifications" LocalizationId="androidUsers"></Localized:Label></th>

                <td class="nowrap">
                    <asp:Label ID="lblAndroidUsers" runat="server"></asp:Label>
                </td>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label5" AssociatedControlID="fuImage" LocalizationClass="Notifications" LocalizationId="appleUsers"></Localized:Label></th>

                <td class="nowrap">
                    <asp:Label ID="lblAppleUsers" runat="server"></asp:Label>
                </td>
                <th class="t-natural">
                    <Localized:Label runat="Server" ID="Label3" AssociatedControlID="fuImage" LocalizationClass="Notifications" LocalizationId="tempUsers"></Localized:Label></th>

                <td class="nowrap">
                    <asp:Label ID="lblTempUsers" runat="server"></asp:Label>
                </td>
               


            </tr>
            <tr>
                <td colspan="3" class="t-center">
                    <Localized:LinkButton runat="server" ID="btnSave" CssClass="button" OnClick="btnSave_Click" LocalizationClass="Notifications" LocalizationId="SaveAd"></Localized:LinkButton>
                </td>

            </tr>
        </table>
    </asp:Panel>
    <script type="text/javascript">
        $(function () {
            $('#<%= txtFromDate.ClientID%>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
            $('#<%= txtToDate.ClientID%>').datepicker({ dateFormat: 'dd/mm/yy' }).val();
        });

    </script>
</asp:Content>
