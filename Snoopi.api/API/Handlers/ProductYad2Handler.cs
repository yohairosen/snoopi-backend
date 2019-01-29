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
using Snoopi.core;

namespace Snoopi.api
{
    public class ProductYad2Handler : ApiHandlerBase
    {
        private static string[] AcceptedImageExtensions = new string[] { @".jpg", @".jpeg", @".png" };
        public static bool IsAcceptedImageExtension(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return AcceptedImageExtensions.Contains(ext);
        }

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Response.ContentType = @"application/json";
            Int64 product_id = Request.QueryString["product_id"] != null ? Convert.ToInt64(Request.QueryString["product_id"]) : 0;
            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    ProductYad2 productYad2 = ProductYad2.FetchByID(product_id);

                    if (productYad2 == null) RespondNotFound(Response);

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName(@"price");
                    jsonWriter.WriteValue(productYad2.Price);
                    jsonWriter.WritePropertyName(@"product_name");
                    jsonWriter.WriteValue(productYad2.ProductName ?? "");
                    jsonWriter.WritePropertyName(@"contact_name");
                    jsonWriter.WriteValue(productYad2.ContactName ?? "");
                    jsonWriter.WritePropertyName(@"description");
                    jsonWriter.WriteValue(productYad2.Details ?? "");
                    jsonWriter.WritePropertyName(@"product_image");
                    jsonWriter.WriteValue(productYad2.ProductImage ?? "");
                    jsonWriter.WritePropertyName(@"phone");
                    jsonWriter.WriteValue(productYad2.Phone ?? "");
                    jsonWriter.WritePropertyName(@"city_id");
                    jsonWriter.WriteValue(productYad2.CityId);
                    jsonWriter.WritePropertyName(@"price_id");
                    jsonWriter.WriteValue(productYad2.PriceId);
                    jsonWriter.WritePropertyName(@"status");
                    jsonWriter.WriteValue(Enum.GetName(typeof(StatusType), productYad2.Status).ToLower());

                    jsonWriter.WritePropertyName(@"list_category");
                    jsonWriter.WriteStartArray();

                    List<CategoryYad2> categoryYad2Col = ProductYad2Controller.GetAllCatagoriesOfProduct(product_id);
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

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                JToken jt;
                string product_name = null, description = null, contact_name = null, phone= null;
                decimal? price = null;
                Int64? product_id = null, city_id = null, price_id = null;
                string[] list_category = null;
                List<Int64> LstCategory = new List<Int64>();
                if (Request.Form["city_id"] != null) city_id =Convert.ToInt64(Request.Form["city_id"].ToString());
                if (Request.Form["product_name"] != null) product_name = Request.Form["product_name"].ToString();
                if (Request.Form["price"] != null) price = Convert.ToDecimal(Request.Form["price"].ToString());
                if (Request.Form["product_id"] != null) product_id = Convert.ToInt64(Request.Form["product_id"].ToString());
                if (Request.Form["description"] != null) description = Request.Form["description"].ToString();
                if (Request.Form["contact_name"] != null) contact_name = Request.Form["contact_name"].ToString();
                if (Request.Form["phone"] != null) phone = Request.Form["phone"].ToString();
                if (Request.Form["price_id"] != null) price_id = Convert.ToInt64(Request.Form["price_id"].ToString());

                if (Request.Form["list_category"] != null) list_category = (Request.Form["list_category"]).ToString().Split(',');
                foreach (string item in list_category) { LstCategory.Add(Convert.ToInt64(item)); }
                
                //foreach (JObject obj in list_category)
                //{
                //    Int64 category_id = 0;
                //    if (obj.TryGetValue(@"category_id", out jt)) category_id = jt.Value<Int64>();
                //    LstCategory.Add(category_id);
                //}
                
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        ProductYad2 productYad2;
                        if (product_id != null)
                        {
                            productYad2 = ProductYad2.FetchByID(product_id);
                        }
                        else {
                            productYad2 = new ProductYad2();
                        }
                        Int32 w = Request.Form["w"] != null ? Convert.ToInt32(Request.Form["w"]) : 0;
                        Int32 h = Request.Form["h"] != null ? Convert.ToInt32(Request.Form["h"]) : 0;
                        string fn = null;
                        if (Request.Files.Count == 1)
                        {
                            if (!IsAcceptedImageExtension(Request.Files[0].FileName))
                            {
                                RespondUnauthorized(Response);
                            }
                            fn = MediaUtility.SaveFile(Request.Files[0], @"ProductYad2",  0);
                            if (string.IsNullOrEmpty(fn))
                            {
                                RespondInternalServerError(Response);
                            }
                        }

                        if (product_id != null && productYad2.ProductImage != "") MediaUtility.DeleteImageFilePath(@"ProductYad2", productYad2.ProductImage, w, h, 0); 

                        if (product_name != null) productYad2.ProductName = product_name;
                        if (price != null) productYad2.Price = (decimal)price;
                        if (phone != null) productYad2.Phone = phone;
                        if (contact_name != null) productYad2.ContactName = contact_name;
                        if (city_id != null) productYad2.CityId = (Int64)city_id;
                        if (price_id != null) productYad2.PriceId = (Int64)price_id;
                        if (description != null) productYad2.Details = description;
                        if (fn != null && fn != "") productYad2.ProductImage = fn;
                        productYad2.UpdateDate = DateTime.UtcNow;
                        if (product_id == null) productYad2.CreateDate = DateTime.UtcNow;                       
                        productYad2.Status = StatusType.Wait;
                        productYad2.AppUserId = AppUserId;
                        productYad2.Save();

                        if (LstCategory.Count > 0)
                        {
                            new Query(ProductYad2Category.TableSchema).Where(ProductYad2Category.Columns.ProductId, productYad2.ProductId).Delete().Execute();

                            foreach (Int64 item in LstCategory)
                            {
                                ProductYad2Category p = new ProductYad2Category();
                                p.ProductId = productYad2.ProductId;
                                p.CategoryYad2Id = item;
                                p.Save();
                            }

                        }

                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"product_id");
                        jsonWriter.WriteValue(productYad2.ProductId);

                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }

        public override void Delete(HttpRequest Request, HttpResponse Response, params string[] PathParams)
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
                JToken jt;
                Int64? product_id = null;
                if (inputData.TryGetValue(@"product_id", out jt)) product_id = jt.Value<Int64>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        new Query(ProductYad2Category.TableSchema).Where(ProductYad2Category.Columns.ProductId, product_id).Delete().Execute();
                        Query.New<ProductYad2>().Where(ProductYad2.Columns.ProductId, product_id).Delete().Execute();

                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }
    }
}
