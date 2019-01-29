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
    public class ShoppingCartHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            decimal TotalPrice;
            if (IsAuthorizedRequest(Request, Response, false, out AppUserId))
            {               

                try
                {
                    Response.ContentType = @"application/json";
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {

                            List<BidProductUI> lstProduct = BidController.GetLastShoppingCart(AppUserId, out TotalPrice);
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"products");
                            jsonWriter.WriteStartArray();

                            foreach (BidProductUI item in lstProduct)
                            {
                                jsonWriter.WriteStartObject();

                                jsonWriter.WritePropertyName(@"product_id");
                                jsonWriter.WriteValue(item.ProductId);
                                jsonWriter.WritePropertyName(@"product_name");
                                jsonWriter.WriteValue(item.ProductName);
                                jsonWriter.WritePropertyName(@"product_amount");
                                jsonWriter.WriteValue(item.Amount);
                                jsonWriter.WritePropertyName(@"order_amount");
                                jsonWriter.WriteValue(item.Amount);
                                jsonWriter.WritePropertyName(@"product_image");
                                jsonWriter.WriteValue(item.ProductImage);
                                jsonWriter.WritePropertyName(@"recomended_price");
                                jsonWriter.WriteValue(item.RecomendedPrice);
                                
                               // jsonWriter.WritePropertyName(@"product_price");
                               // jsonWriter.WriteValue(item.Price);

                                jsonWriter.WriteEndObject();
                            }
                             
                            jsonWriter.WriteEndArray();

                            //jsonWriter.WritePropertyName(@"total_price");
                            //jsonWriter.WriteValue(TotalPrice);

                            jsonWriter.WriteEndObject();
                        }
                    }
                }
                catch (Exception) { }
               
            }
            
        }

    }
}
