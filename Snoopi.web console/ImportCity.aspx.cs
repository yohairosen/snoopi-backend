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
    public partial class ImportCity : AdminPageBase
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
            Master.PageTitleHtml = GlobalStrings.GetText(@"ImportCityArea");
            Master.ActiveMenu = "ImportCity";
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
                    CsvDataTable.Columns.Add(new DataColumn("AreaName", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("CityName", typeof(string)));
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
                            DR["AreaName"] = _values[2] == "" ? "" : _values[2].Trim();
                            DR["CityName"] = _values[1] == "" ? "" : _values[1].Trim();
                            if (DR["AreaName"] == null || DR["AreaName"].ToString() == "" || DR["CityName"] == null || DR["CityName"].ToString() == "")
                            {
                                if (comments != "") comments += "<br />";
                                comments += FiltersStrings.GetText(@"ReqCityAndAreaName");
                            }
                            else
                            {
                                Area f = Area.FetchByName(DR["AreaName"].ToString());
                                if (f != null)
                                {
                                    Query q = new Query(City.TableSchema);
                                    q.Where(City.Columns.CityName, DR["CityName"].ToString()).AddWhere(City.Columns.AreaId, f.AreaId);
                                    if (q.GetCount() > 0)
                                    {
                                        if (comments != "") comments += "<br />";
                                        comments += GlobalStrings.GetText(@"AlreadyExistsCity");
                                    }
                                }

                            }
                        }
                        catch (Exception e) { }

                        if (comments != "")
                        {
                            bErrors = true;
                            comments += "<br />" + GlobalStrings.GetText(@"MessageWillNotImported");
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
                        lblErrors.Text = GlobalStrings.GetText(@"ImportErrorsLabel");
                    }
                    else
                    {
                        lblErrors.Text = GlobalStrings.GetText(@"ImportNoErrorsLabel"); ;
                    }

                    dgCity.DataSource = CsvDataTable;
                    dgCity.DataBind();
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
                            Area area = Area.FetchByName(row["AreaName"].ToString());
                            if (area == null)
                            {
                                area = new Area();
                                area.AreaName = row["AreaName"].ToString();
                                area.Save();
                            }
                            City city = new City();
                            city.CityName = row["CityName"].ToString();
                            city.AreaId = area.AreaId;
                            city.Save(); 
                            count++;
                        }

                       
                    }
                    
                    lblImportResult.Text = GlobalStrings.GetText(@"MessageImportSuccess");
                }
                catch
                {
                    lblImportResult.Text = GlobalStrings.GetText(@"MessageImportFailedUnknown");
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
