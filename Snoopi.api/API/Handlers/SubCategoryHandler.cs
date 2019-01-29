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
    public class SubCategoryHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {

                Int64 CategoryId = (Request.QueryString["category_id"] != null ? Int64.Parse(Request.QueryString["category_id"].ToString()) : 0);
                Int64 AnimalId = (Request.QueryString["animal_id"] != null ? Int64.Parse(Request.QueryString["animal_id"].ToString()) : 0);
                Int64 TempUserID = (Request.QueryString["temp_app_user_id"] != null ? Int64.Parse(Request.QueryString["temp_app_user_id"].ToString()) : 0);
                string jsonProducts = Request.QueryString["basket_products"];
                List<int> busketProducts = null;
                if (jsonProducts != null && !String.IsNullOrWhiteSpace(jsonProducts))
                    busketProducts = JArray.Parse(jsonProducts).ToObject<List<int>>();
               
                Int64 AppUserId;
                IsAuthorizedRequest(Request, Response, false, out AppUserId);

                 List<CategoryUI> lstSubCategory;
                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {

                        if (AppUserId == 0 && TempUserID == 0)
                              lstSubCategory = ProductController.GetAllSubCategory(CategoryId, AnimalId);
                        else
                            lstSubCategory = ProductController.GetAllSubCategoryOfUser(busketProducts, CategoryId, AnimalId, AppUserId, TempUserID);
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"sub_categories");
                        jsonWriter.WriteStartArray();

                        foreach (CategoryUI item in lstSubCategory)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"category_id");
                            jsonWriter.WriteValue(item.CategoryId);
                            jsonWriter.WritePropertyName(@"sub_category_id");
                            jsonWriter.WriteValue(item.SubCategoryId);
                            jsonWriter.WritePropertyName(@"sub_category_name");
                            jsonWriter.WriteValue(item.SubCategoryName);
                            jsonWriter.WritePropertyName(@"sub_category_image");
                            jsonWriter.WriteValue(item.SubCategoryImage);
                            jsonWriter.WritePropertyName(@"products_num");
                            jsonWriter.WriteValue(item.SubCategoryProductsNum);

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
