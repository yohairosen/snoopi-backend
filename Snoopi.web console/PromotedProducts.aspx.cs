using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.web;
using Snoopi.web.Localization;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class PromotedProducts : AdminPageBase
{
    #region Members & Properties
    public enum promotedSection
    {
        cat_most_popular,
        cat_most_profitable,
        dog_most_popular,
        dog_most_profitable,
    }

    public int MAX_WEIGHT =5;

    bool HasEditPermission = false;
    string filterSearch = null;
    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }
    private List<ListItem> productsList
    {
        get
        {
            if (Session["promted_products_products_list"] != null)
                return Session["promted_products_products_list"] as List<ListItem>;
            return new List<ListItem>();
        }
        set
        {
            Session["promted_products_products_list"] = value;
        }
    }
    #endregion

    #region Events
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("ProductPromotedAreaHeader");
        Master.ActiveMenu = "PromotedProducts";
        Master.AddClientScriptInclude(@"dgDateManager.js");
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
        if (Request.QueryString["Filter"] != null)
        {
            filterSearch = Request.QueryString["Filter"];
        }
        dgPromtedProducts.PageIndexChanged += dgPromtedProducts_PageIndexChanging;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            productsList = getProductsList();
        int CurrentPageIndex = 0;
        if (!int.TryParse(hfCurrentPageIndex_dgPromtedProducts.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
        if (CurrentPageIndex < 0) CurrentPageIndex = 0;
        dgPromtedProducts.CurrentPageIndex = CurrentPageIndex;
        LoadItems();
        ProductController.GetPromotedProducts(null,6,1296);
    }
    protected void dgPromtedProducts_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
    {
        dgPromtedProducts.CurrentPageIndex = e.NewPageIndex;
        hfCurrentPageIndex_dgPromtedProducts.Value = dgPromtedProducts.CurrentPageIndex.ToString();
        LoadItems();
        ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), "CallMyFunction", "scrollToTop()", true);

    }

    protected void dgProducts_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (e.Item != null && (e.Item.ItemType == ListItemType.Item ||
    e.Item.ItemType == ListItemType.AlternatingItem))
        {
            DataGridItem item = e.Item;
            PromotedProductUI promotedProductUI = (PromotedProductUI)item.DataItem;
            if (promotedProductUI.AreaId == 0 && promotedProductUI.Id != 0)
            {
                promotedProductUI.Weight = promotedProductUI.Weight - MAX_WEIGHT;
                if (promotedProductUI.Weight <= 0)
                    promotedProductUI.Weight = 1;
                else if (promotedProductUI.Weight > MAX_WEIGHT * 2)
                    promotedProductUI.Weight = MAX_WEIGHT;
                (e.Item.FindControl("lblWeight") as HtmlGenericControl).InnerText = promotedProductUI.Weight.ToString();
            }
        }

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadItems();
    }


    protected void Edit_Click(object sender, EventArgs e)
    {
        string[] args = (sender as System.Web.UI.WebControls.Button).CommandArgument.Split(';');

        displayEditFields((sender as System.Web.UI.WebControls.Button).Parent, args[0],args[1]);
    }

    protected void Save_Click(object sender, EventArgs e)
    {
        Control item = (sender as System.Web.UI.WebControls.Button).Parent;
        string product = hfProductSelectedValue.Value;

        if (!string.IsNullOrEmpty(product) && product != "0")
        {

            string section = hfSectionSelectedValue.Value;
            int? weight = hfWeightSelectedValue != null && !string.IsNullOrEmpty(hfWeightSelectedValue.Value) ? (int?)int.Parse(hfWeightSelectedValue.Value) : null;
            if (weight != null)
            {
                int areaId = int.Parse((item.FindControl("spnAreaId") as HtmlGenericControl).InnerText);
                if (areaId == 0)
                    weight += MAX_WEIGHT;
                int id = int.Parse((item.FindControl("spnID") as HtmlGenericControl).InnerText);
                var productItem = Product.FetchByCode(product);
                PromotedProduct promotedProduct = new PromotedProduct()
                {
                    AreaId = areaId,
                    ProductId = productItem.ProductId,
                    Section = section.Trim(),
                    Weight = (int)weight,
                };
                if (id == 0)
                {
                    promotedProduct.IsNewRecord = true;
                    promotedProduct.Insert();
                }
                else
                {
                    promotedProduct.Id = id;
                    promotedProduct.IsNewRecord = false;
                    promotedProduct.Update();
                }

                LoadItems();

            }
        }
    }

    protected void Remove_Click(object sender, EventArgs e)
    {
        Control item = (sender as System.Web.UI.WebControls.Button).Parent;
        string product = (item.FindControl("spnPromotedProduct") as HtmlGenericControl).InnerText;
        long productId;
        if (!string.IsNullOrEmpty(product) && product != "0" && long.TryParse(product, out productId))
        {
            HtmlGenericControl lblWeight = item.FindControl("lblWeight") as HtmlGenericControl;

            string section = (item.FindControl("spnSection") as HtmlGenericControl).InnerText;
            int weight = lblWeight != null ? int.Parse(lblWeight.InnerText) : MAX_WEIGHT;
            int areaId = int.Parse((item.FindControl("spnAreaId") as HtmlGenericControl).InnerText);
            if (areaId == 0)
                weight += MAX_WEIGHT;
            int id = int.Parse((item.FindControl("spnID") as HtmlGenericControl).InnerText);

            PromotedProduct promotedProduct = new PromotedProduct()
            {
                Id = id,
                AreaId = areaId,
                Deleted = DateTime.Now,
                IsNewRecord = false,
                ProductId = productId,
                Section = section.Trim(),
                Weight = weight,

            };
            promotedProduct.Update();
            LoadItems();
        }
    }

    #endregion

    #region Methods
    private void displayEditFields(Control sender,string productSelectedValue="", string sectionSelectedValue="")
    {
        DropDownList ddlProducts = sender.FindControl("ddlPromtedProducts") as DropDownList;
        ddlProducts.DataSource = productsList;
        ddlProducts.DataTextField = "Text";
        ddlProducts.DataValueField = "Value";
        ddlProducts.SelectedValue =!string.IsNullOrEmpty(productSelectedValue)? productSelectedValue:"0";
        ddlProducts.Style["display"] = "";
        ddlProducts.DataBind();
        hfProductSelectedValue.Value = ddlProducts.SelectedValue;

        HtmlGenericControl spnPromotedProduct = sender.FindControl("spnPromotedProduct") as HtmlGenericControl;
        spnPromotedProduct.Style["display"] = "none";

        DropDownList ddlWeight = sender.FindControl("ddlWeight") as DropDownList;
        HtmlGenericControl lblWeight = sender.FindControl("lblWeight") as HtmlGenericControl;
        int weight = !string.IsNullOrEmpty(lblWeight.InnerText.Trim())? Convert.ToInt16(lblWeight.InnerText):MAX_WEIGHT;
        loadDdlWeight(ddlWeight,MAX_WEIGHT);
        ddlWeight.SelectedValue = weight <= 0 || weight > MAX_WEIGHT ? MAX_WEIGHT.ToString() : weight.ToString();

        hfWeightSelectedValue.Value = ddlWeight.SelectedValue;
        ddlWeight.Style["display"] = "";
        lblWeight.Style["display"] = "none";


        DropDownList ddlSection = sender.FindControl("ddlSection") as DropDownList;

        string[] itemValues = System.Enum.GetNames(typeof(promotedSection));

        for (int i = 0; i <= itemValues.Length - 1; i++)
        {
            
            string localizationValue = ResourceManagerAccessor.GetText("PromotedArea", itemValues[i]);
            ListItem item = new ListItem(localizationValue, itemValues[i]);
            ddlSection.Items.Add(item);
        }
        ddlSection.SelectedValue = !string.IsNullOrEmpty(sectionSelectedValue) ? sectionSelectedValue : ddlSection.Items[0].Value;
        ddlSection.Style["display"] = "";
        ddlSection.DataBind();
        hfSectionSelectedValue.Value = ddlSection.SelectedValue;
        HtmlGenericControl lblSection = sender.FindControl("lblSection") as HtmlGenericControl;
        lblSection.Style["display"] = "none";

        Button btnEdit = sender.FindControl("btnEdit") as Button;
        btnEdit.Style["display"] = "none";
        Button btnRemove = sender.FindControl("btnRemove") as Button;
        btnRemove.Style["display"] = "none";
    }

    private void loadDdlWeight(DropDownList ddl,int max)
    {
        for (int i = 1; i <= MAX_WEIGHT; i++)
            ddl.Items.Add(new ListItem() { Text = i.ToString(), Value = i.ToString() });
        ddl.DataBind();
    }

    protected void LoadItems()
    {
        if (!HasEditPermission)
        {
            dgPromtedProducts.Columns[dgPromtedProducts.Columns.Count - 1].Visible = false;
        }

        dgPromtedProducts.VirtualItemCount = ProductController.GetPromotedAreaProducts().Count;
        if (dgPromtedProducts.VirtualItemCount == 0)
        {
            phHasItems.Visible = false;
            phHasNoItems.Visible = true;
            lblNoItems.Text = Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("NoItems");
        }
        else
        {
            phHasItems.Visible = true;
            phHasNoItems.Visible = false;
            if (dgPromtedProducts.PageSize * dgPromtedProducts.CurrentPageIndex > dgPromtedProducts.VirtualItemCount)
            {
                dgPromtedProducts.CurrentPageIndex = 0;
                hfCurrentPageIndex_dgPromtedProducts.Value = dgPromtedProducts.CurrentPageIndex.ToString();
            }
            List<PromotedProductAreaUI> areas = ProductController.GetPromotedAreaProducts(0, dgPromtedProducts.PageSize, dgPromtedProducts.CurrentPageIndex);
            areas = addEmptyRowsToProducts(areas);
            BindList(areas);
        }
    }
    protected void BindList(List<PromotedProductAreaUI> coll)
    {
        dgPromtedProducts.DataSource = coll;
        dgPromtedProducts.DataBind();
        Master.DisableViewState(dgPromtedProducts);
    }


    private List<ListItem> getProductsList()
    {
        List<Product> Products = ProductCollection.FetchAll();
        List<ListItem> items = Products.Where(s => s.IsDeleted == false )
                        .Select(s => new ListItem { Value = s.ProductCode.ToString(), Text = s.ProductCode }).ToList();
        List<ListItem> ddlData = new List<ListItem>();
        ddlData.Add(new ListItem() { Value = "0", Text = Snoopi.web.Resources.PromotedArea.ResourceManager.GetString("NoSelection") });
        foreach (var item in items)
        {
            ddlData.Add(item);
        }
        return ddlData;
    }

    private List<PromotedProductAreaUI> addEmptyRowsToProducts(List<PromotedProductAreaUI> areas)
    {
        foreach (var area in areas)
        {

                    area.ProductPromoted.Add(new PromotedProductUI()
                    {
                        Id=0,
                        ProductId = 0,
                        Section = string.Empty,
                        ProductImage = string.Empty,
                        ProductName = string.Empty,
                        ProductCode = string.Empty,
                        AreaId = area.PromotedAreaId,
                        Weight = MAX_WEIGHT,                    
                    });
        }
        return areas;
    }

    #endregion
}


