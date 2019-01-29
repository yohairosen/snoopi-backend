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
    public class ProductsYad2Handler : ApiHandlerBase
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
                        List<ProductYad2UI> productYad2Col = ProductYad2Controller.GetAllProduct();
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"products");
                        jsonWriter.WriteStartArray();

                        foreach (ProductYad2UI productYad2 in productYad2Col)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"product_id");
                            jsonWriter.WriteValue(productYad2.ProductYad2.ProductId);
                            jsonWriter.WritePropertyName(@"price");
                            jsonWriter.WriteValue(productYad2.ProductYad2.Price);
                            jsonWriter.WritePropertyName(@"product_name");
                            jsonWriter.WriteValue(productYad2.ProductYad2.ProductName ?? "");
                            jsonWriter.WritePropertyName(@"product_image");
                            jsonWriter.WriteValue(productYad2.ProductYad2.ProductImage ?? "");
                            jsonWriter.WritePropertyName(@"city_id");
                            jsonWriter.WriteValue(productYad2.ProductYad2.CityId);
                            jsonWriter.WritePropertyName(@"price_id");
                            jsonWriter.WriteValue(productYad2.ProductYad2.PriceId);
                            jsonWriter.WritePropertyName(@"product_date");
                            jsonWriter.WriteValue(productYad2.ProductYad2.UpdateDate.ToShortDateString());                           
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(productYad2.ProductYad2.Details);
                            jsonWriter.WritePropertyName(@"contact_name");
                            jsonWriter.WriteValue(productYad2.ProductYad2.ContactName ?? "");
                            jsonWriter.WritePropertyName(@"phone");
                            jsonWriter.WriteValue(productYad2.ProductYad2.Phone);
                            jsonWriter.WritePropertyName(@"product_date");
                            jsonWriter.WriteValue(productYad2.ProductYad2.UpdateDate.ToShortDateString());

                            jsonWriter.WritePropertyName(@"list_category");
                            jsonWriter.WriteStartArray();
                            foreach (CategoryYad2 item in productYad2.LstCategory)
                            {
                                jsonWriter.WriteStartObject();
                                jsonWriter.WritePropertyName(@"category_id");
                                jsonWriter.WriteValue(item.CategoryYad2Id);
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
