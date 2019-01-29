using dg.Utilities;
using dg.Utilities.Spreadsheet;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Snoopi.web
{
    public partial class Carts : AdminPageBase
    {
        bool HasEditPermission = false;
        string filterSearch = null;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            if (Request.QueryString["Filter"] != null)
            {
                filterSearch = Request.QueryString["Filter"];
            }
            dgBids.PageIndexChanged += dgBids_PageIndexChanging;
            dgBids.ItemDataBound += DgBids_ItemDataBound; ;
        }

        private void DgBids_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item != null && (e.Item.ItemType == ListItemType.Item  ||
                e.Item.ItemType == ListItemType.AlternatingItem))
            {
                //DataGridItem item = e.Item;
                //var cartUI = (CartUi)item.DataItem;
                //if (cartUI.SupplierId <= 0 && bidUI.IsActive == true)
                //{
                //    if (item.FindControl("btnCancelBid") != null)
                //        ((Button)item.FindControl("btnCancelBid")).Visible = true;
                //}
            }
        }
   
        protected void dgBids_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgBids.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgBids.Value = dgBids.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgBids.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgBids.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {

            Master.PageTitle = Resources.Cart.ResourceManager.GetString("Title");
                //BidsCountLabel.Visible = false;
                //linksSearch.Visible = true;
            Master.PageTitle = Resources.Cart.ResourceManager.GetString("Title");
               Master.ActiveMenu = "Carts";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgBids.Columns[dgBids.Columns.Count - 1].Visible = false;
            }
                  
            List<CartUi> carts = CartController.GetLastNDaysCarts(14);
            dgBids.VirtualItemCount = carts.Count;
            if (dgBids.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = Resources.Cart.ResourceManager.GetString("NoCarts");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgBids.PageSize * dgBids.CurrentPageIndex > dgBids.VirtualItemCount)
                {
                    dgBids.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgBids.Value = dgBids.CurrentPageIndex.ToString();
                }
                
                BindList(carts);
            }



        }
        protected void BindList(List<CartUi> coll)
        {
            dgBids.DataSource = coll;
            dgBids.DataBind();
            Master.DisableViewState(dgBids);
            //lblTotal.Text = dgBids.VirtualItemCount.ToString();
            //if (filterSearch != null)
            //{
            //    lblTotal1.Text = dgBids.VirtualItemCount.ToString();
            //}
        }




        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }       
    }

}
