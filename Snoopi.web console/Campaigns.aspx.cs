using System;
using System.Collections;
using System.Configuration;
using dg.Utilities;
using dg.Utilities.Spreadsheet;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;
using dg.Sql;
using System.Web;
using System.Collections.Generic;

namespace Snoopi.web
{
    public partial class Campaigns : AdminPageBase
    {
        bool HasEditPermission = false;

        protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);

            Master.AddButtonNew(CampaignStrings.GetText(@"NewCampaignButton"), @"EditCampaign.aspx?New=yes", new string[] { Permissions.PermissionKeys.sys_perm });
            dgCampaign.PageIndexChanged += dgCampaign_PageIndexChanged;
        }

        
           

       
        protected void Page_Load(object sender, EventArgs e)
        {
            int CurrentPageIndex = 0;
            if (!int.TryParse(hfCurrentPageIndex_dgCampaign.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
            if (CurrentPageIndex < 0) CurrentPageIndex = 0;
            dgCampaign.CurrentPageIndex = CurrentPageIndex;
            LoadItems();
        }



        protected void Page_PreRender(object sender, EventArgs e)
        {
            Master.PageTitle = CampaignStrings.GetText(@"CampaignTitlePage");
            Master.ActiveMenu = "Campiagn";
            Master.AddClientScriptInclude(@"dgDateManager.js");
        }

        protected void LoadItems(string sortField = "CampaignId", dg.Sql.SortDirection sortDirection = dg.Sql.SortDirection.ASC)
        {
                CampaignCollection col = CampaignController.GetAllCampaign(dgCampaign.PageSize, dgCampaign.CurrentPageIndex);
                BindList(col);
            

        }
        protected void BindList(CampaignCollection coll)
        {
            dgCampaign.ItemDataBound += dgCampaign_ItemDataBound;
            dgCampaign.DataSource = coll;
            dgCampaign.DataBind();
            Master.DisableViewState(dgCampaign);
        }


        void dgCampaign_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgCampaign.CurrentPageIndex = e.NewPageIndex;
            hfCurrentPageIndex_dgCampaign.Value = dgCampaign.CurrentPageIndex.ToString();
            LoadItems();
        }


        protected void dgCampaign_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem ||
                e.Item.ItemType == ListItemType.SelectedItem)
            {
                LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
                lbDelete.Visible = HasEditPermission;
                if (lbDelete.Visible)
                {
                    lbDelete.Attributes.Add("onclick", @"return confirm('" + EmailTemplatesStrings.GetText("MessageConfirmDeleteTemplate").ToJavaScript('\'', false) + @"');return false;");
                }


                LinkButton lbEdit = e.Item.FindControl("lbEdit") as LinkButton;
                lbEdit.Visible = HasEditPermission;
            }
        }


        protected void dgCampaign_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Int64 CampaignId;
            if (e.CommandName.Equals("Delete"))
            {
                CampaignId = Int64.Parse(dgCampaign.DataKeys[index].ToString());
                Campaign c = Campaign.FetchByID(CampaignId);
                if (c.StartDate > DateTime.UtcNow)
                {
                    Query q = new Query(Campaign.TableSchema).Where(Campaign.Columns.CampaignId, CampaignId).Delete();
                        q.Execute();
                        Master.MessageCenter.DisplaySuccessMessage(CampaignStrings.GetText("CampaignDeletedSuccess"));

                }
                else
                {
                    Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText("NoDeleteActiveCampaign"));
                }
               // Response.Redirect("DeleteCampaign.aspx?CampaignId=" + CampaignId);
                LoadItems();
            }
            else if (e.CommandName.Equals("Edit"))
            {
                CampaignId = Int64.Parse(dgCampaign.DataKeys[index].ToString());
                Campaign c = Campaign.FetchByID(CampaignId);
                if (c != null && c.EndDate > DateTime.Now)
                {
                    Response.Redirect("EditCampaign.aspx?CampaignId=" + CampaignId);
                }
                else {
                    Master.MessageCenter.DisplayErrorMessage(CampaignStrings.GetText("NotActiveCampaign"));
                }
                
                LoadItems();
            }


        }


    }

}
