using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using AnyGym.core.DAL;
using System.Text.RegularExpressions;
using dg.Sql.Connector;
using dg.Utilities;
using System.Collections;

namespace AnyGym.core.BL
{
    public static class GymCheckinController
    {
        public static bool Checkin(
            ConnectorBase conn, 
            Int64 AppUserId, Int32 GymId, 
            decimal Latitude, decimal Longitude, 
            out Int64 GymCheckinId,
            out CheckinPointsAction[] PointsGenerated)
        {
            GymCheckinId = 0;
            PointsGenerated = null;

            bool Exists = Query.New<GymCheckin>()
                .Where(GymCheckin.Columns.GymId, GymId)
                .AND(GymCheckin.Columns.AppUserId, AppUserId)
                .AND(GymCheckin.Columns.CheckinDate, dg.Sql.WhereComparision.GreaterThanOrEqual, DateTime.UtcNow.AddMinutes(-Settings.GetSysSettingInt32(Settings.SysSettings.CHECKIN_LIMIT_MINUTES, 50)))
                .GetCount(GymCheckin.Columns.GymCheckinId, conn) > 0L;

            if (Exists)
            {
                return false;
            }
            else
            {
                Query qryInsert = Query.New<GymCheckin>()
                    .Insert(GymCheckin.Columns.GymId, GymId)
                    .Insert(GymCheckin.Columns.AppUserId, AppUserId)
                    .Insert(GymCheckin.Columns.CheckinDate, DateTime.UtcNow)
                    .Insert(GymCheckin.Columns.Latitude, Latitude)
                    .Insert(GymCheckin.Columns.Longitude, Longitude);
                object lastInsert = null;
                qryInsert.Execute(conn, out lastInsert);
                if (!lastInsert.IsNull())
                {
                    GymCheckinId = Convert.ToInt64(lastInsert);
                    GeneratePointsForLastCheckin(conn, AppUserId, out PointsGenerated);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool Checkin(
            ConnectorBase conn,
            Int64 AppUserId, Int32 GymId, string Barcode, Int64? GymTicketId,
            out Int64 GymCheckinId,
            out CheckinPointsAction[] PointsGenerated)
        {
            GymCheckinId = 0;
            PointsGenerated = null;

            bool Exists = Query.New<GymCheckin>()
                .Where(GymCheckin.Columns.GymId, GymId)
                .AND(GymCheckin.Columns.AppUserId, AppUserId)
                .AND(GymCheckin.Columns.CheckinDate, dg.Sql.WhereComparision.GreaterThanOrEqual, DateTime.UtcNow.AddMinutes(-Settings.GetSysSettingInt32(Settings.SysSettings.CHECKIN_LIMIT_MINUTES, 50)))
                .GetCount(GymCheckin.Columns.GymCheckinId, conn) > 0L;

            if (Exists)
            {
                return false;
            }
            else
            {
                Query qrySelect = Query.New<GymTicket>()
                    .Join(JoinType.InnerJoin, Gym.TableSchema, Gym.TableSchema.SchemaName,
                          new JoinColumnPair(GymTicket.TableSchema, GymTicket.Columns.GymId, Gym.Columns.GymId))
                    .Select(GymTicket.TableSchema.SchemaName, GymTicket.Columns.GymId, null, true)
                    .AddSelect(GymTicket.TableSchema.SchemaName, GymTicket.Columns.AppUserId, null)
                    .AddSelectValue(DateTime.UtcNow, @"UtcNow")
                    .AddSelect(Gym.TableSchema.SchemaName, Gym.Columns.AddressLatitude, null)
                    .AddSelect(Gym.TableSchema.SchemaName, Gym.Columns.AddressLongitude, null);

                if (GymTicketId != null)
                {
                    qrySelect.Where(GymTicket.TableSchema.SchemaName, GymTicket.Columns.GymTicketId, WhereComparision.EqualsTo, GymTicketId);
                }
                else
                {
                    qrySelect.Where(GymTicket.TableSchema.SchemaName, GymTicket.Columns.Barcode, WhereComparision.EqualsTo, Barcode);
                }

                Query qryInsert = Query.New<GymCheckin>()
                    .Insert(GymCheckin.Columns.GymId, 0)
                    .Insert(GymCheckin.Columns.AppUserId, 0)
                    .Insert(GymCheckin.Columns.CheckinDate, 0)
                    .Insert(GymCheckin.Columns.Latitude, 0)
                    .Insert(GymCheckin.Columns.Longitude, 0)
                    .SetInsertExpression(qrySelect);
                object lastInsert = null;
                qryInsert.Execute(conn, out lastInsert);
                if (!lastInsert.IsNull())
                {
                    GymCheckinId = Convert.ToInt64(lastInsert);
                    GeneratePointsForLastCheckin(conn, AppUserId, out PointsGenerated);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void GeneratePointsForLastCheckin(
            ConnectorBase conn,
            Int64 AppUserId,
            out CheckinPointsAction[] PointsGenerated)
        {
            object checkinsRes = Query.New<AppUserBalance>()
                .Select(AppUserBalance.Columns.Checkins)
                .Where(AppUserBalance.Columns.AppUserId, AppUserId)
                .ExecuteScalar();
            if (checkinsRes.IsNull())
            {
                PointsGenerated = null;
                return;
            }
            else
            {
                string pointsKey = null;
                AppUserPointsReason pointsReason = AppUserPointsReason.Unknown;
                int checkins = Convert.ToInt32(checkinsRes);
                switch (checkins)
                {
                    case 10: pointsKey = Settings.PointsSettings.CHECKIN_10th; pointsReason = AppUserPointsReason.Checkin10th; break;
                    case 25: pointsKey = Settings.PointsSettings.CHECKIN_25th; pointsReason = AppUserPointsReason.Checkin25th; break;
                    case 50: pointsKey = Settings.PointsSettings.CHECKIN_50th; pointsReason = AppUserPointsReason.Checkin50th; break;
                    case 75: pointsKey = Settings.PointsSettings.CHECKIN_75th; pointsReason = AppUserPointsReason.Checkin75th; break;
                    case 100: pointsKey = Settings.PointsSettings.CHECKIN_100th; pointsReason = AppUserPointsReason.Checkin100th; break;
                    case 250: pointsKey = Settings.PointsSettings.CHECKIN_250th; pointsReason = AppUserPointsReason.Checkin250th; break;
                    case 500: pointsKey = Settings.PointsSettings.CHECKIN_500th; pointsReason = AppUserPointsReason.Checkin500th; break;
                    case 750: pointsKey = Settings.PointsSettings.CHECKIN_750th; pointsReason = AppUserPointsReason.Checkin750th; break;
                    case 1000: pointsKey = Settings.PointsSettings.CHECKIN_1000th; pointsReason = AppUserPointsReason.Checkin1000th; break;
                }

                List<CheckinPointsAction> pointsList = new List<CheckinPointsAction>();
                pointsList.Add(new CheckinPointsAction(Settings.GetPoints(Settings.PointsSettings.CHECKIN), AppUserPointsReason.Checkin));
                if (pointsKey != null)
                {
                    pointsList.Add(new CheckinPointsAction(Settings.GetPoints(pointsKey), pointsReason));
                }
                pointsList.RemoveAll((CheckinPointsAction action) => { return action.Points == 0; });
                foreach (CheckinPointsAction action in pointsList)
                {
                    Query.New<AppUserPoints>()
                    .Insert(AppUserPoints.Columns.AppUserId, AppUserId)
                    .Insert(AppUserPoints.Columns.Date, DateTime.UtcNow)
                    .Insert(AppUserPoints.Columns.Points, action.Points)
                    .Insert(AppUserPoints.Columns.Reason, action.Reason)
                    .Execute();
                }

                PointsGenerated = pointsList.ToArray();
            }
        }
        public struct CheckinPointsAction
        {
            public CheckinPointsAction(Int64 Points, AppUserPointsReason Reason)
            {
                this.Points=Points;
                this.Reason=Reason;
            }
            public Int64 Points;
            public AppUserPointsReason Reason;
        }
    }
}
