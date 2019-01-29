using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;

namespace Snoopi.web
{
    public partial class EditEmailTemplates : AdminPageBase
    {
        private bool HasEditPermission = false;

        protected override string[] AllowedPermissions
        {
            get { return new string[] { Permissions.PermissionKeys.sys_perm }; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(EmailTemplatesStrings.GetText(@"NewTemplateButton"), @"EditEmailTemplate.aspx", new string[] { Permissions.PermissionKeys.sys_perm });

            dgTemplates.PageIndexChanged += dgTemplates_PageIndexChanging;

            HasEditPermission = Permissions.UserHasPermission(SessionHelper.UserId(), Permissions.PermissionKeys.sys_perm);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgTemplates.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgTemplates.CurrentPageIndex = CurrentPageIndex;

            LoadItems();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = EmailTemplatesStrings.GetText(@"EmailTemplatesPageTitle");
            Master.ActiveMenu = "EmailTemplates";
        }

        protected void LoadItems()
        {
            if (!HasEditPermission)
            {
                dgTemplates.Columns[dgTemplates.Columns.Count - 1].Visible = false;
            }

            dgTemplates.VirtualItemCount = (int)EmailTemplateController.GetItemCount();
            if (dgTemplates.VirtualItemCount == 0)
            {
                phHasItems.Visible = false;
                phHasNoItems.Visible = true;
            }
            else
            {
                phHasItems.Visible = true;
                phHasNoItems.Visible = false;

                int limit = dgTemplates.PageSize;
                int offset = dgTemplates.CurrentPageIndex * dgTemplates.PageSize;

                EmailTemplateCollection items = EmailTemplateController.GetItems(
                    EmailTemplate.Columns.Name, dg.Sql.SortDirection.ASC,
                    limit, offset);

                BindList(items);
            }
        }
        protected void BindList(EmailTemplateCollection coll)
        {
            dgTemplates.ItemDataBound += dgTemplates_ItemDataBound;
            dgTemplates.DataSource = coll;
            dgTemplates.DataBind();
            Master.DisableViewState(dgTemplates);
            lblTotal.Text = dgTemplates.VirtualItemCount.ToString();
        }

        protected void dgTemplates_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
        {
            dgTemplates.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgTemplates.Value = dgTemplates.CurrentPageIndex.ToString();
            LoadItems();
        }
        protected void dgTemplates_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || 
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                HyperLink hlEdit = e.Item.FindControl("hlEdit") as HyperLink;
                hlEdit.Visible = HasEditPermission;

                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = HasEditPermission;
                if (lbDelete.Visible)
                {
                    lbDelete.Attributes.Add("onclick", @"return confirm('" + EmailTemplatesStrings.GetText("MessageConfirmDeleteTemplate").ToJavaScript('\'', false) + @"');return false;");
                }
            }
        }
        protected string FormatEditUrl(object item)
        {
            return string.Format("EditEmailTemplate.aspx?EmailTemplateId={0}", ((EmailTemplate)item).EmailTemplateId);
        }
        protected void lbDelete_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName.Equals("doDelete"))
            {
                if (Permissions.UserHasAnyPermissionIn(SessionHelper.UserId(), "sys_edit_emails"))
                {
                    Int64 EmailTemplateId = Convert.ToInt64(e.CommandArgument);
                    string Name = EmailTemplateController.GetItemName(EmailTemplateId) ?? @"";
                    EmailTemplateController.Delete(EmailTemplateId);
                    Master.MessageCenter.DisplaySuccessMessage(string.Format(EmailTemplatesStrings.GetText("MessageTemplateDeleted"), Name.ToHtml()), true);

                    LoadItems();
                }
                else
                {
                    Master.MessageCenter.DisplayWarningMessage(GlobalStrings.GetText(@"NoPermissionsForAction"));
                }
            }
        }
    }
}
