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
    public class ProductsYad2FilterHandler : ApiHandlerBase
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
                        PriceFilterCollection priceFilterCol = PriceFilterCollection.FetchAll();
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"price-title");
                        jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.TITLE_PRICES) ?? @"");

                        jsonWriter.WritePropertyName(@"list_price_filter");
                        jsonWriter.WriteStartArray();

                        foreach (PriceFilter item in priceFilterCol)
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"price_id");
                            jsonWriter.WriteValue(item.PriceId);
                            jsonWriter.WritePropertyName(@"price_name");
                            jsonWriter.WriteValue(item.PriceName);                           
                            jsonWriter.WriteEndObject();
                        }
                        jsonWriter.WriteEndArray();

                        jsonWriter.WritePropertyName(@"category-title");
                        jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.TITLE_CATEGORIES) ?? @"");

                        jsonWriter.WritePropertyName(@"list_category");
                        jsonWriter.WriteStartArray();

                        CategoryYad2Collection categoryYad2Col = CategoryYad2Collection.FetchAll();
                        foreach (CategoryYad2 item in categoryYad2Col)
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"category_id");
                            jsonWriter.WriteValue(item.CategoryYad2Id);
                            jsonWriter.WritePropertyName(@"category_name");
                            jsonWriter.WriteValue(item.CategoryYad2Name);
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
