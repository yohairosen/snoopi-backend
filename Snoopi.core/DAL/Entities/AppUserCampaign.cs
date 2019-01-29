using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * AppUserCampaign
 * AppUserCampaign
 * AppUserCampaignId:       PRIMARY KEY; INT64; AUTOINCREMENT;
 * AppUserId:               INT64;
 * CampaignId:              INT64;
 * */

namespace Snoopi.core.DAL
{
    public partial class AppUserCampaignCollection : AbstractRecordList<AppUserCampaign, AppUserCampaignCollection>
    {
    }

    public partial class AppUserCampaign : AbstractRecord<AppUserCampaign>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AppUserCampaignId = "AppUserCampaignId";
            public static string AppUserId = "AppUserId";
            public static string CampaignId = "CampaignId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppUserCampaign";
                schema.AddColumn(Columns.AppUserCampaignId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CampaignId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AppUserCampaignId = 0;
        internal Int64 _AppUserId = 0;
        internal Int64 _CampaignId = 0;
        #endregion

        #region Properties
        public Int64 AppUserCampaignId
        {
            get { return _AppUserCampaignId; }
            set { _AppUserCampaignId = value; }
        }
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public Int64 CampaignId
        {
            get { return _CampaignId; }
            set { _CampaignId = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return AppUserCampaignId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.CampaignId, CampaignId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AppUserCampaignId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.CampaignId, CampaignId);
            qry.Where(Columns.AppUserCampaignId, AppUserCampaignId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AppUserCampaignId = Convert.ToInt64(reader[Columns.AppUserCampaignId]);
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            CampaignId = Convert.ToInt64(reader[Columns.CampaignId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppUserCampaign FetchByID(Int64 AppUserCampaignId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserCampaignId, AppUserCampaignId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserCampaign item = new AppUserCampaign();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AppUserCampaignId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserCampaignId, AppUserCampaignId);
            return qry.Execute();
        }
        public static AppUserCampaign FetchByID(ConnectorBase conn, Int64 AppUserCampaignId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserCampaignId, AppUserCampaignId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppUserCampaign item = new AppUserCampaign();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AppUserCampaignId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserCampaignId, AppUserCampaignId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class AppUserCampaign
    {

        public static Int64 FetchByCampaignIdAppUserId(Int64 AppUserId, Int64 CampaignId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CampaignId, CampaignId)
                .AddWhere(Columns.AppUserId, AppUserId).OrderBy(Columns.AppUserCampaignId, SortDirection.DESC);
            return qry.GetCount();
        }
    
    }
}
