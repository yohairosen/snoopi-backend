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
    public partial class EditSupplier : AdminPageBase
    {
        Int64 SupplierId;
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
                if (Int64.TryParse(Request.QueryString[@"SupplierId"], out SupplierId))
                {
                    AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
                    if (supplier == null)
                    {
                        Master.LimitAccessToPage();
                    }
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            Master.AddButtonNew(SuppliersStrings.GetText(@"NewSupplierButton"), @"EditSupplier.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
            ddlIsProduct.Items.Add(new ListItem(SuppliersStrings.GetText(@"IsProduct"), @"prod"));
            ddlIsProduct.Items.Add(new ListItem(SuppliersStrings.GetText(@"IsService"), @"ser"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hfOriginalSupplierId.Value = SupplierId.ToString();

                LoadView();
            }
            else
            {
                if (hfOriginalSupplierId.Value.Length > 0 && hfOriginalSupplierId.Value != SupplierId.ToString())
                {
                    Http.Respond404(true);
                }
            }
        }

        protected void LoadView()
        {
            rangeValidatorTxtPrecent.ErrorMessage = SuppliersStrings.GetText(@"RangeBetween0And100Error");
            if (SupplierId > 0)
            {
                SupplierUI supplier = SupplierController.GetSupplierUI(SupplierId);
                txtBusinessName.Text = supplier.BusinessName;
                txtEmail.Text = supplier.Email;
                txtEmail.Enabled = true;
               // txtEmail.Enabled = false;
                ddlIsProduct.SelectedValue = @"prod";
                if (!supplier.IsProduct)
                    ddlIsProduct.SelectedValue = @"ser";
    
                //chkIsProduct.Checked = supplier.IsProduct;
                chkIsPremium.Checked = supplier.IsPremium;
                //chkIsService.Checked = supplier.IsService;
                services.Visible = supplier.IsService;
                txtContactName.Text = supplier.ContactName;
                txtContactPhone.Text = supplier.ContactPhone;
                txtPhone.Text = supplier.Phone;
                chkIsLocked.Checked = supplier.IsLocked;
                txtStreet.Text = supplier.Street;
                txtHouseNum.Text = supplier.HouseNum;
                txtPrecent.Text = supplier.Precent.ToString();
                txtSumPerMonth.Text = supplier.SumPerMonth.ToString();
                //chkAllowChangeStatusJoinBid.Checked = supplier.AllowChangeStatusJoinBid;
                //chkIsStatusJoinBid.Checked = supplier.StatusJoinBid;
                //txtMaxWinningsNum.Text = supplier.MaxWinningsNum.ToString();
                txtMastercardCode.Text = supplier.MastercardCode;
            }
            //cities list
            GetCities(SupplierId);
            //services list

            GetServices(SupplierId);
        }

        private void GetServices(Int64 SupplierId)
        {
            ServiceCollection services = ServiceCollection.FetchAll();
            int index = 0;
            //ddlEducationLevel.Items.Add(new ListItem(GlobalStrings.GetText(@"NoneForDropDowns"), "0"));
            ddlServices.Items.Clear();
            foreach (var item in services)
            {
                ddlServices.Items.Add(new ListItem(item.ServiceName, item.ServiceId.ToString()));
                if (SupplierController.IsSelectedService(SupplierId, item.ServiceId))
                {
                    ddlServices.Items[index].Selected = true;
                }
                index++;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = SuppliersStrings.GetText(IsNewMode ? @"NewSupplierPageTitle" : @"EditSupplierPageTitle");
            Master.ActiveMenu = IsNewMode ? "NewSupplier" : "Suppliers";
            Master.AddClientScriptInclude(@"dgDatePicker.js");
            
            rfvPasswordRequired.Visible = rfvPasswordRequired.Enabled = IsNewMode;
            rfvConfirmPasswordRequired.Visible = rfvConfirmPasswordRequired.Enabled = IsNewMode;
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string SupplierEmail = null;
            bool EmailChanged = false;

            AppSupplier supplier = null;
            if (IsNewMode)
            {
                Membership.UserCreateResults results = Membership.CreateSupplier(txtEmail.Text, txtPassword.Text.Trim(), Convert.ToInt64(ddlCity.SelectedValue), out supplier);
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
                SupplierId = supplier.SupplierId;
                SupplierEmail = supplier.Email;
                //supplier.OrderDisplay = OrderDisplay.GetLastOrder() + 1;
            }
            else
            {
                supplier = core.DAL.AppSupplier.FetchByID(SupplierId);
                SupplierEmail = supplier.Email;
            }
            supplier.BusinessName = txtBusinessName.Text;

            if (ddlIsProduct.SelectedValue == "prod")
            {
                supplier.IsProduct = true;
                supplier.IsService = false;
            }
            else
            {
                supplier.IsProduct = false;
                supplier.IsService = true;
            }
            //supplier.IsProduct = chkIsProduct.Checked;
            //supplier.IsService = chkIsService.Checked;
            supplier.IsPremium = chkIsPremium.Checked;
            supplier.IsLocked = chkIsLocked.Checked;
            supplier.ContactName = txtContactName.Text;
            supplier.ContactPhone = txtContactPhone.Text;
            supplier.Phone = txtPhone.Text;
            supplier.CityId = Convert.ToInt64(ddlCity.SelectedValue);
            supplier.Street = txtStreet.Text;
            supplier.HouseNum = txtHouseNum.Text;
            try
            {
                string city = ddlCity.SelectedItem.Text;
                //var address = (city != "" ? city + " " : "") +" "+ (txtStreet.Text != "" ? txtStreet.Text+" " : "") + (txtHouseNum.Text != "" ? txtHouseNum.Text : "");
                var locationService = new GoogleLocationService();
                var point = (city.Trim() != "" ? locationService.GetLatLongFromAddress(city) : new MapPoint());
                supplier.AddressLocation = new Geometry.Point(point.Latitude, point.Longitude);           

            }
            catch (Exception) {
                supplier.AddressLocation = new Geometry.Point(0, 0);   
            }
            supplier.HouseNum = txtHouseNum.Text;

            supplier.Precent = txtPrecent.Text != "" ?Convert.ToInt32(txtPrecent.Text):0;
            supplier.SumPerMonth = txtSumPerMonth.Text != "" ? Convert.ToInt32(txtSumPerMonth.Text) : 0;            
            //supplier.StatusJoinBid = chkIsStatusJoinBid.Checked;
            //supplier.AllowChangeStatusJoinBid = chkAllowChangeStatusJoinBid.Checked;
            //supplier.MaxWinningsNum =txtMaxWinningsNum.Text != "" ? Convert.ToInt32(txtMaxWinningsNum.Text) : 0;
            supplier.MastercardCode = txtMastercardCode.Text;
            supplier.Save();

            if (IsNewMode) { 
                SupplierId = supplier.SupplierId;
                //if (chkIsStatusJoinBid.Checked == false)//handel
                //{
                //    (new Query(SupplierProduct.TableSchema).Where(SupplierProduct.Columns.SupplierId, SupplierId).Delete()).Execute();
                //    ProductCollection pcol = ProductCollection.FetchByQuery(new Query(Product.TableSchema).Where(Product.Columns.IsDeleted, false));
                //    foreach (Product item in pcol)
                //    {
                //        SupplierProduct sp = new SupplierProduct();
                //        sp.SupplierId = SupplierId;
                //        sp.ProductId = item.ProductId;
                //        sp.Gift = "";
                //        sp.Save();
                //    }
                //}
            
            }
            //if (chkIsService.Checked)
            if (ddlIsProduct.SelectedValue != "prod")
            {
                foreach (ListItem item in ddlServices.Items)
                {
                    if (item.Selected)
                    {
                        SupplierService supplierService = SupplierService.FetchByID(Convert.ToInt64(item.Value), SupplierId);
                        if (supplierService == null)
                        {
                            supplierService = new SupplierService();
                            supplierService.SupplierId = SupplierId;
                            supplierService.ServiceId = Convert.ToInt64(item.Value);
                            supplierService.Save();
                        }
                    }
                    else
                    {
                        SupplierService.Delete(Convert.ToInt64(item.Value), SupplierId);
                    }
                }
            }
            else
            {
                SupplierController.DeleteAllSupplierServices(SupplierId);
            }

            if (supplier.Email != txtEmail.Text.Trim().NormalizeEmail())
            {
                if (AppSupplier.FetchByEmail(txtEmail.Text.Trim().NormalizeEmail()) != null)
                {
                    Master.MessageCenter.DisplayWarningMessage(AppUsersStrings.GetText(@"MessageEmailChangeFailed"));
                }
                else
                {
                    supplier.Email = txtEmail.Text.Trim().NormalizeEmail();
                    supplier.UniqueIdString = supplier.Email;//email.NormalizeEmail();
                    SupplierEmail = supplier.Email;
                    EmailChanged = true;
                }
            }

            SupplierEmail = supplier.Email;
            supplier.Save();

            if (txtPassword.Text.Length > 0)
            {
                if (txtConfirmPassword.Text != txtPassword.Text)
                {
                    Master.MessageCenter.DisplayErrorMessage(SuppliersStrings.GetText(@"SupplierNewPasswordConfirmInvalid"));
                    return;
                }
                Membership.UserPasswordChangeResults results;
                results = Membership.ChangeSupplierPassword(supplier.Email, txtPassword.Text);
                switch (results)
                {
                    default:
                        Master.MessageCenter.DisplayWarningMessage(SuppliersStrings.GetText(@"MessagePasswordChangeFailedUnknown"));
                        break;
                    case Membership.UserPasswordChangeResults.PasswordDoNotMatch:
                        Master.MessageCenter.DisplayWarningMessage(SuppliersStrings.GetText(@"MessagePasswordChangeBadOldPassword"));
                        break;
                    case Membership.UserPasswordChangeResults.Success:
                        break;
                }
            }

            if (IsNewMode)
            {
                string successMessage = SuppliersStrings.GetText(@"MessageSupplierCreated");
                string url = @"EditSupplier.aspx?Email=" + SupplierEmail+"&SupplierId="+supplier.SupplierId;
                url += @"&message-success=" + Server.UrlEncode(successMessage);
                Response.Redirect(url, true);
            }
            else
            {
                string successMessage = SuppliersStrings.GetText(@"MessageSupplierSaved");
                if (EmailChanged)
                {
                    string url = @"EditSupplier.aspx?message-success=" + Server.UrlEncode(successMessage) + "&SupplierId=" + supplier.SupplierId;
                    if (SupplierId != supplier.SupplierId)
                    {
                        url += @"&Email=" + SupplierEmail;
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

        private void GetCities(Int64 SupplierId)
        {
            if (ddlCity.Items.Count == 0)
            {
                Query q = new Query(City.TableSchema).SelectAll().OrderBy(City.Columns.CityName, dg.Sql.SortDirection.ASC);
                CityCollection citiesList = CityCollection.FetchByQuery(q);

                Int64 selectedCityId = SupplierId == 0 ? citiesList[0].CityId : AppSupplier.FetchByID(SupplierId).CityId;

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

        protected void ddlIsProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlIsProduct.SelectedValue != "prod")
            {
                services.Visible = true;
                RequiredFieldValidator1.Enabled = false;
            }
        }
}
}
