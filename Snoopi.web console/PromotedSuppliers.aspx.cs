using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class PromotedSuppliers : AdminPageBase
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("SuplierPromotedAreaHeader");
        Master.ActiveMenu = "PromotedSuppliers";
        Master.AddClientScriptInclude(@"dgDateManager.js");
    }



    bool HasEditPermission = false;
    string filterSearch = null;
    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }


    protected void Page_Init(object sender, EventArgs e)
    {
        HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
        if (Request.QueryString["Filter"] != null)
        {
            filterSearch = Request.QueryString["Filter"];
        }
        dgPromtedSuppliers.PageIndexChanged += dgPromtedSuppliers_PageIndexChanging;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            suppliersList = getSuppliersList();
            BindServicesList();
            ddlAreas.DataSource = SupplierPromotedController.GetPromotedArea();
            ddlAreas.DataBind();
        }
        int CurrentPageIndex = 0;
        if (!int.TryParse(hfCurrentPageIndex_dgPromtedSuppliers.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
        if (CurrentPageIndex < 0) CurrentPageIndex = 0;
        dgPromtedSuppliers.CurrentPageIndex = CurrentPageIndex;
        LoadItems();
    }
    protected void dgPromtedSuppliers_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
    {
        dgPromtedSuppliers.CurrentPageIndex = e.NewPageIndex;
        hfCurrentPageIndex_dgPromtedSuppliers.Value = dgPromtedSuppliers.CurrentPageIndex.ToString();
        LoadItems();
        ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), "CallMyFunction", "scrollToTop()", true);

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadItems();
    }

    protected void hlDelete_Click(object sender, EventArgs e)
    {
        Button btn = (Button)(sender);
        var arguments = btn.CommandArgument;

        if (SupplierPromotedController.Delete(Convert.ToInt16(arguments.ToString())))
        {
            Master.MessageCenter.DisplaySuccessMessage(Snoopi.web.Resources.PromotedArea.ResourceManager.GetString(@"DeleteMessage"));
            LoadItems();
        }

    }
    protected void LoadItems()
    {


        if (!HasEditPermission)
        {
            dgPromtedSuppliers.Columns[dgPromtedSuppliers.Columns.Count - 1].Visible = false;
        }
        int areaId =!string.IsNullOrEmpty(ddlAreas.SelectedValue) ? int.Parse(ddlAreas.SelectedValue) : 0;
        string[] servicesIds = getSelectedServiced();
        dgPromtedSuppliers.VirtualItemCount = SupplierPromotedController.GetPromotedAreaSuppliers(servicesIds,areaId ).Count;
        if (dgPromtedSuppliers.VirtualItemCount == 0)
        {
            phHasItems.Visible = false;
            phHasNoItems.Visible = true;
            lblNoItems.Text = Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("NoItems");
        }
        else
        {
            phHasItems.Visible = true;
            phHasNoItems.Visible = false;
            if (dgPromtedSuppliers.PageSize * dgPromtedSuppliers.CurrentPageIndex > dgPromtedSuppliers.VirtualItemCount)
            {
                dgPromtedSuppliers.CurrentPageIndex = 0;
                hfCurrentPageIndex_dgPromtedSuppliers.Value = dgPromtedSuppliers.CurrentPageIndex.ToString();
            }
            List<SupplierPromotedUI> areas = SupplierPromotedController.GetPromotedAreaSuppliers(servicesIds,areaId, dgPromtedSuppliers.PageSize, dgPromtedSuppliers.CurrentPageIndex);
            BindList(areas);
        }



    }

    private string[] getSelectedServiced()
    {
        string[] selected = new string[ddlServices.Items.Count];
        int index = 0;
        foreach (ListItem item in ddlServices.Items)
        {
            if (item.Selected)
            {
                selected[index++] = item.Value;
            }
        }
        return selected;
    }

    private void BindServicesList()
    {
        ServiceCollection services = ServiceCollection.FetchAll();
        ddlServices.Items.Clear();
        foreach (var item in services)
        {
            ddlServices.Items.Add(new ListItem(item.ServiceName, item.ServiceId.ToString()));
        }
        ddlServices.DataBind();
   }

    protected void BindList(List<SupplierPromotedUI> coll)
    {
        if (coll != null && coll.Count > 0)
        {
            dgPromtedSuppliers.DataSource = coll;
            dgPromtedSuppliers.DataBind();
            Master.DisableViewState(dgPromtedSuppliers);
        }
    }
    private List<ListItem> suppliersList
    {
        get
        {
            if (Session["promted_suppliers_suppliers_list"] != null)
                return Session["promted_suppliers_suppliers_list"] as List<ListItem>;
            return new List<ListItem>();
        }
        set
        {
            Session["promted_suppliers_suppliers_list"] = value;
        }
    }

    private List<ListItem> getSuppliersList()
    {
        List<AppSupplier> suppliers = AppSupplierCollection.FetchAll();
        List<ListItem> items = suppliers.Where(s => s.IsDeleted == false && s.IsLocked == false && s.IsService == true)
                        .Select(s => new ListItem { Value = s.SupplierId.ToString(), Text = s.BusinessName }).ToList();
        List<ListItem> ddlData = new List<ListItem>();
        ddlData.Add(new ListItem() { Value = "0", Text = Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("NoSelection") });
        foreach (var item in items)
        {
            ddlData.Add(item);
        }
        return ddlData;
    }
    protected string FormatEditUrl(object item)
    {
        return string.Format("EditPromotedSuppliers.aspx?Id={0}", HttpUtility.UrlEncode(((SupplierPromotedAreaUI)item).Id.ToString()));
    }


 
}
