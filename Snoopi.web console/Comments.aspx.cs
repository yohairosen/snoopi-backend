using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
using dg.Utilities.Spreadsheet;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;
using dg.Sql;
using System.Web;
using System.Collections.Generic;

namespace Snoopi.web
{
    public partial class Comments : AdminPageBase
    {
        bool HasEditPermission = true;
        Int64 SupplierId;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            Int64.TryParse(Request.QueryString[@"SupplierId"], out SupplierId);

            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            dgComments.PageIndexChanged += dgComments_PageIndexChanging;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (SupplierId > 0)
                {
                    lblSearchBusiness.Visible = ddlSuppliers.Visible = false;
                }
                else
                {
                    lblSearchBusiness.Visible = ddlSuppliers.Visible = true;
                    //fill suppliers dropDown
                    if (ddlSuppliers.Items.Count == 0)
                    {
                        List<AppSupplier> suppliers = SupplierController.GetAllSuppliers();
                        foreach (var item in suppliers)
                        {
                            ddlSuppliers.Items.Add(new ListItem(item.BusinessName, item.SupplierId.ToString()));
                        }
                    }
                }
                //fill status dropDown 
                string[] names = Enum.GetNames(typeof(CommentStatus));
                Array values = Enum.GetValues(typeof(CommentStatus));
                for (int i = 0; i < names.Length; i++)
                {
                    ddlStatus.Items.Add(new ListItem(CommentsStrings.GetText(names[i]), ((int)values.GetValue(i)).ToString()));
                }

            }
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgComments.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgComments.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
            if (supplier != null)
            {
                Master.PageTitleHtml = string.Format(CommentsStrings.GetText(@"CommentsSupplierPageTitle"), supplier.BusinessName);
            }
            else
            {
                Master.PageTitle = CommentsStrings.GetText(@"CommentsPageTitle");
            }
            Master.ActiveMenu = "Comments";

            Master.AddClientScriptInclude(@"dgDateManager.js");
        }


        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgComments.Columns[dgComments.Columns.Count - 1].Visible = false;
            }
            List<Int64> SuppliersIdList = new List<Int64>();
            if (SupplierId > 0)
            {
                SuppliersIdList.Add(SupplierId);
            }
            else if (FillSupplierList().Count>0)
            {
                SuppliersIdList = FillSupplierList();
            }
            List<int> StatusIdList = new List<int>();
            if (FillStatusList().Count>0)
                StatusIdList = FillStatusList();

            dgComments.VirtualItemCount = CommentsController.GetAllCommentsUI(SuppliersIdList, StatusIdList).Count;

            if (dgComments.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = CommentsStrings.GetText(@"MessageNoCommentsFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgComments.PageSize * dgComments.CurrentPageIndex > dgComments.VirtualItemCount)
                {
                    dgComments.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgComments.Value = dgComments.CurrentPageIndex.ToString();
                }
                List<CommentUI> items = CommentsController.GetAllCommentsUI(SuppliersIdList, StatusIdList, dgComments.PageSize, dgComments.CurrentPageIndex);
                BindList(items);
            }

        }
        private List<Int64> FillSupplierList()
        {
            List<Int64> SuppliersIdList = new List<Int64>();
            foreach (ListItem item in ddlSuppliers.Items)
            {
                if (item.Selected)
                {
                    SuppliersIdList.Add(Convert.ToInt64(item.Value));
                }
            }
            return SuppliersIdList;
        }
        private List<int> FillStatusList()
        {
            List<int> StatusIdList = new List<int>();
            foreach (ListItem item in ddlStatus.Items)
            {
                if (item.Selected)
                {
                    StatusIdList.Add(Convert.ToInt32(item.Value));
                }
            }
            return StatusIdList;
        }
        protected void BindList(List<CommentUI> coll)
        {
            dgComments.ItemDataBound += dgComments_ItemDataBound;
            dgComments.DataSource = coll;
            dgComments.DataBind();
            Master.DisableViewState(dgComments);
            lblTotal.Text = dgComments.VirtualItemCount.ToString();
        }

        protected void dgComments_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
                dgComments.CurrentPageIndex = e.NewPageIndex;
                hfCurrentPageIndex_dgComments.Value = dgComments.CurrentPageIndex.ToString();
                LoadItems();
           
        }

        protected void dgComments_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                Button hlApprove = e.Item.FindControl("hlApprove") as Button;
                Button hlCancel = e.Item.FindControl("hlCancel") as Button;
                if (((CommentUI)e.Item.DataItem).Status == CommentStatus.Approved)
                {
                    hlCancel.Visible = true;
                    hlApprove.Visible = false;
                }
                else if (((CommentUI)e.Item.DataItem).Status == CommentStatus.Denied)
                {
                    hlCancel.Visible = false;
                    hlApprove.Visible = true;
                }
                else if (((CommentUI)e.Item.DataItem).Status == CommentStatus.Wait)
                {
                    hlCancel.Visible = true;
                    hlApprove.Visible = true;
                }

            }
        }

        protected void dgComments_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                switch (((Button)e.CommandSource).CommandName)
                {

                    case "approve":
                        CommentsController.ApproveComment(Convert.ToInt64(((Button)e.CommandSource).CommandArgument));
                        ((Button)e.CommandSource).Visible = false;
                        (e.Item.FindControl("hlCancel") as Button).Visible = true;
                        LoadItems();
                        break;

                    // Add other cases here, if there are multiple ButtonColumns in 
                    // the DataGrid control.
                    case "cancel":
                        CommentsController.CancelComment(Convert.ToInt64(((Button)e.CommandSource).CommandArgument));
                        (e.Item.FindControl("hlApprove") as Button).Visible = true;
                        LoadItems();
                        ((Button)e.CommandSource).Visible = false;
                        break;
                    default:
                        // Do nothing.
                        break;

                }
            }
            catch(Exception ex){}

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dgComments.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgComments.Value = dgComments.CurrentPageIndex.ToString();
            LoadItems();
        }
    }
}
