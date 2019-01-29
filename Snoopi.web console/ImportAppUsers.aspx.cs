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

namespace Snoopi.web
{
    public partial class ImportAppUsers : AdminPageBase
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
            Master.PageTitleHtml = AppUsersStrings.GetText(@"ImportAppUsersPageTitle");
            Master.ActiveMenu = "ImportAppUsers";
        }

        protected void btnAcceptFile_Click(object sender, EventArgs e)
        {
            if (fuImportFile.HasFile)
            {
                string[] allowedFileTypes = { "text/csv", "application/vnd.ms-excel" };
                if (allowedFileTypes.Contains(fuImportFile.PostedFile.ContentType)) 
                {
                    string filename = Path.GetRandomFileName() + "_" + Path.GetFileName(fuImportFile.FileName);
                    string filePath = Path.Combine(Settings.GetSetting(Settings.Keys.API_TEMP_UPLOAD_FOLDER), filename);
                    string fullFilePath = filePath;
                    if (!fullFilePath.Contains(@":/") && !fullFilePath.Contains(@":\"))
                    {
                        fullFilePath = Server.MapPath(filePath);
                    }

                    //Save that file in temp folder
                    fuImportFile.SaveAs(fullFilePath);

                    ReadCSVFile(fullFilePath);

                    phErrors.Visible = true;
                    phAppUsersList.Visible = true;
                    phImportResult.Visible = false;
                }
                else
                {
                    Master.MessageCenter.DisplayErrorMessage(AppUsersStrings.GetText(@"ImportFileWrongFormat"));
                    return;
                }               
            }
            else
            {
                lblErrors.Text = AppUsersStrings.GetText(@"MessageFileNotSelected");
                phErrors.Visible = true;
                phAppUsersList.Visible = false;
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
                    CsvDataTable.Columns.Add(new DataColumn("Email", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Password", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("FirstName", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("LastName", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("IsLocked", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Phone", typeof(string)));
                    CsvDataTable.Columns.Add(new DataColumn("Comments", typeof(string)));

                    bool bErrors = false;
                    int line = -1;
                    int numAppUsersToImport = 0;
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
                            DR["Email"] = _values[0] == "" ? "" : _values[0].Trim();
                            DR["Password"] = _values[1] == "" ? "" : _values[1].Trim();
                            DR["FirstName"] = _values[2] == "" ? "" : _values[2].Trim();
                            DR["LastName"] = _values[3] == "" ? "" : _values[3].Trim();                
                            DR["IsLocked"] = _values[4] == "" ? "" : _values[4].Trim();
                            DR["Phone"] = _values[5] == "" ? "" : _values[5].Trim('"');

                             //Email must be unique
                            if (core.DAL.AppUser.FetchByEmail(DR["Email"].ToString()) != null)
                            {
                                if (comments != "") comments += "<br />";
                                comments += AppUsersStrings.GetText(@"MessageEmailAlreadyExists");
                            }
                        }
                        catch (Exception e)
                        {
                            if (comments != "") comments += "<br />";
                            comments += AppUsersStrings.GetText(@"MessageLineParsingError") + ": " + e.Message;
                        }

                        if (comments != "")
                        {
                            bErrors = true;
                            comments += "<br />" + AppUsersStrings.GetText(@"MessageAppUsersWillNotImported");
                        }
                        else
                        {
                            numAppUsersToImport++;
                        }

                        DR["Comments"] = comments;
                        CsvDataTable.Rows.Add(DR);

                        _values = csv.ReadRow();
                    }

                    if (bErrors)
                    {
                        lblErrors.Text = AppUsersStrings.GetText(@"ImportErrorsLabel"); ;
                    }
                    else
                    {
                        lblErrors.Text = AppUsersStrings.GetText(@"ImportNoErrorsLabel"); ;
                    }

                    dgAppUsers.DataSource = CsvDataTable;
                    dgAppUsers.DataBind();
                    lblTotal.Text = CsvDataTable.Rows.Count.ToString();
                    lblTotalToImport.Text = numAppUsersToImport.ToString();
                    btnImport.Enabled = numAppUsersToImport == 0 ? false : true;
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
                    foreach (DataRow appUserRow in CsvDataTable.Rows)
                    {
                        if (appUserRow["Comments"].ToString() == "")
                        {
                            AppUser app_user = null;
                            AppMembership.AppUserCreateResults results = AppMembership.CreateAppUser(appUserRow["Email"].ToString(), appUserRow["Password"].ToString(), @"", out app_user);
                            if (results != AppMembership.AppUserCreateResults.Success)
                            {
                                throw new Exception();
                            }
                            app_user.FirstName = appUserRow["FirstName"].ToString();
                            app_user.LastName = appUserRow["LastName"].ToString();
                            app_user.IsLocked = appUserRow["IsLocked"].ToString() == "0" ? false : true;;
                            app_user.Phone = appUserRow["Phone"].ToString();
                           // app_user.OrderDisplay = OrderDisplay.GetLastOrder() + 1;

                            app_user.Save();
                            count++;
                        }
                    }
                    lblImportResult.Text = AppUsersStrings.GetText(@"MessageImportSuccess");
                }
                catch
                {
                    lblImportResult.Text = AppUsersStrings.GetText(@"MessageImportFailedUnknown");
                }
                phImportResult.Visible = true;
                lblTotalImported.Text = count.ToString();
                btnImport.Enabled = false;
                phErrors.Visible = false;
                phAppUsersList.Visible = false;
            }
        }
    }
}
