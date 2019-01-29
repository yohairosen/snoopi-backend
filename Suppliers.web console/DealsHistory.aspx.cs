using dg.Utilities.Spreadsheet;
using Snoopi.core.BL;
using Snoopi.web;
using Snoopi.web.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DealsHistory : System.Web.UI.Page
{

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!SuppliersSessionHelper.IsProductSupplier())
        {
            Response.Redirect("404.aspx");
        }
    }


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
        DateTime? start = null;
        DateTime? end = null;
        if (year != 0 && month != 0 && from != 0) { start = new DateTime(year, month, from); end = (to != 0 ? new DateTime(year, month, to) : end = new DateTime(year, month, DateTime.DaysInMonth(year, month))); }
        else if (year != 0 && month != 0) { start = new DateTime(year, month, 1); end = new DateTime(year, month, DateTime.DaysInMonth(year, month)); }
        else if (year != 0) { start = new DateTime(year, 1, 1); end = new DateTime(year, 12, 31); }
       dgOrders.DataSource = OrderController.GetOrderSupplierHistory(SuppliersSessionHelper.SupplierId(), start, end);
       dgOrders.DataBind();
    
    }

    
    protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        
        List<int> LstDays = new List<int>();
        if (ddlyear.SelectedValue == "0" || ddlMonth.SelectedValue == "0")
        {
            Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText("ChooseYearOrMonth"));
            return;
        }
        DateTime Date = new DateTime(Convert.ToInt32(ddlyear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue),1);
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

    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Data.DataTable dt = new System.Data.DataTable();

        dt.Columns.Add(new System.Data.DataColumn(SupplierProfileStrings.GetText(@"OrderId"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierProfileStrings.GetText(@"OrderDate"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierProfileStrings.GetText(@"BidId"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierProfileStrings.GetText(@"TotalPrice"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierProfileStrings.GetText(@"ApprovedDeal"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(SupplierProfileStrings.GetText(@"DealDetails"), typeof(string)));


        int year = Convert.ToInt32(ddlyear.SelectedValue);
        int month = Convert.ToInt32(ddlMonth.SelectedValue);
        int from = Convert.ToInt32(ddlDayFrom.SelectedValue);
        int to = Convert.ToInt32(ddlDayTo.SelectedValue);
        DateTime? start = null;
        DateTime? end = null;
        if (year != 0 && month != 0 && from != 0) { start = new DateTime(year, month, from); end = (to != 0 ? new DateTime(year, month, to) : end = new DateTime(year, month, DateTime.DaysInMonth(year, month))); }
        else if (year != 0 && month != 0) { start = new DateTime(year, month, 1); end = new DateTime(year, month, DateTime.DaysInMonth(year, month)); }
        else if (year != 0) { start = new DateTime(year, 1, 1); end = new DateTime(year, 12, 31); }
        List<OrderUI> orders = OrderController.GetOrderSupplierHistoryExcel(SuppliersSessionHelper.SupplierId(), start, end);
        foreach (OrderUI order in orders)
        {
            int i = 0;
            System.Data.DataRow row = dt.NewRow();
            row[i++] = order.OrderId;
            row[i++] = order.OrderDate;
            row[i++] = order.BidId;
            row[i++] = order.Price;
            row[i++] = GlobalStrings.GetYesNo(order.IsPayed);
            row[i++] = ListTostring(order.LstProduct);
            dt.Rows.Add(row);
        }

        SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, true, true);

        Response.Clear();
        Response.AddHeader(@"content-disposition", @"attachment;filename=Deals_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
        Response.Charset = @"UTF-8";
        Response.ContentEncoding = System.Text.Encoding.UTF8;
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = ex.FileContentType;
        Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
        Response.Write(ex.ToString());
        Response.End();
    }

    public string ListTostring(List<BidProductUI> LstProduct)
    {
        string products = "";
        foreach (var item in LstProduct)
        {
            products += ProductsStrings.GetText("ProductName") + ":" + item.ProductName + " " + ProductsStrings.GetText("ProductAmountExcel") + ":" + item.ProductAmount +
                ProductsStrings.GetText("OrderAmount") + ":" + item.Amount + " " + ProductsStrings.GetText("ProductCode") + ":" + item.ProductCode+", ";
        }
        return products.Substring(0, products.Length);
    }

    protected void dgOrders_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Footer)
	    {
            e.Item.Cells[2].Text = ProductsStrings.GetText("OrderCount") + " $Count";
            e.Item.Cells[3].Text = SupplierProfileStrings.GetText("Shekel") + " $Sum " + ProductsStrings.GetText("OrderSum");

	    }
        
    }
    //protected void PrintAllPages(object sender, EventArgs e)
    //{
        
    //    dgOrders.AllowPaging = false;
    //    dgOrders.DataBind();
    //  StringWriter sw = new StringWriter();
    //  HtmlTextWriter hw = new HtmlTextWriter(sw);
    //   dgOrders.RenderControl(hw);
    //    string gridHTML = sw.ToString().Replace("\"", "'")
    //        .Replace(System.Environment.NewLine, "");
    //    StringBuilder sb = new StringBuilder();
    //    sb.Append("<script type = 'text/javascript'>");
    //    sb.Append("window.onload = new function(){");
    //    sb.Append("var printWin = window.open('', '', 'left=0");
    //    sb.Append(",top=0,width=1000,height=600,status=0');");
    //    sb.Append("printWin.document.write(\"");
    //    sb.Append(gridHTML);
    //    sb.Append("\");");
    //    sb.Append("printWin.document.close();");
    //    sb.Append("printWin.focus();");
    //    sb.Append("printWin.print();");
    //    sb.Append("printWin.close();};");
    //    sb.Append("</script>");
    //    ClientScript.RegisterStartupScript(this.GetType(), "GridPrint", sb.ToString());
    //    dgOrders.AllowPaging = true;
    //    LoadItem();
    //    //dgOrders.DataBind();
    //}
}