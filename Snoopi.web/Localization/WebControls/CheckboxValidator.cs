using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using Snoopi.web.Resources;
using System.Web.UI.WebControls;

[assembly: TagPrefix("Snoopi.web.Localization.WebControls", "Snoopi.web")]
namespace Snoopi.web.Localization.WebControls
{
    [ToolboxData("<{0}:CheckboxValidator ID=\"CheckboxValidatorId\" runat=\"server\" />")]
    public class CheckboxValidator : System.Web.UI.WebControls.BaseValidator
    {
        private Control _ControlToValidate;
        private List<object> _CheckBoxes;
        private int _MinimumChecked = 1;
        private int _MaximumChecked = 1;

        public int MinimumChecked
        {
            get { return _MinimumChecked; }
            set { _MinimumChecked = value; }
        }
        public int MaximumChecked
        {
            get { return _MaximumChecked != int.MaxValue ? _MaximumChecked : 0; }
            set { _MaximumChecked = value != 0 ? value : int.MaxValue; }
        }
        private int GetCheckedCount()
        {
            int count = 0;
            foreach (object checkBox in _CheckBoxes)
            {
                bool selected = false;
                if (checkBox is ListItem) selected = ((ListItem)checkBox).Selected;
                if (checkBox is CheckBox) selected = ((CheckBox)checkBox).Checked;
                if (selected) count++;
            }
            return count;
        }
        private List<object> GetCheckBoxes()
        {
            List<object> checkBoxes = new List<object>();

            if (_ControlToValidate is CheckBox)
            {
                checkBoxes.Add((CheckBox)_ControlToValidate);
            }
            else
            {
                checkBoxes = GetCheckBoxes(_ControlToValidate);
            }

            return checkBoxes;
        }
        private List<object> GetCheckBoxes(Control parent)
        {
            List<object> checkBoxes = new List<object>();

            if (parent is CheckBoxList)
            {
                CheckBoxList checkBoxList = (CheckBoxList)parent;
                foreach (ListItem item in checkBoxList.Items)
                {
                    checkBoxes.Add(item);
                }
            }
            else
            {
                foreach (Control control in parent.Controls)
                {
                    if (control is CheckBox) checkBoxes.Add((CheckBox)control);
                    else checkBoxes.AddRange(GetCheckBoxes(control));
                }
            }

            return checkBoxes;
        }
        protected override bool EvaluateIsValid()
        {
            int count = GetCheckedCount();
            return count >= MinimumChecked && count <= MaximumChecked;
        }
        protected override bool ControlPropertiesValid()
        {
            _ControlToValidate = this.NamingContainer.FindControl(ControlToValidate);
            bool isValid = _ControlToValidate is CheckBox || _ControlToValidate is CheckBoxList || _ControlToValidate.Controls.Count > 0;
            if (isValid) _CheckBoxes = GetCheckBoxes();

            return isValid;
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (EnableClientScript)
            {
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "evaluationfunction", "Validators_CheckBoxValidator", false);
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "CheckedCount", GetCheckedCount().ToString(), false);
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (EnableClientScript)
            {
                string onclick = string.Format("if(this.checked) document.getElementById('{0}').CheckedCount++; else document.getElementById('{0}').CheckedCount--;", ClientID);
                foreach (object checkBox in GetCheckBoxes())
                {
                    if (checkBox is CheckBox) ((CheckBox)checkBox).Attributes.Add("onclick", onclick);
                    if (checkBox is ListItem) ((ListItem)checkBox).Attributes.Add("onclick", onclick);
                }

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidationFunction", string.Format(@"
                    function Validators_CheckBoxValidator(sender) {{ return sender.CheckedCount >= {0} && sender.CheckedCount <= {1}; }}
                    ", MinimumChecked, MaximumChecked), true);
            }
        }

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
