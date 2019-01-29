using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.core.DAL.Entities;
using Snoopi.web;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Web.UI;

public partial class EditAdCompany : AdminPageBase
{
    Int64 CompanyId;
    bool IsNewMode = false;

    protected override void VerifyAccessToThisPage()
    {
        string[] permissions = Permissions.PermissionsForUser(SessionHelper.UserId());
        if (!permissions.Contains(Permissions.PermissionKeys.sys_perm))
        {
            Master.LimitAccessToPage();
        }
        IsNewMode = Request.QueryString[@"New"] != null;

        if (!IsNewMode)
        {
            if (Int64.TryParse(Request.QueryString[@"CompanyId"], out CompanyId))
            {
                AdCompany adCompany = AdCompany.FetchByID(CompanyId);
                if (adCompany == null)
                {
                    Master.LimitAccessToPage();
                }
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            hfOriginalCompanyId.Value = CompanyId.ToString();

            LoadView();
        }
        else
        {
            if (hfOriginalCompanyId.Value.Length > 0 && hfOriginalCompanyId.Value != CompanyId.ToString())
            {
                Http.Respond404(true);
            }
        }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = AdvertisementsStrings.GetText(IsNewMode ? @"NewAdCompanyPageTitle" : @"EditAdCompanyPageTitle");
        Master.ActiveMenu = IsNewMode ? "EditAdCompany" : "AdCompanies";
        Master.AddClientScriptInclude(@"dgDatePicker.js");
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string Email = null;
        bool EmailChanged = false;
        string AdCompanyEmail = null;

        AdCompany company = null;
        if (IsNewMode)
        {
            Membership.UserCreateResults results = Membership.CreateAdCompany(txtEmail.Text, out company);
            switch (results)
            {
                default:
                case Membership.UserCreateResults.UnknownError:
                    Master.MessageCenter.DisplayErrorMessage(SuppliersStrings.GetText(@"MessageCreateFailedUnknown"));
                    return;
                case Membership.UserCreateResults.AlreadyExists:
                    Master.MessageCenter.DisplayErrorMessage(SuppliersStrings.GetText(@"MessageCreateFailedAlreadyExists"));
                    return;
                case Membership.UserCreateResults.InvalidEmailAddress:
                    Master.MessageCenter.DisplayErrorMessage(SuppliersStrings.GetText(@"MessageCreateFailedEmailAddressInvalid"));
                    return;
                case Membership.UserCreateResults.Success:
                    break;
            }
            CompanyId = company.CompanyId;
            company.CreatedDate = DateTime.Now;
            AdCompanyEmail = company.Email;
        }
        else
        {
            company = AdCompany.FetchByID(CompanyId);
            AdCompanyEmail = company.Email;
        }
        company.BusinessName = txtBusinessName.Text;
        company.ContactName = txtContactName.Text;
        company.ContactPhone = txtContactPhone.Text;
        company.Phone = txtPhone.Text;
        company.Description = txtDescription.Text;
        company.Save();

        if (IsNewMode)
        {
            CompanyId = company.CompanyId;
        }

        if (company.Email != txtEmail.Text.Trim().NormalizeEmail())
        {
            if (AdCompany.FetchByEmail(txtEmail.Text.Trim().NormalizeEmail()) != null)
            {
                Master.MessageCenter.DisplayWarningMessage(AppUsersStrings.GetText(@"MessageEmailChangeFailed"));
            }
            else
            {
                company.Email = txtEmail.Text.Trim().NormalizeEmail();
                EmailChanged = true;
            }
        }

        AdCompanyEmail = company.Email;
        company.Save();

        if (IsNewMode)
        {
            string successMessage = AdvertisementsStrings.GetText(@"MessageCompanyCreated");
            string url = @"EditAdCompany.aspx?Email=" + AdCompanyEmail + "&CompanyId=" + company.CompanyId;
            url += @"&message-success=" + Server.UrlEncode(successMessage);
            Response.Redirect(url, true);
        }
        else
        {
            string successMessage = SuppliersStrings.GetText(@"MessageSupplierSaved");
            if (EmailChanged)
            {
                string url = @"EditAdCompany.aspx?message-success=" + Server.UrlEncode(successMessage) + "&CompanyId=" + company.CompanyId;
                if (CompanyId != company.CompanyId)
                {
                    url += @"&Email=" + AdCompanyEmail;
                }
                Response.Redirect(url, true);
            }
            else
            {
                Master.MessageCenter.DisplaySuccessMessage(successMessage);
                LoadView();
            }
        }
    }
    protected void LoadView()
    {
        if (CompanyId > 0)
        {
            AdCompany adCompany = AdCompany.FetchByID(CompanyId);
            txtBusinessName.Text = adCompany.BusinessName;
            txtEmail.Text = adCompany.Email;
            txtEmail.Enabled = true;
            txtContactName.Text = adCompany.ContactName;
            txtContactPhone.Text = adCompany.ContactPhone;
            txtPhone.Text = adCompany.Phone;
            txtDescription.Text = adCompany.Description;
        }
    }
}

