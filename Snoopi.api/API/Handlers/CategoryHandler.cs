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
    public class CategoryHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                Int64 AnimalId = (Request.QueryString["animal_id"] != null ? Int64.Parse(Request.QueryString["animal_id"].ToString()) : 0);
                Int64 TempUserID = (Request.QueryString["temp_app_user_id"] != null ? Int64.Parse(Request.QueryString["temp_app_user_id"].ToString()) : 0);
                string jsonProducts = Request.QueryString["basket_products"];
                List<int> busketProducts = null;
                if (jsonProducts != null && !String.IsNullOrWhiteSpace(jsonProducts))
                   busketProducts = JArray.Parse(jsonProducts).ToObject<List<int>>();
               
                Response.ContentType = @"application/json";
                Int64 AppUserId;
                IsAuthorizedRequest(Request, Response, false, out AppUserId);

                List<CategoryUI> lstCategoryUI;
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                         if (AppUserId == 0 && TempUserID == 0)
                              lstCategoryUI = ProductController.GetAllCategory(AnimalId);
                         else
                             lstCategoryUI = ProductController.GetAllCategoriesOfUser(busketProducts, AnimalId, AppUserId, TempUserID);
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"categories");
                        jsonWriter.WriteStartArray();

                        foreach (CategoryUI item in lstCategoryUI)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"category_id");
                            jsonWriter.WriteValue(item.CategoryId);
                            jsonWriter.WritePropertyName(@"category_name");
                            jsonWriter.WriteValue(item.CategoryName);
                            jsonWriter.WritePropertyName(@"category_image");
                            jsonWriter.WriteValue(item.CategoryImage);
                            jsonWriter.WritePropertyName(@"products_num");
                            jsonWriter.WriteValue(item.ProductsNum);

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
