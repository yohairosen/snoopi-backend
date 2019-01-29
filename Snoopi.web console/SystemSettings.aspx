<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SystemSettings.aspx.cs" Inherits="Snoopi.web.EditSystemSettings" MasterPageFile="Template.master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>
<%@ Register Src="~/SettingsEmailTemplateChoiceControl.ascx" TagPrefix="Custom" TagName="SettingsEmailTemplateChoiceControl" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHead">

  
<script type="text/javascript">
    // <![CDATA[ 

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

    function Validate()
    {
        if ($("#IsValid").val() == "true") return true;
        alert('<%= SystemSettingsStrings.GetText("FileSize")%>');
        return false;
    }

    function updateMailSettingsHostName() {
        var hn = dgTools.$('<%= txtMailServerHostName.ClientID %>');
        if (hn == null) return;
        var ssl = dgTools.$('<%= chkMailServerSsl.ClientID %>');
        if (ssl == null) return;
        var port = dgTools.$('<%= txtMailServerPort.ClientID %>');
        if (port == null) return;

        if (hn.value.trim().match(/^\s*smtp\.gmail\.com\s*$/i) && ssl.checked && (port.value == '25' || port.value == '' || port.value == '587' || port.value == '465')) {
            port.value = '587';
        }
    }
    function updateMailSettingsSsl() {
        var hn = dgTools.$('<%= txtMailServerHostName.ClientID %>');
        if (hn == null) return;
        var ssl = dgTools.$('<%= chkMailServerSsl.ClientID %>');
        if (ssl == null) return;
        var port = dgTools.$('<%= txtMailServerPort.ClientID %>');
        if (port == null) return;

        if (ssl.checked && (port.value == '25' || port.value == '' || port.value == '587' || port.value == '465')) {
            if (hn.value.trim().match(/^\s*smtp\.gmail\.com\s*$/i)) port.value = 587;
            else port.value = '465';
        }
        else if (!ssl.checked && (port.value == '25' || port.value == '' || port.value == '587' || port.value == '465')) {
            port.value = '25';
        }
    }
    function numericField(f) {
        var v = f.value.replace(/[^0-9.,]*/g, '');
        if (v != f.value) f.value = v;
    }
    function numericFieldWhole(f) {
        var v = f.value.replace(/[^0-9]*/g, '');
        if (v != f.value) f.value = v;
    }
// ]]>
</script>
    <style>
        .body-content img {
            max-height:100px;
        }

    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" Runat="Server">
    <input type="hidden" id="IsValid" value="true"/>
     <Localized:Panel ID="PnlBanners" runat="Server" DefaultButton="btnSave" LocalizationClass="SystemSettings" LocalizationId="BannersSettingsPanel">
        <Localized:Label runat="server" LocalizationClass="SystemSettings" LocalizationId="HomeBanner"></Localized:Label>
          <asp:FileUpload ID="fuImage" runat="server" CssClass="file-upload" />
         <asp:Image ID="HomeImage" runat="server" CssClass="image-small" />
         <Localized:Button ID="btnDeleteImage" FileUploadName="fuImage" ImageName="HomeImage" runat="server" OnClientClick="return Validate();" CssClass="button-02" LocalizationClass="Products" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" CausesValidation="false" />
         <br /><br />
         <Localized:Label runat="server" LocalizationClass="SystemSettings" LocalizationId="CategoryBanner"></Localized:Label>
         <asp:FileUpload ID="fuCategoryImage" runat="server" CssClass="file-upload" />
         <asp:Image ID="CategoryImage" runat="server" CssClass="image-small" />
         <Localized:Button ID="BtnCategoryImage" FileUploadName="fuCategoryImage" OnClientClick="return Validate();" ImageName="CategoryImage" runat="server" CssClass="button-02" LocalizationClass="Products" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click"  CausesValidation="false"/>
         <br /><br />
         <Localized:Label runat="server" LocalizationClass="SystemSettings" LocalizationId="SubCategoryBanner"></Localized:Label>
          <asp:FileUpload ID="fuSubCategoryImage" runat="server"  CssClass="file-upload"/>
          <asp:Image ID="SubCategoryImage" runat="server" CssClass="image-small" />
          <Localized:Button ID="BtnSubCategoryImage" FileUploadName="fuSubCategoryImage" OnClientClick="return Validate();" ImageName="SubCategoryImage" runat="server" CssClass="button-02" LocalizationClass="Products" LocalizationId="DeleteImage" OnClick="btnDeleteImage_Click" CausesValidation="false" />
    </Localized:Panel>
    <Localized:Panel runat="Server" DefaultButton="btnSave" LocalizationClass="SystemSettings" LocalizationId="SpecialSettingsPanel">

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtRadiusSupplier" LocalizationClass="SystemSettings" LocalizationId="RadiusSupplier"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtRadiusSupplier" onchange="numericField(this)" onkeypress="numericField(this)"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtActiveBid" LocalizationClass="SystemSettings" LocalizationId="ActiveBid"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtActiveBid" onchange="numericField(this)" onkeypress="numericField(this)"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtRateSupplier" LocalizationClass="SystemSettings" LocalizationId="RateSupplier"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtRateSupplier" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />

        <Localized:Label ID="Label4" runat="Server" BlockMode="true" AssociatedControlID="txtYad2" LocalizationClass="SystemSettings" LocalizationId="Yad2Expiry"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtYad2" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
        
           <Localized:Label ID="Label6" runat="Server" BlockMode="true" AssociatedControlID="txtOfferEnd" LocalizationClass="SystemSettings" LocalizationId="OfferEnd"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtOfferEnd" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />

          <Localized:Label ID="Label5" runat="Server" BlockMode="true" AssociatedControlID="txtOfferMinPrice" LocalizationClass="SystemSettings" LocalizationId="OfferMinPrice"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtOfferMinPrice" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
        
        <Localized:Label ID="Label8" runat="Server" BlockMode="true" AssociatedControlID="txtDeviationLowestThreshold" LocalizationClass="SystemSettings" LocalizationId="DeviationLowestThreshold"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtDeviationLowestThreshold" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />

        <Localized:Label ID="Label9" runat="Server" BlockMode="true" AssociatedControlID="txtDeviationPercentage" LocalizationClass="SystemSettings" LocalizationId="DeviationPercentage"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtDeviationPercentage" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
  
        <Localized:Label ID="Label10" runat="Server" BlockMode="true" AssociatedControlID="txtStartWorkingHour" LocalizationClass="SystemSettings" LocalizationId="StartWorkingHour"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtStartWorkingHour" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
        
         <Localized:Label ID="Label11" runat="Server" BlockMode="true" AssociatedControlID="txtEndWorkingHour" LocalizationClass="SystemSettings" LocalizationId="EndWorkingHour"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtEndWorkingHour" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
 
         <Localized:Label ID="Label15" runat="Server" BlockMode="true" AssociatedControlID="cbIsSendingMessagesActive" LocalizationClass="SystemSettings" LocalizationId="IsSendingMessagesActive"></Localized:Label>
        <asp:CheckBox CssClass="" runat="server" ID="cbIsSendingMessagesActive"></asp:CheckBox><br /><br />


         <Localized:Label ID="Label12" runat="Server" BlockMode="true" AssociatedControlID="txtMessageExpiration1" LocalizationClass="SystemSettings" LocalizationId="MessageExpiration1"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMessageExpiration1" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
 
         <Localized:Label ID="Label13" runat="Server" BlockMode="true" AssociatedControlID="txtMessageExpiration2" LocalizationClass="SystemSettings" LocalizationId="MessageExpiration2"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMessageExpiration2" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
 
         <Localized:Label ID="Label14" runat="Server" BlockMode="true" AssociatedControlID="txtMessageExpiration3" LocalizationClass="SystemSettings" LocalizationId="MessageExpiration3"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMessageExpiration3" onchange="numericField(this)" onkeypress="numericFieldWhole(this)"></asp:TextBox><br /><br />
 
           </Localized:Panel>

    <Localized:Panel runat="Server" DefaultButton="btnSave" LocalizationClass="SystemSettings" LocalizationId="WebSettingsPanel">
        <Localized:Label ID="lblMinAndVersion" runat="Server" BlockMode="true" AssociatedControlID="txtMinAndVersion" LocalizationClass="SystemSettings" LocalizationId="MinAndVersionLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMinAndVersion"></asp:TextBox><br /><br />

        <Localized:Label ID="lblMinIosVersion" runat="Server" BlockMode="true" AssociatedControlID="txtMinIosVersion" LocalizationClass="SystemSettings" LocalizationId="MinIosVersionLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMinIosVersion"></asp:TextBox><br /><br />

        <Localized:Label ID="lblMinAndSuppVersion" runat="Server" BlockMode="true" AssociatedControlID="txtMinAndSuppVersion" LocalizationClass="SystemSettings" LocalizationId="MinAndSuppVersionLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMinAndSuppVersion"></asp:TextBox><br /><br />

        <Localized:Label ID="lblMinIosSuppVersion" runat="Server" BlockMode="true" AssociatedControlID="txtMinIosSuppVersion" LocalizationClass="SystemSettings" LocalizationId="MinIosSuppVersionLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMinIosSuppVersion"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtWebRootUrl" LocalizationClass="SystemSettings" LocalizationId="WebRootUrlLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtWebRootUrl"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtApiRootUrl" LocalizationClass="SystemSettings" LocalizationId="ApiRootUrlLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtApiRootUrl"></asp:TextBox><br /><br />
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtTempUploadFolder" LocalizationClass="SystemSettings" LocalizationId="TempUploadFolderLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtTempUploadFolder"></asp:TextBox><br /><br />
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtAppUsersUploadFolder" LocalizationClass="SystemSettings" LocalizationId="AppUsersUploadFolderLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtAppUsersUploadFolder"></asp:TextBox><br /><br />
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtApiTempUploadFolder" LocalizationClass="SystemSettings" LocalizationId="ApiTempUploadFolderLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtApiTempUploadFolder"></asp:TextBox><br /><br />
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtApiAppUsersUploadFolder" LocalizationClass="SystemSettings" LocalizationId="ApiAppUsersUploadFolderLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtApiAppUsersUploadFolder"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtSecureUploadFolder" LocalizationClass="SystemSettings" LocalizationId="SecureUploadFolderLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtSecureUploadFolder"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtPrivacyPolicyUrl" LocalizationClass="SystemSettings" LocalizationId="PrivacyPolicyUrlLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtPrivacyPolicyUrl"></asp:TextBox><br /><br />


    </Localized:Panel>
    
    <Localized:Panel runat="Server" DefaultButton="btnSave" LocalizationClass="SystemSettings" LocalizationId="AppUserSettingsPanel">
    
        <Localized:CheckBox CssClass="checkbox-wrapper" runat="server" ID="chkAppUserVerifyEmail" LocalizationClass="SystemSettings" LocalizationId="AppUserVerifyEmailLabel" /><br /><br />
        
        <Localized:Label ID="Label1" runat="Server" LocalizationClass="SystemSettings" LocalizationId="EmailTemplateNewAppUserWelcomeLabel"></Localized:Label>
        <Custom:SettingsEmailTemplateChoiceControl runat="Server" id="setcEmailTemplateNewAppUserWelcome"></Custom:SettingsEmailTemplateChoiceControl><br /><br />

        <Localized:Label ID="Label2" runat="Server" LocalizationClass="SystemSettings" LocalizationId="EmailTemplateNewAppUserWelcomeVerifyEmailLabel"></Localized:Label>
        <Custom:SettingsEmailTemplateChoiceControl runat="Server" id="setcEmailTemplateNewAppUserWelcomeVerifyEmail"></Custom:SettingsEmailTemplateChoiceControl><br /><br />

        <Localized:Label ID="Label3" runat="Server" LocalizationClass="SystemSettings" LocalizationId="EmailTemplateAppUserForgotPasswordLabel"></Localized:Label>
        <Custom:SettingsEmailTemplateChoiceControl runat="Server" id="setcEmailTemplateAppUserForgotPassword"></Custom:SettingsEmailTemplateChoiceControl><br /><br />

        <Localized:Label ID="Label7" runat="Server" LocalizationClass="SystemSettings" LocalizationId="EmailTemplateAppuserGiftLabel"></Localized:Label>
        <Custom:SettingsEmailTemplateChoiceControl runat="Server" id="setcEmailTemplateAppuserGift"></Custom:SettingsEmailTemplateChoiceControl><br /><br />

    </Localized:Panel>
    
    <Localized:Panel runat="Server" DefaultButton="btnSave" LocalizationClass="SystemSettings" LocalizationId="MailSettingsPanel">
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtAdminEmail" LocalizationClass="SystemSettings" LocalizationId="AdminEmailLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtAdminEmail"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtAdminPhone" LocalizationClass="SystemSettings" LocalizationId="AdminPhoneLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtAdminPhone"></asp:TextBox><br /><br />
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtDefaultEmailFrom" LocalizationClass="SystemSettings" LocalizationId="DefaultEmailFromLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtDefaultEmailFrom"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtDefaultEmailFromName" LocalizationClass="SystemSettings" LocalizationId="DefaultEmailFromNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtDefaultEmailFromName"></asp:TextBox><br /><br />
    
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtDefaultEmailReplyTo" LocalizationClass="SystemSettings" LocalizationId="DefaultEmailReplyToLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtDefaultEmailReplyTo"></asp:TextBox><br /><br />

        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtDefaultEmailReplyToName" LocalizationClass="SystemSettings" LocalizationId="DefaultEmailReplyToNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtDefaultEmailReplyToName"></asp:TextBox><br /><br />
                
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtMailServerHostName" LocalizationClass="SystemSettings" LocalizationId="MailServerHostNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMailServerHostName" onchange="updateMailSettingsHostName()"></asp:TextBox><br /><br />
        
        <Localized:CheckBox CssClass="checkbox-wrapper" runat="server" ID="chkMailServerAuthentication" LocalizationClass="SystemSettings" LocalizationId="MailServerAuthenticationLabel" /><br /><br />
        
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtMailServerUserName" LocalizationClass="SystemSettings" LocalizationId="MailServerUserNameLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMailServerUserName"></asp:TextBox><br /><br />
        
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtMailServerPassword" LocalizationClass="SystemSettings" LocalizationId="MailServerPasswordLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text t-left ltr" runat="server" ID="txtMailServerPassword" TextMode="Password"></asp:TextBox><br /><br />
        
        <Localized:Label runat="Server" BlockMode="true" AssociatedControlID="txtMailServerPort" LocalizationClass="SystemSettings" LocalizationId="MailServerPortLabel"></Localized:Label>
        <asp:TextBox CssClass="input-text ltr" runat="server" ID="txtMailServerPort"></asp:TextBox><br /><br />
        
        <Localized:CheckBox CssClass="checkbox-wrapper" runat="server" ID="chkMailServerSsl" LocalizationClass="SystemSettings" LocalizationId="MailServerSslLabel" /><br /><br />
  
        <Localized:Button ID="btnTestMailSettings" runat="server" ButtonStyle="ButtonStyle2" OnClick="btnTestMailSettings_Click" LocalizationClass="SystemSettings" LocalizationId="TestMailButton" />
         
        <Localized:Button ID="GenerateSiteMap" runat="server" ButtonStyle="ButtonStyle2" OnClick="btnGenerateSiteMap_Click" LocalizationClass="SystemSettings" LocalizationId="GenerateSiteMap" />

    </Localized:Panel>
    
    <Localized:Panel runat="Server" DefaultButton="btnSave" LocalizationClass="SystemSettings" LocalizationId="EmailTemplateSettingsPanel">

        <Localized:Label runat="Server" LocalizationClass="SystemSettings" LocalizationId="EmailTemplateUserForgotPasswordLabel"></Localized:Label>
        <Custom:SettingsEmailTemplateChoiceControl runat="Server" id="setcEmailTemplateUserForgotPassword"></Custom:SettingsEmailTemplateChoiceControl><br /><br />
        
    </Localized:Panel>

    <Localized:CheckBox CssClass="checkbox-wrapper" runat="server" ID="chkSaveConfirm" LocalizationClass="SystemSettings" LocalizationId="SaveConfirmLabel" /> &nbsp;
    <Localized:LinkButton runat="server" ID="btnSave" ButtonStyle="ButtonStyle1" OnClientClick="return Validate();"  OnClick="btnSave_Click" LocalizationClass="SystemSettings" LocalizationId="SaveButton"></Localized:LinkButton>

    <Localized:CheckboxValidator runat="server" ControlToValidate="chkSaveConfirm" Display="None" LocalizationClass="SystemSettings" LocalizationId="SaveConfirmRequired"></Localized:CheckboxValidator>

</asp:Content>

