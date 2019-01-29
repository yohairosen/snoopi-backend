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
    public class SearchProductsHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                Int64 CategoryId = (Request.QueryString["category_id"] != null ? Int64.Parse(Request.QueryString["category_id"].ToString()) : 0);
                Int64 SubCategoryId = (Request.QueryString["sub_category_id"] != null ? Int64.Parse(Request.QueryString["sub_category_id"].ToString()) : 0);
                Int64 AnimalId = (Request.QueryString["animal_id"] != null ? Int64.Parse(Request.QueryString["animal_id"].ToString()) : 0);
                Int64 TempUserID = (Request.QueryString["temp_app_user_id"] != null ? Int64.Parse(Request.QueryString["temp_app_user_id"].ToString()) : 0);
                string jsonProducts = Request.QueryString["basket_products"];
                string productName = Request.QueryString["product_name"];
                bool isAutoCompleteSerch = (Request.QueryString["auto_complete"] != null ? Boolean.Parse(Request.QueryString["auto_complete"].ToString()) :false);

                List<int> busketProducts = null;
                if (jsonProducts != null && !String.IsNullOrWhiteSpace(jsonProducts))
                    busketProducts = JArray.Parse(jsonProducts).ToObject<List<int>>();

                Response.ContentType = @"application/json";

                Int64 AppUserId;
                IsAuthorizedRequest(Request, Response, false, out AppUserId);        
                List<MaxMinProductUI> lstProductUI = null;
                List<FilterUI> lstFiltersUI = null;
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {                      
                        lstProductUI = ProductController.GetProductsByProductName(busketProducts,productName, CategoryId, SubCategoryId, AnimalId, AppUserId, TempUserID);
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"products");
                        jsonWriter.WriteStartArray();
                        if (isAutoCompleteSerch)
                        {
                            foreach (var item in lstProductUI)
                            {
                                jsonWriter.WriteStartObject();
                                jsonWriter.WritePropertyName(@"product_name");
                                jsonWriter.WriteValue(item.ProductName);
                                jsonWriter.WriteEndObject();
                            }
                        }
                        else
                        {

                            lstFiltersUI = ProductController.GetAllFiltersBySubCategory(CategoryId, SubCategoryId);

                            foreach (var item in lstProductUI)
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
                                jsonWriter.WritePropertyName(@"recomended_price");
                                jsonWriter.WriteValue(item.RecomendedPrice == 0 ? 0 : item.RecomendedPrice);
                                jsonWriter.WritePropertyName(@"description");
                                jsonWriter.WriteValue(item.Description);
                                jsonWriter.WritePropertyName(@"amount");
                                jsonWriter.WriteValue(item.Amount);

                                jsonWriter.WritePropertyName(@"min_price");
                                jsonWriter.WriteValue(item.MinPrice);
                                jsonWriter.WritePropertyName(@"max_price");
                                jsonWriter.WriteValue(item.MaxPrice);

                                jsonWriter.WritePropertyName(@"list_filter");
                                jsonWriter.WriteStartArray();
                                foreach (ProductFilter subItem in item.LstFilters)
                                {
                                    jsonWriter.WriteStartObject();

                                    jsonWriter.WritePropertyName(@"filter_id");
                                    jsonWriter.WriteValue(subItem.FilterId);

                                    jsonWriter.WritePropertyName(@"sub_filter_id");
                                    jsonWriter.WriteValue(subItem.SubFilterId);

                                    jsonWriter.WriteEndObject();
                                }
                                jsonWriter.WriteEndArray();

                                jsonWriter.WriteEndObject();
                            }

                            jsonWriter.WriteEndArray();


                            jsonWriter.WritePropertyName(@"filters");
                            jsonWriter.WriteStartArray();

                            foreach (FilterUI item in lstFiltersUI)
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
