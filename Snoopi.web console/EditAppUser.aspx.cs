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
using System.Web;
using System.Globalization;
using GoogleMaps.LocationServices;

namespace Snoopi.web
{
    public partial class EditAppUser : AdminPageBase
    {
        Int64 AppUserId;
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
                if (!Int64.TryParse(Request.QueryString[@"AppUserId"], out AppUserId))
                {
                    string email = Request.QueryString[@"Email"];
                    if (email != null && email.Length > 0)
                    {
                        AppUser app_user = core.DAL.AppUser.FetchByEmail(email);
                        if (app_user == null)
                        {
                            Master.LimitAccessToPage();
                        }
                        else
                        {
                            AppUserId = app_user.AppUserId;
                        }
                    }
                    else
                    {
                        AppUserId = SessionHelper.UserId();
                    }
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(AppUsersStrings.GetText(@"NewAppUserButton"), @"EditAppUser.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hfOriginalAppUserId.Value = AppUserId.ToString();

                LoadView();
            }
            else
            {
                if (hfOriginalAppUserId.Value.Length > 0 && hfOriginalAppUserId.Value != AppUserId.ToString())
                {
                    Http.Respond404(true);
                }
            }
        }

        protected void LoadView()
        {
            if (AppUserId > 0)
            {
                AppUser app_user = core.DAL.AppUser.FetchByID(AppUserId);

                txtEmail.Text = app_user.Email;
                txtEmail.Enabled = false;
                chkIsLocked.Checked = app_user.IsLocked;
                txtFirstName.Text = app_user.FirstName;
                txtlastName.Text = app_user.LastName;
                txtPhone.Text = app_user.Phone;
                //profileImageFile = app_user.ProfileImage;
                txtStreet.Text = app_user.Street;
                ddlCity.SelectedValue = app_user.CityId.ToString();
                txtAptNum.Text = app_user.ApartmentNumber;
                txtHouseNum.Text = app_user.HouseNum;
                txtFloor.Text = app_user.Floor;
                txtStreet.Text = app_user.Street;
            }
            GetCities(AppUserId);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = AppUsersStrings.GetText(IsNewMode ? @"NewAppUserPageTitle" : @"EditAppUserPageTitle");
            Master.ActiveMenu = IsNewMode ? "NewAppUser" : "AppUsers";
            Master.AddClientScriptInclude(@"dgDatePicker.js");

           //trCurrentPassword.Visible = !IsNewMode && AppUserId == SessionHelper.UserId();
            rfvPasswordRequired.Visible = rfvPasswordRequired.Enabled = IsNewMode;
            rfvConfirmPasswordRequired.Visible = rfvConfirmPasswordRequired.Enabled = IsNewMode;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string AppUserEmail = null;
            bool EmailChanged = false;

            AppUser app_user = null;
            if (IsNewMode)
            {
                AppMembership.AppUserCreateResults results = AppMembership.CreateAppUser(txtEmail.Text, txtPassword.Text.Trim(), @"", out app_user);
                switch (results)
                {
                    default:
                    case AppMembership.AppUserCreateResults.UnknownError:
                        Master.MessageCenter.DisplayErrorMessage(AppUsersStrings.GetText(@"MessageCreateFailedUnknown"));
                        return;
                    case AppMembership.AppUserCreateResults.AlreadyExists:
                        Master.MessageCenter.DisplayErrorMessage(AppUsersStrings.GetText(@"MessageCreateFailedAlreadyExists"));
                        return;
                    case AppMembership.AppUserCreateResults.InvalidEmailAddress:
                        Master.MessageCenter.DisplayErrorMessage(AppUsersStrings.GetText(@"MessageCreateFailedEmailAddressInvalid"));
                        return;
                    case AppMembership.AppUserCreateResults.Success:
                        break;
                }
                AppUserId = app_user.AppUserId;
                AppUserEmail = app_user.Email;
                //app_user.OrderDisplay = OrderDisplay.GetLastOrder() + 1;
            }
            else
            {
                app_user = core.DAL.AppUser.FetchByID(AppUserId);
                AppUserEmail = app_user.Email;
            }
            app_user.CityId = Convert.ToInt64(ddlCity.SelectedValue);
            app_user.IsLocked = chkIsLocked.Checked;
            app_user.FirstName = txtFirstName.Text;
            app_user.LastName = txtlastName.Text;
            app_user.Phone = txtPhone.Text;
            app_user.Street = txtStreet.Text;
            app_user.CityId = Int64.Parse(ddlCity.SelectedValue);
            app_user.ApartmentNumber = txtAptNum.Text;
            app_user.HouseNum = txtHouseNum.Text;
            app_user.Floor = txtFloor.Text;

            try
            {
                string city = ddlCity.SelectedItem.Text;
                //var address = (city != "" ? city + " " : "") + " " + (txtStreet.Text != "" ? txtStreet.Text + " " : "") + (txtHouseNum.Text != "" ? txtHouseNum.Text : "");
                var locationService = new GoogleLocationService();
                var point = (city.Trim() != "" ? locationService.GetLatLongFromAddress(city) : new MapPoint());
                app_user.AddressLocation = new Geometry.Point(point.Latitude, point.Longitude);
            }
            catch
            {
                Master.MessageCenter.DisplayErrorMessage(AppUsersStrings.GetText(@"MessageAddressInvalid"));
                return;
            }
            app_user.Save();
            if (IsNewMode) AppUserId = app_user.AppUserId;

            if (app_user.Email != txtEmail.Text.Trim().NormalizeEmail())
            {
                if (AppUser.FetchByEmail(txtEmail.Text.Trim().NormalizeEmail()) != null)
                {
                    Master.MessageCenter.DisplayWarningMessage(AppUsersStrings.GetText(@"MessageEmailChangeFailed"));
                }
                else
                {
                    app_user.Email = txtEmail.Text.Trim().NormalizeEmail();
                    app_user.UniqueIdString = app_user.Email;
                    AppUserEmail = app_user.Email;
                    EmailChanged = true;
                }
            }

            AppUserEmail = app_user.Email;

            app_user.Save();
 
            if (txtPassword.Text.Length > 0)
            {
                AppMembership.AppUserPasswordChangeResults results;
                //if (AppUserId == SessionHelper.UserId())
                //{
                //    results = AppMembership.ChangeAppUserPassword(app_user.Email, txtCurrentPassword.Text, txtPassword.Text);
                //}
                //else
                //{
                    results = AppMembership.ChangeAppUserPassword(app_user.Email, txtPassword.Text);
                //}
                switch (results)
                {
                    default:
                        Master.MessageCenter.DisplayWarningMessage(AppUsersStrings.GetText(@"MessagePasswordChangeFailedUnknown"));
                        break;
                    case AppMembership.AppUserPasswordChangeResults.PasswordDoNotMatch:
                        Master.MessageCenter.DisplayWarningMessage(AppUsersStrings.GetText(@"MessagePasswordChangeBadOldPassword"));
                        break;
                    case AppMembership.AppUserPasswordChangeResults.Success:
                        break;
                }
            }
 
            if (IsNewMode)
            {
                string successMessage = AppUsersStrings.GetText(@"MessageAppUserCreated");
                string url = @"EditAppUser.aspx?Email=" + AppUserEmail;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
            }
            else
            {
                string successMessage = AppUsersStrings.GetText(@"MessageAppUserSaved");
                if (EmailChanged)
                {
                    string url = @"EditAppUser.aspx?message-success=" + Server.UrlEncode(successMessage);
                    if (AppUserId != SessionHelper.UserId())
                    {
                        url += @"&Email=" + AppUserEmail;
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

        private void GetCities(Int64 AppUserId)
        {
            if (ddlCity.Items.Count == 0)
            {
                Query q = new Query(City.TableSchema).SelectAll().OrderBy(City.Columns.CityName, dg.Sql.SortDirection.ASC);
                CityCollection citiesList = CityCollection.FetchByQuery(q);

                Int64 selectedCityId = AppUserId == 0 ? citiesList[0].CityId : AppUser.FetchByID(AppUserId).CityId;

                int index = 0;
                foreach (City city in citiesList)
                {
                    ddlCity.Items.Add(new ListItem(city.CityName, city.CityId.ToString()));

                    //check if to select this item 
                    if (selectedCityId == city.CityId)
                    {
                        ddlCity.Items[index].Selected = true;
                    }
                    index++;
                }
            }
        }


    }
}
