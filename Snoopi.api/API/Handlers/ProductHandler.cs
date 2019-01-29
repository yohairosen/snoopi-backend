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
    public class ProductHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {

                Int64 ProductId = (Request.QueryString["product_id"] != null ? Int64.Parse(Request.QueryString["product_id"].ToString()) : 0);
                Int64 cityId = (Request.QueryString["city_id"] != null ? Int64.Parse(Request.QueryString["city_id"].ToString()) : 0);
                string jsonProducts = Request.QueryString["basket_products"];
                List<int> busketProducts = null;
                if (jsonProducts != null && !String.IsNullOrWhiteSpace(jsonProducts))
                    busketProducts = JArray.Parse(jsonProducts).ToObject<List<int>>();
                Int64 AppUserId;
                IsAuthorizedRequest(Request, Response, false, out AppUserId);
                if (AppUserId > 0)
                    cityId = AppUser.FetchByID(AppUserId).CityId;

                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        decimal minPrice, maxprice;
                        Product product = ProductController.GetProductById(ProductId, busketProducts, out minPrice, out maxprice, cityId);

                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"product_id");
                        jsonWriter.WriteValue(product.ProductId);
                        jsonWriter.WritePropertyName(@"product_image");
                        jsonWriter.WriteValue(product.ProductImage);
                        jsonWriter.WritePropertyName(@"product_name");
                        jsonWriter.WriteValue(product.ProductName);
                        jsonWriter.WritePropertyName(@"products_code");
                        jsonWriter.WriteValue(product.ProductCode);
                        jsonWriter.WritePropertyName(@"description");
                        jsonWriter.WriteValue(product.Description);
                        jsonWriter.WritePropertyName(@"amount");
                        jsonWriter.WriteValue(product.Amount);
                        jsonWriter.WritePropertyName(@"min_price");
                        jsonWriter.WriteValue(minPrice);
                        jsonWriter.WritePropertyName(@"max_price");
                        jsonWriter.WriteValue(maxprice); 
                        jsonWriter.WritePropertyName(@"recomended_price");
                        jsonWriter.WriteValue(product.RecomendedPrice == 0 ? 0 : product.RecomendedPrice);
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception) { }



        }

    }
}
