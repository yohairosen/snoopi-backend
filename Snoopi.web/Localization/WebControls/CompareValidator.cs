﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using Snoopi.web.Resources;

[assembly: TagPrefix("Snoopi.web.Localization.WebControls", "Snoopi.web")]
namespace Snoopi.web.Localization.WebControls
{
    [ToolboxData("<{0}:CompareValidator ID=\"CompareValidatorId\" runat=\"server\" />")]
    public class CompareValidator : System.Web.UI.WebControls.CompareValidator
    {
        protected override void Render(HtmlTextWriter writer)
        {
            if ((!string.IsNullOrEmpty(this.ID) || !string.IsNullOrEmpty(this.LocalizationId)) && this.ErrorMessage.Length == 0)
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
                this.ErrorMessage = value; 
            }
            base.Render(writer);
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
