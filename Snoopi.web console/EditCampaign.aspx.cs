using System;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core;
using dg.Utilities;
using Snoopi.core.DAL;
using System.Collections.Generic;
using dg.Sql;
using dg.Sql.Connector;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace Snoopi.web
{
    public partial class EditCampaign : AdminPageBase
    {
        Int64 CampaignId;

        bool IsNewMode = false;

        protected override void VerifyAccessToThisPage()
        {
            //string[] permissions = Permissions.PermissionsForCampaign(SessionHelper.CampaignId());
            //if (!permissions.Contains(Permissions.PermissionKeys.sys_edit_Campaigns))
            //{
            //    Master.LimitAccessToPage();
            //}
            IsNewMode = Request.QueryString[@"New"] != null;

            if (!IsNewMode)
            {
                if (!Int64.TryParse(Request.QueryString[@"CampaignId"], out CampaignId))
                {
                    Master.LimitAccessToPage();
                }
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(CampaignStrings.GetText(@"NewCampaignButton"), @"EditCampaign.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hfOriginalCampaignId.Value = CampaignId.ToString();

                LoadView();
            }
            else
            {
                if (hfOriginalCampaignId.Value.Length > 0 && hfOriginalCampaignId.Value != CampaignId.ToString())
                {
                    Http.Respond404(true);
                }
            }
        }
        protected void LoadView()
        {
            if (CampaignId > 0)
            {
                core.DAL.Campaign campaign = core.DAL.Campaign.FetchByID(CampaignId);

                cbIsDiscount.Checked = campaign.IsDiscount;
                cbIsGift.Checked = campaign.IsGift;
                cStartDate.SelectedDate = campaign.StartDate;
                cEndDate.SelectedDate = campaign.EndDate;
                txtDestinationCount.Text = campaign.DestinationCount.ToString();
                txtDestinationSum.Text = campaign.DestinationSum.ToString();
                txtCampaignName.Text = campaign.CampaignName;
                txtRemarks.Text = campaign.Remarks;
                txtPrecentDiscount.Text = campaign.PrecentDiscount.ToString();
                txtImplemationCount.Text = campaign.ImplemationCount.ToString();
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = CampaignStrings.GetText(IsNewMode ? @"EditCampiagn" : @"EditCampiagn");
            Master.ActiveMenu = IsNewMode ? "NewCampaign" : "Campaigns";

        }

        public string PermissionDescription(object permission)
        {
            return CampaignStrings.GetText(@"Perm_" + permission.ToString());
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            if (cStartDate.SelectedDate != null && cEndDate.SelectedDate != null)
            {
                WhereList w = new WhereList();
                w.Add(new Where(WhereCondition.AND, Campaign.TableSchema.SchemaName, Campaign.Columns.StartDate, WhereComparision.LessThanOrEqual, cStartDate.SelectedDate));
                w.Add(new Where(WhereCondition.AND, Campaign.TableSchema.SchemaName, Campaign.Columns.EndDate, WhereComparision.GreaterThanOrEqual, cStartDate.SelectedDate));
                if (CampaignId != 0) w.Add(new Where(WhereCondition.AND ,Campaign.TableSchema.SchemaName,Campaign.Columns.CampaignId, WhereComparision.NotEqualsTo, CampaignId));
                WhereList w1 = new WhereList();
                w1.Add(new Where(WhereCondition.AND, Campaign.TableSchema.SchemaName ,Campaign.Columns.StartDate, WhereComparision.LessThanOrEqual, cEndDate.SelectedDate));
                w1.Add(new Where(WhereCondition.AND, Campaign.TableSchema.SchemaName, Campaign.Columns.EndDate, WhereComparision.GreaterThanOrEqual, cEndDate.SelectedDate));
                if (CampaignId != 0) w1.Add(new Where(WhereCondition.AND, Campaign.TableSchema.SchemaName, Campaign.Columns.CampaignId, WhereComparision.NotEqualsTo, CampaignId));
                Query q = new Query(Campaign.TableSchema);
                q.OR(w1);
                q.OR(w);               
                if (q.GetCount() > 0)
                {
                    Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText(@"ErrorDate"));
                    return;
                }

            }
            else
            {
                Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText(@"ErrorDate"));
                return;
            }
            Campaign campaign = Campaign.FetchByID(CampaignId);
            if (campaign == null) campaign = new Campaign();
            campaign.IsDiscount = cbIsDiscount.Checked;
            campaign.IsGift = cbIsGift.Checked;
            campaign.StartDate = cStartDate.SelectedDate;
            campaign.EndDate = cEndDate.SelectedDate;
            campaign.DestinationCount = txtDestinationCount.Text != "" ? Convert.ToInt32(txtDestinationCount.Text) : 0;
            campaign.DestinationSum = txtDestinationSum.Text != "" ? Convert.ToInt32(txtDestinationSum.Text) : 0;
            campaign.CampaignName = txtCampaignName.Text;
            campaign.Remarks = txtRemarks.Text;
            campaign.PrecentDiscount = cbIsDiscount.Checked ? (txtPrecentDiscount.Text != "" ? Convert.ToInt32(txtPrecentDiscount.Text) : 0 ): 0;
            campaign.ImplemationCount = txtImplemationCount.Text != "" ? Convert.ToInt32(txtImplemationCount.Text) : 0;
            campaign.Save();
            CampaignId = campaign.CampaignId;
            if (IsNewMode)
            {
                string successMessage = CampaignStrings.GetText(@"MessageCampaignCreated");
                string url = @"EditCampaign.aspx?CampaignId=" + CampaignId;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
            }
            else
            {
                string successMessage = CampaignStrings.GetText(@"MessagCampaignUpdate");
                string url = @"EditCampaign.aspx?CampaignId=" + CampaignId;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);

            }
        }
        protected void ValidateCheckbox_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (cbIsDiscount.Checked && cbIsGift.Checked || !cbIsDiscount.Checked && !cbIsGift.Checked)
            {
                args.IsValid = false;
                Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText(@"ErrorCheckBox"));
            }
        }
        protected void Date_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool result = !(cStartDate.SelectedDate == null || cStartDate.SelectedDate == new DateTime(0001, 1, 1, 0, 0, 0) ||
                cEndDate.SelectedDate == null || cEndDate.SelectedDate == new DateTime(0001, 1, 1, 0, 0, 0) || cEndDate.SelectedDate <= cStartDate.SelectedDate);
            if (result == false)
            {
                args.IsValid = result;
                Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText(@"ErrorDate"));
            }
        }

        protected void NumericOnly_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string pattern = @"^[0-9]*$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            bool result = true;

            MatchCollection matches = rgx.Matches(txtImplemationCount.Text);
            if (matches.Count == 0 && txtImplemationCount.Text != "") result = false;
            matches = rgx.Matches(txtDestinationSum.Text);
            if (matches.Count == 0 && (txtImplemationCount.Text != "" || txtDestinationSum.Text != "")) result = false;
            matches = rgx.Matches(txtDestinationCount.Text);
            if (matches.Count == 0 && (txtImplemationCount.Text != "" || txtDestinationSum.Text != "")) result = false;
            matches = rgx.Matches(txtPrecentDiscount.Text);
            if (matches.Count == 0 && cbIsDiscount.Checked) result = false;
            if (result ==  false)
            {
                args.IsValid = result;
                Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText(@"NumericOnly"));
            }
        }
        protected void ValidateCountOrSum_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txtDestinationCount.Text != "" && txtDestinationSum.Text != "" && txtDestinationCount.Text != "0" && txtDestinationSum.Text != "0")
            {
                args.IsValid = false;
                Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText(@"CountOrSum"));
            }
        }
}
}
