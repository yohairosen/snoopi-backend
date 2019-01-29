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
    public partial class ImportProducts : AdminPageBase
    {
        static DataTable CsvDataTable = null;

        protected override void VerifyAccessToThisPage()
        {
            string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
            if (!permissions.Contains(Permissions.PermissionKeys.sys_perm))
            {
                Master.LimitAccessToPage();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitleHtml = ProductsStrings.GetText(@"ImportProductsPageTitle");
            Master.ActiveMenu = "ImportProducts";
        }

        protected void btnAcceptFile_Click(object sender, EventArgs e)
        {
            if (fuImportFile.HasFile)
            {
                string[] allowedFileTypes = { "text/csv", "application/vnd.ms-excel" };
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
                    CsvDataTable.Columns.Add(new DataColumn("ProductNum", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Amount", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("RecomendedPrice", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Description", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("AnimalType", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("CategoryId", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("SubCategoryId", typeof(string)));
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
                            DR["ProductNum"] = _values[2] == "" ? "" : _values[2].Trim();
                            DR["Amount"] = _values[3] == "" ? "" : _values[3].Trim();
                            DR["RecomendedPrice"] = _values[4] == "" ? "" : _values[4].Trim();
                            DR["Description"] = _values[5] == "" ? "" : _values[5].Trim();
                            DR["AnimalType"] = _values[6] == "" ? "" : _values[6].Trim();
                            DR["CategoryId"] = _values[7] == "" ? "" : _values[7].Trim();
                            DR["SubCategoryId"] = _values[8] == "" ? "" : _values[8].Trim();




                            if (DR["ProductCode"] == null || DR["ProductCode"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += ProductsStrings.GetText(@"MessageProductCodeEmpty");
                            }
                            else
                            {
                                //Code must be unique
                                if (Product.FetchByCode(DR["ProductCode"].ToString()) != null)
                                {
                                    if (comments != "") comments += "<br />";
                                    comments += ProductsStrings.GetText(@"MessageCodeAlreadyExists");
                                }
                            }

                            if (DR["ProductNum"] != null && DR["ProductNum"].ToString() != "")
                            {
                                string regExPattern2 = @"^\d+$";
                                System.Text.RegularExpressions.Regex pattern2 = new System.Text.RegularExpressions.Regex(regExPattern2);
                                if (!pattern2.IsMatch(DR["ProductNum"].ToString()))
                                {
                                    if (comments != "") comments += "<br />";
                                    comments += ProductsStrings.GetText(@"ProductNumInvalid");
                                }
                                //product num must be unique
                                else if (Product.FetchByProductNum(Convert.ToInt64(DR["ProductNum"])) != null)
                                {
                                    if (comments != "") comments += "<br />";
                                    comments += ProductsStrings.GetText(@"ProductNumAlreadyExists");
                                }
                            }

                            if (DR["ProductName"] == null || DR["ProductName"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += ProductsStrings.GetText(@"MessageProductNameEmpty");
                            }

                            if (DR["AnimalType"] == null || DR["AnimalType"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += ProductsStrings.GetText(@"MessageAnimalTypeEmpty");
                            }
                            else
                            {
                                string animals = DR["AnimalType"].ToString();
                                List<string> result = animals.Split(',').ToList();
                                foreach (string animal in result)
                                {
                                    if (Animal.FetchByName(animal.Trim()) == null)
                                    {
                                        if (comments != "") comments += "<br />";
                                        comments += ProductsStrings.GetText(@"MessageAnimalTypeDoesntExist");
                                    }
                                }
                            }
                            Category category = null;
                            SubCategory subCategory = null;
                            if (DR["CategoryId"] == null || DR["CategoryId"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += ProductsStrings.GetText(@"MessageCategoryNameEmpty");
                            }
                            else
                            {
                                category = Category.FetchByID(DR["CategoryId"].ToString());
                                if (category == null)
                                {
                                    if (comments != "") comments += "<br />";
                                    comments += ProductsStrings.GetText(@"MessageCategoryDoesntExists");
                                }
                            }


                            if (DR["SubCategoryId"] == null || DR["SubCategoryId"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += ProductsStrings.GetText(@"MessageSubCategoryNameEmpty");
                            }
                            else
                            {
                                subCategory = SubCategory.FetchByID(Int64.Parse(DR["SubCategoryId"].ToString()), Int64.Parse(DR["CategoryId"].ToString()));
                                if (subCategory == null)
                                {
                                    if (comments != "") comments += "<br />";
                                    comments += ProductsStrings.GetText(@"MessageSubCategoryDoesntExists");
                                }
                            }
 
                        }
                        catch(System.FormatException e)
                        {

                            if (comments != "") comments += "<br />";
                            comments += ProductsStrings.GetText(@"MessageCategoriesOrSubCAtegoriesNotNumeric") + ": " + e.Message;

                        }
                        catch (Exception e)
                        {
                            if (comments != "") comments += "<br />";
                            comments += ProductsStrings.GetText(@"MessageLineParsingError") + ": " + e.Message;
                        }

                        if (comments != "")
                        {
                            bErrors = true;
                            comments += "<br />" + ProductsStrings.GetText(@"MessageProductsWillNotImported");
                        }
                        else
                        {
                            numProductsToImport++;
                        }

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

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (CsvDataTable != null)
            {
                int count = 0;
                try
                {
                    foreach (DataRow productRow in CsvDataTable.Rows)
                    {
                        if (productRow["Comments"].ToString() == "")
                        {
                            Product product = null;
                            if (productRow["ProductCode"].ToString() != "")
                            {
                                product = Product.FetchByCode(productRow["ProductCode"].ToString());
                            }
                            if (product == null && productRow["ProductNum"].ToString() != "")
                            {
                                product = Product.FetchByProductNum(Convert.ToInt64(productRow["ProductNum"]));
                            }

                            if (product == null)
                            {
                                product = new Product();
                            }

                            product.ProductCode = productRow["ProductCode"].ToString();
                            product.ProductNum = productRow["ProductNum"].ToString() == "" ? (Int64?)null : Convert.ToInt64(productRow["ProductNum"]);
                            product.ProductImage = "";
                            product.ProductName = productRow["ProductName"].ToString();
                            product.Amount = productRow["Amount"] != null ? productRow["Amount"].ToString() : "";
                            product.Description = productRow["Description"] != null ? productRow["Description"].ToString() : "";         
                            product.CategoryId = Category.FetchByID(productRow["CategoryId"].ToString()).CategoryId;
                            product.SubCategoryId = SubCategory.FetchByID(Int64.Parse(productRow["SubCategoryId"].ToString()), product.CategoryId).SubCategoryId;
                            product.IsDeleted = false;
                            product.RecomendedPrice = productRow["RecomendedPrice"] == null ? 0 : Convert.ToDecimal(productRow["RecomendedPrice"].ToString());
                            product.Save();

                            string animals = productRow["AnimalType"].ToString();
                            List<string> result = animals.Split(',').ToList();
                            foreach (string animal in result)
                            {
                                ProductAnimal productAnimal = new ProductAnimal();
                                productAnimal.ProductId = product.ProductId;
                                productAnimal.AnimalId = Animal.FetchByName(animal.Trim()).AnimalId;
                                productAnimal.Save();
                            }
                            count++;
                        }
                    }
                    lblImportResult.Text = ProductsStrings.GetText(@"MessageImportSuccess");
                }
                catch
                {
                    lblImportResult.Text = ProductsStrings.GetText(@"MessageImportFailedUnknown");
                }
                phImportResult.Visible = true;
                lblTotalImported.Text = count.ToString();
                btnImport.Enabled = false;
                phErrors.Visible = false;
                phProductsList.Visible = false;
            }
        }

    }
}
