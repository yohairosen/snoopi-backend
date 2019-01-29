using dg.Utilities;
using Snoopi.core.BL;
using Snoopi.core.DAL;
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

public partial class EditPromotedSuppliers : AdminPageBase
{
    int Id;
    bool IsNewMode = false;
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
            if (int.TryParse(Request.QueryString[@"Id"], out Id))
            {
                SupplierPromotedArea supplierPromotedArea = SupplierPromotedArea.FetchByID(Id);
                if (supplierPromotedArea == null)
                {
                    Master.LimitAccessToPage();
                }
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            hfOriginalId.Value = Id.ToString();
            BindServicesList();
            ddlAreas.DataSource = SupplierPromotedController.GetPromotedArea();
            ddlAreas.DataBind();
            BindSuppliersList();
            LoadView();
        }
        else
        {
            if (hfOriginalId.Value.Length > 0 && hfOriginalId.Value != Id.ToString())
            {
                Http.Respond404(true);
            }
        }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = ResourceManagerAccessor.GetText("PromotedArea", IsNewMode ? @"NewPromotedSupplier" : @"EditPromotedSupplier"); 
        Master.ActiveMenu = IsNewMode ? "EditPromotedSuppliers" : "PromotedSuppliers";
        Master.AddClientScriptInclude(@"dgDatePicker.js");
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {

        string datepickerFromDate = txtFromDate.Text;
        DateTime dtFromDate = DateTime.Now;
        DateTime.TryParse(datepickerFromDate, out dtFromDate);

        string datepickerToDate = txtToDate.Text;
        DateTime dtToDate = DateTime.Now;
        DateTime.TryParse(datepickerToDate, out dtToDate);
        int areaId = int.Parse(ddlAreas.SelectedValue);
        int serviceId = int.Parse(ddlServices.SelectedValue);
        if (dtFromDate > dtToDate || string.IsNullOrEmpty(ddlPromtedSuppliers.SelectedValue) || ddlPromtedSuppliers.SelectedValue=="0")
        {
            return;
        }
        int supplierId = int.Parse(ddlPromtedSuppliers.SelectedValue);

        int itemId = Id;
        SupplierPromotedArea spa = new SupplierPromotedArea()
        {
            IsNewRecord = true,
            PromotedAreaId = areaId,
            SupplierId = supplierId,
            StartTime = dtFromDate,
            EndTime = dtToDate,
            Id = itemId,
            ServiceId=serviceId,
        };
        if (itemId == 0)// insert
        {
            spa.Insert();
        }
        else //update
        {
            spa.Update();
        }
        

        if (IsNewMode)
        {
            string successMessage = ResourceManagerAccessor.GetText("PromotedArea", @"MessageSupplierCreated");
            string url = @"EditPromotedSuppliers.aspx?Id=" + spa.Id;
            url += @"&message-success=" + Server.UrlEncode(successMessage);
            Response.Redirect(url, true);
        }
        else
        {
            string successMessage = SuppliersStrings.GetText(@"MessageSupplierSaved");
            Master.MessageCenter.DisplaySuccessMessage(successMessage);
            LoadView();

        }
    }
    protected void LoadView()
    {
        if (Id > 0)
        {
            SupplierPromotedArea supplierPromotedArea = SupplierPromotedArea.FetchByID(Id);
            if (supplierPromotedArea != null)
            {
                //txtBusinessName.Text = adCompany.BusinessName;
                ddlAreas.SelectedValue = supplierPromotedArea.PromotedAreaId.ToString();
                ddlServices.SelectedValue = supplierPromotedArea.ServiceId.ToString();
                ddlPromtedSuppliers.SelectedValue = supplierPromotedArea.SupplierId.ToString();
                txtFromDate.Text = supplierPromotedArea.StartTime.ToShortDateString();
                txtToDate.Text = supplierPromotedArea.EndTime.ToShortDateString();
            }
        }
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
    private void BindSuppliersList()
    {
        if(suppliersList==null || suppliersList.Count<=0)
        {
            suppliersList = getSuppliersList();
        }
        ddlPromtedSuppliers.DataSource = suppliersList;
        ddlPromtedSuppliers.DataTextField = "Text";
        ddlPromtedSuppliers.DataValueField = "Value";
        //ddlPromtedSuppliers.SelectedValue = args[0];
        ddlPromtedSuppliers.Style["display"] = "";
        ddlPromtedSuppliers.DataBind();
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
}