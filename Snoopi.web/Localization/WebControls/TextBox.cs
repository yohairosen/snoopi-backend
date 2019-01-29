using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Globalization;
using Snoopi.web.Resources;
using dg.Utilities;

[assembly: TagPrefix("Snoopi.web.Localization.WebControls", "Snoopi.web")]
namespace Snoopi.web.Localization.WebControls
{
    [ToolboxData("<{0}:TextBox ID=\"TextBoxId\" runat=\"server\" />")]
    public class TextBox : System.Web.UI.WebControls.TextBox
    {
        public enum TextBoxLength
        {
            Short,
            Normal,
            Medium,
            Long,
        }

        protected override void OnInit(EventArgs e)
        {
            if (CssClass.Length == 0) 
            {
                CssClass = @"input-text";
                switch (Length)
                {
                    case TextBoxLength.Short:
                        CssClass += " short";
                        break;
                    default:
                    case TextBoxLength.Normal:
                        CssClass += " normal";
                        break;
                    case TextBoxLength.Medium:
                        CssClass += " medium";
                        break;
                    case TextBoxLength.Long:
                        CssClass += " long";
                        break;
                }
            }
            base.OnInit(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), @"input-placeholder", @"
(function(){ 
    window.RegisterPlaceholderForInput = function(input, placeholder) {
        dgTools.observe(input, 'focus', function(){if (input.value==placeholder) { input.value=''; dgTools.dom.removeClassName(input, 'placeholder'); }});
        dgTools.observe(input, 'blur', function(){if (input.value=='') { input.value=placeholder; dgTools.dom.addClassName(input, 'placeholder'); }});
        if (input.value == '' || input.value == placeholder) {
            input.value=placeholder; 
            dgTools.dom.addClassName(input, 'placeholder');
        }
    }
})();", true);
            base.OnLoad(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (_PlaceholderText != null)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), @"input-placeholder_" + this.ClientID,
                    string.Format(@"RegisterPlaceholderForInput(document.getElementById('{0}'), '{1}');", this.ClientID, _PlaceholderText.ToJavaScript('\'', false)),
                    true);
            }
            base.Render(writer);
        }

        private TextBoxLength _Length = TextBoxLength.Normal;
        public TextBoxLength Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        private string _Placeholder = null;
        private string _PlaceholderText = null;
        public string Placeholder
        {
            get { return _Placeholder; }
            set 
            {
                string _Text = this.Text;

                _Placeholder = value;
                _PlaceholderText = null;
                if ((!string.IsNullOrEmpty(this.ID) || !string.IsNullOrEmpty(this.Placeholder)))
                {
                    string locId = Placeholder;
                    if (locId == null || locId.Length == 0) locId = this.ID;

                    string locCls = LocalizationClass;
                    if (locCls != null && locCls.Length > 0)
                    {
                        _PlaceholderText = ResourceManagerAccessor.GetText(locCls, locId);
                    }
                    if (_PlaceholderText == null) _PlaceholderText = Global.ResourceManager.GetString(locId);
                    if (_PlaceholderText != null && _PlaceholderText.Length == 0)
                    {
                        _PlaceholderText = null;
                    }
                }

                if (_Text.Length == 0 && _PlaceholderText != null)
                {
                    this.Text = _PlaceholderText;
                }
                else
                {
                    this.Text = _Text;
                }
            }
        }

        private string _LocalizationClass = null;
        public string LocalizationClass
        {
            get { return _LocalizationClass; }
            set { _LocalizationClass = value; }
        }

        public override string Text
        {
            get
            {
                if (base.Text == _PlaceholderText)
                {
                    return @"";
                }
                return base.Text;
            }
            set 
            {
                base.Text = value;
            }
        }
    }
}