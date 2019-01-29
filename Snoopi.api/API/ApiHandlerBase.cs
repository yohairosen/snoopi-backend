using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using Snoopi.core.BL;
using System.IO;
using dg.Utilities.WebApiServices;
using Snoopi.core.DAL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;

namespace Snoopi.api
{
    public abstract class ApiHandlerBase : IRestHandlerTarget
    {
        protected bool IsAuthorizedRequest(HttpRequest request, HttpResponse response, bool automaticResponseOnFail, out Int64 AppUserId)
        {
            JObject noJSON;
            return IsAuthorizedRequest(request, response, automaticResponseOnFail, out noJSON, out AppUserId);
        }
        protected bool IsAuthorizedRequest(HttpRequest request, HttpResponse response, bool automaticResponseOnFail, out JObject inputJson, out Int64 appUserId)
        {
            inputJson = null;

            bool hasFormData = request.HttpMethod == "POST" || request.HttpMethod == "PUT";
            bool hasRequestBody = hasFormData || request.ContentLength > 0;

            if ((request.Headers["Authorization"] != null &&
                request.Headers["Authorization"].StartsWith(@"Token ", StringComparison.Ordinal)) ||
                (request.HttpMethod == "GET" && request.QueryString["access_token"] != null))
            {
                string token = request.Headers["Authorization"];
                if (token != null)
                {
                    token = token.Substring(6);
                }
                else
                {
                    token = request.QueryString["access_token"];
                }

                Int64 authTokenId;
                if (AuthTokens.ValidateAppUserAuthToken(token, false, out appUserId, out authTokenId))
                {
                    return true;
                }
                else
                {
                    if (automaticResponseOnFail)
                    {
                        RespondForbidden(response);
                    }
                    return false;
                }
            }
            else
            { // Deprecated

                string authTokenSecret = null, authTokenKey = null;
                if (hasRequestBody
                    &&
                    (request.ContentType.StartsWith("application/x-www-form-urlencoded") ||
                    request.ContentType.StartsWith("multipart/form-data")))
                {
                    authTokenSecret = request.Form[@"auth_token_secret"] ?? "";
                    authTokenKey = request.Form[@"auth_token_key"] ?? "";
                }
                else if (hasRequestBody && request.ContentType.StartsWith("application/json"))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(request.InputStream))
                        {
                            using (JsonTextReader jsonReader = new JsonTextReader(reader))
                            {
                                inputJson = JObject.Load(jsonReader);
                            }
                        }
                    }
                    catch
                    {
                        RespondBadRequest(response);
                    }

                    if (inputJson != null)
                    {
                        JToken jt;
                        if (inputJson.TryGetValue(@"auth_token_secret", out jt)) authTokenSecret = jt.Value<string>() ?? @"";
                        if (inputJson.TryGetValue(@"auth_token_key", out jt)) authTokenKey = jt.Value<string>() ?? @"";
                    }
                }
                else
                {
                    authTokenSecret = request.QueryString[@"auth_token_secret"] ?? "";
                    authTokenKey = request.QueryString[@"auth_token_key"] ?? "";

                    if (hasFormData && authTokenSecret.Length == 0 && authTokenKey.Length == 0)
                    {
                        authTokenSecret = request.Form[@"auth_token_secret"] ?? "";
                        authTokenKey = request.Form[@"auth_token_key"] ?? "";
                    }
                }

                if (authTokenSecret != null && authTokenSecret.Length > 0 &&
                    authTokenKey != null && authTokenKey.Length > 0)
                {
                    Int64 appUserAuthTokenId;
                    if (AuthTokens.ValidateAppUserAuthToken(authTokenSecret, authTokenKey, false, out appUserId, out appUserAuthTokenId))
                    {
                        return true;
                    }
                    else
                    {
                        if (automaticResponseOnFail)
                        {
                            RespondForbidden(response);
                        }
                        return false;
                    }
                }
                else
                {
                    appUserId = 0;
                }
                if (automaticResponseOnFail)
                {
                    RespondBadRequest(response);
                }

            }

            return false;
        }


        protected bool IsAuthorizedRequestSupplier(HttpRequest request, HttpResponse response, bool automaticResponseOnFail, out Int64 AppSupplierId)
        {
            JObject noJSON;
            return IsAuthorizedRequestSupplier(request, response, automaticResponseOnFail, out noJSON, out AppSupplierId);
        }
        protected bool IsAuthorizedRequestSupplier(HttpRequest request, HttpResponse response, bool automaticResponseOnFail, out JObject inputJson, out Int64 AppSupplierId)
        {
            inputJson = null;

            bool hasFormData = request.HttpMethod == "POST" || request.HttpMethod == "PUT";
            bool hasRequestBody = hasFormData || request.ContentLength > 0;

            if ((request.Headers["Authorization"] != null &&
                request.Headers["Authorization"].StartsWith(@"Token ", StringComparison.Ordinal)) ||
                (request.HttpMethod == "GET" && request.QueryString["access_token"] != null))
            {
                string token = request.Headers["Authorization"];
                if (token != null)
                {
                    token = token.Substring(6);
                }
                else
                {
                    token = request.QueryString["access_token"];
                }

                Int64 authTokenId;
                if (AuthTokens.ValidateAppSupplierAuthToken(token, false, out AppSupplierId, out authTokenId))
                {
                    var supplier = AppSupplier.FetchByID(AppSupplierId);
                    bool _locked = supplier != null ? supplier.IsLocked : true;            
                    return true;
                }
                else
                {
                    if (automaticResponseOnFail)
                    {
                        RespondForbidden(response);
                    }
                    return false;
                }
            }
            else
            { // Deprecated

                string authTokenSecret = null, authTokenKey = null;
                if (hasRequestBody
                    &&
                    (request.ContentType.StartsWith("application/x-www-form-urlencoded") ||
                    request.ContentType.StartsWith("multipart/form-data")))
                {
                    authTokenSecret = request.Form[@"auth_token_secret"] ?? "";
                    authTokenKey = request.Form[@"auth_token_key"] ?? "";
                }
                else if (hasRequestBody && request.ContentType.StartsWith("application/json"))
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(request.InputStream))
                        {
                            using (JsonTextReader jsonReader = new JsonTextReader(reader))
                            {
                                inputJson = JObject.Load(jsonReader);
                            }
                        }
                    }
                    catch
                    {
                        RespondBadRequest(response);
                    }

                    if (inputJson != null)
                    {
                        JToken jt;
                        if (inputJson.TryGetValue(@"auth_token_secret", out jt)) authTokenSecret = jt.Value<string>() ?? @"";
                        if (inputJson.TryGetValue(@"auth_token_key", out jt)) authTokenKey = jt.Value<string>() ?? @"";
                    }
                }
                else
                {
                    authTokenSecret = request.QueryString[@"auth_token_secret"] ?? "";
                    authTokenKey = request.QueryString[@"auth_token_key"] ?? "";

                    if (hasFormData && authTokenSecret.Length == 0 && authTokenKey.Length == 0)
                    {
                        authTokenSecret = request.Form[@"auth_token_secret"] ?? "";
                        authTokenKey = request.Form[@"auth_token_key"] ?? "";
                    }
                }

                if (authTokenSecret != null && authTokenSecret.Length > 0 &&
                    authTokenKey != null && authTokenKey.Length > 0)
                {
                    Int64 AppSupplierAuthTokenId;
                    if (AuthTokens.ValidateAppSupplierAuthToken(authTokenSecret, authTokenKey, false, out AppSupplierId, out AppSupplierAuthTokenId))
                    {
                        return true;
                    }
                    else
                    {
                        if (automaticResponseOnFail)
                        {
                            RespondForbidden(response);
                        }
                        return false;
                    }
                }
                else
                {
                    AppSupplierId = 0;
                }
                if (automaticResponseOnFail)
                {
                    RespondBadRequest(response);
                }

            }

            return false;
        }
        


        protected void RespondOk(HttpResponse Response)
        {
            RespondError(Response, HttpStatusCode.OK, @"OK");
        }
        protected void RespondNotFound(HttpResponse Response)
        {
            RespondError(Response, HttpStatusCode.NotFound, @"not-found");
        }
        protected void RespondBadRequest(HttpResponse Response)
        {
            RespondError(Response, HttpStatusCode.BadRequest, @"bad-request");
        }
        protected void RespondBadRequest(HttpResponse Response, string JsonErrorCode, string JsonErrorDescription)
        {
            RespondError(Response, HttpStatusCode.BadRequest, JsonErrorCode, JsonErrorDescription);
        }
        protected void RespondForbidden(HttpResponse Response)
        {
            RespondError(Response, HttpStatusCode.Forbidden, @"authorization-error");
        }
        protected void RespondUnauthorized(HttpResponse Response)
        {
            RespondError(Response, HttpStatusCode.Unauthorized, @"not-allowed");
        }
        protected void RespondInternalServerError(HttpResponse Response)
        {
            RespondError(Response, HttpStatusCode.InternalServerError, @"internal-server-error");
        }
        protected void RespondError(HttpResponse Response, HttpStatusCode HttpStatusCode, string JsonErrorCode)
        {
            RespondError(Response, HttpStatusCode, JsonErrorCode, null);
        }
        protected void RespondError(HttpResponse Response, HttpStatusCode HttpStatusCode, string JsonErrorCode, string JsonErrorDescription)
        {
            Response.ClearContent();
            Response.ContentType = @"application/json";
            Response.StatusCode = (int)HttpStatusCode;
            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName(@"error");
                    jsonWriter.WriteValue(JsonErrorCode == null ? @"unknown" : JsonErrorCode);

                    if (JsonErrorDescription != null)
                    {
                        jsonWriter.WritePropertyName(@"description");
                        jsonWriter.WriteValue(JsonErrorDescription);
                    }

                    jsonWriter.WriteEndObject();
                }
            }
            Response.End();
        }
        protected bool RestBooleanValue(string ArgumentValue)
        {
            return ArgumentValue == @"1" || ArgumentValue == @"true";
        }

        #region IRestHandlerTarget
        public virtual void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        public virtual void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        public virtual void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        public virtual void Delete(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        public virtual void Head(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        public virtual void Options(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        public virtual void Patch(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Utilities
        public static decimal? DistanceInKmFromArgument(string argument)
        {
            if (argument == null) return null;
            argument = argument.ToLowerInvariant();
            decimal dTry;
            if (argument.EndsWith(@"km"))
            {
                if (decimal.TryParse(argument.Substring(0, argument.Length - 2), out dTry)) return dTry;
            }
            else if (argument.EndsWith(@"m"))
            {
                if (decimal.TryParse(argument.Substring(0, argument.Length - 1), out dTry)) return dTry / 1000m;
            }
            else if (argument.EndsWith(@"ft"))
            {
                if (decimal.TryParse(argument.Substring(0, argument.Length - 2), out dTry)) return dTry * 0.0003048m;
            }
            else if (argument.EndsWith(@"mi"))
            {
                if (decimal.TryParse(argument.Substring(0, argument.Length - 2), out dTry)) return dTry * 1.60934m;
            }
            else if (argument.EndsWith(@"yd"))
            {
                if (decimal.TryParse(argument.Substring(0, argument.Length - 2), out dTry)) return dTry * 0.0009144m;
            }
            else
            { // Km
                if (decimal.TryParse(argument, out dTry)) return dTry;
            }
            return null;
        }
        #endregion

    }
}
