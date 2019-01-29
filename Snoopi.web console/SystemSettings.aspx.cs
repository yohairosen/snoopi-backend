using System;
using System.IO;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.core.BL;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using System.Linq;
using Snoopi.core;
using System.Text;
using System.Collections.Generic;

namespace Snoopi.web
{
    public partial class EditSystemSettings : AdminPageBase
    {
        protected override string[] AllowedPermissions
        {
            get { return new string[] { Permissions.PermissionKeys.sys_perm }; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            chkMailServerSsl.InputAttributes[@"onchange"] = "updateMailSettingsSsl()";
            chkMailServerSsl.InputAttributes[@"onclick"] = "updateMailSettingsSsl()";

            if (!Page.IsPostBack)
            {
                LoadView();
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = SystemSettingsStrings.GetText(@"EditSettingsPageTitle");
            Master.ActiveMenu = "SystemSettings";
        }
        protected void LoadView()
        {
            //txtMatchRadiusFrom.Text = Settings.GetSettingDecimal(Settings.Keys.MATCH_RADIUS_FROM, 0m).ToString(@"0.##");
            //txtMatchRadiusTo.Text = Settings.GetSettingDecimal(Settings.Keys.MATCH_RADIUS_TO, 0m).ToString(@"0.##");
           // txtMatchDateDiff.Text = Settings.GetSettingDecimal(Settings.Keys.MATCH_DATE_DIFF, 0m).ToString();

            txtWebRootUrl.Text = Settings.GetSetting(Settings.Keys.WEB_ROOT_URL) ?? @"";
            txtApiRootUrl.Text = Settings.GetSetting(Settings.Keys.API_ROOT_URL) ?? @"";
            txtAppUsersUploadFolder.Text = Settings.GetSetting(Settings.Keys.APPUSERS_UPLOAD_FOLDER) ?? @"";
            txtApiAppUsersUploadFolder.Text = Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER) ?? @"";
            txtTempUploadFolder.Text = Settings.GetSetting(Settings.Keys.TEMP_UPLOAD_FOLDER) ?? @"";
            txtApiTempUploadFolder.Text = Settings.GetSetting(Settings.Keys.API_TEMP_UPLOAD_FOLDER) ?? @"";
            txtSecureUploadFolder.Text = Settings.GetSetting(Settings.Keys.SECURE_UPLOAD_FOLDER) ?? @"";
            txtPrivacyPolicyUrl.Text = Settings.GetSetting(Settings.Keys.PRIVACY_POLICY_URL) ?? @"";

            chkAppUserVerifyEmail.Checked = Settings.GetSettingBool(Settings.Keys.APPUSER_VERIFY_EMAIL, false);

            txtAdminEmail.Text = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL) ?? @"";
            txtAdminPhone.Text = Settings.GetSetting(Settings.Keys.ADMIN_PHONE) ?? @"";
            txtDefaultEmailFrom.Text = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM) ?? @"";
            txtDefaultEmailFromName.Text = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME) ?? @"";
            txtDefaultEmailReplyTo.Text = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO) ?? @"";
            txtDefaultEmailReplyToName.Text = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME) ?? @"";

            txtMailServerHostName.Text = Settings.GetSetting(Settings.Keys.MailSettings.MAIL_SERVER_HOSTNAME) ?? @"";
            chkMailServerAuthentication.Checked = Settings.GetSettingBool(Settings.Keys.MailSettings.MAIL_SERVER_AUTHENTICAION, false);
            txtMailServerUserName.Text = Settings.GetSetting(Settings.Keys.MailSettings.MAIL_SERVER_USERNAME) ?? @"";
            txtMailServerPassword.Text = txtMailServerPassword.Attributes["value"] = Settings.GetSetting(Settings.Keys.MailSettings.MAIL_SERVER_PASSWORD) ?? @"";
            txtMailServerPort.Text = Settings.GetSettingInt32(Settings.Keys.MailSettings.MAIL_SERVER_PORT, 25).ToString();
            chkMailServerSsl.Checked = Settings.GetSettingBool(Settings.Keys.MailSettings.MAIL_SERVER_SSL, false);


            txtRadiusSupplier.Text = Settings.GetSetting(Settings.Keys.SUPPLIER_RADIUS);
            txtOfferEnd.Text = Settings.GetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS);
            txtActiveBid.Text = Settings.GetSetting(Settings.Keys.END_BID_TIME_MIN);
            txtYad2.Text = Settings.GetSetting(Settings.Keys.YAD_2_EXPIRY_DAY);
            txtRateSupplier.Text = Settings.GetSetting(Settings.Keys.RATE_SUPPLIER_AFTER_ORDER_HOUR);
            txtOfferMinPrice.Text = Settings.GetSetting(Settings.Keys.MIN_PRICE_FOR_OFFER_BIDS) ?? @"";
            txtDeviationLowestThreshold.Text = Settings.GetSetting(Settings.Keys.DEVIATION_LOWEST_THRESHOLD) ?? @"";
            txtDeviationPercentage.Text = Settings.GetSetting(Settings.Keys.DEVIATION_PERCENTAGE) ?? @"";
            txtStartWorkingHour.Text = Settings.GetSetting(Settings.Keys.START_WORKING_TIME) ?? @"";
            txtEndWorkingHour.Text = Settings.GetSetting(Settings.Keys.END_WORKING_TIME) ?? @"";
            cbIsSendingMessagesActive.Checked = Settings.GetSettingBool(Settings.Keys.IS_SYSTEM_ACTIVE, true);
            txtMessageExpiration1.Text = Settings.GetSetting(Settings.Keys.MESSAGE_EXPIRATION_SUPPLIER) ?? @"";
            txtMessageExpiration2.Text = Settings.GetSetting(Settings.Keys.MESSAGE_EXPIRATION_PREMIUM) ?? @"";
            txtMessageExpiration3.Text = Settings.GetSetting(Settings.Keys.MESSAGE_EXPIRATION_SPECIAL_DEAL) ?? @"";
            

            CategoryImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_CATEGORY), 0, 64, 64);
            CategoryImage.ImageUrl = CategoryImage.ImageUrl.Contains(".") ? CategoryImage.ImageUrl : "";
            ImageFileHandler(fuCategoryImage, CategoryImage, BtnCategoryImage, CategoryImage.ImageUrl);

            SubCategoryImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY), 0, 64, 64);
            SubCategoryImage.ImageUrl = SubCategoryImage.ImageUrl.Contains(".") ? SubCategoryImage.ImageUrl : "";
            ImageFileHandler(fuSubCategoryImage, SubCategoryImage, BtnSubCategoryImage, SubCategoryImage.ImageUrl);

            HomeImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_HOME), 0, 64, 64);
            HomeImage.ImageUrl = HomeImage.ImageUrl.Contains(".") ? HomeImage.ImageUrl : "";
            ImageFileHandler(fuImage, HomeImage, btnDeleteImage, HomeImage.ImageUrl);


            setcEmailTemplateNewAppUserWelcome.LoadFromSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME);
            setcEmailTemplateNewAppUserWelcomeVerifyEmail.LoadFromSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME_VERIFY_EMAIL);
            setcEmailTemplateAppUserForgotPassword.LoadFromSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_APPUSER_FORGOT_PASSWORD);
            setcEmailTemplateUserForgotPassword.LoadFromSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_USER_FORGOT_PASSWORD);
            setcEmailTemplateAppuserGift.LoadFromSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_APPUSER_GIFT);

            txtMinAndVersion.Text = Settings.GetSetting(Settings.Keys.MIN_ANDROID_VERSION) ?? @"";
            txtMinIosVersion.Text = Settings.GetSetting(Settings.Keys.MIN_IOS_VERSION) ?? @"";

            txtMinAndSuppVersion.Text = Settings.GetSetting(Settings.Keys.SUPPLIER_MIN_ANDROID_VERSION) ?? @"";
            txtMinIosSuppVersion.Text = Settings.GetSetting(Settings.Keys.SUPPLIER_MIN_IOS_VERSION) ?? @"";
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            decimal dTry;
            int iTry;

            if(!decimal.TryParse(txtRadiusSupplier.Text.Trim(), out dTry)) dTry = 20;
            Settings.SetSetting(Settings.Keys.SUPPLIER_RADIUS, dTry);
            if (!int.TryParse(txtOfferEnd.Text.Trim(), out iTry)) iTry = 24;
            Settings.SetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS, iTry);
            if (!decimal.TryParse(txtActiveBid.Text.Trim(), out dTry)) dTry = 60;
            Settings.SetSetting(Settings.Keys.END_BID_TIME_MIN, dTry);
            if (!int.TryParse(txtYad2.Text.Trim(), out iTry)) iTry = 30;
            Settings.SetSetting(Settings.Keys.YAD_2_EXPIRY_DAY, iTry);
            if (!int.TryParse(txtRateSupplier.Text.Trim(), out iTry)) iTry = 24;
            Settings.SetSetting(Settings.Keys.RATE_SUPPLIER_AFTER_ORDER_HOUR, iTry);
            Settings.SetSetting(Settings.Keys.MIN_PRICE_FOR_OFFER_BIDS, txtOfferMinPrice.Text.Trim());
            Settings.SetSetting(Settings.Keys.DEVIATION_LOWEST_THRESHOLD, txtDeviationLowestThreshold.Text.Trim());
            Settings.SetSetting(Settings.Keys.DEVIATION_PERCENTAGE, txtDeviationPercentage.Text.Trim());
            Settings.SetSetting(Settings.Keys.START_WORKING_TIME, txtStartWorkingHour.Text.Trim());
            Settings.SetSetting(Settings.Keys.END_WORKING_TIME, txtEndWorkingHour.Text.Trim());
            Settings.SetSetting(Settings.Keys.IS_SYSTEM_ACTIVE, cbIsSendingMessagesActive.Checked);
            Settings.SetSetting(Settings.Keys.MESSAGE_EXPIRATION_SUPPLIER, txtMessageExpiration1.Text.Trim());
            Settings.SetSetting(Settings.Keys.MESSAGE_EXPIRATION_PREMIUM, txtMessageExpiration2.Text.Trim());
            Settings.SetSetting(Settings.Keys.MESSAGE_EXPIRATION_SPECIAL_DEAL, txtMessageExpiration3.Text.Trim());


            Settings.SetSetting(Settings.Keys.WEB_ROOT_URL, txtWebRootUrl.Text.Trim());
            Settings.SetSetting(Settings.Keys.API_ROOT_URL, txtApiRootUrl.Text.Trim());
            Settings.SetSetting(Settings.Keys.APPUSERS_UPLOAD_FOLDER, txtAppUsersUploadFolder.Text.Trim());
            Settings.SetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER, txtApiAppUsersUploadFolder.Text.Trim());
            Settings.SetSetting(Settings.Keys.TEMP_UPLOAD_FOLDER, txtTempUploadFolder.Text.Trim());
            Settings.SetSetting(Settings.Keys.API_TEMP_UPLOAD_FOLDER, txtApiTempUploadFolder.Text.Trim());
            Settings.SetSetting(Settings.Keys.SECURE_UPLOAD_FOLDER, txtSecureUploadFolder.Text.Trim());

            Settings.SetSetting(Settings.Keys.PRIVACY_POLICY_URL, txtPrivacyPolicyUrl.Text.Trim());

            Settings.SetSetting(Settings.Keys.APPUSER_VERIFY_EMAIL, chkAppUserVerifyEmail.Checked);

            Settings.SetSetting(Settings.Keys.ADMIN_EMAIL, txtAdminEmail.Text.Trim());
            Settings.SetSetting(Settings.Keys.ADMIN_PHONE, txtAdminPhone.Text.Trim());
            Settings.SetSetting(Settings.Keys.DEFAULT_EMAIL_FROM, txtDefaultEmailFrom.Text.Trim());
            Settings.SetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME, txtDefaultEmailFromName.Text.Trim());
            Settings.SetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO, txtDefaultEmailReplyTo.Text.Trim());
            Settings.SetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME, txtDefaultEmailReplyToName.Text.Trim());

            Settings.SetSetting(Settings.Keys.MailSettings.MAIL_SERVER_HOSTNAME, txtMailServerHostName.Text.Trim());
            Settings.SetSetting(Settings.Keys.MailSettings.MAIL_SERVER_AUTHENTICAION, chkMailServerAuthentication.Checked);
            Settings.SetSetting(Settings.Keys.MailSettings.MAIL_SERVER_USERNAME, txtMailServerUserName.Text.Trim());
            Settings.SetSetting(Settings.Keys.MailSettings.MAIL_SERVER_PASSWORD, txtMailServerPassword.Text.Trim());
            Settings.SetSetting(Settings.Keys.MailSettings.MAIL_SERVER_SSL, chkMailServerSsl.Checked);

            Settings.SetSetting(Settings.Keys.MIN_ANDROID_VERSION, txtMinAndVersion.Text.Trim());
            Settings.SetSetting(Settings.Keys.MIN_IOS_VERSION, txtMinIosVersion.Text.Trim());
            
            Settings.SetSetting(Settings.Keys.SUPPLIER_MIN_ANDROID_VERSION, txtMinAndSuppVersion.Text.Trim());
            Settings.SetSetting(Settings.Keys.SUPPLIER_MIN_IOS_VERSION, txtMinIosSuppVersion.Text.Trim());

            if (!int.TryParse(txtMailServerPort.Text.Trim(), out iTry)) iTry = 25;
            Settings.SetSetting(Settings.Keys.MailSettings.MAIL_SERVER_PORT, iTry);

            setcEmailTemplateNewAppUserWelcome.SaveToSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME);
            setcEmailTemplateNewAppUserWelcomeVerifyEmail.SaveToSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_NEW_APPUSER_WELCOME_VERIFY_EMAIL);
            setcEmailTemplateAppUserForgotPassword.SaveToSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_APPUSER_FORGOT_PASSWORD);
            setcEmailTemplateUserForgotPassword.SaveToSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_USER_FORGOT_PASSWORD);
            setcEmailTemplateAppuserGift.SaveToSettingsWithKey(Settings.Keys.EMAIL_TEMPLATE_APPUSER_GIFT);

            Master.MessageCenter.DisplaySuccessMessage(SystemSettingsStrings.GetText(@"MessageSaveSuccess"));

            if (fuCategoryImage.HasFile)
            {
                MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_CATEGORY), 64, 64, 0);
                string fn = MediaUtility.SaveFile(fuCategoryImage.PostedFile, "Banners", 0);
                Settings.SetSetting(Settings.Keys.BANNER_CATEGORY, fn);
                CategoryImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_CATEGORY), 0, 64, 64);
                ImageFileHandler(fuCategoryImage, CategoryImage, BtnCategoryImage, CategoryImage.ImageUrl);
            }
            else if (Settings.GetSetting(Settings.Keys.BANNER_CATEGORY) != "" && fuCategoryImage.Visible)
            {
                MediaUtility.DeleteImageFilePath("Product", Settings.GetSetting(Settings.Keys.BANNER_CATEGORY), 64, 64, 0);
                Settings.SetSetting(Settings.Keys.BANNER_CATEGORY, "");
            }

            if (fuSubCategoryImage.HasFile)
            {
                MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY), 64, 64, 0);
                string fn = MediaUtility.SaveFile(fuSubCategoryImage.PostedFile, "Banners", 0);
                Settings.SetSetting(Settings.Keys.BANNER_SUB_CATEGORY, fn);
                SubCategoryImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY), 0, 64, 64);
                ImageFileHandler(fuSubCategoryImage, SubCategoryImage, BtnSubCategoryImage, SubCategoryImage.ImageUrl);
            }
            else if (Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY) != "" && fuSubCategoryImage.Visible)
            {
                MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY), 64, 64, 0);
                Settings.SetSetting(Settings.Keys.BANNER_SUB_CATEGORY, "");
            }

            if (fuImage.HasFile)
            {
                MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_HOME), 64, 64, 0);
                string fn = MediaUtility.SaveFile(fuImage.PostedFile, "Banners", 0);
                Settings.SetSetting(Settings.Keys.BANNER_HOME, fn);
                HomeImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_HOME), 0, 64, 64);
                ImageFileHandler(fuImage, HomeImage, btnDeleteImage, HomeImage.ImageUrl);
            }
            else if (Settings.GetSetting(Settings.Keys.BANNER_HOME) != "" && fuImage.Visible)
            {
                MediaUtility.DeleteImageFilePath("Banners", Settings.GetSetting(Settings.Keys.BANNER_HOME), 64, 64, 0);
                Settings.SetSetting(Settings.Keys.BANNER_HOME, "");
            }


            LoadView();
        }
        protected void btnDeleteImage_Click(object sender, System.EventArgs e)
        {
            Button b = (Button)sender;
            System.Web.UI.WebControls.Image img = (PnlBanners.FindControl(b.Attributes["ImageName"].ToString()) as System.Web.UI.WebControls.Image);
            System.Web.UI.WebControls.FileUpload fu = (PnlBanners.FindControl(b.Attributes["FileUploadName"].ToString()) as System.Web.UI.WebControls.FileUpload);

            ImageFileHandler(fu, img, b);
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

        private struct CategoryItem
        {
            internal int CategoryId { get; set; }
            internal int SubCategoryId { get; set; }
            internal int AnimalId { get; set; }
            internal string AnimalName { get; set; }
        }

        /// <summary>
        /// Send a test email and reports if successful
        /// </summary>
        protected void btnTestMailSettings_Click(object sender, EventArgs e)
        {
            try
            {
                MailMessage mail = EmailTemplateController.BuildMailMessage(
                    txtDefaultEmailFrom.Text.Trim(),
                    txtDefaultEmailFromName.Text.Trim(),
                    txtDefaultEmailReplyTo.Text.Trim(),
                    txtDefaultEmailReplyToName.Text.Trim(),
                    txtAdminEmail.Text.Trim(),
                    null,
                    null,
                    "Test mail from " + txtDefaultEmailFrom.Text.Trim(),
                    @"Success",
                    null,
                    null);

                SmtpClient smtp = new SmtpClient();
                if (txtMailServerHostName.Text.Trim().Length > 0)
                {
                    smtp.Host = txtMailServerHostName.Text.Trim();
                    int port;
                    if (!int.TryParse(txtMailServerPort.Text.Trim(), out port)) port = 25;
                    smtp.Port = port;
                }
                if (chkMailServerAuthentication.Checked) smtp.Credentials = new System.Net.NetworkCredential(txtMailServerUserName.Text.Trim(), txtMailServerPassword.Text.Trim());
                smtp.EnableSsl = chkMailServerSsl.Checked;
                smtp.Send(mail);

                Master.MessageCenter.DisplaySuccessMessage(string.Format(SystemSettingsStrings.GetText(@"MessageTestMailSuccess"), txtAdminEmail.Text.Trim()));
            }
            catch (Exception ex)
            {
                Master.MessageCenter.DisplayErrorMessage(string.Format(SystemSettingsStrings.GetText(@"MessageTestMailFailed"), ex.Message));
            }
        }

        protected void btnGenerateSiteMap_Click(object sender, EventArgs e)
        {
            try
            {
                string[] lines = File.ReadAllLines(@"C:\Logs\urls.txt");
                string prefix = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><urlset xmlns = 'http://www.sitemaps.org/schemas/sitemap/0.9' > " + '\n';
                var outputBuilder = new StringBuilder(prefix);

                string item = @" <url>
                                        <loc> https://www.snoopi-app.com/ </loc>
                                        <priority> 1.0 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2017-12-27</lastmod>
                                      </url>" + '\n';
                outputBuilder.Append(item);

                item = @" <url>
                                        <loc> https://www.snoopi-app.com/dog/1/52/ </loc>
                                        <priority> 0.9 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2017-12-27</lastmod>
                                      </url>" + '\n';
                outputBuilder.Append(item);

                item = @" <url>
                                        <loc> https://www.snoopi-app.com/cat/2/52/ </loc>
                                        <priority> 0.9 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2017-12-27</lastmod>
                                      </url>" + '\n';
                outputBuilder.Append(item);

                item = @" <url>
                                        <loc> https://www.snoopi-app.com/dog/1/44/ </loc>
                                        <priority> 0.8 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2017-12-27</lastmod>
                                      </url>" + '\n';
                outputBuilder.Append(item);

                item = @" <url>
                                        <loc> https://www.snoopi-app.com/cat/2/40/ </loc>
                                        <priority> 0.7 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2017-12-27</lastmod>
                                      </url>" + '\n';
                outputBuilder.Append(item);

                item = @" <url>
                                        <loc> https://www.snoopi-app.com/giftbox </loc>
                                        <priority> 0.6 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2017-12-27</lastmod>
                                      </url>" + '\n';
                outputBuilder.Append(item);

                var subCategoriesDic = new Dictionary<string, CategoryItem>();
                var products = ProductController.GetProductsWithAnimalId();
                var categories = new HashSet<string>();
                var productStringBuilder = new StringBuilder();

                foreach (var product in products)
                {
                    int animalId = product.AnimalName.ToLower() == "dog" ? 1 : 2;
                    item = String.Format(@" <url>
                                        <loc> https://www.snoopi-app.com/products/{0}/{1} </loc>
                                       <priority> 0.4 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2018-01-05</lastmod>
                                      </url>" + '\n', product.ProductId, animalId);
                    productStringBuilder.Append(item);
                    string key = product.SubCategoryId + '|' + product.AnimalName;
                    if (!subCategoriesDic.ContainsKey(key))
                    {
                        var subCategory = new CategoryItem
                        {
                            CategoryId = (int)product.CategoryId,
                            SubCategoryId = (int)product.SubCategoryId,
                            AnimalId = animalId,
                            AnimalName = product.AnimalName
                        };
                        subCategoriesDic.Add(key, subCategory);
                    }
                    if (categories.Add(product.CategoryId + product.AnimalName))
                    {
                        //string weight = "0.6";
                        //if (product.CategoryId == 52)
                        //    weight = "0.9";
                        //else if (product.CategoryId == 44 && animalId == 1)
                        //    weight = "0.8";
                        //else if (product.CategoryId == 40 && animalId == 2)
                        //    weight = "0.7";

                        string weight = "0.6";
                        if (product.CategoryId == 52 || product.CategoryId == 44 && animalId == 1 || product.CategoryId == 40 && animalId == 2)
                            continue;

                        item = String.Format(@" <url>
                                        <loc> https://www.snoopi-app.com/{0}/{1}/{2}/ </loc>
                                      <priority> " + weight + @" </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2018-01-05</lastmod>
                                      </url>" + '\n', product.AnimalName, animalId, product.CategoryId);
                        outputBuilder.Append(item);
                    }
                }

                foreach (var subCategory in subCategoriesDic.Values)
                {
                    item = String.Format(@" <url>
                                        <loc> https://www.snoopi-app.com/{0}/{1}/{2}/{3} </loc>
                                      <priority> 0.5 </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2018-01-05</lastmod>
                                      </url>" + '\n', subCategory.AnimalName, subCategory.AnimalId, subCategory.CategoryId, subCategory.SubCategoryId);
                    outputBuilder.Append(item);
                }
                outputBuilder.Append(productStringBuilder);

                foreach (string line in lines)
                {
                    string url = line.Split('|')[0];
                    string weight = line.Split('|')[1];
                    item = @" <url>
                                        <loc> https://www.snoopi-app.com/" + url + @"</loc>
                                        <priority>" + weight + @" </priority>
                                        <changefreq> monthly </changefreq>
                                        <lastmod>2018-01-05</lastmod>
                                      </url>" + '\n';
                    outputBuilder.Append(item);
                }

                outputBuilder.Append('\n' + "</urlset>");
                File.WriteAllText("C:/Logs/sitemap.xml", outputBuilder.ToString());
            }
            catch (Exception ex)
            {
                Master.MessageCenter.DisplayErrorMessage(SystemSettingsStrings.GetText(@"GenerateSiteMapFailed") + " " + ex.Message);
            }
        }
    }
}
