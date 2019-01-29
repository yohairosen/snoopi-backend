using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Snoopi.core;

namespace Snoopi.api
{
    public class SupplierForgotPasswordHandler : ApiHandlerBase
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

            string email = inputData.Value<string>(@"email") ?? "";

            Response.ContentType = @"application/json";

            string key = AppMembership.GenerateRecoveryKeySupplier(email);
            AppSupplier user = null;
            if (key != null)
            {
                user = AppSupplier.FetchByEmail(email);
            }

            if (key == null || user == null)
            {
                RespondNotFound(Response);
            }
            else
            {
                EmailMessagingService.SendPasswordRecoveryMailForSupplier(user, key, "he-IL");

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }
    }
}
