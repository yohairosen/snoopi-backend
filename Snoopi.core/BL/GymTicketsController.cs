using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using AnyGym.core.DAL;
using System.Text.RegularExpressions;
using dg.Sql.Connector;

namespace AnyGym.core.BL
{
    public static class GymTicketsController
    {
        public static Int64 GetGymsAvailableTicketsCount(Int32 GymId)
        {
            Query qry = Query.New<GymTicket>()
                .Where(GymTicket.Columns.GymId, GymId)
                .AND(GymTicket.Columns.Status, GymTicketStatus.Available);
            return qry.GetCount(GymTicket.Columns.GymTicketId);
        }
        public static Int64 GetAccountsAvailableTicketsCount(Int32 GymAccountId)
        {
            Query qry = Query.New<GymTicket>()
                .Join(JoinType.InnerJoin, Gym.TableSchema, Gym.TableSchema.SchemaName,
                    new JoinColumnPair(GymTicket.TableSchema, GymTicket.Columns.GymId, Gym.Columns.GymId))
                .Where(Gym.Columns.GymAccountId, GymAccountId)
                .AND(GymTicket.Columns.Status, GymTicketStatus.Available);
            return qry.GetCount(GymTicket.Columns.GymTicketId);
        }
        public static Int64 GetTotalAvailableTicketsCount()
        {
            Query qry = Query.New<GymTicket>()
                .Where(GymTicket.Columns.Status, GymTicketStatus.Available);
            return qry.GetCount(GymTicket.Columns.GymTicketId);
        }
        public static string CleanupBarcode(string Barcode)
        {
            return (new Regex(@"[^0-9A-Za-z]")).Replace(Barcode, @"")
                .Replace(@"o", @"0").Replace(@"O", @"0")
                .ToUpper();
        }
        public static bool RedeemTicket(Int32 GymAccountId, Int32 GymId, string Barcode)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Barcode = CleanupBarcode(Barcode);
                Query qry = Query.New<GymTicket>()
                    .Update(GymTicket.Columns.Status, GymTicketStatus.Redeemed)
                    .Update(GymTicket.Columns.RedeemDate, DateTime.UtcNow)
                    .Where(GymTicket.TableSchema.SchemaName, GymTicket.Columns.Barcode, dg.Sql.WhereComparision.EqualsTo, Barcode)
                    .AND(GymTicket.TableSchema.SchemaName, GymTicket.Columns.Status, dg.Sql.WhereComparision.EqualsTo, GymTicketStatus.Available);

                if (GymAccountId > 0)
                {
                    qry.Join(JoinType.InnerJoin, Gym.TableSchema, Gym.TableSchema.SchemaName,
                        new JoinColumnPair(GymTicket.TableSchema, GymTicket.Columns.GymId, Gym.Columns.GymId))
                        .AND(Gym.TableSchema.SchemaName, Gym.Columns.GymAccountId, dg.Sql.WhereComparision.EqualsTo, GymAccountId);
                }
                if (GymId > 0)
                {
                    qry.AND(GymTicket.TableSchema.SchemaName, GymTicket.Columns.GymId, dg.Sql.WhereComparision.EqualsTo, GymId);
                }

                bool result = qry.Execute(conn) == 1;

                try
                {
                    Int64 AppUserId = 0;
                    using (DataReaderBase reader =
                            Query.New<GymTicket>()
                                .Select(GymTicket.Columns.GymId)
                                .AddSelect(GymTicket.Columns.AppUserId)
                                .Where(GymTicket.Columns.Barcode, Barcode)
                                .LimitRows(1)
                                .ExecuteReader(conn))
                    {
                        if (reader.Read())
                        {
                            GymId = reader.GetInt32(0);
                            AppUserId = reader.GetInt64(1);
                        }
                    }

                    if (AppUserId > 0)
                    {
                        Int64 GymCheckinId;
                        GymCheckinController.CheckinPointsAction[] PointsGenerated;
                        GymCheckinController.Checkin(conn, AppUserId, GymId, Barcode, null, out GymCheckinId, out PointsGenerated);
                    }
                }
                catch { }

                return result;
            }
        }
        public static bool RedeemTicket(Int64 GymTicketId)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<GymTicket>()
                    .Update(GymTicket.Columns.Status, GymTicketStatus.Redeemed)
                    .Update(GymTicket.Columns.RedeemDate, DateTime.UtcNow)
                    .Where(GymTicket.TableSchema.SchemaName, GymTicket.Columns.GymTicketId, dg.Sql.WhereComparision.EqualsTo, GymTicketId)
                    .AND(GymTicket.TableSchema.SchemaName, GymTicket.Columns.Status, dg.Sql.WhereComparision.EqualsTo, GymTicketStatus.Available);

                bool result = qry.Execute(conn) == 1;

                try
                {
                    Int64 AppUserId = 0;
                    Int32 GymId = 0;
                    using (DataReaderBase reader =
                            Query.New<GymTicket>()
                                .Select(GymTicket.Columns.GymId)
                                .AddSelect(GymTicket.Columns.AppUserId)
                                .Where(GymTicket.Columns.GymTicketId, GymTicketId)
                                .LimitRows(1)
                                .ExecuteReader(conn))
                    {
                        if (reader.Read())
                        {
                            GymId = reader.GetInt32(0);
                            AppUserId = reader.GetInt64(1);
                        }
                    }

                    if (AppUserId > 0)
                    {
                        Int64 GymCheckinId;
                        GymCheckinController.CheckinPointsAction[] PointsGenerated;
                        GymCheckinController.Checkin(conn, AppUserId, GymId, null, GymTicketId, out GymCheckinId, out PointsGenerated);
                    }
                }
                catch { }

                return result;
            }
        }
        public static bool RefundTicket(Int64 GymTicketId)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<GymTicket>()
                    .Update(GymTicket.Columns.Status, GymTicketStatus.Refunded)
                    .Update(GymTicket.Columns.RedeemDate, DateTime.UtcNow)
                    .Where(GymTicket.Columns.GymTicketId, GymTicketId)
                    .AND(GymTicket.Columns.Status, dg.Sql.WhereComparision.NotEqualsTo, GymTicketStatus.Refunded);
                
                bool result = qry.Execute(conn) == 1;

                if (result)
                {
                    decimal Amount = 0m;
                    Int64 AppUserId = 0;
                    using (DataReaderBase reader =
                            Query.New<GymTicket>()
                                .Select(GymTicket.Columns.Amount)
                                .AddSelect(GymTicket.Columns.AppUserId)
                                .Where(GymTicket.Columns.GymTicketId, GymTicketId)
                                .LimitRows(1)
                                .ExecuteReader(conn))
                    {
                        if (reader.Read())
                        {
                            Amount = reader.GetDecimal(0);
                            AppUserId = reader.GetInt64(1);
                        }
                    }

                    if (AppUserId > 0)
                    {
                        AppUserCreditController.AddCreditToAppUser(AppUserId, Amount);
                    }
                }

                return result;
            }
        }
        public static GymTicket TicketByBarcode(Int32 GymAccountId, Int32 GymId, string Barcode)
        {
            Barcode = CleanupBarcode(Barcode);
            Query qry = Query.New<GymTicket>()
                .Where(GymTicket.TableSchema.SchemaName,GymTicket.Columns.Barcode,dg.Sql.WhereComparision.EqualsTo, Barcode)
                .AND(GymTicket.TableSchema.SchemaName,GymTicket.Columns.Status, dg.Sql.WhereComparision.EqualsTo,GymTicketStatus.Available);

            if (GymAccountId > 0)
            {
                qry.Join(JoinType.InnerJoin, Gym.TableSchema, Gym.TableSchema.SchemaName, 
                    new JoinColumnPair(GymTicket.TableSchema, GymTicket.Columns.GymId, Gym.Columns.GymId))
                    .AND(Gym.TableSchema.SchemaName, Gym.Columns.GymAccountId, dg.Sql.WhereComparision.EqualsTo, GymAccountId);
            }
            if (GymId > 0)
            {
                qry.Where(GymTicket.TableSchema.SchemaName, GymTicket.Columns.GymId, dg.Sql.WhereComparision.EqualsTo, GymId);
            }

            GymTicketCollection coll = GymTicketCollection.FetchByQuery(qry);
            if (coll.Count == 1) return coll[0];
            return null;
        }
    }
}
