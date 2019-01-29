using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;
using dg.Sql;
using System.Web;

namespace Snoopi.web
{
    public partial class Prices : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = PricesStrings.GetText(@"PricesPageTitle");
            Master.ActiveMenu = "Prices";
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                gvPrices.Columns[gvPrices.Columns.Count - 1].Visible = false;
            }

            Query qry = new Query(core.DAL.PriceFilter.TableSchema);
            qry.OrderBy(core.DAL.PriceFilter.Columns.PriceName, dg.Sql.SortDirection.ASC);

            PriceFilterCollection items = PriceFilterCollection.FetchByQuery(qry);
            if (items == null || items.Count == 0)
            {
                phHasNoItems.Visible = true;
                items = new PriceFilterCollection();
                items.Add(new PriceFilter());
                lblTotal.Text = "0";
            }
            else
            {
                phHasNoItems.Visible = false;
                lblTotal.Text = items.Count.ToString();

            }
            gvPrices.DataSource = items;
            gvPrices.DataBind();

            if (phHasNoItems.Visible) gvPrices.Rows[0].Visible = false;
        }

        protected void gvPrices_RowEdit(object sender, GridViewEditEventArgs e)
        {
            gvPrices.EditIndex = e.NewEditIndex;
            LoadItems();
        }

        protected void gvPrices_RowUpdate(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            GridViewRow row = gvPrices.Rows[index];
            Int64 PriceId = Int64.Parse(gvPrices.DataKeys[index].Value.ToString());
            TextBox tb = (TextBox)row.Cells[0].FindControl("txtPriceName");

            if (tb.Text == "")
            {
                Master.MessageCenter.DisplayErrorMessage(PricesStrings.GetText(@"PriceNameRequired"));
                return;
            }
            PriceFilter p = PriceFilter.FetchByName(tb.Text);
            if (p != null && p.PriceId != PriceId)
            {
                Master.MessageCenter.DisplayErrorMessage(PricesStrings.GetText(@"MessageSaveFailedNameAlreadyExists"));
                return;
            }

            p = PriceFilter.FetchByID(PriceId);
            p.PriceName = tb.Text;
            p.Save();

            gvPrices.EditIndex = -1;

            LoadItems();
        }

        protected void gvPrices_RowCancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPrices.EditIndex = -1;
            LoadItems();
        }

        protected void gvPrices_RowDelete(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            Int64 PriceId = Int64.Parse(gvPrices.DataKeys[index].Value.ToString());

            Query qry = new Query(ProductYad2.TableSchema)
                .Where(ProductYad2.Columns.PriceId, PriceId);

            if (qry.GetCount() > 0)
            {
                Master.MessageCenter.DisplayErrorMessage(PricesStrings.GetText(@"MessageDeleteFailedInUse"));
                return;
            }
            PriceFilter.Delete(PriceId);
            LoadItems();
        }

        protected void gvPrices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("AddNew")) return;

            GridViewRow row = gvPrices.FooterRow;
            TextBox tb = (TextBox)row.FindControl("txtNewPriceName");
            if (tb.Text == "")
            {
                Master.MessageCenter.DisplayErrorMessage(PricesStrings.GetText(@"PriceNameRequired"));
                return;
            }
            if (PriceFilter.FetchByName(tb.Text) != null)
            {
                Master.MessageCenter.DisplayErrorMessage(PricesStrings.GetText(@"MessageSaveFailedNameAlreadyExists"));
                return;
            }
            PriceFilter p = new PriceFilter();
            p.PriceName = tb.Text;
            p.Save();

            LoadItems();
        }

    }
}
