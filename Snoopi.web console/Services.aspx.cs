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
    public partial class Services : AdminPageBase
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
            Master.PageTitle = ServicesStrings.GetText(@"ServicesPageTitle");
            Master.ActiveMenu = "Services";
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                gvServices.Columns[gvServices.Columns.Count - 1].Visible = false;
            }

            Query qry = new Query(core.DAL.Service.TableSchema);
            qry.OrderBy(core.DAL.Service.Columns.ServiceName, dg.Sql.SortDirection.ASC);

            ServiceCollection items = ServiceCollection.FetchByQuery(qry);
            if (items == null || items.Count == 0)
            {
                phHasNoItems.Visible = true;
                //if no records, create collection with dummy record for gv data source only so that footer row will still be displayed, but will not be saved in cache
                items = new ServiceCollection();
                items.Add(new Service());
                lblTotal.Text = "0";
            }
            else
            {
                phHasNoItems.Visible = false;
                lblTotal.Text = items.Count.ToString();

            }
            gvServices.DataSource = items;
            gvServices.DataBind();

            //if first row is just a dummy - hide it
            if (phHasNoItems.Visible) gvServices.Rows[0].Visible = false;
        }

        protected void gvServices_RowEdit(object sender, GridViewEditEventArgs e)
        {
            gvServices.EditIndex = e.NewEditIndex;
            LoadItems();
        }

        protected void gvServices_RowUpdate(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            GridViewRow row = gvServices.Rows[index];
            Int64 ServiceId = Int64.Parse(gvServices.DataKeys[index].Value.ToString());
            TextBox tb = (TextBox)row.Cells[0].FindControl("txtServiceName");
            TextBox tb1 = (TextBox)row.Cells[0].FindControl("txtServiceComment");
            //CheckBox cb = (CheckBox)row.Cells[0].FindControl("cbIsHomeService");
            //study program name cannot ne empty
            if (tb.Text == "")
            {
                Master.MessageCenter.DisplayErrorMessage(ServicesStrings.GetText(@"ServiceNameRequired"));
                return;
            }
            //service name must be unique
            Service service = Service.FetchByName(tb.Text);
            if (service != null && service.ServiceId != ServiceId)
            {
                Master.MessageCenter.DisplayErrorMessage(ServicesStrings.GetText(@"MessageSaveFailedNameAlreadyExists"));
                return;
            }

            service = Service.FetchByID(ServiceId);
            service.ServiceName = tb.Text;
            service.ServiceComment = tb1.Text;
            //service.IsHomeService = cb.Checked;
            service.Save();

            gvServices.EditIndex = -1;

            // refresh the grid view data
            LoadItems();
        }

        protected void gvServices_RowCancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvServices.EditIndex = -1;
            LoadItems();
        }

        protected void gvServices_RowDelete(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            Int64 ServiceId = Int64.Parse(gvServices.DataKeys[index].Value.ToString());

            //check if this service is in use
            Query bidService = new Query(core.DAL.BidService.TableSchema)
                .Where(BidService.Columns.ServiceId, ServiceId);
            Query supplierService = new Query(core.DAL.SupplierService.TableSchema)
                .Where(SupplierService.Columns.ServiceId, ServiceId);

            if (bidService.GetCount() > 0 || supplierService.GetCount() > 0)
            {
                Master.MessageCenter.DisplayErrorMessage(ServicesStrings.GetText(@"MessageDeleteFailedInUse"));
                return;
            }
            Service.Delete(ServiceId);

            // refresh the grid view data
            LoadItems();
        }

        protected void gvServices_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("AddNew")) return;

            GridViewRow row = gvServices.FooterRow;
            TextBox tb = (TextBox)row.FindControl("txtNewServiceName");
            TextBox tb1 = (TextBox)row.FindControl("txtNewServiceComment");
            //CheckBox cb = (CheckBox)row.Cells[0].FindControl("cbIsHomeService");
            //study program name cannot ne empty
            if (tb.Text == "")
            {
                Master.MessageCenter.DisplayErrorMessage(ServicesStrings.GetText(@"ServiceNameRequired"));
                return;
            }
            //study program name must be unique
            if (core.DAL.Service.FetchByName(tb.Text) != null)
            {
                Master.MessageCenter.DisplayErrorMessage(ServicesStrings.GetText(@"MessageCreateFailedNameAlreadyExists"));
                return;
            }
            //Insert new User status
            Service service = new Service();
            service.ServiceName = tb.Text;
            service.ServiceComment = tb1.Text;
            //service.IsHomeService = cb.Checked;
            service.Save();

            // refresh the grid view data
            LoadItems();
        }

    }
}
