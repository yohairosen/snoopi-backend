using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;
using Snoopi.infrastructure;
using Snoopi.infrastructure.Loggers;
using System.Diagnostics;
using System.Web;

/*
 * Bid
 * Bid
 * BidId:             PRIMARY KEY; INT64; AUTOINCREMENT;
 * StartDate:         DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * EndDate:           DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * AppUserId:         INT64; NULLABLE; 
 * TempAppUserId:     INT64; NULLABLE; 
 * IsSendOffer:       BOOL; DEFAULT false;
 * @INDEX:            NAME(ix_Bid);[BidId];
 * */

namespace Snoopi.core.DAL
{

    public partial class BidCollection : AbstractRecordList<Bid, BidCollection>
    {
    }

    public partial class Bid : AbstractRecord<Bid>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string BidId = "BidId";
            public static string StartDate = "StartDate";
            public static string EndDate = "EndDate";
            public static string AppUserId = "AppUserId";
            public static string TempAppUserId = "TempAppUserId";
            public static string IsSendOffer = "IsSendOffer";
            public static string IsActive = "IsActive";
            public static string CityId = "CityId";
            public static string Deleted = "Deleted";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Bid";
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.StartDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.EndDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.TempAppUserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.IsSendOffer, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsActive, typeof(bool), 0, 0, 0, false, false, false, true);
                schema.AddColumn(Columns.Deleted, typeof(DateTime), 0, 0, 0, false, false, true, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Bid", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.BidId);
                schema.AddForeignKey("fk_bid_city", Columns.CityId, City.TableSchema.SchemaName, City.Columns.CityId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _BidId = 0;
        internal DateTime _StartDate = DateTime.UtcNow;
        internal DateTime _EndDate = DateTime.UtcNow;
        internal Int64? _AppUserId = null;
        internal Int64? _TempAppUserId = null;
        internal bool _IsSendOffer = false;
        internal long _cityId = 0;
        internal bool _isActive = true;
        internal DateTime ? _deleted;

        #endregion

        #region Properties
        public Int64 BidId
        {
            get { return _BidId; }
            set { _BidId = value; }
        }
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }
        public Int64? AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public Int64? TempAppUserId
        {
            get { return _TempAppUserId; }
            set { _TempAppUserId = value; }
        }
        public bool IsSendOffer
        {
            get { return _IsSendOffer; }
            set { _IsSendOffer = value; }
        }

        public Int64 CityId
        {
            get { return _cityId; }
            set { _cityId = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        public DateTime ? Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }

        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return BidId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.StartDate, StartDate);
            qry.Insert(Columns.EndDate, EndDate);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.TempAppUserId, TempAppUserId);
            qry.Insert(Columns.IsSendOffer, IsSendOffer);
            qry.Insert(Columns.CityId, CityId);
            qry.Insert(Columns.IsActive, IsActive);
            qry.Insert(Columns.Deleted, Deleted);
            Helpers.LogState(this);
            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                BidId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.StartDate, StartDate);
            qry.Update(Columns.EndDate, EndDate);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.TempAppUserId, TempAppUserId);
            qry.Update(Columns.IsSendOffer, IsSendOffer);
            qry.Update(Columns.CityId, CityId);
            qry.Update(Columns.IsActive, IsActive);
            qry.Update(Columns.Deleted, Deleted);

            qry.Where(Columns.BidId, BidId);
            Helpers.LogState(this);
            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            StartDate = Convert.ToDateTime(reader[Columns.StartDate]);
            EndDate = Convert.ToDateTime(reader[Columns.EndDate]);
            AppUserId = IsNull(reader[Columns.AppUserId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.AppUserId]);
            TempAppUserId = IsNull(reader[Columns.TempAppUserId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.TempAppUserId]);
            IsSendOffer = Convert.ToBoolean(reader[Columns.IsSendOffer]);
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            IsActive = Convert.ToBoolean(reader[Columns.IsActive]);
            Deleted = IsNull(reader[Columns.Deleted]) ? (DateTime?)null : Convert.ToDateTime(reader[Columns.Deleted]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Bid FetchByID(Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.BidId, BidId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Bid item = new Bid();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.BidId, BidId);
            return qry.Execute();
        }
        public static Bid FetchByID(ConnectorBase conn, Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.BidId, BidId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Bid item = new Bid();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.BidId, BidId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Bid 
    {
        public static Bid FetchByAppUserId(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId)
                .AddWhere(Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow).OrderBy(Columns.EndDate, SortDirection.DESC);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Bid item = new Bid();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static Bid FetchByTempAppUserId(Int64 TempAppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.TempAppUserId, TempAppUserId)
                .AddWhere(Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow).OrderBy(Columns.EndDate, SortDirection.DESC);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Bid item = new Bid();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

       
    }
    

}
