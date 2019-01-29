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
    public class SupplierAuthValidateHandler : ApiHandlerBase
    {
        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);            

            Int64 SupplierId;
            if (IsAuthorizedRequestSupplier(Request, Response, true, out SupplierId))
            {
                Response.ContentType = @"application/json";
                List<object> SupplierStatus = new List<object>();
                try
                {
                    
                    AppMembership.AppSupplierLoggedInAction(SupplierId,out SupplierStatus);
                }
                catch { }

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"user_id");
                        jsonWriter.WriteValue(SupplierId);

                        jsonWriter.WritePropertyName(@"status");
                        jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[0] : false);

                        jsonWriter.WritePropertyName(@"allow_change_status_join_bids");
                        jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[1] : false);

                        jsonWriter.WritePropertyName(@"is_auto_join_bid");
                        jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[2] : false);

                        jsonWriter.WritePropertyName(@"max_winning_num");
                        jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[3] : 0);

                        jsonWriter.WritePropertyName(@"is_service_supplier");
                        jsonWriter.WriteValue(SupplierStatus.Count > 0 ? SupplierStatus[4] : false);

                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }
    }
}
