using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using System.IO;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.BL;
using Snoopi.core.DAL;

namespace Snoopi.api
{
    public class AppUserResetNotificationHandler : ApiHandlerBase
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
            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";
                try
                {
                    Query.New<AppUser>()
                        .Where(AppUser.Columns.AppUserId, AppUserId)
                        .Update(AppUser.Columns.UnreadNotificationCount, 0)
                        .Execute();
                }
                catch (Exception ex)
                {
                    RespondError(Response, HttpStatusCode.InternalServerError, "db-error", ex.Message);
                }
                RespondOk(Response);
            }
        }
    }
}
