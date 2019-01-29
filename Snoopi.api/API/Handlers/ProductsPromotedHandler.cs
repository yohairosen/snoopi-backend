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
    public class ProductsPromotedHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                string jsonProducts = Request.QueryString["basket_products"];
                long cityId = (Request.QueryString["city_id"] != null ? Int64.Parse(Request.QueryString["city_id"].ToString()) : 0);
                int numberOfItems = (Request.QueryString["num_of_items"] != null ? Int32.Parse(Request.QueryString["num_of_items"].ToString()) : 0);
                Int64 AppUserId;
                IsAuthorizedRequest(Request, Response, false, out AppUserId);
                if (AppUserId > 0)
                    cityId = AppUser.FetchByID(AppUserId).CityId;

                List<int> busketProducts = null;
                if (jsonProducts != null && !String.IsNullOrWhiteSpace(jsonProducts))
                    busketProducts = JArray.Parse(jsonProducts).ToObject<List<int>>();
                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        var lstProductUI = ProductController.GetPromotedProducts(busketProducts, numberOfItems, cityId);
                        jsonWriter.WriteStartArray();
                        foreach (var section in lstProductUI)
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(section.Key);
                            jsonWriter.WriteStartArray();
                            foreach (var item in section.Value)
                            {
                                jsonWriter.WriteStartObject();
                                jsonWriter.WritePropertyName(@"product_id");
                                jsonWriter.WriteValue(item.ProductId);
                                jsonWriter.WritePropertyName(@"product_image");
                                jsonWriter.WriteValue(item.ProductImage);
                                jsonWriter.WritePropertyName(@"product_name");
                                jsonWriter.WriteValue(item.ProductName);
                                jsonWriter.WritePropertyName(@"products_code");
                                jsonWriter.WriteValue(item.ProductCode);
                                jsonWriter.WritePropertyName(@"min_price");
                                jsonWriter.WriteValue(item.MinPrice);
                                jsonWriter.WritePropertyName(@"max_price");
                                jsonWriter.WriteValue(item.MaxPrice); 
                                jsonWriter.WriteEndObject();
                            }
                            jsonWriter.WriteEndArray();
                            jsonWriter.WriteEndObject();

                        }
                        jsonWriter.WriteEndArray();
                    }
                }
            }
            catch (Exception ex) { }
        }

    }
}
