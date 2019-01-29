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
    public partial class CatagoriesYad2 : AdminPageBase
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
            Master.PageTitle = Yad2Strings.GetText(@"CategoriesYad2PageTitle");
            Master.ActiveMenu = "CategoriesYad2";
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                gvCatagoriesYad2.Columns[gvCatagoriesYad2.Columns.Count - 1].Visible = false;
            }

            Query qry = new Query(core.DAL.CategoryYad2.TableSchema);
            qry.OrderBy(core.DAL.CategoryYad2.Columns.CategoryYad2Name, dg.Sql.SortDirection.ASC);

            CategoryYad2Collection items = CategoryYad2Collection.FetchByQuery(qry);
            if (items == null || items.Count == 0)
            {
                phHasNoItems.Visible = true;
                //if no records, create collection with dummy record for gv data source only so that footer row will still be displayed, but will not be saved in cache
                items = new CategoryYad2Collection();
                items.Add(new CategoryYad2());
                lblTotal.Text = "0";
            }
            else
            {
                phHasNoItems.Visible = false;
                lblTotal.Text = items.Count.ToString();

            }
            gvCatagoriesYad2.DataSource = items;
            gvCatagoriesYad2.DataBind();

            //if first row is just a dummy - hide it
            if (phHasNoItems.Visible) gvCatagoriesYad2.Rows[0].Visible = false;
        }

        protected void gvCatagoriesYad2_RowEdit(object sender, GridViewEditEventArgs e)
        {
            gvCatagoriesYad2.EditIndex = e.NewEditIndex;
            LoadItems();
        }

        protected void gvCatagoriesYad2_RowUpdate(object sender, GridViewUpdateEventArgs e)
        {
            int index = e.RowIndex;
            GridViewRow row = gvCatagoriesYad2.Rows[index];
            Int64 CategoryId = Int64.Parse(gvCatagoriesYad2.DataKeys[index].Value.ToString());
            TextBox tb = (TextBox)row.Cells[0].FindControl("txtCategoryName");

            //study program name cannot ne empty
            if (tb.Text == "")
            {
                Master.MessageCenter.DisplayErrorMessage(Yad2Strings.GetText(@"CategoryNameRequired"));
                return;
            }
            //category name must be unique
            CategoryYad2 category = CategoryYad2.FetchByName(tb.Text);
            if (category != null && category.CategoryYad2Id != CategoryId)
            {
                Master.MessageCenter.DisplayErrorMessage(Yad2Strings.GetText(@"MessageSaveFailedNameAlreadyExists"));
                return;
            }

            category = CategoryYad2.FetchByID(CategoryId);
            category.CategoryYad2Name = tb.Text;
            category.Save();

            gvCatagoriesYad2.EditIndex = -1;

            // refresh the grid view data
            LoadItems();
        }

        protected void gvCatagoriesYad2_RowCancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCatagoriesYad2.EditIndex = -1;
            LoadItems();
        }

        protected void gvCatagoriesYad2_RowDelete(object sender, GridViewDeleteEventArgs e)
        {
            int index = e.RowIndex;
            Int64 CategoryId = Int64.Parse(gvCatagoriesYad2.DataKeys[index].Value.ToString());

            //check if this service is in use
            Query productCategory = new Query(ProductYad2Category.TableSchema)
                .Where(ProductYad2Category.Columns.CategoryYad2Id, CategoryId);

            if (productCategory.GetCount() > 0)
            {
                Master.MessageCenter.DisplayErrorMessage(Yad2Strings.GetText(@"MessageDeleteFailedInUse"));
                return;
            }
            CategoryYad2.Delete(CategoryId);

            // refresh the grid view data
            LoadItems();
        }

        protected void gvCatagoriesYad2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals("AddNew")) return;

            GridViewRow row = gvCatagoriesYad2.FooterRow;
            TextBox tb = (TextBox)row.FindControl("txtNewCategoryName");
            //study program name cannot ne empty
            if (tb.Text == "")
            {
                Master.MessageCenter.DisplayErrorMessage(Yad2Strings.GetText(@"CategoryNameRequired"));
                return;
            }
            //study program name must be unique
            if (core.DAL.CategoryYad2.FetchByName(tb.Text) != null)
            {
                Master.MessageCenter.DisplayErrorMessage(Yad2Strings.GetText(@"MessageSaveFailedNameAlreadyExists"));
                return;
            }
            //Insert new User status
            CategoryYad2 category = new CategoryYad2();
            category.CategoryYad2Name = tb.Text;
            category.Save();

            // refresh the grid view data
            LoadItems();
        }

    }
}
