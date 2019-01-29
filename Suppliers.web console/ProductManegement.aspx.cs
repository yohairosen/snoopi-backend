using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.web.Localization;
using Snoopi.core.DAL;
using Snoopi.web;
using dg.Sql;
using System.Text.RegularExpressions;
using dg.Utilities.Spreadsheet;

public partial class ProductManegement : System.Web.UI.Page
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
            LoadItem();
            ddlAnimalType.DataSource = AnimalCollection.FetchAll();
            ddlAnimalType.DataValueField = "AnimalId";
            ddlAnimalType.DataTextField = "AnimalName";
            ddlAnimalType.DataBind();
            ddlAnimalType.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));


            ddlCategory.DataSource = CategoryCollection.FetchAll();
            ddlCategory.DataValueField = "CategoryId";
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));

            ddlSubCategory.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));

        }
        ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
        scriptManager.RegisterPostBackControl(this.btnExport);

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {

    }

    public void LoadItem()
    {
        Int64 AnimalId = (ddlAnimalType.SelectedValue != null && Int64.TryParse(ddlAnimalType.SelectedValue, out AnimalId) && AnimalId > 0 ? AnimalId : 0);
        Int64 CategoryId = (ddlCategory.SelectedValue != null && Int64.TryParse(ddlCategory.SelectedValue, out CategoryId) && CategoryId > 0 ? CategoryId : 0);
        Int64 SubCategoryId = (ddlSubCategory.SelectedValue != null && Int64.TryParse(ddlSubCategory.SelectedValue, out SubCategoryId) && SubCategoryId > 0 ? SubCategoryId : 0);
        dgProducts.DataSource = ProductController.GetAllProductBySupplier(SuppliersSessionHelper.SupplierId(), AnimalId, CategoryId, SubCategoryId, txtSearch.Text.Trim(), cbIsExists.Checked);
        dgProducts.DataBind();

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        bool result = false;

        string[] arr = hfProductIds.Value.Split(',');

        foreach (DataGridItem item in dgProducts.Items)
        {
            object obj = dgProducts.DataKeys[item.ItemIndex];
            if (obj != null && !arr.Contains(obj.ToString())) continue;
            CheckBox cb = item.FindControl("cbIsExist") as CheckBox;
            TextBox txtGift = item.FindControl("txtGift") as TextBox;
            TextBox txtPrice = item.FindControl("txtPrice") as TextBox;
            Int64 ProductId = obj != null ? Convert.ToInt64(obj) : 0;
            SupplierProduct sp = SupplierProduct.FetchByID(SuppliersSessionHelper.SupplierId(), ProductId);
            if (sp != null && cb != null && cb.Checked)
            {
                if (txtGift != null) sp.Gift = txtGift.Text;
                if (txtPrice != null && !String.IsNullOrEmpty(txtPrice.Text))
                    sp.Price = Convert.ToDecimal(txtPrice.Text);
                if (sp.Price > 0)
                {
                    sp.Save();
                    result = true;
                    check_price_deviation(sp);
                }

            }
            else if (sp == null && cb != null && cb.Checked)
            {
                sp = new SupplierProduct();
                if (txtGift != null) sp.Gift = txtGift.Text;
                if (txtPrice != null && !String.IsNullOrEmpty(txtPrice.Text)) sp.Price = Convert.ToDecimal(txtPrice.Text);
                sp.SupplierId = SuppliersSessionHelper.SupplierId();
                sp.ProductId = ProductId;
                sp.CreateDate = DateTime.UtcNow;
                sp.Save();
                result = true;
                check_price_deviation(sp);
            }
            else if (sp != null && cb != null && !cb.Checked)
            {
                SupplierProduct.Delete(SuppliersSessionHelper.SupplierId(), ProductId);
                result = true;
            }

        }
        if (result)
        {
            Master.MessageCenter.DisplaySuccessMessage(ProductsStrings.GetText("SaveSuccess"));
        }
        LoadItem();


    }
    protected void btnRecommend_Click(object sender, EventArgs e)
    {
        Response.Redirect("/Contact.aspx?type=Recomment");
    }
    protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItem();
    }
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        Int64 CategoryId = (ddlCategory.SelectedValue != null && Int64.TryParse(ddlCategory.SelectedValue, out CategoryId) && CategoryId > 0 ? CategoryId : 0);

        ddlSubCategory.DataSource = ProductController.GetAllSubCategory(CategoryId);
        ddlSubCategory.DataBind();
        ddlSubCategory.Enabled = ddlSubCategory.Items.Count > 0 ? true : false;
        ddlSubCategory.Items.Insert(0, new ListItem(ProductsStrings.GetText("Choose"), "0"));

        LoadItem();

    }
    protected void ddlAnimalType_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadItem();
    }
    protected void btnSearchByCode_Click(object sender, EventArgs e)
    {
        LoadItem();
    }
    protected void cbIsExists_CheckedChanged(object sender, EventArgs e)
    {
        LoadItem();
    }

    protected void btnSaveAll_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        Int64 SupplierId = SuppliersSessionHelper.SupplierId();
        (new Query(SupplierProduct.TableSchema).Where(SupplierProduct.Columns.SupplierId, SupplierId).Delete()).Execute();
        ProductCollection pcol = ProductCollection.FetchByQuery(new Query(Product.TableSchema).Where(Product.Columns.IsDeleted, false));
        foreach (Product item in pcol)
        {
            SupplierProduct sp = new SupplierProduct();
            sp.SupplierId = SupplierId;
            sp.ProductId = item.ProductId;
            sp.Gift = "";
            sp.Save();
            check_price_deviation(sp);
        }
        LoadItem();
    }
    protected void btnResetAll_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        Int64 SupplierId = SuppliersSessionHelper.SupplierId();
        (new Query(SupplierProduct.TableSchema).Where(SupplierProduct.Columns.SupplierId, SupplierId).Delete()).Execute();
        LoadItem();
    }

    private void check_price_deviation(SupplierProduct sp)
    {
        decimal priceThrshold, deviationPercentage;
        decimal.TryParse(Settings.GetSetting(Settings.Keys.DEVIATION_LOWEST_THRESHOLD), out priceThrshold);
        decimal.TryParse(Settings.GetSetting(Settings.Keys.DEVIATION_PERCENTAGE), out deviationPercentage);
        var product = Product.FetchByID(sp.ProductId);
        var deviation = PriceDeviation.FetchByID(sp.SupplierId, sp.ProductId);
        bool isDeviated = product.RecomendedPrice > priceThrshold && sp.Price < product.RecomendedPrice * (100 - deviationPercentage) / 100;
        if (isDeviated)
        {
            var supplier = AppSupplier.FetchByID(sp.SupplierId);
            deviation = deviation ?? new PriceDeviation();
            deviation.ProductId = sp.ProductId;
            deviation.ProductName = product.ProductName;
            deviation.RecommendedPrice = product.RecomendedPrice;
            deviation.SupplierId = supplier.SupplierId;
            deviation.SupplierName = supplier.BusinessName;
            deviation.ActualPrice = sp.Price;
            deviation.DeviationPercentage = 100 - 100 * sp.Price / product.RecomendedPrice;
            deviation.IsApproved = false;
            deviation.TimeOfApproval = DateTime.MinValue;
            deviation.Save();
        }
        else if (deviation != null)
            PriceDeviation.Delete(sp.SupplierId, sp.ProductId);
    }

    //protected void NumericOnly_ServerValidate(object source, ServerValidateEventArgs args)
    //{
    //    string pattern = @"^[0-9]*$";

    //    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
    //    bool result = true;
    //    string[] arr = hfProductIds.Value.Split(',');

    //    foreach (DataGridItem item in dgProducts.Items)
    //    {
    //        if (result)
    //        {
    //            object obj = dgProducts.DataKeys[item.ItemIndex];
    //            if (obj != null && !arr.Contains(obj.ToString())) continue;
    //            TextBox txtPrice = item.FindControl("txtPrice") as TextBox;

    //            MatchCollection matches = rgx.Matches(txtPrice.Text);
    //            if (matches.Count == 0 && txtPrice.Text != "") result = false;

    //            if (result == false)
    //            {
    //                args.IsValid = result;
    //                Master.MessageCenter.DisplayErrorMessage(ProductsStrings.GetText(@"NumericOnly"));

    //                // txtPrice.Text = ProductsStrings.GetText(@"NumericOnly");
    //            }
    //        }

    //    }

    //}

    protected void btnImport_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        if (!SuppliersSessionHelper.IsProductSupplier()) return;
        if (SuppliersSessionHelper.SupplierId() == 0) return;

        Response.Redirect(@"ImportProductsForSupplier.aspx", true);


    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Data.DataTable dt = new System.Data.DataTable();

        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductName"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductCode"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Amount"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"RecomendedPrice"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Description"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"AnimalType"), typeof(string)));
        dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductPrice"), typeof(string)));
        //dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"Category"), typeof(string)));
      //  dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"SubCategory"), typeof(string)));
       // dt.Columns.Add(new System.Data.DataColumn(ProductsStrings.GetText(@"ProductRate"), typeof(string)));

        string searchCode = "%" + txtSearch.Text.Trim() + "%";

        List<ProductUI> products = ProductController.GetAllProductUI(searchCode, Int64.Parse(ddlCategory.SelectedValue), Int64.Parse(ddlSubCategory.SelectedValue));
        foreach (ProductUI product in products)
        {
            int i = 0;
            System.Data.DataRow row = dt.NewRow();
            row[i++] = product.ProductName;
            row[i++] = product.ProductCode;
            row[i++] = product.Amount;
            row[i++] = product.RecomendedPrice;
            row[i++] = product.Description;
            row[i++] = ProductController.ConvertListToString(product.AnimalLst);
            SupplierProduct sup_prd = SupplierProduct.FetchByID(SuppliersSessionHelper.SupplierId(), product.ProductId);
            if (sup_prd != null)
                row[i++] = sup_prd.Price;


            //row[i++] = product.CategoryName;
           // row[i++] = product.SubCategoryName;
           // row[i++] = product.ProductRate;
            dt.Rows.Add(row);
        }

        SpreadsheetWriter ex = SpreadsheetWriter.FromDataTable(dt, false, true);

        Response.Clear();
        Response.AddHeader(@"content-disposition", @"attachment;filename=ProductsExport_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + "." + ex.FileExtension);
        Response.Charset = @"UTF-8";
        Response.ContentEncoding = System.Text.Encoding.UTF8;
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        Response.ContentType = ex.FileContentType;
        Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
        Response.Write(ex.ToString());
        Response.End();
    }
}