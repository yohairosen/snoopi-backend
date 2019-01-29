using System;
using System.Collections;
using System.Configuration;
using Snoopi.core.DAL;
using System.Web.UI.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using dg.Utilities;
using Snoopi.web.WebControls;
using System.Web.UI;
using Snoopi.core;
using System.IO;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using dg.Sql;
using dg.Utilities.CSV;
using System.Linq;

namespace Snoopi.web
{
    public partial class ImportProductsForSupplier : System.Web.UI.Page
    {
        static DataTable CsvDataTable = null;

        //protected override void VerifyAccessToThisPage()
        //{
        //    string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
        //    //if (!permissions.Contains(Permissions.PermissionKeys.sys_perm))
        //    //{
        //    //    Master.LimitAccessToPage();
        //    //}
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
         //   Master.PageTitleHtml = ProductsStrings.GetText(@"ImportProductsPageTitle");
          //  Master.ActiveMenu = "ImportProducts";
        }

        protected void btnAcceptFile_Click(object sender, EventArgs e)
        {
            if (fuImportFile.HasFile)



            {
                string[] allowedFileTypes = { "text/csv","text/xml","application/vnd.ms-excel" };
                if (allowedFileTypes.Contains(fuImportFile.PostedFile.ContentType))
                {
                    string filename = Path.GetRandomFileName() + "_" + Path.GetFileName(fuImportFile.FileName);
                    string filePath = Path.Combine(Settings.GetSetting(Settings.Keys.TEMP_UPLOAD_FOLDER), filename);
                    string fullFilePath = filePath;
                    if (!fullFilePath.Contains(@":/") && !fullFilePath.Contains(@":\"))
                    {
                        fullFilePath = Server.MapPath(filePath);
                    }

                    //Save that file in temp folder
                    fuImportFile.SaveAs(fullFilePath);

                    ReadCSVFile(fullFilePath);

                    phErrors.Visible = true;
                    phProductsList.Visible = true;
                    phImportResult.Visible = false;
                }
                else
                {
                    Master.MessageCenter.DisplayErrorMessage(ProductsStrings.GetText(@"ImportFileWrongFormat"));
                    return;
                }
            }
            else
            {
                lblErrors.Text = ProductsStrings.GetText(@"MessageFileNotSelected");
                phErrors.Visible = true;
                phProductsList.Visible = false;
                phImportResult.Visible = false;
            }
        }



        private void ReadCSVFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (CsvReader csv = new CsvReader(fs))
                {

                    string[] _values = null;

                    CsvDataTable = new DataTable();
                    CsvDataTable.Columns.Add(new DataColumn("Line", typeof(int)));
                    CsvDataTable.Columns.Add(new DataColumn("ProductName", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("ProductCode", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Amount", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("RecomendedPrice", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Description", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("AnimalType", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Price", typeof(string)));
                    //CsvDataTable.Columns.Add(new DataColumn("CategoryId", typeof(string)));
                    //CsvDataTable.Columns.Add(new DataColumn("SubCategoryId", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Comments", typeof(string)));

                    bool bErrors = false;
                    int line = -1;
                    int numProductsToImport = 0;
                    _values = csv.ReadRow();
                    while (_values != null)
                    {
                        if (++line == 0)
                        {
                            _values = csv.ReadRow();
                            continue;
                        }

                        DataRow DR = CsvDataTable.NewRow();
                        string comments = "";

                        try
                        {
                            DR["Line"] = line;
                            DR["ProductName"] = _values[0] == "" ? "" : _values[0].Trim();
                            DR["ProductCode"] = _values[1] == "" ? "" : _values[1].Trim();
                            //DR["ProductNum"] = _values[2] == "" ? "" : _values[2].Trim();
                            DR["Amount"] = _values[2] == "" ? "" : _values[2].Trim();
                            DR["RecomendedPrice"] = _values[3] == "" ? "" : _values[3].Trim();
                            DR["Description"] = _values[4] == "" ? "" : _values[4].Trim();
                            DR["AnimalType"] = _values[5] == "" ? "" : _values[5].Trim();
                            DR["Price"] = _values[6] == "" ? "" : _values[6].Trim();
                       

                            if (DR["ProductCode"] == null || DR["ProductCode"].ToString() == "")
                            {
                                comments = ProductsStrings.GetText(@"MessageProductCodeEmpty");
                            }
                            else
                            {
                                //Code must be unique
                                if (Product.FetchByCode(DR["ProductCode"].ToString()) != null )
                                {
                                    Product prod = Product.FetchByCode(DR["ProductCode"].ToString());
                                    if (SupplierProduct.FetchByID(SuppliersSessionHelper.SupplierId(), prod.ProductId) != null)
                                    {
                                        //Update
                                        if (DR["Price"] == null || DR["Price"].ToString() == "")
                                        {
                                            //delete from SupplierProduct
                                            comments = ProductsStrings.GetText(@"MessageCodeRemoveProductFromSupplier");//MessageCodeRemoveProductFromSupplier
                                            numProductsToImport++;
                                        }
                                        else //Update price
                                       
                                        {
                                            comments = ProductsStrings.GetText(@"MessageCodeUppdatePriceInSupplier");
                                            numProductsToImport++;
                                        }
                                    }
                                    else
                                    {   
                                        //insert
                                        if (DR["Price"] == null || DR["Price"].ToString() == "")
                                        {
                                            //if (comments != "") comments += "<br />";
                                            comments = ProductsStrings.GetText(@"MessageCodeNotInsertProductToSupplierMissingPrice");
                                        }
                                        else
                                        {
                                        
                                            comments = ProductsStrings.GetText(@"MessageCodeInsertProductToSupplier");
                                            numProductsToImport++;
                                        }
                                   }
                                       
                                   
                                }
                            }
                        }
                     
                        catch (Exception e)
                        {
                  
                            comments = ProductsStrings.GetText(@"MessageLineParsingError") + ": " + e.Message;
                        }

                        //if (comments != "")
                        //{
                        //    bErrors = true;
                        //    comments += "<br />" + ProductsStrings.GetText(@"MessageProductsWillNotImported");
                        //}
                        //else
                        //{
                        //    numProductsToImport++;
                        //}

                        DR["Comments"] = comments;
                        CsvDataTable.Rows.Add(DR);

                        _values = csv.ReadRow();
                    }

                    if (bErrors)
                    {
                        lblErrors.Text = ProductsStrings.GetText(@"ImportErrorsLabel");
                    }
                    else
                    {
                        lblErrors.Text = ProductsStrings.GetText(@"ImportNoErrorsLabel"); ;
                    }

                    dgProducts.DataSource = CsvDataTable;
                    dgProducts.DataBind();
                    lblTotal.Text = CsvDataTable.Rows.Count.ToString();
                    lblTotalToImport.Text = numProductsToImport.ToString();
                    btnImport.Enabled = numProductsToImport == 0 ? false : true;
                }
            }
        }

        private void check_price_deviation(SupplierProduct sp)
        {
            decimal priceThrshold, deviationPercentage;
            decimal.TryParse(Settings.GetSetting(Settings.Keys.DEVIATION_LOWEST_THRESHOLD), out priceThrshold);
            decimal.TryParse(Settings.GetSetting(Settings.Keys.DEVIATION_PERCENTAGE), out deviationPercentage);
            var product = Product.FetchByID(sp.ProductId);
            var deviation = PriceDeviation.FetchByID(sp.SupplierId, sp.ProductId);
            bool isDeviated = sp.Price > priceThrshold && sp.Price < product.RecomendedPrice * (100 - deviationPercentage) / 100;
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

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (CsvDataTable != null)
            {
                int count = 0;
                try
                {
                    foreach (DataRow productRow in CsvDataTable.Rows)
                    {
                        if (productRow["Comments"].ToString() != "")
                        {
                            SupplierProduct suplierprod = null;
                            Product prod = null;
                            if ((productRow["Comments"].ToString()).Equals(ProductsStrings.GetText(@"MessageCodeRemoveProductFromSupplier")))
                            {
                                //Delete product from Supplier
                                prod = Product.FetchByCode(productRow["ProductCode"].ToString());
                                if (prod != null)
                                {
                                    suplierprod = SupplierProduct.FetchByID(SuppliersSessionHelper.SupplierId(), prod.ProductId);
                                    if (suplierprod != null)
                                    //delete
                                    {
                                        SupplierProduct.Delete(SuppliersSessionHelper.SupplierId(), suplierprod.ProductId);
                                        count++;
                                    }

                                }
                            }
                            
                            else  if ((productRow["Comments"].ToString()).Equals(ProductsStrings.GetText(@"MessageCodeInsertProductToSupplier")))
                            {
                                //Insert product to supplier
                                prod = Product.FetchByCode(productRow["ProductCode"].ToString());
                                if (prod != null)
                                    if (!ProductController.IsSupplierProduct(prod.ProductId, SuppliersSessionHelper.SupplierId()))
                                    {
                                        SupplierProduct sp = new SupplierProduct();
                                        sp.SupplierId = SuppliersSessionHelper.SupplierId();
                                        sp.ProductId = prod.ProductId;
                                        sp.CreateDate = DateTime.UtcNow;
                                        sp.Price = Convert.ToDecimal(productRow["Price"]);
                                        sp.Gift = "";
                                        sp.Save();
                                        check_price_deviation(sp);
                                        count++;
                                    }
                                  

                            }
                            else if ((productRow["Comments"].ToString()).Equals(ProductsStrings.GetText(@"MessageCodeUppdatePriceInSupplier")))
                            {
                                //Update price
                                prod = Product.FetchByCode(productRow["ProductCode"].ToString());
                                if (prod != null)
                                    if (ProductController.IsSupplierProduct(prod.ProductId, SuppliersSessionHelper.SupplierId()))
                                    {
                                        SupplierProduct sp = new SupplierProduct();
                                        sp.SupplierId = SuppliersSessionHelper.SupplierId();
                                        sp.Price = Convert.ToDecimal(productRow["Price"]);
                                        sp.ProductId = prod.ProductId;
                                        sp.CreateDate = DateTime.UtcNow;
                                        sp.Gift = "";
                                        sp.Update();
                                        check_price_deviation(sp);
                                        count++;
                                    }
                            }
                          
                        }
            
                    }

                    lblImportResult.Text = ProductsStrings.GetText(@"MessageImportSuccess");
                }
                catch (Exception ex)
                {
                    lblImportResult.Text = ProductsStrings.GetText(@"MessageImportFailedUnknown");
                }
               
                phImportResult.Visible = true;
                lblTotalImported.Text = count.ToString();
                btnImport.Enabled = false;
             //   btnAcceptFile.Visible = false;
                phErrors.Visible = false;
                phProductsList.Visible = false;
            }
        }

    }
}
