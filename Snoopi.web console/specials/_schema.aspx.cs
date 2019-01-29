using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dg.Sql.Connector;
using dg.Sql;
using Snoopi.core.DAL;
using dg.Sql.Sql.Spatial;
using dg.Sql.Phrases;
using Snoopi.core.BL;
using dg.Utilities;

namespace Snoopi.web.specials
{
    public partial class schema : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            //if (!Request.IsLocal) Http.Respond404(true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Snoopi.core.DAL.DbSchema.CreateOrUpgrade();

            User user;
            Snoopi.core.BL.Membership.CreateUser("user@gmail.com", @"1234", out user);
            foreach (string key in Permissions.SystemPermissionKeys)
            {
                Snoopi.core.BL.Membership.AddPermissionToUser(null, Permissions.PermissionIdByKey(key), user.UserId);
            }
        }
    }
}