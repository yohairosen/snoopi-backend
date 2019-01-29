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
    public class SignupHandler : ApiHandlerBase
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
            string lang_code = inputData.Value<string>(@"lang_code") ?? "";
            bool isTestUser = inputData.Value<bool>(@"is_test_user");

            Int64 tempAppUserId = inputData.Value<Int64>(@"temp_app_user_id");

            Response.ContentType = @"application/json";
            if (isTestUser)
            {
                User userBack;
                Membership.TestUser(email, password.Trim(), out userBack);
                RespondBadRequest(Response, @"is_succeeded", (userBack != null).ToString());
            }

            AppUser user;
            AppMembership.AppUserCreateResults res = AppMembership.CreateAppUser(email, password, lang_code, out user);

            switch (res)
            {
                case AppMembership.AppUserCreateResults.Success:
                    {
                        using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                        {
                            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                            {
                                if (!user.IsVerified && Settings.GetSettingBool(Settings.Keys.APPUSER_VERIFY_EMAIL, false))
                                {
                                    jsonWriter.WriteStartObject();

                                    jsonWriter.WritePropertyName(@"needs_verification");
                                    jsonWriter.WriteValue(true);

                                    jsonWriter.WriteEndObject();
                                }
                                else
                                {
                                    AppUserAuthToken at = AuthTokens.GenerateAuthTokenForAppUserId(user.AppUserId, 0);

                                    jsonWriter.WriteStartObject();

                                    //jsonWriter.WritePropertyName(@"auth_token_secret");
                                    //jsonWriter.WriteValue(at.Secret.ToString(@"N"));

                                    //jsonWriter.WritePropertyName(@"auth_token_key");
                                    //jsonWriter.WriteValue(at.Key);
                                    if (tempAppUserId != 0)
                                    {
                                        BidController.UpdateTempAppUserBidsToUserBid(tempAppUserId, user.AppUserId);
                                    }
                                    jsonWriter.WritePropertyName(@"access_token");
                                    jsonWriter.WriteValue(AuthTokens.AccessToken(at));

                                    jsonWriter.WritePropertyName(@"user_id");
                                    jsonWriter.WriteValue(user.AppUserId);

                                    jsonWriter.WriteEndObject();
                                }
                            }
                        }
                    }
                    break;
                case AppMembership.AppUserCreateResults.AlreadyExists:
                    {
                        RespondBadRequest(Response, @"user-already-exists", @"the supplied email address is in use");
                    }
                    break;
                case AppMembership.AppUserCreateResults.InvalidEmailAddress:
                    {
                        RespondBadRequest(Response, @"invalid-email", @"the supplied email address is invalid");
                    }
                    break;
                default:
                case AppMembership.AppUserCreateResults.UnknownError:
                    {
                        RespondInternalServerError(Response);
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
