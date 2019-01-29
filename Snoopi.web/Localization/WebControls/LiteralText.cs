using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using Snoopi.web.Resources;

[assembly: TagPrefix("Snoopi.web.Localization.WebControls", "Snoopi.web")]
namespace Snoopi.web.Localization.WebControls
{
    [ToolboxData("<{0}:Literal ID=\"LabelId\" runat=\"server\" />")]
    public class LiteralText : System.Web.UI.WebControls.WebControl
    {
        protected override void Render(HtmlTextWriter writer)
        {
            if ((!string.IsNullOrEmpty(this.ID) || !string.IsNullOrEmpty(this.LocalizationId)))
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
                
                if (HtmlEncoded)
                {
                    writer.WriteEncodedText(value);
                }
                else
                {
                    writer.Write(value);
                }
            }
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
        
        private bool _HtmlEncoded = true;
        public bool HtmlEncoded
        {
            get { return _HtmlEncoded; }
            set { _HtmlEncoded = value; }
        }
    }
}