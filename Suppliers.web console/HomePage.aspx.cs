using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using Snoopi.web;
public partial class HomePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SuppliersSessionHelper.IsProductSupplier())
        {
            LinkButtonHistory.Visible = false;
            LinkButtonProducts.Visible = false;
        }
        else
        {
            LinkButtonClicks.Visible = false;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        LinkButtonProfile.Text = MenuStrings.GetText(@"Profile");
        LinkButtonHistory.Text = MenuStrings.GetText(@"DealsHistory");
        LinkButtonProducts.Text = MenuStrings.GetText(@"ProductManegement");
        LinkButtonClicks.Text = MenuStrings.GetText(@"ClicksHistory");
    }
}