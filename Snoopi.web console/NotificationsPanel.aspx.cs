using Snoopi.core.BL;
using Snoopi.core.DAL.Entities;
using Snoopi.web;
using Snoopi.web.Localization;
using Snoopi.web.Localization.Strings.Accessors;
using Snoopi.web.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

public partial class NotificationPanel : AdminPageBase
{
    bool HasEditPermission = true;
    protected override string[] AllowedPermissions { get { return new string[] { Permissions.PermissionKeys.sys_perm }; } }

    #region Events
    protected void Page_Init(object sender, EventArgs e)
    {
        HasEditPermission = Permissions.PermissionsForUser(SessionHelper.UserId()).Contains(Permissions.PermissionKeys.sys_perm);
        dgNotifications.PageIndexChanged += dgNotifications_PageIndexChanging;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        int CurrentPageIndex = 0;
        if (!int.TryParse(hfCurrentPageIndex_dgNotifications.Value, out CurrentPageIndex)) CurrentPageIndex = 0;
        if (CurrentPageIndex < 0) CurrentPageIndex = 0;
        dgNotifications.CurrentPageIndex = CurrentPageIndex;
        LoadItems();
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Master.PageTitle = NotificationStrings.GetText(@"AllNotifications");
        Master.ActiveMenu = "Notifications";
        Master.AddClientScriptInclude(@"dgDateManager.js");
    }
    protected void dgNotifications_PageIndexChanging(object sender, DataGridPageChangedEventArgs e)
    {
        dgNotifications.CurrentPageIndex = e.NewPageIndex;
        hfCurrentPageIndex_dgNotifications.Value = dgNotifications.CurrentPageIndex.ToString();
        LoadItems();
    }
    protected void dgNotifications_ItemDataBound(object sender, DataGridItemEventArgs e)
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
    protected void hlSendPush_Click(object sender, EventArgs e)
    {
        try
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            var notificationToSend = NotificationFilter.FetchByID(int.Parse(arguments));
            var users = NotificationGroups.GetUsersOfFilter(notificationToSend);
            try
            {
                Task.Run(() => Snoopi.core.FcmService.SendTemplateToMany(notificationToSend.Name, notificationToSend.MessageTemplate, users)).Wait();
            }
            catch(Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(AppDomain.CurrentDomain.BaseDirectory + @"\Output\push-log.txt"))
                {
                    sw.WriteLine(@" ------------" + DateTime.Now + "--------------------" + '\n' + "Exception  " + ex.Message + " CallStack : " + ex.StackTrace);
                }
            }
            Master.MessageCenter.DisplaySuccessMessage(NotificationStrings.GetText(@"NotificationSent"));
        }
        catch (Exception ex)
        {
            Master.MessageCenter.DisplayErrorMessage(NotificationStrings.GetText(@"DeleteErrorMessage") + " \n" + ex.Message + '\n' + ex.StackTrace);
        }
    }

    protected void hlChechNumOfUsers(object sender, EventArgs e)
    {
        try
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            var notificationToSend = NotificationFilter.FetchByID(int.Parse(arguments));
            var users = NotificationGroups.GetUsersOfFilter(notificationToSend);
            Master.MessageCenter.DisplaySuccessMessage(NotificationStrings.GetText(@"NotificationSent"));
        }
        catch (Exception ex)
        {
            Master.MessageCenter.DisplayErrorMessage(NotificationStrings.GetText(@"DeleteErrorMessage") + " \n" + ex.Message + '\n' + ex.StackTrace);
        }
    }


    protected void hlDelete_Click(object sender, EventArgs e)
    {
        try
        {
            Button btn = (Button)(sender);
            var arguments = btn.CommandArgument;
            var notificationToDelete = NotificationFilter.FetchByID(int.Parse(arguments));
            notificationToDelete.Deleted = DateTime.Now;
            notificationToDelete.Save();
            Master.MessageCenter.DisplaySuccessMessage(NotificationStrings.GetText(@"DeleteMessage"));
            LoadItems();
        }
        catch (Exception ex)
        {
            Master.MessageCenter.DisplayErrorMessage(NotificationStrings.GetText(@"DeleteErrorMessage") + " \n" + ex.Message + '\n' + ex.StackTrace);
        }
    }
    #endregion

    #region Methods
    protected void LoadItems()
    {
        if (!HasEditPermission)
        {
            dgNotifications.Columns[dgNotifications.Columns.Count - 1].Visible = false;
        }
        var allNotifications = NotificationFilterCollection.FetchAll().Where(x => x.Deleted == null);
        dgNotifications.VirtualItemCount = allNotifications.Count();
        if (dgNotifications.VirtualItemCount == 0)
        {
            phHasItems.Visible = false;
            phHasNoItems.Visible = true;
            lblNoItems.Text = NotificationStrings.GetText(@"MessageNoNotificationsFound");
        }
        else
        {
            phHasItems.Visible = true;
            phHasNoItems.Visible = false;
            if (dgNotifications.PageSize * dgNotifications.CurrentPageIndex > dgNotifications.VirtualItemCount)
            {
                dgNotifications.CurrentPageIndex = 0;
                hfCurrentPageIndex_dgNotifications.Value = dgNotifications.CurrentPageIndex.ToString();
            }

            BindList(allNotifications.ToList());
        }
    }
    protected void BindList(List<NotificationFilter> coll)
    {
        dgNotifications.ItemDataBound += dgNotifications_ItemDataBound;
        dgNotifications.DataSource = coll;
        dgNotifications.DataBind();
        lblTotal.Text = dgNotifications.VirtualItemCount.ToString();
        Master.DisableViewState(dgNotifications);
    }
    protected string FormatEditUrl(object item)
    {
        return string.Format("NotificationEdit.aspx?filterId={0}", HttpUtility.UrlEncode(((NotificationFilter)item).Id.ToString()));
    }
    #endregion

}