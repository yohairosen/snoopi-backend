using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using Snoopi.web.Resources;

[assembly: TagPrefix("Snoopi.web.Localization.WebControls", "Snoopi.web")]
namespace Snoopi.web.Localization.WebControls
{
    [ToolboxData("<{0}:Label ID=\"LabelId\" runat=\"server\" />")]
    public class Label : System.Web.UI.WebControls.Label
    {
        protected override void OnInit(EventArgs e)
        {
            if (CssClass.Length == 0) CssClass = (CssClass.Length > 0 ? CssClass + " " : "") + (_BlockMode ? "label-line" : @"label");
            base.OnInit(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if ((!string.IsNullOrEmpty(this.ID) || !string.IsNullOrEmpty(this.LocalizationId)) && this.Text.Length == 0)
            {
                string value = null;

                string locId = LocalizationId;
                if (locId == null || locId.Length == 0) locId = this.ID;

                string locCls = LocalizationClass;
                if (locCls != null && locCls.Length > 0)
                {
                    value = ResourceManagerAccessor.GetText(locCls, locId);
                }
                if (value == null) value = Global.ResourceManager.GetString(locId);
                this.Text = value;
            }
            base.Render(writer);
        }

        private bool _BlockMode = false;
        public bool BlockMode
        {
            get { return _BlockMode; }
            set { _BlockMode = value; }
        }

        private string _LocalizationId = null;
        public string LocalizationId
        {
            get { return _LocalizationId; }
            set { _LocalizationId = value; }
        }

        private string _LocalizationClass = null;
        public string LocalizationClass
        {
            get { return _LocalizationClass; }
            set { _LocalizationClass = value; }
        }
    }
}