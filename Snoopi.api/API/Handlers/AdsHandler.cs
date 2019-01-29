using Newtonsoft.Json;
using Snoopi.core.BL;
using System;
using System.IO;
using System.Web;

namespace Snoopi.api
{
    public class AdsHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                int adId = (Request.QueryString["prom_id"] != null ? Int32.Parse(Request.QueryString["prom_id"].ToString()) : 0);
                var banner = AdvertisementController.GetAdvertisementForSite(adId, DateTime.Now);
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        jsonWriter.WritePropertyName(@"prom_img_id");
                        jsonWriter.WriteValue(banner.Id);
                        jsonWriter.WritePropertyName(@"prom_href");
                        jsonWriter.WriteValue(banner.Href);
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception ex) { }
        }

    }
}
