using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Snoopi.api
{
    public class SupplierEventHandler: ApiHandlerBase
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



            Response.ContentType = @"application/json";

            JToken jt;

            Int64 AppUserId=0;
            IsAuthorizedRequest(Request, Response, false, out AppUserId);
            if (AppUserId != 0)
            {

                bool _locked = AppUser.FetchByID(AppUserId) != null ? AppUser.FetchByID(AppUserId).IsLocked : true;
                if (_locked)
                {
                    RespondError(Response, HttpStatusCode.BadRequest, @"appuser-locked");
                    return;
                }
            }

            Int64 SupplierId = 0;
            if (inputData.TryGetValue(@"supplier_id", out jt)) SupplierId = jt.Value<Int64>();

            string eventType = "";
            if (inputData.TryGetValue(@"event_type", out jt)) eventType = jt.Value<string>();

            ServiceController.AddClickEvent(AppUserId, SupplierId, eventType);
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
}
