using Snoopi.core.BL;
using Snoopi.web;
using Snoopi.web.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ClicksHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            List<int> LstYear = new List<int>();
            int start = DateTime.Now.AddYears(-20).Year;
            int end = DateTime.Now.Year;
            for (int i = start; i <= end; i++) LstYear.Add(i);
            ddlyear.DataSource = LstYear;
            ddlyear.DataBind();
            ddlyear.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));

            List<int> LstMonth = new List<int>();
            for (int i = 1; i <= 12; i++) LstMonth.Add(i);
            ddlMonth.DataSource = LstMonth;
            ddlMonth.DataBind();
            ddlMonth.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));
            ddlDayFrom.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));
            ddlDayTo.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));

        }
        LoadItem();
    }

    public void LoadItem()
    {
        int year = Convert.ToInt32(ddlyear.SelectedValue);
        int month = Convert.ToInt32(ddlMonth.SelectedValue);
        int from = Convert.ToInt32(ddlDayFrom.SelectedValue);
        int to = Convert.ToInt32(ddlDayTo.SelectedValue);
        DateTime start = DateTime.MinValue;
        DateTime end = DateTime.MinValue;
        if (year != 0 && month != 0 && from != 0) { start = new DateTime(year, month, from); end = (to != 0 ? new DateTime(year, month, to) : end = new DateTime(year, month, DateTime.DaysInMonth(year, month))); }
        else if (year != 0 && month != 0) { start = new DateTime(year, month, 1); end = new DateTime(year, month, DateTime.DaysInMonth(year, month)); }
        else if (year != 0) { start = new DateTime(year, 1, 1); end = new DateTime(year, 12, 31); }
        //    dgOrders.DataSource = OrderController.GetOrderSupplierHistory(SuppliersSessionHelper.SupplierId(), start, end);
        //     dgOrders.DataBind();
        SupplierServiceUI serviceSupplier = ServiceController.GetServiceSuppliersAndNumEvents( SuppliersSessionHelper.SupplierId(),start, end);

            txtClickNum.Text = serviceSupplier != null? serviceSupplier.ClickNum.ToString():"0";
            txtClicksToCallNum.Text =serviceSupplier != null? serviceSupplier.ClickToCallNum.ToString():"0";

    }

    protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
    {

        List<int> LstDays = new List<int>();
        if (ddlyear.SelectedValue == "0" || ddlMonth.SelectedValue == "0")
        {
            Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText("ChooseYearOrMonth"));
            return;
        }
        DateTime Date = new DateTime(Convert.ToInt32(ddlyear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue), 1);
        for (int i = 1; i <= DateTime.DaysInMonth(Convert.ToInt32(ddlyear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue)); i++) LstDays.Add(i);
        ddlDayFrom.DataSource = LstDays;
        ddlDayFrom.DataBind();
        ddlDayFrom.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));
        ddlDayFrom.Enabled = true;
        ddlDayTo.DataSource = LstDays;
        ddlDayTo.DataBind();
        ddlDayTo.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));
        ddlDayTo.Enabled = true;
    }

}