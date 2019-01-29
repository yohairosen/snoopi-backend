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
    public class LoginHandler : ApiHandlerBase
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
            string password = inputData.Value<string>(@"password") ?? "";

            Response.ContentType = @"application/json";

            Int64 AppUserId;
            AppMembership.AppUserAuthenticateResults res = AppMembership.AuthenticateAppUser(email, password, out AppUserId);
            switch (res)
            {
                case AppMembership.AppUserAuthenticateResults.Success:
                    {
                        AppUserAuthToken at = AuthTokens.GenerateAuthTokenForAppUserId(AppUserId, 0);

                        try
                        {
                            AppMembership.AppUserLoggedInAction(AppUserId);
                        }
                        catch { }

                        using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                        {
                            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                            {
                                jsonWriter.WriteStartObject();

                                jsonWriter.WritePropertyName(@"access_token");
                                jsonWriter.WriteValue(AuthTokens.AccessToken(at));

                                jsonWriter.WritePropertyName(@"user_id");
                                jsonWriter.WriteValue(AppUserId);

                                Int64? cityId = AppUser.FetchByID(AppUserId).CityId;
                                jsonWriter.WritePropertyName(@"is_city_exists");
                                jsonWriter.WriteValue(cityId != null && cityId != 0);

                                jsonWriter.WriteEndObject();
                            }
                        }
                    }
                    break;
                default:
                case AppMembership.AppUserAuthenticateResults.LoginError:
                    {
                        RespondBadRequest(Response);
                    }
                    break;
                case AppMembership.AppUserAuthenticateResults.NotVerified:
                    {
                        RespondError(Response, HttpStatusCode.Forbidden, @"not-verified");
                    }
                    break;
                case AppMembership.AppUserAuthenticateResults.NoMatch:
                    {
                        RespondError(Response, HttpStatusCode.Forbidden, @"no-match");
                    }
                    break;
                case AppMembership.AppUserAuthenticateResults.Locked:
                    {
                        RespondError(Response, HttpStatusCode.Forbidden, @"locked");
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
