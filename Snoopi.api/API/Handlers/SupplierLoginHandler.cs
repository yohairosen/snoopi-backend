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
    public class SupplierLoginHandler : ApiHandlerBase
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

            Int64 SupplierId;
            AppMembership.AppUserAuthenticateResults res = AppMembership.AuthenticateAppSupplier(email, password, out SupplierId);
            switch (res)
            {
                case AppMembership.AppUserAuthenticateResults.Success:
                    {
                        List<object> SupplierStatus = new List<object>();
                        AppSupplierAuthToken at = AuthTokens.GenerateAuthTokenForAppSupplierId(SupplierId, 0);

                        try
                        {
                            AppMembership.AppSupplierLoggedInAction(SupplierId, out SupplierStatus);
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
                                jsonWriter.WriteValue(SupplierId);

                                jsonWriter.WritePropertyName(@"status");
                                jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[0] : false);

                                jsonWriter.WritePropertyName(@"allow_change_status_join_bids");
                                jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[1] : false);

                                jsonWriter.WritePropertyName(@"is_auto_join_bid");
                                jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[2] : false);
                                
                                jsonWriter.WritePropertyName(@"is_service_supplier");
                                jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[4] : false);

                                jsonWriter.WritePropertyName(@"max_winning_num");
                                jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[3] : 0);

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
