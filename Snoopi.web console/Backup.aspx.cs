using System;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Net.Mail;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.Common;
using dg.Sql.Connector;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using dg.Utilities;
using Snoopi.core.DAL;
using Snoopi.core.BL;
using System.IO.Compression;

namespace Snoopi.web
{
    public partial class Backup : AdminPageBase
    {
        protected override string[] AllowedPermissions
        {
            get { return new string[] { Permissions.PermissionKeys.sys_perm }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.MessageCenter.RegisterValidationSummaryForValidationGroup(@"Restore");
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = DatabaseBackupStrings.GetText(@"BackupPageTitle");
            Master.ActiveMenu = "Backup";

            chkAcceptRestore1.Checked = chkAcceptRestore2.Checked = false;
        }
        protected void btnBackup_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    using (ConnectorBase conn = ConnectorBase.NewInstance())
                    {
                        Response.Clear();
                        Response.AddHeader(@"content-disposition", @"attachment;filename=MySqlBackup_" + DateTime.UtcNow.ToString(@"yyyy_MM_dd_HH_mm_ss") + (chkBackupToGzip.Checked ? @".gz" : ".sql"));
                        Response.Charset = @"UTF-8";
                        Response.ContentEncoding = System.Text.Encoding.UTF8;
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        if (chkBackupToGzip.Checked)
                        {
                            Response.ContentType = @"application/x-gzip";
                        }
                        else
                        {
                            Response.ContentType = @"application/octet-stream";
                        }

                        MySqlBackup.BackupOptions options = new MySqlBackup.BackupOptions();
                        options.BOM = true;
                        options.WrapInTransaction = true;
                        options.ExportTableCreate = true;
                        options.ExportTableDrop = true;
                        options.ExportTableData = true;
                        options.ExportRoutines = true;
                        options.ExportTriggers = true;

                        if (chkBackupToGzip.Checked)
                        {
                            using (GZipStream gzipStream = new GZipStream(Response.OutputStream, CompressionMode.Compress))
                            {
                                MySqlBackup.GenerateBackup((MySqlConnector)conn, gzipStream, options);
                            }
                        }
                        else
                        {
                            MySqlBackup.GenerateBackup((MySqlConnector)conn, Response.OutputStream, options);
                        }

                        Response.End();
                    }
                }
                catch (DbException ex)
                {
                    Response.ContentType = @"text/html";
                    try
                    {
                        Response.Headers.Remove(@"content-disposition");
                    }
                    catch
                    {
                        Response.ClearHeaders();
                    }
                    Response.Clear();
                    Master.MessageCenter.DisplayErrorMessage(ex.Message);
                }
            }
        }
        protected void btnRestore_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (fuRestoreFile.PostedFile == null || fuRestoreFile.PostedFile.ContentLength == 0)
                {
                    Master.MessageCenter.DisplayWarningMessage(DatabaseBackupStrings.GetText(@"BackupFileEmptyErrorMessage"));
                    return;
                }

                string fullSqlPath = Files.AquireUploadFileName(@"MySqlBackup.sql", Settings.GetSetting(Settings.Keys.SECURE_UPLOAD_FOLDER), null, true, true, false);
                fuRestoreFile.PostedFile.SaveAs(fullSqlPath);
                Files.TemporaryFileDeleter fileDeleter = new Files.TemporaryFileDeleter(fullSqlPath);
                try
                {
                    string sql = @"";
                    if (Path.GetExtension(fuRestoreFile.PostedFile.FileName).ToLower() == @".gz")
                    {
                        using (GZipStream gzipStream = new GZipStream(File.Open(fullSqlPath, FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(gzipStream, Encoding.UTF8, true))
                            {
                                sql = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        sql = File.ReadAllText(fullSqlPath, Encoding.UTF8);
                    }

                    using (ConnectorBase conn = ConnectorBase.NewInstance())
                    {
                        conn.ExecuteScript(sql);
                    }

                    fileDeleter.DeleteFile();
                    Master.MessageCenter.DisplaySuccessMessage(DatabaseBackupStrings.GetText(@"RestoreFinishedMessage"));
                }
                catch (System.Exception ex)
                {

                    Master.MessageCenter.DisplayErrorMessage(string.Format(DatabaseBackupStrings.GetText(@"RestoreGeneralError"), ex.Message));
                }
            }
        }

        protected void Validate_ChkAcceptRestore1(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = chkAcceptRestore1.Checked;
        }
        protected void Validate_ChkAcceptRestore2(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = chkAcceptRestore1.Checked;
        }
    }
}
