using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snoopi.web.Localization;
using Snoopi.core.BL;
using System.Net.Mail;
using System.Web.Mail;
using System.IO;
using dg.Utilities;

public partial class Contact : System.Web.UI.Page
{

    public enum ContactType : int {
        Choose  = 0 ,
        Recomment = 1,
        Help = 2,
        Other = 3

    }

    private static string[] AcceptedImageExtensions = new string[] { @".jpg", @".jpeg", @".png" ,@".bmp"};
    public static bool IsAcceptedImageExtension(string fileName)
    {
        string ext = System.IO.Path.GetExtension(fileName).ToLower();
        return AcceptedImageExtensions.Contains(ext);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlContactType.DataSource = Enum.GetNames(typeof(ContactType)).
            Select(o => new { Value = o, Text = ContactStrings.GetText(o) });
            ddlContactType.DataTextField = "Text";

            lblPhone.Text = Settings.GetSetting(Settings.Keys.ADMIN_PHONE);

            ddlContactType.DataValueField = "Value";
            ddlContactType.DataBind();
            ddlContactType.SelectedIndex = 0;
            if (Request.QueryString["type"] != null) ddlContactType.SelectedValue = Request.QueryString["type"].ToString();
        }
    }

    protected void revEml_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!txtEmail.Text.IsValidEmail())
        {
            args.IsValid = false;
            Master.MessageCenter.DisplayErrorMessage(SupplierProfileStrings.GetText(@"EmailWrong"));
        }
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;
        string fileName = "";
        if (FUploadImage.FileName != "" && !IsAcceptedImageExtension(FUploadImage.FileName))
        {
            Master.MessageCenter.DisplayErrorMessage(ContactStrings.GetText("InvalidExtension"));
            return;
        }
        else
        {
            fileName = Server.MapPath(Path.Combine(Settings.Keys.CONTACT_FOLDER, FUploadImage.FileName));
            try
            {
                FUploadImage.SaveAs(fileName);
            }

            catch (Exception) { }
        }
        bool result = false;
       string toList = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);
            string fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
            string fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
            string replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
            string replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);
            string subject = ContactStrings.GetText("ContactTitle");
            string body = "<table>" +
                "<tr><td>" + ContactStrings.GetText("FirstName") + "</td><td>" + txtFirstName.Text + "</td></tr>" +
                "<tr><td>" + ContactStrings.GetText("Email") + "</td><td>" + txtEmail.Text + "</td></tr>" +
                "<tr><td>" + ContactStrings.GetText("Phone") + "</td><td>" + txtPhone.Text + "</td></tr>" +
                "<tr><td>" + ContactStrings.GetText("ContactType") + "</td><td>" + ddlContactType.SelectedItem.Text + "</td></tr>" +
                "<tr><td>" + ContactStrings.GetText("ContactDetails") + "</td><td>" + txtContactDetails.Text + "</td></tr>" +
                "</table>";
            System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                fromEmail, fromName, replyToEmail, replyToName,
                toList, null, null, subject, body, new string[] { fileName }, System.Net.Mail.MailPriority.Normal);
            result = EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
        
        if (result == true)
        {
            txtEmail.Text = "";
            txtFirstName.Text = "";
            txtContactDetails.Text = "";
            txtPhone.Text = "";

            Master.MessageCenter.DisplaySuccessMessage(ContactStrings.GetText(@"MessageSend"));
        }
        else
        {
            Master.MessageCenter.DisplayErrorMessage(ContactStrings.GetText(@"Error"));        
        }
    }
}