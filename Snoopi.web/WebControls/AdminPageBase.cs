using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Snoopi.core.DAL;
using Snoopi.core.BL;
using System.Web;

namespace Snoopi.web.WebControls
{
    public abstract class AdminPageBase : System.Web.UI.Page
    {
        #region Page Controls
        #endregion

        #region Variable Members
        #endregion

        #region Properties

        #endregion

        #region Events
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            AdminMasterPage master = Master as AdminMasterPage;
            master.CheckAuthenticated();
            VerifyAccessToThisPage();

            System.Web.HttpBrowserCapabilities browser = Request.Browser;
            if (browser.Browser == "IE" && browser.MajorVersion < 7)
            {
                if (Request.CurrentExecutionFilePath != @"/UnsupportedBrowser.aspx" && Request.CurrentExecutionFilePath != @"/Login.aspx")
                {
                    Response.Redirect(@"/UnsupportedBrowser.aspx", true);
                }
            }
            else
            {
                if (Request.CurrentExecutionFilePath == @"/UnsupportedBrowser.aspx")
                {
                    Response.Redirect(@"/", true);
                }
            }
        }
        #endregion


        protected virtual string[] AllowedPermissions { get { return null; } }
        protected virtual void VerifyAccessToThisPage()
        {
            AdminMasterPage master = Master as AdminMasterPage;
            master.LimitToPermissions(AllowedPermissions);
        }            
    }
}
