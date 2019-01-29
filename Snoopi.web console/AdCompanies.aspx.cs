using Snoopi.core.BL;
using Snoopi.core.DAL.Entities;
using Snoopi.web.Localization;
using Snoopi.web.Localization.Strings.Accessors;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdCompanies : AdminPageBase
{
    bool HasEditPermission = true;
    
    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

    #region Events
    protected void Page_Init(object sender, EventArgs e)
    {
        //HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_edit_users);

        Master.AddButtonNew(AdvertisementsStrings.GetText(@"NewSupplierButton"), @"EditSupplier.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });

        dgCompanies.PageIndexChanged += dgCompanies_PageIndexChanging;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        int CurrentPageIndex = 0;
        if (!int.TryParse(hfCurrentPageIndex_dgCompanies.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
        if (CurrentPageIndex < 0) CurrentPageIndex = 0;
        dgCompanies.CurrentPageIndex = CurrentPageIndex;
        LoadItems();
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = AdvertisementsStrings.GetText(@"AllCompanies");
        Master.ActiveMenu = "AdCompanies";
        Master.AddClientScriptInclude(@"dgDateManager.js");
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadItems();
    }
    protected void BindList(List<AdCompany> coll)
    {
        dgCompanies.ItemDataBound += dgCompanies_ItemDataBound;
        dgCompanies.DataSource = coll;
        dgCompanies.DataBind();
        lblTotal.Text = dgCompanies.VirtualItemCount.ToString();
        Master.DisableViewState(dgCompanies); 
    }
    protected void dgCompanies_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
    {
        dgCompanies.CurrentPageIndex = e.NewPageIndex;
        hfCurrentPageIndex_dgCompanies.Value = dgCompanies.CurrentPageIndex.ToString();
        LoadItems();
    }
    protected void dgCompanies_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item ||
            e.Item.ItemType == ListItemType.AlternatingItem ||
            e.Item.ItemType == ListItemType.SelectedItem)
        {
            HyperLink hlEdit = e.Item.FindControl("hlEdit") as HyperLink;
            hlEdit.Visible = HasEditPermission;

            Button hlDelete = e.Item.FindControl("hlDelete") as Button;
            hlDelete.Visible = HasEditPermission;

        }
    }
    protected void hlDelete_Click(object sender, EventArgs e)
    {
        Button btn = (Button)(sender);
        var arguments = btn.CommandArgument;

        AdvertisementController.DeleteCompany(Convert.ToInt16(arguments.ToString()));
            Master.MessageCenter.DisplaySuccessMessage(ResourceManagerAccessor.GetText("Companies", "DeleteMessage"));
        LoadItems();
    }
    #endregion

    #region Methods
    protected void LoadItems()
    {
        if (!HasEditPermission)
        {
            dgCompanies.Columns[dgCompanies.Columns.Count - 1].Visible = false;
        }
        string searchName = "%" + txtSearchName.Text.Trim() + "%";
        string searchPhone = "%" + txtSearchPhone.Text.Trim() + "%";
        dgCompanies.VirtualItemCount = AdvertisementController.GetAllCompaniesUI(true, searchName, searchPhone).Count;
        if (dgCompanies.VirtualItemCount == 0)
        {
            phHasItems.Visible = false;
            phHasNoItems.Visible = true;
            lblNoItems.Text = AdvertisementsStrings.GetText(@"MessageNoCompaniesFound");
        }
        else
        {
            phHasItems.Visible = true;
            phHasNoItems.Visible = false;
            if (dgCompanies.PageSize * dgCompanies.CurrentPageIndex > dgCompanies.VirtualItemCount)
            {
                dgCompanies.CurrentPageIndex = 0;
                hfCurrentPageIndex_dgCompanies.Value = dgCompanies.CurrentPageIndex.ToString();
            }
            List<AdCompany> items = AdvertisementController.GetAllCompaniesUI(true, searchName, searchPhone, dgCompanies.PageSize, dgCompanies.CurrentPageIndex);
            BindList(items);
        }
    }
    protected string FormatEditUrl(object item)
    {
        return string.Format("EditAdCompany.aspx?CompanyId={0}", HttpUtility.UrlEncode(((AdCompany)item).CompanyId.ToString()));
    }
    protected string FormatDeleteUrl(object item)
    {///
        return string.Format("DeleteSupplier.aspx?CompanyId={0}", HttpUtility.UrlEncode(((AdCompany)item).CompanyId.ToString()));
    }
    #endregion

}
