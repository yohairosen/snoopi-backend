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
    public partial class ImportFilters : AdminPageBase
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
            Master.PageTitleHtml = FiltersStrings.GetText(@"ImportFiltersPageTitle");
            Master.ActiveMenu = "ImportFilter";
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
                    CsvDataTable.Columns.Add(new DataColumn("FilterName", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("SubFilterName", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Comments", typeof(string)));

                    bool bErrors = false;
                    int line = -1;
                    int numToImport = 0;
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
                            DR["FilterName"] = _values[0] == "" ? "" : _values[1].Trim();
                            DR["SubFilterName"] = _values[1] == "" ? "" : _values[2].Trim();
                            if (DR["FilterName"] == null || DR["FilterName"].ToString() == "" || DR["SubFilterName"] == null || DR["SubFilterName"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += FiltersStrings.GetText(@"MessageFilterOrSubFilterNameEmpty");
                            }
                            else
                            {
                                Filter f = Filter.FetchByName(DR["FilterName"].ToString());

                                Query q = new Query(Filter.TableSchema);
                                q.Join(JoinType.InnerJoin, SubFilter.TableSchema , SubFilter.TableSchema.SchemaName, new JoinColumnPair(Filter.TableSchema, Filter.Columns.FilterId, SubFilter.Columns.FilterId));
                                q.Where(Filter.Columns.FilterName, DR["FilterName"].ToString()).AddWhere(SubFilter.Columns.SubFilterName,DR["SubFilterName"].ToString());
                                if (q.GetCount() > 0)
                                {
                                    if (comments != "") comments += "<br />";
                                    comments += FiltersStrings.GetText(@"AlreadyExistsComment");
                                }

                            }
                        }
                        catch (Exception e) { }

                        if (comments != "")
                        {
                            bErrors = true;
                            comments += "<br />" + FiltersStrings.GetText(@"MessageWillNotImported");
                        }
                        else
                        {
                            numToImport++;
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

                    dgFilter.DataSource = CsvDataTable;
                    dgFilter.DataBind();
                    lblTotal.Text = CsvDataTable.Rows.Count.ToString();
                    lblTotalToImport.Text = numToImport.ToString();
                    btnImport.Enabled = numToImport == 0 ? false : true;
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
                    foreach (DataRow row in CsvDataTable.Rows)
                    {
                        if (row["Comments"].ToString() == "")
                        {
                            Filter f = Filter.FetchByName(row["FilterName"].ToString());
                            if (f == null)
                            {
                                f = new Filter();
                                f.FilterName = row["FilterName"].ToString();
                                f.Save();
                            }
                            SubFilter sub = new SubFilter();
                            sub.SubFilterName = row["SubFilterName"].ToString();
                            sub.FilterId = f.FilterId;
                            sub.Save(); 
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
