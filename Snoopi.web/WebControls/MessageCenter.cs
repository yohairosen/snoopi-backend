using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using dg.Utilities;

namespace Snoopi.web.WebControls
{
    public class MessageCenter : WebControl
    {
        #region Member Variables

        protected Panel pnlMessageCenter = new Panel();
        protected Panel pnlValidation = null;

        protected ValidationSummary validationSummary;

        List<Panel> lstMessages = new List<Panel>();

        private bool _HasValidationSummary = true;
        private bool _ValidationSummaryShowMessageBox = false;
        private bool _ValidationSummaryShowSummary = true;
        private ValidationSummaryDisplayMode _ValidationSummaryDisplayMode = ValidationSummaryDisplayMode.BulletList;

        private bool _NeedResetPanelsVisibility = true;

        #endregion

        #region Constructors

        public MessageCenter()
        {

        }

        #endregion

        #region Methods

        #region Public

        public void DisplayMessage(string messageClass, string message, bool isLiteral)
        {
            ResetPanelsVisibility(false);

            Panel pnlMessage = new Panel();
            pnlMessage.Visible = true;
            pnlMessage.CssClass = messageClass;
            Label lblMessage = new Label();
            lblMessage.Text = isLiteral ? message : message.ToHtml().Replace("\n", @"<br />");
            HtmlGenericControl icon = new HtmlGenericControl("span");
            icon.Attributes[@"class"] = @"icon";
            pnlMessage.Controls.Add(icon);
            pnlMessage.Controls.Add(lblMessage);
            lstMessages.Add(pnlMessage);
            pnlMessageCenter.Controls.Add(pnlMessage);
        }
        public void DisplaySuccessMessage(string message, bool isLiteral)
        {
            DisplayMessage(@"message done", message, isLiteral);
        }
        public void DisplaySuccessMessage(string message)
        {
            DisplaySuccessMessage(message, false);
        }

        public void DisplayInformationMessage(string message, bool isLiteral)
        {
            DisplayMessage(@"message info", message, isLiteral);
        }
        public void DisplayInformationMessage(string message)
        {
            DisplayInformationMessage(message, false);
        }

        public void DisplayWarningMessage(string message, bool isLiteral)
        {
            DisplayMessage(@"message warning", message, isLiteral);
        }
        public void DisplayWarningMessage(string message)
        {
            DisplayWarningMessage(message, false);
        }

        public void DisplayErrorMessage(string message, bool isLiteral)
        {
            DisplayMessage(@"message error", message, isLiteral);
        }
        public void DisplayErrorMessage(string message)
        {
            DisplayErrorMessage(message, false);
        }

        public void ResetPanelsVisibility(bool force)
        {
            if (!force && !_NeedResetPanelsVisibility) return;
            _NeedResetPanelsVisibility = false;

            EnsureChildControls();
            if (HasValidationSummary) pnlValidation.Visible = true;
            foreach (Panel pnl in lstMessages)
            {
                pnlMessageCenter.Controls.Remove(pnl);
                pnl.Visible = false;
                pnl.Dispose();
            }
            lstMessages.Clear();
        }

        public void RegisterValidationSummaryForValidationGroup(string ValidationGroup)
        {
            EnsureChildControls();
            if (HasValidationSummary && pnlValidation != null)
            {
                validationSummary = new ValidationSummary();
                validationSummary.ValidationGroup = ValidationGroup;
                validationSummary.DisplayMode = ValidationSummaryDisplayMode;
                validationSummary.CssClass = @"message error";
                validationSummary.ShowMessageBox = ValidationSummaryShowMessageBox;
                validationSummary.ShowSummary = ValidationSummaryShowSummary;
                pnlValidation.Controls.Add(validationSummary);
            }
        }

        #endregion

        #region Protected

        protected override void CreateChildControls()
        {
            Controls.Clear();

            pnlValidation = new Panel();
            validationSummary = new ValidationSummary();
            validationSummary.DisplayMode = ValidationSummaryDisplayMode;
            validationSummary.CssClass = @"message error";
            validationSummary.ShowMessageBox = ValidationSummaryShowMessageBox;
            validationSummary.ShowSummary = ValidationSummaryShowSummary;
            pnlValidation.Controls.Add(validationSummary);

            if (HasValidationSummary) pnlMessageCenter.Controls.Add(pnlValidation);

            this.Controls.Add(pnlMessageCenter);
            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        #endregion

        #endregion

        #region Properties

        public override ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }

        public string Title
        {
            set
            {
                pnlMessageCenter.GroupingText = value;
            }
            get
            {
                return pnlMessageCenter.GroupingText;
            }
        }

        public bool HasValidationSummary
        {
            set { _HasValidationSummary = value; }
            get { return _HasValidationSummary; }
        }

        public ValidationSummaryDisplayMode ValidationSummaryDisplayMode
        {
            set { _ValidationSummaryDisplayMode = value; }
            get { return _ValidationSummaryDisplayMode; }
        }

        public bool ValidationSummaryShowMessageBox
        {
            set { _ValidationSummaryShowMessageBox = value; }
            get { return _ValidationSummaryShowMessageBox; }
        }

        public bool ValidationSummaryShowSummary
        {
            set { _ValidationSummaryShowSummary = value; }
            get { return _ValidationSummaryShowSummary; }
        }

        #endregion
    }
}
