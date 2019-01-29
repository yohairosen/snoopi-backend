using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Snoopi.core.DAL;
using Snoopi.core.BL;
using System.Web;
using System.Reflection;
using System.Threading;

namespace Snoopi.web.WebControls
{
    public partial class AdminMasterPage : System.Web.UI.MasterPage
    {
        #region Page Controls
        protected HtmlGenericControl Body;
        protected HtmlGenericControl dvPageTitle;
        protected Literal ltScripts;
        protected List<string> lstRegisteredScripts;
        protected Literal ltTopMenu;
        protected Literal ltSideMenu;
        protected HtmlGenericControl dvSearch;
        protected Button btnSearch;
        protected TextBox txtSearchKeywords;
        protected MessageCenter mcMessageCenter;
        protected Panel pSideControls;
        protected Label lbVersion;
        #endregion

        #region Variable Members
        private StringBuilder sbHeaderScripts = null;
        private EventHandler _SearchActionEventHandler = null;
        #endregion

        #region Properties

        public string PageTitle
        {
            get { return dvPageTitle.InnerText; }
            set { dvPageTitle.InnerText = value; }
        }
        public string PageTitleHtml
        {
            get { return dvPageTitle.InnerHtml; }
            set { dvPageTitle.InnerHtml = value; }
        }
        public string SearchKeywords
        {
            get { return txtSearchKeywords.Text.Trim(); }
            set { txtSearchKeywords.Text = value.Trim(); }
        }
        public bool HasSearch
        {
            get { return dvSearch.Visible; }
            set { dvSearch.Visible = value; }
        }
        public MessageCenter MessageCenter
        {
            get { return mcMessageCenter; }
        }

        public string ActiveMenu { get; set; }

        #endregion

        #region Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CheckAuthenticated();
            InitCulture();

            AddClientScriptInclude(@"dgTools.js");
            AddClientScriptInclude(@"dgAutoCssSwitcher.js");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string[] messages = Request.QueryString.GetValues(@"message-error");
            if (messages != null)
            {
                foreach (string message in messages)
                {
                    if (message.Length > 0)
                    {
                        MessageCenter.DisplayErrorMessage(message);
                    }
                }
                RemoveFormActionQueryStringByKey(@"message-error");
            }
            messages = Request.QueryString.GetValues(@"message-warning");
            if (messages != null)
            {
                foreach (string message in messages)
                {
                    if (message.Length > 0)
                    {
                        MessageCenter.DisplayWarningMessage(message);
                    }
                }
                RemoveFormActionQueryStringByKey(@"message-warning");
            }
            messages = Request.QueryString.GetValues(@"message-info");
            if (messages != null)
            {
                foreach (string message in messages)
                {
                    if (message.Length > 0)
                    {
                        MessageCenter.DisplayInformationMessage(message);
                    }
                }
                RemoveFormActionQueryStringByKey(@"message-info");
            }
            messages = Request.QueryString.GetValues(@"message-success");
            if (messages != null)
            {
                foreach (string message in messages)
                {
                    if (message.Length > 0)
                    {
                        MessageCenter.DisplaySuccessMessage(message);
                    }
                }
                RemoveFormActionQueryStringByKey(@"message-success");
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Page.Title = Resources.AdminMasterPage.GeneralPageTitle;

            Body.Attributes[@"class"] = Resources.Global.direction;

            ltTopMenu.Text = BuildTopMenuConfigItems();
            ltSideMenu.Text = BuildSideMenuConfigItems();

            btnSearch.Text = Resources.AdminMasterPage.SearchSubmit;

            //App Version label
            Assembly web = Assembly.GetExecutingAssembly();
            AssemblyName webName = web.GetName();
            lbVersion.Text = "Version: " + webName.Version.ToString();

            if (sbHeaderScripts != null) ltScripts.Text = sbHeaderScripts.ToString();
        }

        public event EventHandler SearchActionEventHandler
        {
            add
            {
                if (_SearchActionEventHandler == null || !_SearchActionEventHandler.GetInvocationList().Contains(value))
                {
                    _SearchActionEventHandler += value;
                }
            }
            remove
            {
                _SearchActionEventHandler -= value;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            _SearchActionEventHandler.Invoke(sender, e);
        }

        #endregion

        public void AddButton(string buttonClass, string buttonText, EventHandler handler)
        {
            HtmlGenericControl span = new HtmlGenericControl(@"span");
            span.InnerText = buttonText;
            LinkButton lb = new LinkButton();
            lb.CssClass = buttonClass;
            lb.Click += handler;
            lb.Controls.Add(span);
            pSideControls.Controls.Add(lb);
        }
        public void AddButton(string buttonClass, string buttonText, string href)
        {
            HtmlGenericControl span = new HtmlGenericControl(@"span");
            span.InnerText = buttonText;
            HyperLink lb = new HyperLink();
            lb.CssClass = buttonClass;
            lb.NavigateUrl = href;
            lb.Controls.Add(span);
            pSideControls.Controls.Add(lb);
        }
        public void AddButtonNew(string buttonText, EventHandler handler)
        {
            AddButton(@"btn-create", buttonText, handler);
        }
        public void AddButtonNew(string buttonText, string href)
        {
            AddButton(@"btn-create", buttonText, href);
        }
        public void AddButtonAction(string buttonText, EventHandler handler)
        {
            AddButton(@"btn-action", buttonText, handler);
        }
        public void AddButtonAction(string buttonText, string href)
        {
            AddButton(@"btn-action", buttonText, href);
        }

        public void AddButton(string buttonClass, string buttonText, EventHandler handler, string[] allowedPermissions)
        {
            if (allowedPermissions.Length == 0 || Permissions.UserHasAnyPermissionIn(SessionHelper.UserId(), allowedPermissions))
            {
                AddButton(buttonClass, buttonText, handler);
            }
        }
        public void AddButton(string buttonClass, string buttonText, string href, string[] allowedPermissions)
        {
            if (allowedPermissions.Length == 0 || Permissions.UserHasAnyPermissionIn(SessionHelper.UserId(), allowedPermissions))
            {
                AddButton(buttonClass, buttonText, href);
            }
        }

        public void AddButtonNew(string buttonText, EventHandler handler, string[] allowedPermissions)
        {
            AddButton(@"btn-create", buttonText, handler, allowedPermissions);
        }
        public void AddButtonNew(string buttonText, string href, string[] allowedPermissions)
        {
            AddButton(@"btn-create", buttonText, href, allowedPermissions);
        }
        public void AddButtonAction(string buttonText, EventHandler handler, string[] allowedPermissions)
        {
            AddButton(@"btn-action", buttonText, handler, allowedPermissions);
        }
        public void AddButtonAction(string buttonText, string href, string[] allowedPermissions)
        {
            AddButton(@"btn-action", buttonText, href, allowedPermissions);
        }

        private bool _IsAuthenticated = false;
        public void CheckAuthenticated()
        {
            if (!_IsAuthenticated)
            {
                _IsAuthenticated = SessionHelper.IsAuthenticated();
                if (!_IsAuthenticated)
                {
                    Response.Redirect(@"/Login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery), true);
                }
            }
        }
        public void InitCulture()
        {
            string langCode = SessionHelper.LangCode();
            try
            {
                Page.UICulture = Page.Culture = langCode;
            }
            catch
            {
                Page.UICulture = Page.Culture = @"he";
            }
            //Page.UICulture = Page.Culture = "en"; // Temporary override
        }
        protected void RemoveFormActionQueryStringByKey(string key)
        {
            string action = Page.Form.Action;
            if (action.Length == 0) action = Request.RawUrl;
            int idx = action.IndexOf("?" + key + @"=");
            if (idx < 0) idx = action.IndexOf("&" + key + @"=");
            while (idx > -1)
            {
                int idx2 = action.IndexOf("&", idx + 1);
                if (idx2 > idx)
                {
                    action = action.Remove(idx + 1, idx2 - idx);
                }
                else
                {
                    action = action.Remove(idx);
                }
                Page.Form.Action = action;

                idx = action.IndexOf("?" + key + @"=");
                if (idx < 0) idx = action.IndexOf("&" + key + @"=");
            }
        }

        public void SetDirectionByCurrentLang(params object[] controls)
        {
            WebControl c;
            foreach (object obj in controls)
            {
                c = obj as WebControl;
                if (null != c)
                {
                    if (c.GetType().FullName == "CKEditor.NET.CKEditorControl")
                    {
                        PropertyInfo prop = c.GetType().GetProperty("Language", BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            prop.SetValue(c, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, null);
                        }

                        prop = c.GetType().GetProperty("ContentsLangDirection", BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            Type contentsLangDirectionsType = Type.GetType(@"CKEditor.NET.contentsLangDirections", false);
                            if (contentsLangDirectionsType != null)
                            {
                                object value = Enum.Parse(contentsLangDirectionsType, Resources.Global.direction == @"rtl" ? "Rtl" : @"Ltr");
                                if (value != null) prop.SetValue(c, value, null);
                            }
                        }
                    }
                    else
                    {
                        c.Style[@"direction"] = Resources.Global.direction;
                    }
                }
            }
        }

        public void LimitToPermissions(params string[] permissions)
        {
            if (permissions.Length == 0 || Permissions.UserHasAnyPermissionIn(SessionHelper.UserId(), permissions)) return;
            Http.Respond404(true);
        }
        public void LimitAccessToPage()
        {
            Http.Respond404(true);
        }

        public void AddClientScript(string javascript, bool bGenerateScriptTags)
        {
            if (sbHeaderScripts == null) sbHeaderScripts = new StringBuilder();
            if (bGenerateScriptTags)
            {
                sbHeaderScripts.Append("<script type=\"text/javascript\">/* <![CDATA[*/" + javascript + "\n// ]]></script>");
            }
            else sbHeaderScripts.Append(javascript);
        }
        public void AddClientScriptInclude(string filePath)
        {
            if (sbHeaderScripts == null) sbHeaderScripts = new StringBuilder();
            if (lstRegisteredScripts == null) lstRegisteredScripts = new List<string>();
            if (lstRegisteredScripts.Contains(filePath)) return;
            lstRegisteredScripts.Add(filePath);

            int protoidx = filePath.IndexOf(@"://");
            if (!filePath.StartsWith(@"/") && (protoidx == -1 || (protoidx < filePath.IndexOf(@"?") || filePath.IndexOf(@"?") > -1)))
            {
                filePath = @"/resources/js/" + filePath;
                if (!filePath.Contains(@"?culture=") && !filePath.Contains(@"&culture="))
                {
                    filePath += (filePath.Contains(@"?") ? @"&" : @"?") + @"culture=" + Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                }
                if (lstRegisteredScripts.Contains(filePath)) return;
                lstRegisteredScripts.Add(filePath);
            }
            sbHeaderScripts.Append(@"<script src=""" + filePath + @""" language=""javascript"" type=""text/javascript""></script>");
        }

        public void DisableViewState(DataGrid DataGrid)
        {
            foreach (DataGridItem DataGridItem in DataGrid.Items)
            {
                DataGridItem.EnableViewState = false;
            }
        }

        delegate string RenderSideMenuDelegate(MenuConfig.MenuConfigItem[] items, int level, out bool isSelected);
        public string BuildMenuConfigItems(MenuConfig MenuConfig)
        {
            Int64 UserId = SessionHelper.UserId();
            string[] permissions = Permissions.PermissionsForUser(UserId);
                

            RenderSideMenuDelegate mainMenuDelegate = null;
            mainMenuDelegate = delegate(MenuConfig.MenuConfigItem[] subItems, int level, out bool isSelected)
            {
                bool opened = false, bIsSelected, bHide, firstSelection = true;
                isSelected = false;
                StringBuilder sb = null;
                string name, href, subMenu;
                foreach (MenuConfig.MenuConfigItem item in subItems)
                {
                    bHide = false;

                    if (bHide == false && item.Allow != null && item.Allow.Length > 0)
                    {
                        bHide = true;
                        foreach (string perm in item.Allow)
                        {
                            if (permissions.Contains(perm))
                            {
                                bHide = false;
                                break;
                            }
                        }
                    } 
                    //if (!bHide)
                    //{
                    //    if (item.SpecialCondition != null)
                    //    {
                    //        if (item.SpecialCondition.Contains(@"special...") && conditions...) bHide = true;
                    //    }
                    //}
                    if (!bHide)
                    {
                        if (sb == null) sb = new StringBuilder();
                        if (!opened && level > 0)
                        {
                            sb.Append(@"<ul>");
                            opened = true;
                        }
                        name = Resources.Menu.ResourceManager.GetString(item.Name);
                        href = Page.ResolveUrl(item.Link);

                        string onclick = "";
                        if (!string.IsNullOrEmpty(item.OnClick))
                        {
                            onclick = @" onclick=""" + item.OnClick.ToHtml() + @"""";
                        }

                        subMenu = mainMenuDelegate(item.Items, level + 1, out bIsSelected);
                        if (bIsSelected) isSelected = true;
                        if (!bIsSelected && item.Name == ActiveMenu)
                        {
                            bIsSelected = isSelected = true;
                        }
                        if (bIsSelected && firstSelection)
                        {
                            sb.AppendFormat(@"<li class=""active""><a href=""{0}""{2}><span>{1}</span></a>", href.ToHtml(), name.ToHtml(), onclick);
                            firstSelection = false;
                        }
                        else
                        {
                            sb.AppendFormat(@"<li><a href=""{0}""{2}><span>{1}</span></a>", href.ToHtml(), name.ToHtml(), onclick);
                        }
                        if (subMenu != null) sb.Append(subMenu);
                        sb.Append(@"</li>");
                    }
                }
                if (opened) sb.Append(@"</ul>");
                if (sb != null) return sb.ToString();
                return null;
            };

            bool tempIsSelected;
            return mainMenuDelegate(MenuConfig.MenuItems, 0, out tempIsSelected);
        }
        public string BuildTopMenuConfigItems()
        {
            return BuildMenuConfigItems(MenuConfig.CurrentTopMenu);
        }
        public string BuildSideMenuConfigItems()
        {
            return BuildMenuConfigItems(MenuConfig.CurrentSideMenu);
        }
        protected void Nevigate_Click(object sender, EventArgs e)
        {
            LinkButton b = (LinkButton)sender;
            Response.Redirect(b.Attributes["url"].ToString());
        }
    }
}
