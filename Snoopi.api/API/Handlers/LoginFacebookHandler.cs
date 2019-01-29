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

namespace Snoopi.api
{
    public class LoginFacebookHandler : ApiHandlerBase
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

            string accessToken = inputData.Value<string>(@"access_token") ?? "";

            Response.ContentType = @"application/json";

            AppUser user;
            AppMembership.AppUserFacebookConnectResults res = AppMembership.ConnectAppUserToFacebook(accessToken, out user);
            switch (res)
            {
                case AppMembership.AppUserFacebookConnectResults.Success:
                    {
                        AppUserAuthToken at = AuthTokens.GenerateAuthTokenForAppUserId(user.AppUserId, 0);

                        using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                        {
                            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                            {
                                jsonWriter.WriteStartObject();                               

                                jsonWriter.WritePropertyName(@"access_token");
                                jsonWriter.WriteValue(AuthTokens.AccessToken(at));

                                jsonWriter.WritePropertyName(@"user_id");
                                jsonWriter.WriteValue(user.AppUserId);

                                Int64? cityId = user.CityId;
                                jsonWriter.WritePropertyName(@"is_city_exists");
                                jsonWriter.WriteValue(cityId != null && cityId != 0);

                                jsonWriter.WriteEndObject();
                            }
                        }
                    }
                    break;
                default:
                case AppMembership.AppUserFacebookConnectResults.LoginError:
                    {
                        RespondForbidden(Response);
                    }
                    break;
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }
    }
}
