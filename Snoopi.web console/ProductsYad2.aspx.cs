using dg.Utilities.Spreadsheet;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Snoopi.web
{
    public partial class ProductsYad2 : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
            dgProductsYad2.PageIndexChanged += dgProductsYad2_PageIndexChanging;
        }
        protected void dgProductsYad2_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgProductsYad2.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgProductsYad2.Value = dgProductsYad2.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //fill status dropDown 
                string[] names = Enum.GetNames(typeof(StatusType));
                Array values = Enum.GetValues(typeof(StatusType));
                for (int i = 0; i < names.Length; i++)
                {
                    ddlStatus.Items.Add(new ListItem(Yad2Strings.GetText(names[i]), ((int)values.GetValue(i)).ToString()));
                }
            }
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgProductsYad2.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgProductsYad2.CurrentPageIndex = CurrentPageIndex;
            LoadItems();

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = Yad2Strings.GetText(@"ProductsYad2PageTitle");
            Master.ActiveMenu = "ProductsYad2";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgProductsYad2.Columns[dgProductsYad2.Columns.Count - 1].Visible = false;
            }
            List<int> StatusIdList = new List<int>();
            //if (IsSearch)
            if (FillStatusList().Count > 0)
            {
                StatusIdList = FillStatusList();
            }
            dgProductsYad2.VirtualItemCount = ProductYad2Controller.GetAllProductsYad2(StatusIdList).Count;
            if (dgProductsYad2.VirtualItemCount == 0)
            {
                phHasNoItems.Visible = true;
                phHasItems.Visible = false;
            }
            else
            {
                phHasNoItems.Visible = false;
                phHasItems.Visible = true;
                if (dgProductsYad2.PageSize * dgProductsYad2.CurrentPageIndex > dgProductsYad2.VirtualItemCount)
                {
                    dgProductsYad2.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgProductsYad2.Value = dgProductsYad2.CurrentPageIndex.ToString();
                }
                List<ProductYad2UI> productsYad2 = ProductYad2Controller.GetAllProductsYad2(StatusIdList, dgProductsYad2.PageSize, dgProductsYad2.CurrentPageIndex);
                BindList(productsYad2);
            }

        }
        protected void BindList(List<ProductYad2UI> coll)
        {
            dgProductsYad2.ItemDataBound += dgProductsYad2_ItemDataBound;
            dgProductsYad2.DataSource = coll;
            dgProductsYad2.DataBind();
            Master.DisableViewState(dgProductsYad2);
            lblTotal.Text = dgProductsYad2.VirtualItemCount.ToString();
        }


        protected void dgProductsYad2_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                //LinkButton lbEdit = e.Item.FindControl("lbDetails") as LinkButton;
                //lbEdit.Visible = HasEditPermission;
                LinkButton hlApprove = e.Item.FindControl("hlApprove") as LinkButton;
                LinkButton hlCancel = e.Item.FindControl("hlCancel") as LinkButton;
                if (((ProductYad2UI)e.Item.DataItem).Status == StatusType.Approved)
                {
                    hlCancel.Visible = true;
                    hlApprove.Visible = false;
                }
                else if (((ProductYad2UI)e.Item.DataItem).Status == StatusType.Denied)
                {
                    hlCancel.Visible = false;
                    hlApprove.Visible = true;
                }
                else
                {
                    hlCancel.Visible = true;
                    hlApprove.Visible = true;
                }

            }
        }


        protected void dgProductsYad2_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 ProductId;
            switch (((LinkButton)e.CommandSource).CommandName)
            {

                case "approve":
                    ProductId = Int64.Parse(dgProductsYad2.DataKeys[index].ToString());
                    pnlRemark.Visible = true;
                    pnlStatusRemarks.Visible = true;
                    btnApprove.Visible = true;
                    hiddenFieldProductId.Value = ProductId.ToString();
                    break;

                case "cancel":
                    ProductId = Int64.Parse(dgProductsYad2.DataKeys[index].ToString());
                    pnlRemark.Visible = true;
                    pnlStatusRemarks.Visible = true;
                    btnReject.Visible = true;
                    hiddenFieldProductId.Value = ProductId.ToString();
                    break;
                default:
                    // Do nothing.
                    break;

            }
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            ProductYad2Controller.DenyProduct(Int64.Parse(hiddenFieldProductId.Value), txtStatusRemarks.Text, string.Format(Snoopi.web.Localization.PushStrings.GetText("PushYad2Deny")));
            string url = @"ProductsYad2.aspx";
            Response.Redirect(url, true);
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            ProductYad2Controller.ApproveProduct(Int64.Parse(hiddenFieldProductId.Value), txtStatusRemarks.Text, string.Format(Snoopi.web.Localization.PushStrings.GetText("PushYad2Accepted")));
            string url = @"ProductsYad2.aspx";
            Response.Redirect(url, true);
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string url = @"ProductsYad2.aspx";
            Response.Redirect(url, true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dgProductsYad2.CurrentPageIndex = 0;
            hfCurrentPageIndex_dgProductsYad2.Value = dgProductsYad2.CurrentPageIndex.ToString();
            LoadItems();
          //  LoadItems(true);
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
        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"ProductName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"LstCategory"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"Price"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"CityName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"ContactName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"Details"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(Yad2Strings.GetText(@"Status"), typeof(string)));


            List<int> StatusIdList = FillStatusList();
            List<ProductYad2UI> productsYad2 = ProductYad2Controller.GetAllProductsYad2(StatusIdList);

            foreach (ProductYad2UI product in productsYad2)
            {
                System.Data.DataRow row = dt.NewRow();
                row[0] = product.ProductName;
                row[1] = (product.LstCategory.Count == 0 || product.LstCategory == null) ? "" : String.Join(", ", product.LstCategory.Select(p => p.CategoryYad2Name));
                row[2] = product.Price;
                row[3] = product.CityName;
                row[4] = product.ContactName;
                row[5] = "\"" + product.Phone + "\"";
                row[6] = product.Details;
                row[7] = Yad2Strings.GetText(Enum.GetName(typeof(StatusType), product.Status));
                dt.Rows.Add(row);
            }
            System.Data.DataRow sumRow = dt.NewRow();
            sumRow[0] = Yad2Strings.GetText(@"SumPrice") + " " + productsYad2.Sum(p => p.Price);
            dt.Rows.Add(sumRow);
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=ProductsYad2Export_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }
        #endregion
    }

}
