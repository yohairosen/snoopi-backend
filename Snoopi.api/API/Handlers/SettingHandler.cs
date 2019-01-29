using dg.Sql;
using dg.Sql.Connector;
using Newtonsoft.Json;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Snoopi.core.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Snoopi.api 
{
    public class SettingHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Response.ContentType = @"application/json";

            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    double EndBidTime = Convert.ToDouble(Settings.GetSettingDecimal(Settings.Keys.END_BID_TIME_MIN,15));
                    int SuppliedWithinHour = Settings.GetSettingInt32(Settings.Keys.SUPPLIED_WITHIN_HOUR, 24);
                    string HomeBanner = Settings.GetSetting(Settings.Keys.BANNER_HOME);
                    string CategoryBanner = Settings.GetSetting(Settings.Keys.BANNER_CATEGORY);
                    string SubCategoryBanner = Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY);
                    string AdminEmail = Settings.GetSetting(Settings.Keys.ADMIN_EMAIL);
                    string AdminPhone = Settings.GetSetting(Settings.Keys.ADMIN_PHONE);
                    string MinPriceOfferBids = Settings.GetSetting(Settings.Keys.MIN_PRICE_FOR_OFFER_BIDS);
                    string PrivacyPolicyUrl = Settings.GetSetting(Settings.Keys.PRIVACY_POLICY_URL);

                    jsonWriter.WriteStartObject();

                    //general settings
                    jsonWriter.WritePropertyName(@"setting");
                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName(@"end_bid_time_min");
                    jsonWriter.WriteValue(EndBidTime);

                    jsonWriter.WritePropertyName(@"supplied_within_hour");
                    jsonWriter.WriteValue(SuppliedWithinHour);

                    jsonWriter.WritePropertyName(@"home_banner");
                    jsonWriter.WriteValue(HomeBanner);


                    jsonWriter.WritePropertyName(@"category_banner");
                    jsonWriter.WriteValue(CategoryBanner);

                    jsonWriter.WritePropertyName(@"sub_category_banner");
                    jsonWriter.WriteValue(SubCategoryBanner);

                    jsonWriter.WritePropertyName(@"admin_email");
                    jsonWriter.WriteValue(AdminEmail);

                    jsonWriter.WritePropertyName(@"admin_phone");
                    jsonWriter.WriteValue(AdminPhone);


                    jsonWriter.WritePropertyName(@"min_price_for_bids_offer");
                    jsonWriter.WriteValue(MinPriceOfferBids);
                    //////////

                    jsonWriter.WritePropertyName(@"privacy_policy_url");
                    jsonWriter.WriteValue(PrivacyPolicyUrl);

                    jsonWriter.WritePropertyName(@"services");
                    jsonWriter.WriteStartArray();

                    foreach (Service item in ServiceController.GetAllService())
                    {
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"service_id");
                        jsonWriter.WriteValue(item.ServiceId);
                        jsonWriter.WritePropertyName(@"service_name");
                        jsonWriter.WriteValue(item.ServiceName);

                        jsonWriter.WriteEndObject();
                    }

                    jsonWriter.WriteEndArray();

                    ////////////


                    PriceFilterCollection priceFilterCol = PriceFilterCollection.FetchAll();
                  

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

                    ///////////////
                    List<FilterUI> lstFilterUI = ProductController.GetAllFilter();

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


                    ///////////////


                    jsonWriter.WritePropertyName(@"animal_list");
                    jsonWriter.WriteStartArray();

                    Query q = new Query(Animal.TableSchema);
                    using (DataReaderBase reader = q.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"animal_id");
                            jsonWriter.WriteValue(reader[Animal.Columns.AnimalId]);
                            jsonWriter.WritePropertyName(@"animal_name");
                            jsonWriter.WriteValue(reader[Animal.Columns.AnimalName] ?? "");

                            jsonWriter.WriteEndObject();
                        }
                    }
                    jsonWriter.WriteEndArray();

                    ///////////////////

                    CityCollection cities = CityCollection.FetchAll();
                    List<City> c = new List<City>();
                    c = cities;
                    c= c.OrderBy(x => x.CityName).ToList();

                    jsonWriter.WritePropertyName(@"cities-title");
                    jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.TITLE_CITIES) ?? @"");

                    jsonWriter.WritePropertyName(@"cities");
                    jsonWriter.WriteStartArray();


                    foreach (City item in c)
                    {
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"city_id");
                        jsonWriter.WriteValue(item.CityId);
                        jsonWriter.WritePropertyName(@"city_name");
                        jsonWriter.WriteValue(item.CityName);

                        jsonWriter.WriteEndObject();


                    }

                    jsonWriter.WriteEndArray();
                    //////////////


                   jsonWriter.WriteEndObject();
                }
                    
                }
            }
        }
    }


