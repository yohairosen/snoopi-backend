<%@ Page Language="C#" MasterPageFile="~/Template.master" AutoEventWireup="true" CodeFile="EditEmailTemplateCodes.aspx.cs" Inherits="Snoopi.web.EditEmailTemplateCodes" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Template.master" %>
<%@ Register Namespace="Snoopi.web.Localization.WebControls" Assembly="Snoopi.web" TagPrefix="Localized" %>
<%@ Register Namespace="CKEditor.NET" Assembly="CKEditor.NET" TagPrefix="CKEditor" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" Runat="Server">

    <dl>
        <dt class="ltr">#USEREMAIL#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_USEREMAIL")%></dd>
        <dt class="ltr">#RESETLINK#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_RESETLINK")%></dd>
        <dt class="ltr">#USERFULLNAME#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_USERFULLNAME")%></dd>
        <dt class="ltr">#CAMPAIGNNUMBER#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_CAMPAIGNNUMBER")%></dd>
         <dt class="ltr">#GIFT#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_GIFT")%></dd>
        <dt class="ltr">#USERID#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_USERID")%></dd>
        <dt class="ltr">#PRODUCTNAME#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_PRODUCTNAME")%></dd>
        <dt class="ltr">#PRODUCTDESCIPTION#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_PRODUCTDESCIPTION")%></dd>
        <dt class="ltr">#PRODUCTCODE#</dt>
        <dd><%= EmailTemplatesStrings.GetHtml(@"Code_PRODUCTCODE")%></dd>   
    </dl>
        
</asp:Content>
