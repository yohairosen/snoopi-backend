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
    public class AppUserAPNSTokenHandler : ApiHandlerBase
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
                        if (Query.New<AppUserAPNSToken>()
                            .Where(AppUserAPNSToken.Columns.AppUserId, AppUserId)
                            .AND(AppUserAPNSToken.Columns.Token, PathParams[0])
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
                            RespondNotFound(Response);
                        }
                    }
                }
            }
        }

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
                                
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        //if (Query.New<AppUserAPNSToken>()
                        //    .Where(AppUserAPNSToken.Columns.AppUserId, AppUserId)
                        //    .AND(AppUserAPNSToken.Columns.Token, PathParams[0])
                        //    .LimitRows(1)
                        //    .GetCount(@"*") == 0)
                        //{
                            try
                            {
                                Query.New<AppUserAPNSToken>().Delete()
                                .Where(AppUserAPNSToken.Columns.Token, PathParams[0])
                                .Execute();
                               // Query.New<AppUserAPNSToken>().Delete()
                               //.Where(AppUserAPNSToken.Columns.AppUserId, AppUserId)
                               //.Execute();
                                Query.New<AppUserAPNSToken>()
                                .Insert(AppUserAPNSToken.Columns.AppUserId, AppUserId)
                                .Insert(AppUserAPNSToken.Columns.Token, PathParams[0])
                                .Execute();
                            }
                            catch { }
                        //}

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
                        if (Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, PathParams[0])
                            .Execute() > 0)
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WriteEndObject();
                        }
                        else
                        {
                            RespondNotFound(Response);
                        }
                    }
                }
            }
        }
    }
}
