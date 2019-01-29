using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Globalization;
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

namespace Snoopi.api
{
    public class AppUserGcmTokenHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        if (Query.New<AppUserGcmToken>()
                            .Where(AppUserGcmToken.Columns.AppUserId, AppUserId)
                            .AND(AppUserGcmToken.Columns.Token, PathParams[0])
                            .LimitRows(1)
                            .GetCount(@"*") > 0)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"token");
                            jsonWriter.WriteValue(PathParams[0]);

                            jsonWriter.WriteEndObject();
                        }
                        else
                        {
                            RespondError(Response, HttpStatusCode.NotFound, "gcm_token_not_found");
                        }
                    }
                }
            }
        }

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";
                                
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        try
                        {
                            Query.New<AppUserGcmToken>().Delete()
                            .Where(AppUserGcmToken.Columns.Token, PathParams[0])
                            .Execute();
                            //Query.New<AppUserGcmToken>().Delete()
                            // .Where(AppUserGcmToken.Columns.AppUserId, AppUserId)
                            // .Execute();
                            Query.New<AppUserGcmToken>()
                            .Insert(AppUserGcmToken.Columns.AppUserId, AppUserId)
                            .Insert(AppUserGcmToken.Columns.Token, PathParams[0])
                            .Execute();
                        }
                        catch { }

                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"token");
                        jsonWriter.WriteValue(PathParams[0]);

                        jsonWriter.WriteEndObject();
                    }                    

                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }

        public override void Delete(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        if (Query.New<AppUserGcmToken>().Delete()
                            .Where(AppUserGcmToken.Columns.Token, PathParams[0])
                            .AND(AppUserGcmToken.Columns.AppUserId, AppUserId)
                            .Execute() > 0)
                        {
                            RespondOk(Response);
                        }
                        else
                        {
                            RespondError(Response, HttpStatusCode.NotFound, "gcm_token_not_found");
                        }
                    }
                }
            }
        }
    }
}
