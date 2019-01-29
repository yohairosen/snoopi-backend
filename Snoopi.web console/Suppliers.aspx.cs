using dg.Utilities.Spreadsheet;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Snoopi.web
{
    public partial class Suppliers : AdminPageBase
    {
        bool HasEditPermission = true;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            //HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_edit_users);

            Master.AddButtonNew(SuppliersStrings.GetText(@"NewSupplierButton"), @"EditSupplier.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });

            dgSuppliers.PageIndexChanged += dgSuppliers_PageIndexChanging;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgSuppliers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgSuppliers.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = SuppliersStrings.GetText(@"SuppliersPageTitle");
            Master.ActiveMenu = "Suppliers";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgSuppliers.Columns[dgSuppliers.Columns.Count - 1].Visible = false;
            }
            string searchName = "%" + txtSearchName.Text.Trim() + "%";
            string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
            dgSuppliers.VirtualItemCount = SupplierController.GetAllSuppliersUI(true, searchName, searchPhone).Count;
            if (dgSuppliers.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = SuppliersStrings.GetText(@"MessageNoSuppliersFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgSuppliers.PageSize * dgSuppliers.CurrentPageIndex > dgSuppliers.VirtualItemCount)
                {
                    dgSuppliers.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgSuppliers.Value = dgSuppliers.CurrentPageIndex.ToString();
                }
                List<SupplierUI> items = SupplierController.GetAllSuppliersUI(true, searchName, searchPhone, dgSuppliers.PageSize, dgSuppliers.CurrentPageIndex);
                BindList(items);
            }


        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }
        protected void BindList(List<SupplierUI> coll)
        {
            dgSuppliers.ItemDataBound += dgSuppliers_ItemDataBound;
            dgSuppliers.DataSource = coll;
            dgSuppliers.DataBind();
            lblTotal.Text = dgSuppliers.VirtualItemCount.ToString();
            Master.DisableViewState(dgSuppliers);
        }

        protected void dgSuppliers_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgSuppliers.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgSuppliers.Value = dgSuppliers.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void dgSuppliers_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                HyperLink hlEdit = e.Item.FindControl("hlEdit") as HyperLink;
                hlEdit.Visible = HasEditPermission;

                HyperLink hlDelete = e.Item.FindControl("hlDelete") as HyperLink;
                hlDelete.Visible = HasEditPermission;

                LinkButton lbComments = e.Item.FindControl("lbComments") as LinkButton;
                lbComments.Visible = HasEditPermission;

                LinkButton lbProducts = e.Item.FindControl("lbProducts") as LinkButton;
                lbProducts.Visible = HasEditPermission;
            }
        }
        protected string FormatEditUrl(object item)
        {
            return string.Format("EditSupplier.aspx?SupplierId={0}", HttpUtility.UrlEncode(((SupplierUI)item).SupplierId.ToString()));
        }
        protected string FormatDeleteUrl(object item)
        {
            return string.Format("DeleteSupplier.aspx?SupplierId={0}", HttpUtility.UrlEncode(((SupplierUI)item).SupplierId.ToString()));
        }
        protected void dgSuppliers_ItemCommand(object sender, DataGridCommandEventArgs e)
        {

            int index = e.Item.ItemIndex;
            Int64 SupplierId;

            if (e.CommandName.Equals("Comments"))
            {
                SupplierId = Int64.Parse(dgSuppliers.DataKeys[index].ToString());
                Response.Redirect("Comments.aspx?SupplierId=" + SupplierId);
                LoadItems();
            }
            else if (e.CommandName.Equals("Products"))
            {
                SupplierId = Int64.Parse(dgSuppliers.DataKeys[index].ToString());
                Response.Redirect("SupplierProducts.aspx?SupplierId=" + SupplierId);
                LoadItems();
            }

        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"BusinessName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Email"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"ContactName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"ContactPhone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"CityName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Street"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"HouseNum"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Precent"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"SumPerMonth"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"CreateDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"citiesSupplied"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"citiesHomeService"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"MaxWinningsNum"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"AvgRate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"SupplierType"), typeof(string)));

            List<SupplierUI> suppliers = new List<SupplierUI>();
            string searchName = "%" + txtSearchName.Text.Trim() + "%";
            string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
            suppliers = SupplierController.GetAllSuppliersUI(true, searchName, searchPhone);
            foreach (SupplierUI supplier in suppliers)
            {
                System.Data.DataRow row = dt.NewRow();
                row[0] = supplier.BusinessName;
                row[1] = supplier.Email;
                row[2] = "\"" + supplier.Phone + "\"";
                row[3] = supplier.ContactName;
                row[4] = "\"" + supplier.ContactPhone + "\"";
                row[5] = supplier.CityName;
                row[6] = supplier.Street;
                row[7] = supplier.HouseNum;
                row[8] = supplier.Precent;
                row[9] = supplier.SumPerMonth;
                row[10] = "\"" + supplier.CreateDate + "\"";
                row[11] = (supplier.citiesSupplied.Count == 0 || supplier.citiesSupplied == null) ? "" : String.Join(", ", supplier.citiesSupplied.Select(o => o.CityName));
                row[12] = (supplier.citiesHomeService.Count == 0 || supplier.citiesHomeService == null) ? "" : String.Join(", ", supplier.citiesHomeService.Select(o => o.CityName));
                row[13] = supplier.MaxWinningsNum;
                row[14] = supplier.AvgRate;
                row[15] = supplier.IsProduct ? SuppliersStrings.GetText(@"IsProduct") : SuppliersStrings.GetText(@"IsService");
                dt.Rows.Add(row);
            }
            System.Data.DataRow sumRow = dt.NewRow();
            sumRow[0] = SuppliersStrings.GetText(@"SumSumPerMonth") + " " + suppliers.Sum(o => o.SumPerMonth);
            dt.Rows.Add(sumRow);
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=SuppliersExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }







        protected void btnExportForCRM_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"SupplierId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"BusinessName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"SupplierServices"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(AppUsersStrings.GetText(@"Address"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"CityName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"ContactPhone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"ContactName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Email"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"SupplierType"), typeof(string)));

            List<SupplierUI> suppliers = new List<SupplierUI>();
            string searchName = "%" + txtSearchName.Text.Trim() + "%";
            string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
            suppliers = SupplierController.GetAllSuppliersUI(true, searchName, searchPhone);

            foreach (SupplierUI supplier in suppliers)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = supplier.SupplierId;
                row[i++] = supplier.BusinessName;

                row[i++] = getSupplierServices(supplier);
                row[i++] = supplier.Street + " " + supplier.HouseNum;
                row[i++] = supplier.CityName;
                row[i++] =   "=\"" + supplier.Phone + "\"";
                row[i++] =  "=\"" + supplier.ContactPhone + "\"";
                row[i++] = supplier.ContactName;
                row[i++] = supplier.Email;
                row[i++] = supplier.IsProduct ? SuppliersStrings.GetText(@"IsProduct") : SuppliersStrings.GetText(@"IsService");
                dt.Rows.Add(row);
            }
            System.Data.DataRow sumRow = dt.NewRow();
            sumRow[0] = SuppliersStrings.GetText(@"SumSumPerMonth") + " " + suppliers.Sum(o => o.SumPerMonth);
            dt.Rows.Add(sumRow);
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=SuppliersExportForCRM_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }

        private string getSupplierServices(SupplierUI supplier)
        {
            if (supplier.IsProduct)
                return SuppliersStrings.GetText(@"IsProduct");
            else 
            {
                List<core.DAL.Service> servicesList = SupplierController.GetServicesBySupplier(supplier.SupplierId);
                StringBuilder services = new StringBuilder("");
             
                foreach (core.DAL.Service service in servicesList)
                {
                    if(!string.IsNullOrEmpty(services.ToString()))
                        services.Append(",");
                    services.Append(service.ServiceName);
                }
                return services.ToString();
            }
          
        }
        #endregion
    }
}
