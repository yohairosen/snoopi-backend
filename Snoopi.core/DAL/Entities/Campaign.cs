using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Campaign
 * Campaign
 * CampaignId:              PRIMARY KEY; INT64; AUTOINCREMENT;
 * IsDiscount:              BOOL; DEFAULT false;
 * PrecentDiscount:         INT32; DEFAULT 0;
 * IsGift:                  BOOL; DEFAULT false;
 * CampaignName:            FIXEDSTRING(255); string.Empty;
 * Remarks:                 FIXEDSTRING(255); string.Empty;
 * StartDate:               DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * EndDate:                 DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * DestinationCount:        INT32; DEFAULT 0;
 * DestinationSum:          DECIMAL; DEFAULT 0;
 * ImplemationCount:        INT32; DEFAULT 0;
 * @INDEX:                  NAME(ix_Campaign_CampaignId);[CampaignId];
 * */

namespace Snoopi.core.DAL
{

    public partial class CampaignCollection : AbstractRecordList<Campaign, CampaignCollection>
    {
    }

    public partial class Campaign : AbstractRecord<Campaign>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CampaignId = "CampaignId";
            public static string IsDiscount = "IsDiscount";
            public static string PrecentDiscount = "PrecentDiscount";
            public static string IsGift = "IsGift";
            public static string CampaignName = "CampaignName";
            public static string Remarks = "Remarks";
            public static string StartDate = "StartDate";
            public static string EndDate = "EndDate";
            public static string DestinationCount = "DestinationCount";
            public static string DestinationSum = "DestinationSum";
            public static string ImplemationCount = "ImplemationCount";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Campaign";
                schema.AddColumn(Columns.CampaignId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.IsDiscount, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.PrecentDiscount, typeof(Int32), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.IsGift, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.CampaignName, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Remarks, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.StartDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.EndDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.DestinationCount, typeof(Int32), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.DestinationSum, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.ImplemationCount, typeof(Int32), 0, 0, 0, false, false, false, 0);

                _TableSchema = schema;

                schema.AddIndex("ix_Campaign_CampaignId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CampaignId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CampaignId = 0;
        internal bool _IsDiscount = false;
        internal Int32 _PrecentDiscount = 0;
        internal bool _IsGift = false;
        internal string _CampaignName = string.Empty;
        internal string _Remarks = string.Empty;
        internal DateTime _StartDate = DateTime.UtcNow;
        internal DateTime _EndDate = DateTime.UtcNow;
        internal Int32 _DestinationCount = 0;
        internal decimal _DestinationSum = 0;
        internal Int32 _ImplemationCount = 0;
        #endregion

        #region Properties
        public Int64 CampaignId
        {
            get { return _CampaignId; }
            set { _CampaignId = value; }
        }
        public bool IsDiscount
        {
            get { return _IsDiscount; }
            set { _IsDiscount = value; }
        }
        public Int32 PrecentDiscount
        {
            get { return _PrecentDiscount; }
            set { _PrecentDiscount = value; }
        }
        public bool IsGift
        {
            get { return _IsGift; }
            set { _IsGift = value; }
        }
        public string CampaignName
        {
            get { return _CampaignName; }
            set { _CampaignName = value; }
        }
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
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
        public Int32 DestinationCount
        {
            get { return _DestinationCount; }
            set { _DestinationCount = value; }
        }
        public decimal DestinationSum
        {
            get { return _DestinationSum; }
            set { _DestinationSum = value; }
        }
        public Int32 ImplemationCount
        {
            get { return _ImplemationCount; }
            set { _ImplemationCount = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return CampaignId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.IsDiscount, IsDiscount);
            qry.Insert(Columns.PrecentDiscount, PrecentDiscount);
            qry.Insert(Columns.IsGift, IsGift);
            qry.Insert(Columns.CampaignName, CampaignName);
            qry.Insert(Columns.Remarks, Remarks);
            qry.Insert(Columns.StartDate, StartDate);
            qry.Insert(Columns.EndDate, EndDate);
            qry.Insert(Columns.DestinationCount, DestinationCount);
            qry.Insert(Columns.DestinationSum, DestinationSum);
            qry.Insert(Columns.ImplemationCount, ImplemationCount);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CampaignId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.IsDiscount, IsDiscount);
            qry.Update(Columns.PrecentDiscount, PrecentDiscount);
            qry.Update(Columns.IsGift, IsGift);
            qry.Update(Columns.CampaignName, CampaignName);
            qry.Update(Columns.Remarks, Remarks);
            qry.Update(Columns.StartDate, StartDate);
            qry.Update(Columns.EndDate, EndDate);
            qry.Update(Columns.DestinationCount, DestinationCount);
            qry.Update(Columns.DestinationSum, DestinationSum);
            qry.Update(Columns.ImplemationCount, ImplemationCount);
            qry.Where(Columns.CampaignId, CampaignId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CampaignId = Convert.ToInt64(reader[Columns.CampaignId]);
            IsDiscount = Convert.ToBoolean(reader[Columns.IsDiscount]);
            PrecentDiscount = Convert.ToInt32(reader[Columns.PrecentDiscount]);
            IsGift = Convert.ToBoolean(reader[Columns.IsGift]);
            CampaignName = (string)reader[Columns.CampaignName];
            Remarks = (string)reader[Columns.Remarks];
            StartDate = Convert.ToDateTime(reader[Columns.StartDate]);
            EndDate = Convert.ToDateTime(reader[Columns.EndDate]);
            DestinationCount = Convert.ToInt32(reader[Columns.DestinationCount]);
            DestinationSum = Convert.ToDecimal(reader[Columns.DestinationSum]);
            ImplemationCount = Convert.ToInt32(reader[Columns.ImplemationCount]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Campaign FetchByID(Int64 CampaignId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CampaignId, CampaignId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Campaign item = new Campaign();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CampaignId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CampaignId, CampaignId);
            return qry.Execute();
        }
        public static Campaign FetchByID(ConnectorBase conn, Int64 CampaignId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CampaignId, CampaignId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Campaign item = new Campaign();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CampaignId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CampaignId, CampaignId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
