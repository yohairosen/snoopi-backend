using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using Snoopi.web.Resources;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using Snoopi.core.BL;

namespace Snoopi.web
{
    public partial class DefaultPage : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            Master.PageTitle = GlobalStrings.GetText(@"DefaultPageTitle");
            ltDescription.Text = GlobalStrings.GetText(@"DefaultPageBody");
        }

        protected override string[] AllowedPermissions
        {
            get { return new string[] { }; }
        }
    }
}
