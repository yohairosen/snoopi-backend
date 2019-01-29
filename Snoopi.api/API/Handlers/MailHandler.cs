using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using Snoopi.core.BL;
using System.IO;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Snoopi.core.DAL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;
using System.Globalization;
using GoogleMaps.LocationServices;
using Snoopi.web.Localization;

namespace Snoopi.api
{
    public class MailHandler : ApiHandlerBase
    {

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            JObject inputData = null;
            try
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        inputData = JObject.Load(jsonReader);
                    }
                }
            }
            catch
            {
                RespondBadRequest(Response);
            }
            string name = inputData.Value<string>(@"name") ?? "";
            string email = inputData.Value<string>(@"email") ?? "";
            string phone = inputData.Value<string>(@"phone") ?? "";
            string content = inputData.Value<string>(@"content") ?? "";

            Response.ContentType = @"application/json";
                
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        try
                        {
                            string subject = GlobalStrings.GetText("ContactUsSubject", new CultureInfo("he-IL"));
                            string body = GlobalStrings.GetText("Email", new CultureInfo("he-IL")) + " : " + email + "<br>" +
                                GlobalStrings.GetText("Phone", new CultureInfo("he-IL")) + " : " + phone + "<br>" +
                                GlobalStrings.GetText("Content", new CultureInfo("he-IL")) + " : " + content + "<br>";
                            string AdminEmail = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);
                            string fromEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM);
                            string replyToEmail = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO);
                            string fromName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_FROM_NAME);
                            string replyToName = Settings.GetSetting(Settings.Keys.DEFAULT_EMAIL_REPLYTO_NAME);
                            System.Net.Mail.MailMessage message = EmailTemplateController.BuildMailMessage(
                                fromEmail, fromName, replyToEmail, replyToName,
                                AdminEmail, null, null, subject, body, null, null);
                            EmailTemplateController.Send(message, EmailLogController.EmailLogType.OnError, true);
                        }
                        catch (Exception) { }                       

                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();

                    }
                
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }

    }
}
