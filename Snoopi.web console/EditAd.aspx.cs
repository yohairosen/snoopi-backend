using dg.Utilities;
using Snoopi.core;
using Snoopi.core.BL;
using Snoopi.core.DAL.Entities;
using Snoopi.web;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EditAd : AdminPageBase
{
    Int64 AdId;
    bool IsNewMode = false;

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            hfOriginalAdId.Value = AdId.ToString();

            LoadView();
        }
        else
        {
            if (hfOriginalAdId.Value.Length > 0 && hfOriginalAdId.Value != AdId.ToString())
            {
                Http.Respond404(true);
            }
        }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = AdvertisementsStrings.GetText(IsNewMode ? @"NewAdPageTitle" : @"EditAdPageTitle");
        Master.ActiveMenu = IsNewMode ? "EditAdvertisement" : "Advertisements";
        Master.AddClientScriptInclude(@"dgDatePicker.js");
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        Advertisement advertisement = null;
        if (!IsNewMode)
        {
            advertisement = Advertisement.FetchByID(AdId);
        }
        else
        {
            advertisement = new Advertisement();
            advertisement.CreatedDate = DateTime.Now;
        }
        advertisement.BunnerId = (BunnerType)Enum.Parse(typeof(BunnerType), ddlBunner.SelectedValue);
        advertisement.CompanyId = Convert.ToInt64(ddlBusinessName.SelectedValue);


        if (fuImage.HasFile)
        {
            MediaUtility.DeleteImageFilePath("Banners", advertisement.FilePath, 64, 64, 0);
            string fn = MediaUtility.SaveFile(fuImage.PostedFile, "Banners", 0);
            advertisement.FilePath = fn;
            HomeImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", advertisement.FilePath, 0, 64, 64);
            ImageFileHandler(fuImage, HomeImage, btnDeleteImage, HomeImage.ImageUrl);
        }
        else if (advertisement.FilePath != "" && fuImage.Visible)
        {
            MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_HOME), 64, 64, 0);
            advertisement.FilePath = "";
        }

        DateTime fDate = DateTime.MinValue;
        DateTime tDate = DateTime.MinValue;
        DateTime.TryParse(txtFromDate.Text, out fDate);
        DateTime.TryParse(txtToDate.Text, out tDate);
        advertisement.FromDate = fDate;
        advertisement.ToDate = tDate;
        advertisement.Href = href.Text;

        advertisement.Save();


        if (IsNewMode)
        {
            string successMessage = AdvertisementsStrings.GetText(@"MessageCompanyCreated");
            string url = @"EditAd.aspx?AdId=" + advertisement.Id;
            url += @"&message-success=" + Server.UrlEncode(successMessage);
            Response.Redirect(url, true);
        }
        else
        {
            string successMessage = AdvertisementsStrings.GetText(@"MessageAdSaved");

            Master.MessageCenter.DisplaySuccessMessage(successMessage);
            LoadView();

        }
    }

    protected void btnDeleteImage_Click(object sender, System.EventArgs e)
    {
        ImageFileHandler(fuImage, HomeImage, btnDeleteImage);
    }
    #endregion

    #region Methods
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
            if (Int64.TryParse(Request.QueryString[@"adId"], out AdId))
            {
                //AdCompany adCompany = AdCompany.FetchByID(AdId);
                //if (adCompany == null)
                //{
                //    Master.LimitAccessToPage();
                //}
            }
        }
        //else
        //{
        Master.ActiveMenu = "EditAdvertisement";
        //}
    }
    protected void LoadView()
    {
        string[] itemNames = System.Enum.GetNames(typeof(BunnerType));
        Array itemValues = System.Enum.GetValues(typeof(BunnerType)).Cast<int>().ToArray();
        for (int i = 0; i <= itemValues.Length - 1; i++)
        {

            string localizationValue = ResourceManagerAccessor.GetText("Advertisements", itemNames[i]);
            ListItem item = new ListItem(localizationValue, itemValues.GetValue(i).ToString());
            ddlBunner.Items.Add(item);

        }
        ddlBunner.DataBind();
        ddlBusinessName.DataSource = AdvertisementController.GetAllCompanies();
        ddlBusinessName.DataValueField = "Key";
        ddlBusinessName.DataTextField = "Value";
        ddlBusinessName.DataBind();

        if (AdId > 0)
        {
            Advertisement advertisement = Advertisement.FetchByID(AdId);

            ddlBusinessName.SelectedValue = advertisement.CompanyId.ToString();
            txtFromDate.Text = advertisement.FromDate.ToString();
            txtToDate.Text = advertisement.ToDate.ToString();
            href.Text = advertisement.Href;

            ddlBunner.SelectedValue = ((int)advertisement.BunnerId).ToString();

            if (!string.IsNullOrEmpty(advertisement.FilePath))
            {
                HomeImage.ImageUrl = MediaUtility.GetImagePath("Banners", advertisement.FilePath, 0, 64, 64);
                HomeImage.ImageUrl = HomeImage.ImageUrl.Contains(".") ? HomeImage.ImageUrl : "";
                ImageFileHandler(fuImage, HomeImage, btnDeleteImage, HomeImage.ImageUrl);

            }
        }
    }

    private void ImageFileHandler(FileUpload fu, System.Web.UI.WebControls.Image img, Button btn, string value = "")
    {
        if (String.IsNullOrEmpty(value))
        {
            fu.Visible = true;
            img.Visible = false;
            btn.Visible = false;
        }
        else
        {
            fu.Visible = false;
            img.Visible = true;
            img.ImageUrl = value;
            btn.Visible = true;
        }
    }
    #endregion
}