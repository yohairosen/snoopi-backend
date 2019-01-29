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
    public class PreOrderPaymentHandler : ApiHandlerBase
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
                    Int64 bid_id = 0, offer_id = 0, campaign_id = 0;
                    string special_instructions = null;
                    if (inputData.TryGetValue(@"offer_id", out jt)) offer_id = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"bid_id", out jt)) bid_id = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"campaign_id", out jt) && jt != null) campaign_id = jt.Value<Int64?>() ?? 0;
                    if (inputData.TryGetValue(@"special_instructions", out jt)) special_instructions = Regex.Replace(jt.Value<string>(), @"\p{Cs}", "");
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
 
                            Order o = Order.FetchByBidId(bid_id);


                            if (o != null)
                            {
                               
                                    Dictionary<string, string> result = BidController.GetDiscount(offer_id, AppUserId);
                                    decimal TotalPrice = result["TotalPrice"] != null ? Convert.ToDecimal(result["TotalPrice"].ToString()) : 0;
                                    decimal PriceAfterDiscount = result["PriceAfterDiscount"] != null ? Convert.ToDecimal(result["PriceAfterDiscount"].ToString()) : 0;
                                    decimal PrecentDiscount = result["PrecentDiscount"] != null ? Convert.ToDecimal(result["PrecentDiscount"].ToString()) : 0;
                                    Int64? CampaignId = result["CampaignId"] != null ? (Int64?)Convert.ToInt64(result["CampaignId"].ToString()) : null;
                                    if (CampaignId != 0)
                                        o.CampaignId = CampaignId;
                                    o.TotalPrice = TotalPrice;
                                    o.PriceAfterDiscount = PriceAfterDiscount;
                                    o.PrecentDiscount = PrecentDiscount;
                                    o.SpecialInstructions = special_instructions;

                                    o.AppUserId = AppUserId;
                                    o.UserPaySupplierStatus = UserPaymentStatus.NotPayed;

                                    o.Save();
                                }

                                jsonWriter.WriteStartObject();
                                jsonWriter.WritePropertyName(@"order_id");
                                jsonWriter.WriteValue(o.OrderId);
                                jsonWriter.WritePropertyName(@"total_price");
                                jsonWriter.WriteValue(o.TotalPrice);
                                jsonWriter.WritePropertyName(@"price_after_discount");
                                jsonWriter.WriteValue(o.PriceAfterDiscount);
                                jsonWriter.WritePropertyName(@"percent_discount");
                                jsonWriter.WriteValue(o.PrecentDiscount);
                                jsonWriter.WritePropertyName(@"special_instructions");
                                jsonWriter.WriteValue(o.SpecialInstructions);
                                jsonWriter.WriteEndObject();

                                if (campaign_id != null && campaign_id != 0)
                                {
                                    AppUserCampaign appUserCampaign = new AppUserCampaign();
                                    appUserCampaign.AppUserId = AppUserId;
                                    appUserCampaign.CampaignId = campaign_id;
                                    appUserCampaign.Save();
                                }

                            
                            else
                            {
                                //{throw new InvalidDataException("Exist in db");}
                                Order order = new Order();

                                Dictionary<string, string> result = BidController.GetDiscount(offer_id, AppUserId);

                                decimal TotalPrice = result["TotalPrice"] != null ? Convert.ToDecimal(result["TotalPrice"].ToString()) : 0;
                                decimal PriceAfterDiscount = result["PriceAfterDiscount"] != null ? Convert.ToDecimal(result["PriceAfterDiscount"].ToString()) : 0;
                                decimal PrecentDiscount = result["PrecentDiscount"] != null ? Convert.ToDecimal(result["PrecentDiscount"].ToString()) : 0;
                                Int64? CampaignId = result["CampaignId"] != null ? (Int64?)Convert.ToInt64(result["CampaignId"].ToString()) : null;
                                if (CampaignId != 0)
                                    order.CampaignId = CampaignId;
                                order.TotalPrice = TotalPrice;
                                order.PriceAfterDiscount = PriceAfterDiscount;
                                order.PrecentDiscount = PrecentDiscount;
                                order.SpecialInstructions = special_instructions;
                                order.BidId = bid_id;
                                order.AppUserId = AppUserId;
                                order.UserPaySupplierStatus = UserPaymentStatus.NotPayed;
                                order.Save();

                                jsonWriter.WriteStartObject();
                                jsonWriter.WritePropertyName(@"order_id");
                                jsonWriter.WriteValue(order.OrderId);
                                jsonWriter.WritePropertyName(@"total_price");
                                jsonWriter.WriteValue(order.TotalPrice);
                                jsonWriter.WritePropertyName(@"price_after_discount");
                                jsonWriter.WriteValue(order.PriceAfterDiscount);
                                jsonWriter.WritePropertyName(@"precent_discount");
                                jsonWriter.WriteValue(order.PrecentDiscount);
                                jsonWriter.WritePropertyName(@"special_instructions");
                                jsonWriter.WriteValue(order.SpecialInstructions);
                                jsonWriter.WriteEndObject();

                                if (campaign_id != null && campaign_id != 0)
                                {
                                    AppUserCampaign appUserCampaign = new AppUserCampaign();
                                    appUserCampaign.AppUserId = AppUserId;
                                    appUserCampaign.CampaignId = campaign_id;
                                    appUserCampaign.Save();
                                }
                            }
                        }
                    }
                }
                catch (InvalidDataException e )
                {

                    RespondError(Response, HttpStatusCode.InternalServerError, e.Message);
                   
                }
                
                catch (Exception)
                {
                    RespondError(Response, HttpStatusCode.InternalServerError, @"db-error");
                }
               
            }


        }
    }
}
