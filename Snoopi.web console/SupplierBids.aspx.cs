using dg.Utilities;
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
    public partial class SupplierBids : AdminPageBase
    {
        bool AllBids = false;
        bool HasEditPermission = false;
        Int64 SupplierId;
        string Action;
        DateTime? ToDate;
        DateTime? FromDate;
        Int64 BidId = 0;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            dgBids.PageIndexChanged += dgBids_PageIndexChanging;

            if (Request.QueryString["id"] == "all")
            {
                AllBids = true;
                SupplierId = -1;
            }
            else
            {
                if (!Int64.TryParse(Request.QueryString["Id"], out SupplierId))
                {
                    SupplierId = 0;
                }
                else
                {
                    AppSupplier s = AppSupplier.FetchByID(SupplierId);
                    if (s == null)
                        HasEditPermission = false;
                }

            }
            if (!String.IsNullOrEmpty(Request.QueryString["ToDate"])) ToDate = Convert.ToDateTime(Request.QueryString["ToDate"].ToString().Replace("%2F", "/"));
            if (!String.IsNullOrEmpty(Request.QueryString["FromDate"])) FromDate = Convert.ToDateTime(Request.QueryString["FromDate"].ToString().Replace("%2F", "/"));
            if (!String.IsNullOrEmpty(Request.QueryString["BidId"])) BidId = Convert.ToInt64(Request.QueryString["BidId"].ToString());
            Action = Request.QueryString["Action"];
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
            if (!IsPostBack)
            {
                if (AllBids)
                {
                    Master.PageTitle =BidString.GetText(@"TitleGlobalSupplierBids");
                }

                else
                {
                    Master.PageTitle = String.Format(BidString.GetText(@"TitleSupplierBids" + Action), AppSupplier.FetchByID(SupplierId).ContactName);
                }
                if (Action == "Active" || Action == "NoWin")
                    dgBids.Columns[dgBids.Columns.Count - 1].Visible = false;
            }
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {

            //Master.ActiveMenu = "Bids";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            phHasItems.Visible = true;
            phHasNoItems.Visible = false;
            //DateTime from, to = new DateTime();
            //DateTime.TryParse(datepickerFrom.Text.ToString(), out from);
            //DateTime.TryParse(datepickerTo.Text.ToString(), out to);
            dgBids.VirtualItemCount = BidController.GetSupplierBids(SupplierId, Action, FromDate, ToDate, BidId).Count;
            if (dgBids.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = BidString.GetText(@"MessageNoBidsFound");
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
                List<BidUI> coll = BidController.GetSupplierBids(SupplierId, Action, FromDate, ToDate, BidId, dgBids.PageSize, dgBids.CurrentPageIndex);
                BindList(coll);

            }
        }
        protected void BindList(List<BidUI> coll)
        {
            dgBids.DataSource = coll;
            dgBids.DataBind();
            if (dgBids.Items.Count > 0 && !(Action == "Win" || Action == "Offers"))
            {

                dgBids.Columns[5].Visible = false;
            }
            Master.DisableViewState(dgBids);
            lblTotal.Text = dgBids.VirtualItemCount.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"BidId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"StartDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"EndDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"TotalPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"Products"), typeof(string)));
            if (Action == "Win" || Action == "Offers")
                dt.Columns.Add(new System.Data.DataColumn(BidString.GetText(@"OrderDate1"), typeof(string)));


            List<BidUI> coll = BidController.GetSupplierBids(SupplierId, Action, FromDate, ToDate, BidId);
            foreach (BidUI bid in coll)
            {
                int i = 0;
                System.Data.DataRow row = dt.NewRow();
                row[i++] = bid.BidId;
                row[i++] = "\"" + bid.StartDate + "\"";
                row[i++] = "\"" + bid.EndDate + "\"";
                row[i++] = bid.Price;
                row[i++] = (bid.LstProduct.Count == 0 || bid.LstProduct == null) ? "" : String.Join(", ", bid.LstProduct.Select(o => o.ProductName));
                if (Action == "Win" || Action == "Offers")
                    row[i++] = "\"" + bid.OrderDate + "\"";
                dt.Rows.Add(row);
            }
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=Supplier" + SupplierId + Action + "BidsExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
