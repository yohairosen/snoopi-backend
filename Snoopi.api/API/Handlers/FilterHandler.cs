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

namespace Snoopi.api
{
    public class FilterHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {

                        List<FilterUI> lstFilterUI = ProductController.GetAllFilter();

                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"filters");
                        jsonWriter.WriteStartArray();

                        foreach (FilterUI item in lstFilterUI)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"filter_id");
                            jsonWriter.WriteValue(item.FilterId);
                            jsonWriter.WritePropertyName(@"filter_name");
                            jsonWriter.WriteValue(item.FilterName);

                            
                            jsonWriter.WritePropertyName(@"list_sub_filter");
                            jsonWriter.WriteStartArray();
                            foreach (SubFilterUI subItem in item.LstSubFilter)
                            {
                                jsonWriter.WriteStartObject();

                                jsonWriter.WritePropertyName(@"filter_id");
                                jsonWriter.WriteValue(subItem.FilterId);
                                jsonWriter.WritePropertyName(@"sub_filter_id");
                                jsonWriter.WriteValue(subItem.SubFilterId);
                                jsonWriter.WritePropertyName(@"sub_filter_name");
                                jsonWriter.WriteValue(subItem.SubFilterName);

                                jsonWriter.WriteEndObject();
                            }
                            jsonWriter.WriteEndArray();
                            jsonWriter.WriteEndObject();

                        }

                        jsonWriter.WriteEndArray();

                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception) { }



        }

    }
}
