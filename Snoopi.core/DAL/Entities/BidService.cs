using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * BidService
 * BidService
 * BidId:             PRIMARY KEY; INT64; AUTOINCREMENT;
 * StartDate:         DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * EndDate:           DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * AppUserId:         INT64; NULLABLE; 
 * TempAppUserId:     INT64; NULLABLE; 
 * ServiceId:         INT64; 
 * IsSendOffer:       BOOL; DEFAULT false;
 * ServiceComment:    FIXEDSTRING(255); DEFAULT string.Empty;    
 * @INDEX:            NAME(ix_Bid);[BidId];
 * @FOREIGNKEY:       NAME(fk_BidService_ServiceId); FOREIGNTABLE(Service); COLUMNS[ServiceId]; FOREIGNCOLUMNS[ServiceId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class BidServiceCollection : AbstractRecordList<BidService, BidServiceCollection>
    {
    }

    public partial class BidService : AbstractRecord<BidService>
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
            public static string ServiceId = "ServiceId";
            public static string IsSendOffer = "IsSendOffer";
            public static string ServiceComment = "ServiceComment";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"BidService";
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.StartDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.EndDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.TempAppUserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ServiceId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.IsSendOffer, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.ServiceComment, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);

                _TableSchema = schema;

                schema.AddIndex("ix_Bid", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.BidId);

                schema.AddForeignKey("fk_BidService_ServiceId", BidService.Columns.ServiceId, Service.TableSchema.SchemaName, Service.Columns.ServiceId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

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
        internal Int64 _ServiceId = 0;
        internal bool _IsSendOffer = false;
        internal string _ServiceComment = string.Empty;
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
        public Int64 ServiceId
        {
            get { return _ServiceId; }
            set { _ServiceId = value; }
        }
        public bool IsSendOffer
        {
            get { return _IsSendOffer; }
            set { _IsSendOffer = value; }
        }
        public string ServiceComment
        {
            get { return _ServiceComment; }
            set { _ServiceComment = value; }
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
            qry.Insert(Columns.ServiceId, ServiceId);
            qry.Insert(Columns.IsSendOffer, IsSendOffer);
            qry.Insert(Columns.ServiceComment, ServiceComment);

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
            qry.Update(Columns.ServiceId, ServiceId);
            qry.Update(Columns.IsSendOffer, IsSendOffer);
            qry.Update(Columns.ServiceComment, ServiceComment);
            qry.Where(Columns.BidId, BidId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            StartDate = Convert.ToDateTime(reader[Columns.StartDate]);
            EndDate = Convert.ToDateTime(reader[Columns.EndDate]);
            AppUserId = IsNull(reader[Columns.AppUserId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.AppUserId]);
            TempAppUserId = IsNull(reader[Columns.TempAppUserId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.TempAppUserId]);
            ServiceId = Convert.ToInt64(reader[Columns.ServiceId]);
            IsSendOffer = Convert.ToBoolean(reader[Columns.IsSendOffer]);
            ServiceComment = (string)reader[Columns.ServiceComment];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static BidService FetchByID(Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.BidId, BidId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    BidService item = new BidService();
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
        public static BidService FetchByID(ConnectorBase conn, Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.BidId, BidId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    BidService item = new BidService();
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

    public partial class BidService
    {
        public static BidService FetchByAppUserId(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId)
                .AddWhere(Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow).OrderBy(Columns.EndDate, SortDirection.DESC);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    BidService item = new BidService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static BidService FetchByTempAppUserId(Int64 TempAppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.TempAppUserId, TempAppUserId)
                .AddWhere(Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow).OrderBy(Columns.EndDate, SortDirection.DESC);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    BidService item = new BidService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

    }


}
