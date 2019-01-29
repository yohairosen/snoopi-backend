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
    public class StartPageHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            string Time = "";
            Int64 BidId = 0;
            bool IsService = false;

            IsAuthorizedRequest(Request, Response, false, out AppUserId);

            Int64 TempAppUserId = (Request.QueryString["temp_app_user_id"] != null ? Int64.Parse(Request.QueryString["temp_app_user_id"].ToString()) : 0);

            if (AppUserId != 0 || TempAppUserId != 0)
            {

                Query qry = new Query(Bid.TableSchema);
                if (AppUserId != 0)
                {
                    qry = new Query(Bid.TableSchema);
                    qry.Where(Bid.Columns.AppUserId, AppUserId);
                    qry.OrderBy(Bid.Columns.EndDate, SortDirection.DESC);
                    qry.LimitRows(1);
                }
                else if (TempAppUserId != 0)
                {
                    qry = new Query(Bid.TableSchema);
                    qry.Where(Bid.Columns.TempAppUserId, TempAppUserId);
                    qry.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThan, DateTime.UtcNow);
                }

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader[Bid.Columns.EndDate] != null)
                        {

                            object Helper = (reader[Bid.Columns.EndDate] != null ? Convert.ToDateTime(reader[Bid.Columns.EndDate]) : DateTime.MinValue);
                            TimeSpan diff = ((DateTime)Helper - DateTime.UtcNow);
                            if (reader[Bid.Columns.EndDate] != null)
                            {
                                string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                                string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                                Time = hours + ":" + minutes;
                            }
                            else
                                Time = "00:00";
                            //Time = Math.Truncate(diff.TotalHours).ToString() + ":" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                            BidId = (reader[Bid.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0);
                            IsService = false;
                        }
                    }
                }

                if (BidId == 0)
                {
                    if (AppUserId != 0)
                    {
                        qry = new Query(BidService.TableSchema);
                        qry.Where(BidService.Columns.AppUserId, AppUserId);
                        qry.AddWhere(BidService.Columns.EndDate, WhereComparision.GreaterThan, DateTime.UtcNow);//.Select(Bid.Columns.EndDate);
                    }
                    if (TempAppUserId != 0)
                    {
                        qry = new Query(BidService.TableSchema);
                        qry.Where(BidService.Columns.TempAppUserId, TempAppUserId);
                        qry.AddWhere(BidService.Columns.EndDate, WhereComparision.GreaterThan, DateTime.UtcNow);//.Select(Bid.Columns.EndDate);
                    }

                    using (DataReaderBase reader = qry.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader[BidService.Columns.EndDate] != null)
                            {

                                object Helper = (reader[BidService.Columns.EndDate] != null ? Convert.ToDateTime(reader[BidService.Columns.EndDate]) : DateTime.MinValue);
                                TimeSpan diff = ((DateTime)Helper - DateTime.UtcNow);
                                if (reader[Bid.Columns.EndDate] != null)
                                {
                                    string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                                    string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                                    Time = hours + ":" + minutes;
                                }
                                else
                                    Time = "00:00";
                                //Time = Math.Truncate(diff.TotalHours).ToString() + ":" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                                BidId = (reader[BidService.Columns.BidId] != null ? Convert.ToInt64(reader[BidService.Columns.BidId]) : 0);
                                IsService = true;
                            }
                        }
                    }


                }
            }

            try
            {

                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"home_banner");
                        jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.BANNER_HOME));

                        jsonWriter.WritePropertyName(@"category_banner");
                        jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.BANNER_CATEGORY));

                        jsonWriter.WritePropertyName(@"sub_category_banner");
                        jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.BANNER_SUB_CATEGORY));

                        jsonWriter.WritePropertyName(@"end_time");
                        jsonWriter.WriteValue(Time);

                        jsonWriter.WritePropertyName(@"bid_id");
                        jsonWriter.WriteValue(BidId);

                        jsonWriter.WritePropertyName(@"is_service");
                        jsonWriter.WriteValue(IsService);

                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception) { }
        }

    }
}
