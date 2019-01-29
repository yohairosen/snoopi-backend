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
    public class SupplierOfferBidHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                
             Int64 SupplierId;
             if (IsAuthorizedRequestSupplier(Request, Response, true, out SupplierId))
             {

                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {
                         Int64 OfferId = Request.QueryString["offer_id"] != null ? Convert.ToInt64(Request.QueryString["offer_id"]) : 0;
                         Int64 bidId = Request.QueryString["bid_id"] != null ? Convert.ToInt64(Request.QueryString["bid_id"]) : 0;
                         bool IsService = Request.QueryString["is_service"] != null ? Convert.ToBoolean(Request.QueryString["is_service"]) : false;
                         MainOffer mainOffer = IsService ? SupplierController.GetServiceOfferById(OfferId, SupplierId) : SupplierController.GetBidOfferById(bidId, SupplierId);
                         jsonWriter.WriteStartObject();

                         jsonWriter.WritePropertyName(@"bid_id");
                         jsonWriter.WriteValue(mainOffer.BidId);
                         jsonWriter.WritePropertyName(@"end_time");
                         jsonWriter.WriteValue(mainOffer.EndBid);
                         jsonWriter.WritePropertyName(@"city");
                         jsonWriter.WriteValue(mainOffer.City);
                         jsonWriter.WritePropertyName(@"service_id");
                         jsonWriter.WriteValue(mainOffer.ServiceId);
                        
                         jsonWriter.WritePropertyName(@"products");
                         jsonWriter.WriteStartArray();
                         if (mainOffer.LstProduct != null && mainOffer.LstProduct.Count > 0)
                         {
                             foreach (BidProductUI item in mainOffer.LstProduct)
                             {
                                 jsonWriter.WriteStartObject();

                                 jsonWriter.WritePropertyName(@"product_id");
                                 jsonWriter.WriteValue(item.ProductId);
                                 jsonWriter.WritePropertyName(@"product_name");
                                 jsonWriter.WriteValue(item.ProductName);
                                 jsonWriter.WritePropertyName(@"product_amount");
                                 jsonWriter.WriteValue(item.ProductAmount);
                                 jsonWriter.WritePropertyName(@"product_image");
                                 jsonWriter.WriteValue(item.ProductImage);
                                 jsonWriter.WritePropertyName(@"order_amount");
                                 jsonWriter.WriteValue(item.Amount);
                                 jsonWriter.WritePropertyName(@"product_price");
                                 jsonWriter.WriteValue(item.Price * item.Amount);
                                
                                 jsonWriter.WriteEndObject();
                             }
                         }

                         jsonWriter.WriteEndArray();

                         jsonWriter.WritePropertyName(@"total_price");
                         jsonWriter.WriteValue(mainOffer.TotalPrice);

                         jsonWriter.WritePropertyName(@"num_of_payments");
                         jsonWriter.WriteValue(mainOffer.NumOfPayments);

                         jsonWriter.WritePropertyName(@"gift");
                         jsonWriter.WriteValue(mainOffer.Gift);

                         jsonWriter.WritePropertyName(@"supplier_remarks");
                         jsonWriter.WriteValue(mainOffer.SupplierRemarks);

                         jsonWriter.WritePropertyName(@"customer_comment");
                         jsonWriter.WriteValue(mainOffer.CustomerComment);

                         jsonWriter.WritePropertyName(@"service_name");
                         jsonWriter.WriteValue(mainOffer.ServiceName);

                         jsonWriter.WriteEndObject();
                     }
                 }
             }
            }
            catch (Exception) { }



        }

    }
}
