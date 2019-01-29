using Snoopi.core.BL;
using Snoopi.web;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Advertisements : AdminPageBase
{
    bool HasEditPermission = true;
    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

    #region Events
    protected void Page_Init(object sender, EventArgs e)
    {
        HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
        dgAds.PageIndexChanged += dgCompanies_PageIndexChanging;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        int CurrentPageIndex = 0;
        if (!int.TryParse(hfCurrentPageIndex_dgCompanies.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
        if (CurrentPageIndex < 0) CurrentPageIndex = 0;
        dgAds.CurrentPageIndex = CurrentPageIndex;
        LoadItems();
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = AdvertisementsStrings.GetText(@"AllAds");
        Master.ActiveMenu = "Advertisements";
        Master.AddClientScriptInclude(@"dgDateManager.js");
    }
    protected void dgCompanies_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
    {
        dgAds.CurrentPageIndex = e.NewPageIndex;
        hfCurrentPageIndex_dgCompanies.Value = dgAds.CurrentPageIndex.ToString();
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

        if (AdvertisementController.DeleteAd(Convert.ToInt16(arguments.ToString())))
        {
            Master.MessageCenter.DisplaySuccessMessage(AdvertisementsStrings.GetText(@"DeleteMessage"));
            LoadItems();
        }
    }
    #endregion

    #region Methods
    protected void LoadItems()
    {
        if (!HasEditPermission)
        {
            dgAds.Columns[dgAds.Columns.Count - 1].Visible = false;
        }
        dgAds.VirtualItemCount = AdvertisementController.GetAllAds().Count;
        if (dgAds.VirtualItemCount == 0)
        {
            phHasItems.Visible = false;
            phHasNoItems.Visible = true;
            lblNoItems.Text = AdvertisementsStrings.GetText(@"MessageNoCompaniesFound");
        }
        else
        {
            phHasItems.Visible = true;
            phHasNoItems.Visible = false;
            if (dgAds.PageSize * dgAds.CurrentPageIndex > dgAds.VirtualItemCount)
            {
                dgAds.CurrentPageIndex = 0;
                hfCurrentPageIndex_dgCompanies.Value = dgAds.CurrentPageIndex.ToString();
            }
            List<AdvertisementUI> items = AdvertisementController.GetAllAds(dgAds.PageSize, dgAds.CurrentPageIndex);
            BindList(items);
        }
    }
    protected void BindList(List<AdvertisementUI> coll)
    {
        dgAds.ItemDataBound += dgCompanies_ItemDataBound;
        dgAds.DataSource = coll;
        dgAds.DataBind();
        lblTotal.Text = dgAds.VirtualItemCount.ToString();
        Master.DisableViewState(dgAds);
    }
    protected string FormatEditUrl(object item)
    {
        return string.Format("EditAd.aspx?adId={0}", HttpUtility.UrlEncode(((AdvertisementUI)item).Id.ToString()));
    }
    #endregion

}