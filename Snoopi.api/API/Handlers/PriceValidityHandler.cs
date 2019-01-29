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
    public class PriceValidityHandler : ApiHandlerBase
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

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                try
                {
                    JToken jt;
                    JArray products = null;
                    Int64 supplierId = 0;
                    decimal totalPrice = 0;
                    var lstProduct = new Dictionary<Int64, int>();

                    if (inputData.TryGetValue(@"products", out jt)) products = jt.Value<JArray>();
                    if (inputData.TryGetValue(@"supplier_id", out jt)) supplierId = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"total_price", out jt) && jt != null) totalPrice = jt.Value<decimal>();
                    foreach (JObject obj in products.Children<JObject>())
                    {
                        Int64 product_id = 0;
                        int amount = 1;
                        if (obj.TryGetValue(@"product_id", out jt)) product_id = jt.Value<Int64>();
                        if (obj.TryGetValue(@"amount", out jt)) amount = jt.Value<int>();
                        lstProduct.Add(product_id, amount);
                    }

                    bool isPriceValid = false;
                    if (supplierId > 0 && totalPrice> 0)
                        isPriceValid = OfferController.IsOfferStillValid(lstProduct, supplierId, totalPrice);
                    if (!isPriceValid)
                        RespondError(Response, HttpStatusCode.ExpectationFailed, @"price-not-valid");

                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            //var o = new Order();
                            //o.TotalPrice = totalPrice;
                            //o.AppUserId = AppUserId;
                            //o.UserPaySupplierStatus = UserPaymentStatus.NotPayed;
                            //o.Save();
                            jsonWriter.WriteStartObject();
                            //jsonWriter.WritePropertyName(@"order_id");
                            //jsonWriter.WriteValue(o.OrderId);
                            jsonWriter.WritePropertyName(@"total_price");
                            jsonWriter.WriteValue(totalPrice);
                            jsonWriter.WriteEndObject();
                        }
                    }
                }
                catch (InvalidDataException e)
                {

                    RespondError(Response, HttpStatusCode.InternalServerError, e.Message);

                }

                catch (Exception ex)
                {
                    RespondError(Response, HttpStatusCode.InternalServerError, @"db-error");
                }

            }


        }
    }
}

