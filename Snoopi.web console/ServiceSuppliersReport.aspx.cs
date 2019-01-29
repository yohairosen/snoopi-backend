﻿using dg.Utilities.Spreadsheet;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using Snoopi.web.Localization.Strings.Accessors;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ServiceSuppliersReport : AdminPageBase
{
    bool HasEditPermission = true;
    string filterSearch = null;

    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

    protected void Page_Init(object sender, EventArgs e)
    {
        //HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_edit_users);
        if (Request.QueryString["Filter"] != null)
        {
            filterSearch = Request.QueryString["Filter"];

        }
        filters.Visible = true;

        Master.AddButtonNew(SuppliersStrings.GetText(@"NewSupplierButton"), @"EditSupplier.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });

        dgSuppliers.PageIndexChanged += dgSuppliers_PageIndexChanging;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            datepickerFrom.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();


        int CurrentPageIndex = 0;
        if (!int.TryParse(hfCurrentPageIndex_dgSuppliers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
        if (CurrentPageIndex < 0) CurrentPageIndex = 0;
        dgSuppliers.CurrentPageIndex = CurrentPageIndex;
        LoadItems();
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {

            filters.Visible = true;
            Master.PageTitle = SupplierEventStrings.GetText(@"ServiceSuppliersReportPageTitle");
        

        Master.ActiveMenu = "ServiceSuppliersReport";
        Master.AddClientScriptInclude(@"dgDateManager.js");
    }

    protected void LoadItems()
    {
        if (!HasEditPermission)
        {
            dgSuppliers.Columns[dgSuppliers.Columns.Count - 1].Visible = false;
        }
        string searchName = txtSearchName.Text.Trim();
        string searchPhone = txtSearchPhone.Text.Trim();
        string searchId = txtSearchSupplierId.Text.Trim();
        string searchCity = txtSearchCity.Text.Trim();
        DateTime from, to = new DateTime();
        DateTime.TryParse(datepickerFrom.Text.ToString(), out from);
        DateTime.TryParse(datepickerTo.Text.ToString(), out to);

        dgSuppliers.VirtualItemCount = ServiceController.GetServiceSuppliersAndNumEvents(searchName, searchPhone, searchId, searchCity, from, to, true).Count;
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
            List<SupplierServiceUI> items = ServiceController.GetServiceSuppliersAndNumEvents(searchName, searchPhone, searchId, searchCity, from, to, true, dgSuppliers.PageSize, dgSuppliers.CurrentPageIndex);
            BindList(items);
        }


    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadItems();
    }
    protected void BindList(List<SupplierServiceUI> coll)
    {
        dgSuppliers.ItemDataBound += dgSuppliers_ItemDataBound;
        dgSuppliers.DataSource = coll;
        dgSuppliers.DataBind();
        lblTotal.Text = dgSuppliers.VirtualItemCount.ToString();
        //lblSumOffers.Text = coll.Sum(b => b.BidNum).ToString();
        //lblSumWin.Text = coll.Sum(b => b.OrderNum).ToString();
        //lblSumNoWin.Text = (coll.Sum(b => b.BidNum) - coll.Sum(b => b.OrderNum)).ToString();
        //lblSumActiveBids.Text = coll.Sum(b => b.ActiveNum).ToString();
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
        }
    }

    #region Excel

    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Data.DataTable dt = new System.Data.DataTable();

        dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"SupplierId"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"BusinessName"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"Phone"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"ContactName"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"ContactPhone"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SuppliersStrings.GetText(@"CityName"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierEventStrings.GetText(@"ClickNum"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierEventStrings.GetText(@"ClickToCallNum"), typeof(string)));


        List<SupplierServiceUI> suppliers = new List<SupplierServiceUI>();
        string searchName = txtSearchName.Text.Trim();
        string searchPhone = txtSearchPhone.Text.Trim();
        string searchId = txtSearchSupplierId.Text.Trim();
        string searchCity = txtSearchCity.Text.Trim();
        DateTime from, to = new DateTime();
        DateTime.TryParse(datepickerFrom.Text.ToString(), out from);
        DateTime.TryParse(datepickerTo.Text.ToString(), out to);
        suppliers = ServiceController.GetServiceSuppliersAndNumEvents(searchName, searchPhone, searchId, searchCity, from, to, true);
        foreach (SupplierServiceUI supplier in suppliers)
        {
            System.Data.DataRow row = dt.NewRow();
            row[0] = supplier.SupplierId;
            row[1] = supplier.BusinessName;
            row[2] = "\"" + supplier.Phone + "\"";
            row[3] = supplier.ContactName;
            row[4] = "\"" + supplier.ContactPhone + "\"";
            row[5] = supplier.CityName;
            row[6] = supplier.ClickNum;
            row[7] = supplier.ClickToCallNum;
            dt.Rows.Add(row);
        }
        //System.Data.DataRow sumRow = dt.NewRow();
        //sumRow[0] = SuppliersStrings.GetText(@"SumSumPerMonth") + " " + suppliers.Sum(o => o.SumPerMonth);
        //dt.Rows.Add(sumRow);
        SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

        Response.Clear();
        Response.AddHeader(@"content-disposition", @"attachment;filename=ServiceSuppliersReport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
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