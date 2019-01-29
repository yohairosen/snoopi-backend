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
using dg.Sql;
using dg.Sql.Connector;
using System.Globalization;
using GoogleMaps.LocationServices;
using System.Text.RegularExpressions;

namespace Snoopi.api
{
    public class SupplierRateHandler : ApiHandlerBase
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
            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                JToken jt;
                Int64 supplier_id = 0;
                string sender_name = null, content = null;
                double rate = 0;
                Int64 BidId = 0;
              
                if (inputData.TryGetValue(@"supplier_id", out jt)) supplier_id = jt.Value<Int64>();
                if (inputData.TryGetValue(@"sender_name", out jt)) sender_name = jt.Value<string>();
                if (inputData.TryGetValue(@"content", out jt)) content = Regex.Replace(jt.Value<string>(), @"\p{Cs}", "");
                if (inputData.TryGetValue(@"rate", out jt)) rate = jt.Value<double>();
                if (inputData.TryGetValue(@"bid_id", out jt)) BidId = jt.Value<Int64>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Comment comment = new Comment();
                        if (supplier_id != null) comment.SupplierId = supplier_id;
                        if (sender_name != null) comment.Name = sender_name;
                        if (content != null) comment.Content = content;
                        if (rate != null) comment.Rate = float.Parse(rate.ToString());
                        comment.AppUserId = AppUserId;
                        comment.Status = CommentStatus.Wait;
                        //if(OfferId != 0) 
                        //     offer = Offer.FetchByID(OfferId);
                        //if (offer != null)
                        //     BidId = offer.BidId;
                        comment.BidId = BidId;
                       
                       // if (BidId != null) comment.BidId = BidId;
                        comment.Save();

                        jsonWriter.WriteStartObject();
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
