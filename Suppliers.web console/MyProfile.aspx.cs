using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Snoopi.web
{
    public partial class MyProfile : SupplierPageBase
    {
        protected override string[] AllowedPermissions
        {
            get { return new string[] { }; }
        }

        private void LoadView()
        {
            SupplierUI supplier = SupplierController.GetSupplierUIForSupplierProfile(SuppliersSessionHelper.SupplierId());
            txtbusiness.Text = supplier.BusinessName;
            txtbusiness.ToolTip = supplier.BusinessName;
            txtContactName.Text = supplier.ContactName;
            txtContactName.ToolTip = supplier.ContactName;
            txtContactPhone.Text = supplier.ContactPhone;
            txtEmail.Text = supplier.Email;
            txtNumber.Text = supplier.HouseNum;
            txtStreet.Text = supplier.Street;
            txtStreet.ToolTip = supplier.Street;
            //City c = City.FetchByID(supplier.CityId);
            txtCity.Text = supplier.CityName;
            txtCity.ToolTip = supplier.CityName;
            txtPhone.Text = supplier.Phone;
            lvCity.DataSource = supplier.citiesSupplied;
            lvCity.DataBind();
            lvHomeCity.DataSource = supplier.citiesHomeService;
            lvHomeCity.DataBind();
            showHideServiceFields(supplier.IsService);
           if (supplier.IsService)
            { 
                txtDescription.Text = supplier.Description;
                txtDiscount.Text = supplier.Discount;
                string ImageFile = "";
                ImageFile = Snoopi.core.MediaUtility.GetImagePath("Supplier", supplier.ProfileImage, 0, 225, 225);
                if (ImageFile.ToLower().Contains(".jpg") || ImageFile.ToLower().Contains(".jpeg") || ImageFile.ToLower().Contains(".png"))
                    imgImage.ImageUrl = ImageFile;
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            
            LoadView();
        }

        private void showHideServiceFields( bool isService)
        {
            if (!isService)
            {
                dvDescription.Visible = false;
                dvDiscount.Visible = false;
                dvHomeServiceCities.Visible = false;
            }
            else
            {
                dvProductsSupplierCities.Visible = false;
            }

        }

    }
}
