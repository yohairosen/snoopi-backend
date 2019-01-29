using dg.Utilities;
using Snoopi.core;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Snoopi.web
{
    public partial class EditMyProfile : SupplierPageBase
    {
        protected override string[] AllowedPermissions
        {
            get { return new string[] { }; }
        }

        private void LoadView()
        {
            SupplierUI supplier = SupplierController.GetSupplierUIForSupplierProfile(SuppliersSessionHelper.SupplierId());
            txtbusiness.Text = supplier.BusinessName;
            txtContactName.Text = supplier.ContactName;
            txtContactPhone.Text = supplier.ContactPhone;
            txtEmail.Text = supplier.Email;
            txtEmail.Enabled = false;
            txtNumber.Text = supplier.HouseNum;
            txtStreet.Text = supplier.Street;

            ddlCity.DataSource = CityCollection.FetchAll();
            ddlCity.DataBind();
            if (supplier.CityId != 0) ddlCity.SelectedValue = supplier.CityId.ToString();
            txtPhone.Text = supplier.Phone;

            rptCity.DataSource = CityController.GetAllCityUIBy(SuppliersSessionHelper.SupplierId(), false);
            rptCity.DataBind();


            rptHomeCity.DataSource = CityController.GetAllCityUIBy(SuppliersSessionHelper.SupplierId(), true);
            rptHomeCity.DataBind();

            showHideServiceFields(supplier.IsService);
            if (supplier.IsService)
            {
                txtDescription.Text = supplier.Description;
                txtDiscount.Text = supplier.Discount;
                string ImageFile = "";
                ImageFile = Snoopi.core.MediaUtility.GetImagePath("Supplier", supplier.ProfileImage, 0, 225, 225);
                if (ImageFile.ToLower().Contains(".jpg") || ImageFile.ToLower().Contains(".jpeg") || ImageFile.ToLower().Contains(".png"))
                    ImageFileHandler(fuImage, imgImage, btnDeleteImage, ImageFile);
            }
            if (supplier.ApprovedTermsDate == null && supplier.IsService)
                IsApprovedTerms.Value = "false";
            else
                IsApprovedTerms.Value = "true";

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadView();
            }

        }

        protected void rptCity_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                AreaUI areaUI = ((AreaUI)e.Item.DataItem);
                if (areaUI.Cities != null && areaUI.Cities.Count > 0)
                {

                    ((CheckBox)e.Item.FindControl("cbArea")).Checked = areaUI.Cities.TrueForAll(r => r.IsSelected == true);
                    foreach (ListItem item in ((CheckBoxList)e.Item.FindControl("cblCities")).Items)
                    {
                        Int64 CityId = Convert.ToInt64(item.Value);
                        CityUI c = areaUI.Cities.Find(r => r.CityId == CityId);
                        item.Selected = c.IsSelected;
                    }
                }
            }
        }

        private void Save(AppSupplier supplier)
        {          
            supplier.BusinessName = txtbusiness.Text;
            supplier.ContactName = txtContactName.Text;
            supplier.Email = txtEmail.Text;
            supplier.ContactPhone = txtContactPhone.Text;
            supplier.HouseNum = txtNumber.Text;
            supplier.Street = txtStreet.Text;
            supplier.CityId = Convert.ToInt64(ddlCity.SelectedValue);
            supplier.Phone = txtPhone.Text;
            supplier.Description = txtDescription.Text;
            supplier.Discount = txtDiscount.Text;
            if (txtPassword.Text.Trim() != "" && txtConfirmPassword.Text.Trim() != "")
            {
                string pwd, salt;
                AppMembership.EncodePassword(txtPassword.Text.Trim(), out pwd, out salt);
                supplier.Password = pwd;
                supplier.PasswordSalt = salt;
            }
            if (supplier.IsService)
            {
                supplier.ApprovedTermsDate = DateTime.Now;
                if (fuImage.HasFile)
                {
                    string fn = MediaUtility.SaveFile(fuImage.PostedFile, "SupplupCityier/225x225", 0, true);
                    supplier.ProfileImage = fn;
                    imgImage.ImageUrl = Snoopi.core.MediaUtility.GetImagePath("Supplier", supplier.ProfileImage, 0, 225, 225);
                    ImageFileHandler(fuImage, imgImage, btnDeleteImage, imgImage.ImageUrl);
                }
                else if (supplier.ProfileImage != "" && fuImage.Visible)
                {
                    MediaUtility.DeleteImageFilePath("Supplier", supplier.ProfileImage, 225, 225, 0);
                    supplier.ProfileImage = "";
                }
            }
            supplier.Save();
            Response.Redirect("MyProfile.aspx");
            Master.MessageCenter.DisplaySuccessMessage(SupplierProfileStrings.GetText(@"Success"));
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            Int64 SupplierId = SuppliersSessionHelper.SupplierId();
            AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
            if (supplier.ApprovedTermsDate != null || !supplier.IsService)
                Save(supplier);
            else
            {
                Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText(@"ErrorApproveTerms"));
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyProfile.aspx");
        }

        protected void revEml_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //if (!txtEmail.Text.IsValidEmail())
            //{
            //    args.IsValid = false;
            //    Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText(@"EmailWrong"));
            //}
            //else
            //{
            //    Int64 SupplierId = SuppliersSessionHelper.SupplierId();
            //    AppSupplier supplier = null;
            //    supplier = AppSupplier.FetchByEmail(txtEmail.Text);
            //    if (supplier != null)
            //    {
            //        args.IsValid = false;
            //        Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText(@"EmailExists"));
            //    }
            //}
        }
        protected void SaveHomeCities_Click(object sender, EventArgs e)
        {
            Int64 SupplierId = SuppliersSessionHelper.SupplierId();
            SupplierController.DeleteAllSupplierHomeCity(SupplierId);
            foreach (RepeaterItem rItem in rptHomeCity.Items)
            {
                foreach (ListItem item in ((CheckBoxList)rItem.FindControl("cblCities")).Items)
                {
                    if (item.Selected == true)
                    {
                        SupplierHomeServiceCity sc = new SupplierHomeServiceCity();
                        sc.CityId = Convert.ToInt64(item.Value);
                        sc.SupplierId = SupplierId;
                        sc.ServiceId = Service.FetchHomeService();
                        sc.Save();
                    }
                }
            }
        }


        protected void btnTerms_Click(object sender, EventArgs e)
        {
            termsLoader.Visible = true;
            if (!Page.IsValid) return;
            if (ApproveTermsCb.Checked)
            {
                Int64 SupplierId = SuppliersSessionHelper.SupplierId();
                AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
                Save(supplier);
            }
            else
            {
                Response.Redirect("EditMyProfile.aspx");
                Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText(@"ErrorApproveTerms"));
            }
            termsLoader.Visible = false;

        }

        protected void btnCities_Click(object sender, EventArgs e)
        {
            Int64 SupplierId = SuppliersSessionHelper.SupplierId();
            SupplierController.DeleteAllSupplierCity(SupplierId);

            foreach (RepeaterItem rItem in rptCity.Items)
            {
                foreach (ListItem item in ((CheckBoxList)rItem.FindControl("cblCities")).Items)
                {
                    if (item.Selected == true)
                    {
                        SupplierCity sc = new SupplierCity();
                        sc.CityId = Convert.ToInt64(item.Value);
                        sc.SupplierId = SupplierId;
                        sc.Save();
                    }
                }
            }

        }

        private void ImageFileHandler(FileUpload fu, System.Web.UI.WebControls.Image img, Button btn, string value = "")
        {
            if (String.IsNullOrEmpty(value))
            {
                fuImage.Visible = true;
                imgImage.Visible = false;
                btnDeleteImage.Visible = false;
            }
            else
            {
                fuImage.Visible = false;
                imgImage.Visible = true;
                imgImage.ImageUrl = value;
                btnDeleteImage.Visible = true;
            }
        }

        protected void btnDeleteImage_Click(object sender, System.EventArgs e)
        {
            ImageFileHandler(fuImage, imgImage, btnDeleteImage);
        }

        private void showHideServiceFields(bool isService)
        {
            if (isService)
            {
                dvProductsSupplierCities.Visible = false;
            }
            else
            {   
                dvDescription.Visible = false;
                dvDiscount.Visible = false;
                dvHomeServiceCities.Visible = false;
            }  
        }
    }
}
