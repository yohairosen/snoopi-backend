using System;
using System.Collections;
using System.Configuration;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;

namespace Snoopi.web
{
    public partial class EditEmailTemplateCodes : AdminPageBase
    {
        protected override string[] AllowedPermissions
        {
            get { return new string[] { Permissions.PermissionKeys.sys_perm }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(EmailTemplatesStrings.GetText(@"NewTemplateButton"), @"EditEmailTemplate.aspx", new string[] { Permissions.PermissionKeys.sys_perm });
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = EmailTemplatesStrings.GetText(@"TitleTemplateCodes");
            Master.ActiveMenu = "EmailTemplateCodes";
        }
    }
}
