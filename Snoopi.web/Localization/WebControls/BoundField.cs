using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using Snoopi.web.Resources;

[assembly: TagPrefix("Snoopi.web.Localization.WebControls", "Snoopi.web")]
namespace Snoopi.web.Localization.WebControls
{
    [ToolboxData("<{0}:BoundField ID=\"BoundFieldId\" runat=\"server\" />")]
    public class BoundField : System.Web.UI.WebControls.BoundField
    {
        private string _HeaderTextLocalizationId = null;
        public string HeaderTextLocalizationId
        {
            get { return _HeaderTextLocalizationId; }
            set { _HeaderTextLocalizationId = value; }
        }

        private string _LocalizationClass = null;
        public string LocalizationClass
        {
            get { return _LocalizationClass; }
            set { _LocalizationClass = value; }
        }

        override public string HeaderText
        {
            get
            {
                string value = null;

                string locId = this.HeaderTextLocalizationId;
                if (locId != null && locId.Length > 0) 
                {
                    string locCls = LocalizationClass;
                    if (locCls != null && locCls.Length > 0)
                    {
                        value = ResourceManagerAccessor.GetText(locCls, locId);
                    }
                    if (value == null) value = Global.ResourceManager.GetString(locId);
                    if (locCls != null && locCls.Length > 0)
                    {
                        return value;
                    }
                }
                return base.HeaderText;
            }
        }
    }
}