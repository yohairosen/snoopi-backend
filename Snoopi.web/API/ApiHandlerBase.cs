using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using System.IO;
using dg.Utilities.WebApiServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;

namespace Snoopi.web.API
{
    public abstract class ApiHandlerBase : IRestHandlerTarget
    {
        protected bool IsAuthorizedGetDeleteHead(HttpResponse Response, bool AutomaticResponseOnFail)
        {
            if (!SessionHelper.IsAuthenticated())
            {
                if (AutomaticResponseOnFail)
                {
                    RespondForbidden(Response);
                }
                return false;
            }
            return true;
        }
        protected bool IsAuthorizedPostPut(HttpResponse Response, bool AutomaticResponseOnFail)
        {
            if (!SessionHelper.IsAuthenticated())
            {
                if (AutomaticResponseOnFail)
                {
                    RespondForbidden(Response);
                }
                return false;
            }
            return true;
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
    }
}
