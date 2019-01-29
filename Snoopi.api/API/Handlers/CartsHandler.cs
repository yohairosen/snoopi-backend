using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Snoopi.api
{
    class CartsHandler : ApiHandlerBase
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
                   
            JToken jt;
           Int64 AppUserId;
            IsAuthorizedRequest(Request, Response, false, out AppUserId);

            Int64 TempAppUserId = 0;
            if (inputData.TryGetValue(@"temp_app_user_id", out jt)) TempAppUserId = jt.Value<Int64>();
            if (AppUserId == 0 && TempAppUserId == 0)
            {
                RespondError(Response, HttpStatusCode.Forbidden, @"authorization-error");
                return;
            }
            var user = AppUser.FetchByID(AppUserId);
            var tempUser = TempAppUser.FetchByID(TempAppUserId);
            if (user == null && tempUser == null)
            {
                RespondError(Response, HttpStatusCode.Forbidden, @"authorization-error");
                return;
            }

            long cityId = 0;

            if (user != null)
            {
                bool _locked = user.IsLocked;
                if (_locked)
                {
                    RespondError(Response, HttpStatusCode.BadRequest, @"appuser-locked");
                    return;
                }
                cityId = user.CityId;
            }
            else if (tempUser != null)
                cityId = tempUser.CityId;

                Response.ContentType = @"application/json";

                try
                {
                    //Int64 order_id = 0;
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

                    int cartId = CartController.CreateCart(AppUserId, TempAppUserId, supplierId, totalPrice, lstProduct);
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"cart_id");
                            jsonWriter.WriteValue(cartId);
                            jsonWriter.WriteEndObject();

                        }
                    }
                }

                catch (Exception)
                {
                    RespondError(Response, HttpStatusCode.InternalServerError, @"db-error");
                }
            }
    }
}
