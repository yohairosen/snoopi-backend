using dg.Utilities;
using Snoopi.core;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core.DAL.Entities;
using Snoopi.web;
using Snoopi.web.Localization.Strings.Accessors;
using Snoopi.web.WebControls;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NotificationEdit : AdminPageBase
{
    int FilterId;
    bool IsNewMode = false;

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            hfOriginalAdId.Value = FilterId.ToString();

            LoadView();
        }
        else
        {
            if (hfOriginalAdId.Value.Length > 0 && hfOriginalAdId.Value != FilterId.ToString())
            {
                Http.Respond404(true);
            }
        }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = NotificationStrings.GetText(IsNewMode ? @"NewNotification" : @"EditNotification");
        Master.ActiveMenu = IsNewMode ? "NotificationEdit" : "Notifications";
        Master.AddClientScriptInclude(@"dgDatePicker.js");
    }

    protected void checkUserNumber_Click(object sender, EventArgs e)
    {
        var filter = GetCurrentFilter();
        var users = NotificationGroups.GetUsersOfFilter(filter);
        lblNumOfUsers.Text = users.Count().ToString();
        lblTempUsers.Text = users.Where(x => x.IsTempUser).Count().ToString();
        //lblWebTempUsers.Text = users.Where(x => x.IsTempUser && string.IsNullOrEmpty(x.FcmToken)).Count().ToString();
        lblAndroidUsers.Text = users.Where(x => !x.IsTempUser && !string.IsNullOrEmpty(x.FcmToken)).Count().ToString();
        lblAppleUsers.Text = users.Where(x => !x.IsTempUser && !string.IsNullOrEmpty(x.ApnToken)).Count().ToString(); 
        lblwebUsers.Text = users.Where(x => !x.IsTempUser && string.IsNullOrEmpty(x.ApnToken) && string.IsNullOrEmpty(x.FcmToken)).Count().ToString();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        var filter = GetCurrentFilter();
        if (fuImage.HasFile)
        {
            MediaUtility.DeleteImageFilePath("Banners", filter.AdImageUrl, 64, 64, 0);
            filter.AdImageUrl = MediaUtility.SaveFile(fuImage.PostedFile, "Banners", 0);
            HomeImage.ImageUrl = MediaUtility.GetImagePath("Banners", filter.AdImageUrl, 0, 64, 64);
            ImageFileHandler(fuImage, HomeImage, btnDeleteImage, HomeImage.ImageUrl);
        }
        else if (filter.AdImageUrl != "" && fuImage.Visible)
        {
            MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_HOME), 64, 64, 0);
            filter.AdImageUrl = "";
        }
        filter.Save();

        //if (IsNewMode)
        //{
        //    string successMessage = NotificationStrings.GetText(@"NotificationFilterCreated");
        //    string url = @"NotificationsPanel.aspx";
        //    url += @"&message-success=" + Server.UrlEncode(successMessage);
        //    Response.Redirect(url, true);
        //}
        //else
        {
            string successMessage = NotificationStrings.GetText(@"FilterSaved");
            Master.MessageCenter.DisplaySuccessMessage(successMessage);
            LoadView();
        }
    }

    private NotificationFilter GetCurrentFilter()
    {
        var filter = new NotificationFilter();
        if (!IsNewMode)
            filter = NotificationFilter.FetchByID(FilterId);
        bool isAuto = cbIsAuto.Checked;
        DateTime fromDate;
        DateTime.TryParse(txtFromDate.Text, out fromDate);
        DateTime toDate;
        DateTime.TryParse(txtToDate.Text, out toDate);
        int animalType, area, priority, maxFrequency, minFrequency, groupId, runEvery;
        filter.AnimalTypeId = int.TryParse(ddlAnimalType.SelectedValue, out animalType) ? animalType : 0;
        filter.AreaId = int.TryParse(ddlArea.SelectedValue, out area) ? area : 0;
        filter.Priority = int.TryParse(txtPriority.Text, out priority) ? priority : 0;
        filter.FromDate =fromDate;
        filter.ToDate = toDate;
        filter.MaxFrequency = int.TryParse(txtMaxFrequency.Text, out maxFrequency) ? maxFrequency : int.MaxValue;
        filter.MinFrequency = int.TryParse(txtMinFrequency.Text, out minFrequency) ? minFrequency : 0;
        filter.Group = (NotificationGroupsEnum)(int.TryParse(ddlFilteringGroup.SelectedValue, out groupId) ? groupId : 0);
        filter.MessageTemplate = txtMessage.Text;
        filter.Name = txtName.Text;
        filter.IsAuto = isAuto;
        filter.RunEvery = int.TryParse(txtRunEvery.Text, out runEvery) ? runEvery : 25;
        return filter;
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
            int.TryParse(Request.QueryString[@"filterId"], out FilterId);
        Master.ActiveMenu = "NotificationEdit";
    }
    protected void LoadView()
    {
        string[] itemNames = Enum.GetNames(typeof(NotificationGroupsEnum));
        Array itemValues = Enum.GetValues(typeof(NotificationGroupsEnum)).Cast<int>().ToArray();
        for (int i = 0; i <= itemValues.Length - 1; i++)
        {
            string localizationValue = NotificationStrings.GetText(itemNames[i]);
            ListItem item = new ListItem(localizationValue, itemValues.GetValue(i).ToString());
            ddlFilteringGroup.Items.Add(item);

        }
        ddlFilteringGroup.DataBind();
        var animals = AnimalCollection.FetchAll();
        animals.Add(new Animal { AnimalId = 0, AnimalName = "All" });
        ddlAnimalType.DataSource = animals.OrderBy(x => x.AnimalId);
        ddlAnimalType.DataValueField = "AnimalId";
        ddlAnimalType.DataTextField = "AnimalName";
        ddlAnimalType.DataBind();

        var areas = AreaCollection.FetchAll();
        areas.Add(new Area { AreaId = 0, AreaName = "All" });
        ddlArea.DataSource = areas.OrderBy(x => x.AreaId);
        ddlArea.DataValueField = "AreaId";
        ddlArea.DataTextField = "AreaName";
        ddlArea.DataBind();

        if (FilterId > 0)
        {
            var notificationFilter = NotificationFilter.FetchByID(FilterId);

            ddlArea.SelectedValue = notificationFilter.AreaId.ToString();
            txtFromDate.Text = notificationFilter.FromDate.ToString();
            txtToDate.Text = notificationFilter.ToDate.ToString();
            txtMaxFrequency.Text = notificationFilter.MaxFrequency.ToString();
            txtMinFrequency.Text = notificationFilter.MinFrequency.ToString();
            txtName.Text = notificationFilter.Name;
            txtPriority.Text = notificationFilter.Priority.ToString();
            ddlAnimalType.SelectedValue = notificationFilter.AnimalTypeId.ToString();
            ddlFilteringGroup.SelectedValue = ((int)notificationFilter.Group).ToString();
            txtMessage.Text = notificationFilter.MessageTemplate;
            txtRunEvery.Text = notificationFilter.RunEvery.ToString();
            cbIsAuto.Checked = notificationFilter.IsAuto;

            if (!string.IsNullOrEmpty(notificationFilter.AdImageUrl))
            {
                HomeImage.ImageUrl = MediaUtility.GetImagePath("Banners", notificationFilter.AdImageUrl, 0, 64, 64);
                HomeImage.ImageUrl = HomeImage.ImageUrl.Contains(".") ? HomeImage.ImageUrl : "";
                ImageFileHandler(fuImage, HomeImage, btnDeleteImage, HomeImage.ImageUrl);
            }
        }
    }

    private void ImageFileHandler(FileUpload fu, Image img, Button btn, string value = "")
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