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
using System.Collections.Generic;
using dg.Utilities.Spreadsheet;
using System.Linq;

namespace Snoopi.web
{
    public partial class Orders : AdminPageBase
    {
        bool HasEditPermission = true;
        List<OrderUI> currentPageItems;
        Boolean _edited = false;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            dgOrders.PageIndexChanged += dgOrders_PageIndexChanging;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //fill suppliers dropDown
                if (ddlSuppliers.Items.Count == 0)
                {
                    List<AppSupplier> suppliers = AppSupplierCollection.FetchAll();
                    foreach (var item in suppliers)
                    {
                        ddlSuppliers.Items.Add(new ListItem(item.BusinessName, item.SupplierId.ToString()));
                    }
                }
                //fill isSendReceived dropdown
                if (ddlIsSendReceived.Items.Count == 0)
                {
                    ddlIsSendReceived.Items.Add(new ListItem(OrdersStrings.GetText(@"yes"), true.ToString()));
                    ddlIsSendReceived.Items.Add(new ListItem(OrdersStrings.GetText(@"no"), false.ToString()));
                }
                //fill status dropdown
                if (ddlStatus.Items.Count == 0)
                {
                    string[] names = Enum.GetNames(typeof(OrderStatus));
                    Array values = Enum.GetValues(typeof(OrderStatus));
                    for (int i = 0; i < names.Length; i++)
                    {
                        ddlStatus.Items.Add(new ListItem(OrdersStrings.GetText(names[i]), ((int)values.GetValue(i)).ToString()));
                    }
                }
                //fill payement status dropdown
                if (ddlPaymentStatus.Items.Count == 0)
                {
                    string[] names = Enum.GetNames(typeof(PaymentStatus));
                    Array values = Enum.GetValues(typeof(PaymentStatus));
                    for (int i = 0; i < names.Length; i++)
                    {
                        ddlPaymentStatus.Items.Add(new ListItem(OrdersStrings.GetText(names[i]), ((int)values.GetValue(i)).ToString()));
                    }
                }
                LoadItems();
            }
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgOrders.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgOrders.CurrentPageIndex = CurrentPageIndex;
     
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = OrdersStrings.GetText(@"OrdersPageTitle");
            Master.ActiveMenu = "Orders";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgOrders.Columns[dgOrders.Columns.Count - 1].Visible = false;
            }
            DateTime from, to = new DateTime();
            if (DateTime.TryParse(datepickerFrom.Value.ToString(), out from))
            {
                TimeSpan tsFrom = new TimeSpan(TimeSelectorFrom.Hour, TimeSelectorFrom.Minute, 0);
                from = from.Date + tsFrom;
            }
            if (DateTime.TryParse(datepickerTo.Value.ToString(), out to))
            {
                TimeSpan tsTo = new TimeSpan(TimeSelectorTo.Hour, TimeSelectorTo.Minute, 0);
                to = to.Date + tsTo;
            }
            //if (from != DateTime.MinValue && to != DateTime.MinValue)
            //{
            //    if (from > to)
            //    {
            //        Master.MessageCenter.DisplayErrorMessage(OrdersStrings.GetText(@"EnterValidDates"));
            //        return;
            //    }
            //}
            List<Int64> SuppliersIdList = FillSupplierList();
            List<bool> IsReceivedSendDate = FillIsSendReceived();
            List<int> StatusIdList = FillStatusList();
            List<int> StatusPayementIdList = FillPayementStatusList();
            string SearchBid = "%" + txtSearchBid.Text.Trim() + "%";
            dgOrders.VirtualItemCount = OrderController.GetAllOrders(from, to, SuppliersIdList, IsReceivedSendDate, StatusIdList, StatusPayementIdList, SearchBid).Count();

            if (dgOrders.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
                lblNoItems.Text = OrdersStrings.GetText(@"MessageNoOrdersFound");
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgOrders.PageSize * dgOrders.CurrentPageIndex > dgOrders.VirtualItemCount)
                {
                    dgOrders.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgOrders.Value = dgOrders.CurrentPageIndex.ToString();
                }
                List<OrderUI> items = OrderController.GetAllOrders(from, to, SuppliersIdList, IsReceivedSendDate, StatusIdList, StatusPayementIdList, SearchBid, dgOrders.PageSize, dgOrders.CurrentPageIndex);
                lblSum.Text = items.Sum(o => o.PaymentForSupplier).ToString();
                currentPageItems = items;
                BindList(items);
            }


        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            pnlSaveStatus.Visible = true;
            pnlSave.Visible = true;
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
        private List<bool> FillIsSendReceived()
        {
            List<bool> IsSendReceived = new List<bool>();
            foreach (ListItem item in ddlIsSendReceived.Items)
            {
                if (item.Selected)
                {
                    IsSendReceived.Add(Convert.ToBoolean(item.Value));
                }
            }
            return IsSendReceived;
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
        private List<int> FillPayementStatusList()
        {
            List<int> StatusIdList = new List<int>();
            foreach (ListItem item in ddlPaymentStatus.Items)
            {
                if (item.Selected)
                {
                    StatusIdList.Add(Convert.ToInt32(item.Value));
                }
            }
            return StatusIdList;
        }
        protected void BindList(List<OrderUI> coll)
        {
            dgOrders.ItemDataBound += dgOrders_ItemDataBound;
            dgOrders.DataSource = coll;
            dgOrders.DataBind();
            //Master.DisableViewState(dgOrders);
            lblTotal.Text = dgOrders.VirtualItemCount.ToString();
        }

        protected void dgOrders_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            
            if (_edited)
            {
                pnlSaveStatus.Visible = true;
                pnlSave.Visible = true;
            }
            else
            {
                dgOrders.CurrentPageIndex = e.NewPageIndex;
                hfCurrentPageIndex_dgOrders.Value = dgOrders.CurrentPageIndex.ToString();
                LoadItems();
            }
        }

       
        protected void dgOrders_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                Int64 OrderId = Int64.Parse(dgOrders.DataKeys[e.Item.ItemIndex].ToString());
                Order o = Order.FetchByID(OrderId);

                //DropDownList ddlTransactionStatus = e.Item.FindControl("ddlTransactionStatus") as DropDownList;
                string[] names = Enum.GetNames(typeof(OrderStatus));
                Array values = Enum.GetValues(typeof(OrderStatus));
                //if (ddlTransactionStatus.Items.Count == 0)
                //{
                //    for (int i = 0; i < names.Length; i++)
                //    {
                //        ddlTransactionStatus.Items.Add(new ListItem(OrdersStrings.GetText(names[i]), ((int)values.GetValue(i)).ToString()));
                //    }
                //    ddlTransactionStatus.SelectedValue = ((int)o.TransactionStatus).ToString();
                //}

                DropDownList ddlPaySupplierStatus = e.Item.FindControl("ddlPaySupplierStatus") as DropDownList;
                string[] names1 = Enum.GetNames(typeof(PaymentStatus));
                Array values1 = Enum.GetValues(typeof(PaymentStatus));
                if (ddlPaySupplierStatus.Items.Count == 0)
                {
                    for (int i = 0; i < names1.Length; i++)
                    {
                        ddlPaySupplierStatus.Items.Add(new ListItem(OrdersStrings.GetText(names1[i]), ((int)values1.GetValue(i)).ToString()));
                    }
                    ddlPaySupplierStatus.SelectedValue = ((int)o.PaySupplierStatus).ToString();
                }
            }
        }
        protected void dgOrders_ItemCommand(object sender, DataGridCommandEventArgs e)
        {

            //int index = e.Item.ItemIndex;
            //Int64 OrderId;

            //if (e.CommandName.Equals("selection"))
            //{
            //    OrderId = Int64.Parse(dgOrders.DataKeys[index].ToString());
            //}
        }
       
        protected void ddlPaySupplierStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            DropDownList list = (DropDownList)sender;

            TableCell cell = list.Parent as TableCell;
            DataGridItem item = cell.Parent as DataGridItem;

            int index = item.ItemIndex;
            Int64 OrderId = Int64.Parse(dgOrders.DataKeys[index].ToString());

            Order o = Order.FetchByID(OrderId);
            o.PaySupplierStatus = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), list.SelectedValue);
            o.Save();
        }
        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            int index = 0;
            List<Int64> orderIds = new List<Int64>();
            foreach (DataGridItem item in dgOrders.Items)
            {
                Int64 OrderId = Int64.Parse(dgOrders.DataKeys[index].ToString());
                CheckBox cb = (CheckBox)item.FindControl("chkStatusPayed");
                if (cb.Checked)
                    orderIds.Add(OrderId);
                index++;
            }
            if (orderIds.Count > 0) OrderController.ChangeStatusToPayed(orderIds);

            //save remarks
            index = 0;
            foreach (DataGridItem item in dgOrders.Items)
            {
                Int64 OrderId = Int64.Parse(dgOrders.DataKeys[index].ToString());
                TextBox tb = (TextBox)item.FindControl("txtRemarks");
                OrderController.UpdateOrderRemarks(OrderId, tb.Text);
                index++;
            }

            //Response.Redirect("Orders.aspx", true);
            pnlSaveStatus.Visible = false;
            pnlSave.Visible = false;
            LoadItems();
        }
        protected void btnDontSave_Click(object sender, EventArgs e)
        {
            pnlSaveStatus.Visible = false;
            pnlSave.Visible = false;
            LoadItems();
        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"SupplierId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"SupplierName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"BidId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"BidEndDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"DonationPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"TotalPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PrecentDiscount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PriceAfterDiscount"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Precent"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PaymentForSupplier"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"IsSendReceived"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"OrderDate"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"TransactionStatus"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PaySupplierStatus"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"Remarks"), typeof(string)));

            DateTime from, to = new DateTime();
            if (DateTime.TryParse(datepickerFrom.Value.ToString(), out from))
            {
                TimeSpan tsFrom = new TimeSpan(TimeSelectorFrom.Hour, TimeSelectorFrom.Minute, 0);
                from = from.Date + tsFrom;
            }
            if (DateTime.TryParse(datepickerTo.Value.ToString(), out to))
            {
                TimeSpan tsTo = new TimeSpan(TimeSelectorTo.Hour, TimeSelectorTo.Minute, 0);
                to = to.Date + tsTo;
            }
            List<Int64> SuppliersIdList = FillSupplierList();
            List<bool> IsReceivedSendDate = FillIsSendReceived();
            List<int> StatusIdList = FillStatusList();
            List<int> StatusPayementIdList = FillPayementStatusList();
            string SearchBid = "%" + txtSearchBid.Text.Trim() + "%";
            List<OrderUI> orders = OrderController.GetAllOrders(from, to, SuppliersIdList, IsReceivedSendDate, StatusIdList, StatusPayementIdList, SearchBid);

            foreach (OrderUI order in orders)
            {
                System.Data.DataRow row = dt.NewRow();
                row[0] = order.SupplierId;
                row[1] = order.SupplierName;
                row[2] = order.BidId;
                row[3] = "\"" + order.BidEndDate + "\"";
                row[4] = order.DonationPrice;
                row[5] = order.TotalPrice;
                row[6] = order.PrecentDiscount + "%";
                row[7] = order.PriceAfterDiscount;
                row[8] = order.Precent + "%";
                row[9] = order.PaymentForSupplier;
                row[10] = GlobalStrings.GetYesNo(order.IsSendReceived);
                row[11] = "\"" + order.OrderDate + "\"";
                row[12] = OrdersStrings.GetText(Enum.GetName(typeof(OrderStatus), order.TransactionStatus));
                row[13] = OrdersStrings.GetText(Enum.GetName(typeof(PaymentStatus), order.PaySupplierStatus));;
                row[14] = order.Remarks;
                dt.Rows.Add(row);
            }
            System.Data.DataRow sumRow = dt.NewRow();
            dt.Rows.Add(sumRow);

            System.Data.DataRow sumRowTotalPrice = dt.NewRow();
            sumRowTotalPrice[0] = OrdersStrings.GetText(@"SumTotalPrice") + " " + orders.Sum(o => o.TotalPrice);
            dt.Rows.Add(sumRowTotalPrice);

            System.Data.DataRow sumRowPriceAfterDiscount = dt.NewRow();
            sumRowPriceAfterDiscount[0] = OrdersStrings.GetText(@"SumPriceAfterDiscount") + " " + orders.Sum(o => o.PriceAfterDiscount);
            dt.Rows.Add(sumRowPriceAfterDiscount);

            System.Data.DataRow sumRowSumPaymentForSupplier = dt.NewRow();
            sumRowSumPaymentForSupplier[0] = OrdersStrings.GetText(@"SumPaymentForSupplier") + " " + orders.Sum(o => o.PaymentForSupplier);
            dt.Rows.Add(sumRowSumPaymentForSupplier);

            System.Data.DataRow sumRowSumDonation = dt.NewRow();
            sumRowSumDonation[0] = OrdersStrings.GetText(@"SumDonation") + " " + orders.Sum(o => o.DonationPrice);
            dt.Rows.Add(sumRowSumDonation);
            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=OrdersExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"ActionType"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"SupplierId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"SupplierName"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Phone"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Email"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"BidId"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"Products"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"TotalPrice"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"PaymentForSupplier"), typeof(string)));
            dt.Columns.Add(new System.Data.DataColumn(OrdersStrings.GetText(@"OrderDate"), typeof(string)));


            DateTime from, to = new DateTime();
            if (DateTime.TryParse(datepickerFrom.Value.ToString(), out from))
            {
                TimeSpan tsFrom = new TimeSpan(TimeSelectorFrom.Hour, TimeSelectorFrom.Minute, 0);
                from = from.Date + tsFrom;
            }
            if (DateTime.TryParse(datepickerTo.Value.ToString(), out to))
            {
                TimeSpan tsTo = new TimeSpan(TimeSelectorTo.Hour, TimeSelectorTo.Minute, 0);
                to = to.Date + tsTo;
            }
            List<Int64> SuppliersIdList = FillSupplierList();
            List<bool> IsReceivedSendDate = FillIsSendReceived();
            List<int> StatusIdList = FillStatusList();
            List<int> StatusPayementIdList = FillPayementStatusList();
            string SearchBid = "%" + txtSearchBid.Text.Trim() + "%";
            List<OrderUI> orders = OrderController.GetAllOrders(from, to, SuppliersIdList, IsReceivedSendDate, StatusIdList, StatusPayementIdList, SearchBid);

            foreach (OrderUI order in orders)
            {
                var supplier = SupplierController.GetSupplierUI(order.SupplierId);
                System.Data.DataRow row = dt.NewRow();
                row[0] = "רכישה";
                row[1] = order.SupplierId;
                row[2] = order.SupplierName;
                row[3] = supplier==null?"": "=\"" + supplier.Phone + "\"";
                row[4] = supplier == null ? "" : supplier.Email;
                row[5] = order.BidId;
                row[6] = GetStringProduct(BidController.GetProductsByBid(order.BidId));
                row[7] = order.TotalPrice;
                row[8] = order.PaymentForSupplier;
                row[9] = order.OrderDate;

               
                dt.Rows.Add(row);
            }
            System.Data.DataRow sumRow = dt.NewRow();
            dt.Rows.Add(sumRow);

            SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            Response.Clear();
            Response.AddHeader(@"content-disposition", @"attachment;filename=OrdersExportFoCRM_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            Response.Charset = @"UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = ex.FileContentType;
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.Write(ex.ToString());
            Response.End();
        }
        #endregion
        protected void chkStatusPayed_CheckedChanged(object sender, EventArgs e)
        {
            _edited = true;
        }
        protected void txtRemarks_TextChanged(object sender, EventArgs e)
        {
            _edited = true;
        }


        public static string GetStringProduct(List<BidProductUI> lst)
        {
            string Products = "";
            foreach (BidProductUI item in lst)
            {
                Products += item.Amount.ToString() + " " + item.ProductName + "; ";
            }
            return Products.Substring(0, Products.Length>0? Products.Length - 1:0);
        }

    }
}
