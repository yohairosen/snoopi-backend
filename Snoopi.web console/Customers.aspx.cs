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
    public partial class Customers : AdminPageBase
    {
        bool HasEditPermission = false;
        string CustomerId;
        CustomerType customerType;
        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            dgCustomers.PageIndexChanged += dgCustomers_PageIndexChanging;

            if (Request.QueryString["Id"] != null)
            {
                CustomerId = Request.QueryString["Id"].ToString();
            }

            if (Request.QueryString["Type"] != null)
            {
                customerType = (CustomerType)int.Parse(Request.QueryString["Type"]);
            }
        }

        protected void dgCustomers_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgCustomers.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgCustomers.Value = dgCustomers.CurrentPageIndex.ToString();
            LoadItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgCustomers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgCustomers.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
           
            Master.ActiveMenu = "Customers";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems()
        {
           
            
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;
                if (dgCustomers.PageSize * dgCustomers.CurrentPageIndex > dgCustomers.VirtualItemCount)
                {
                    dgCustomers.CurrentPageIndex = 0;
                    hfCurrentPageIndex_dgCustomers.Value = dgCustomers.CurrentPageIndex.ToString();
                }
                CustomerUI customer = BidController.GetCustomerData(CustomerId, customerType);
                Master.PageTitle = customerType == CustomerType.AppUser ? BidString.GetText(@"CustomersPageTitle") + customer.CustomerName : BidString.GetText(@"CustomersTempPageTitle"); 
                BindList(customer);
            



        }
        protected void BindList(CustomerUI c)
        {
            List<CustomerUI> coll = new List<CustomerUI>();
            coll.Add(c);
            dgCustomers.DataSource = coll;
            dgCustomers.DataBind();
            if (customerType == CustomerType.Temp)
            {
                dgCustomers.Columns[5].Visible = false;
                dgCustomers.Columns[6].Visible = false;
            }
            Master.DisableViewState(dgCustomers);
        }




        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        #region Excel

        protected void btnExport_Click(object sender, EventArgs e)
        {
            //System.Data.DataTable dt = new System.Data.DataTable();

            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"Email"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"FirstName"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"LastName"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"IsLocked"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"Phone"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"Address"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"IsAdv"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"LastLogin"), typeof(string)));
            //dt.Columns.Add(new System.Data.DataColumn(CustomersStrings.GetText(@"CreateDate"), typeof(string)));

            //string searchName = "%" + txtSearchName.Text.Trim() + "%";
            //string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
            //DateTime from, to = new DateTime();
            //DateTime.TryParse(dpSearchCreateDateFrom.Value.ToString(), out from);
            //DateTime.TryParse(dpSearchCreateDateTo.Value.ToString(), out to);

            //List<BidUI> app_users = BidUI.GetAllBidUI(from, to, searchName, searchPhone);

            //foreach (BidUI Bid in app_users)
            //{
            //    System.Data.DataRow row = dt.NewRow();
            //    row[0] = Bid.Email;
            //    row[1] = Bid.FirstName;
            //    row[2] = Bid.LastName;
            //    row[3] = GlobalStrings.GetYesNo(Bid.IsLocked);
            //    row[4] = "\"" + Bid.Phone + "\"";
            //    row[5] = Bid.Street + " " + Bid.HouseNum + "\n"
            //        + CustomersStrings.GetText(@"Floor") + " " + Bid.Floor + "\n"
            //        + CustomersStrings.GetText(@"ApartmentNumber") + " " + Bid.ApartmentNumber + "\n"
            //        + Bid.CityName;
            //    row[6] = GlobalStrings.GetYesNo(Bid.IsAdv);
            //    row[7] = "\"" + Bid.LastLogin + "\"";
            //    row[8] = "\"" + Bid.CreateDate + "\"";
            //    dt.Rows.Add(row);
            //}

            //SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

            //Response.Clear();
            //Response.AddHeader(@"content-disposition", @"attachment;filename=CustomersExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
            //Response.Charset = @"UTF-8";
            //Response.ContentEncoding = System.Text.Encoding.UTF8;
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = ex.FileContentType;
            //Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            //Response.Write(ex.ToString());
            //Response.End();
        }
        #endregion

    }

}
