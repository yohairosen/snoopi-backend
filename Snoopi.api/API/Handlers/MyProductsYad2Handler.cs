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
    public class MyProductsYad2Handler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                Int64 AppUserId;
                if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
                {
                    Response.ContentType = @"application/json";
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {


                            ProductYad2Collection productYad2Col = ProductYad2Controller.GetAllProductByAppUserId(AppUserId);
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"products");
                            jsonWriter.WriteStartArray();

                            foreach (ProductYad2 productYad2 in productYad2Col)
                            {
                                jsonWriter.WriteStartObject();

                                jsonWriter.WritePropertyName(@"product_id");
                                jsonWriter.WriteValue(productYad2.ProductId);
                                jsonWriter.WritePropertyName(@"product_name");
                                jsonWriter.WriteValue(productYad2.ProductName ?? "");
                                jsonWriter.WritePropertyName(@"product_date");
                                jsonWriter.WriteValue(productYad2.UpdateDate.ToShortDateString());
                                jsonWriter.WritePropertyName(@"status");
                                jsonWriter.WriteValue(Enum.GetName(typeof(StatusType), productYad2.Status).ToLower());

                                jsonWriter.WriteEndObject();
                            }
                            jsonWriter.WriteEndArray();
                            jsonWriter.WriteEndObject();
                        }
                    }
                }
            }
            catch (Exception) { }



        }

    }
}
