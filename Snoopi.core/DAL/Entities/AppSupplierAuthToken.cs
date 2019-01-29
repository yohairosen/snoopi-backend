using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppSupplierAuthToken
 * AppSupplierAuthToken
 * AppSupplierAuthTokenId:  PRIMARY KEY; INT64; AUTOINCREMENT;
 * SupplierId:              INT64; The parent AppSupplier
 * Secret:                  GUID; Random secret
 * Key:                     FIXEDSTRING(128); Key - contains SupplierId data
 * CreatedDate:             DATETIME; DEFAULT DateTime.MinValue; Expiry time
 * Expiry:                  DATETIME; DEFAULT DateTime.MinValue; Expiry time
 * @INDEX:                  NAME(ix_AppSupplierAuthToken_SecretKey); [Secret,Key]; UNIQUE; 
 * @FOREIGNKEY:             NAME(fk_AppSupplierAuthToken_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_AppSupplierAuthToken_SupplierId); [SupplierId];
 * @INDEX:                  NAME(ix_AppSupplierAuthToken_Expiry); [Expiry ASC];
 * @INDEX:                  NAME(ix_AppSupplierAuthToken_CreatedDate); [CreatedDate ASC];
 * */

namespace Snoopi.core.DAL
{
    public partial class AppSupplierAuthTokenCollection : AbstractRecordList<AppSupplierAuthToken, AppSupplierAuthTokenCollection>
    {
    }

    public partial class AppSupplierAuthToken : AbstractRecord<AppSupplierAuthToken>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AppSupplierAuthTokenId = "AppSupplierAuthTokenId";
            public static string SupplierId = "SupplierId"; // The parent AppSupplier
            public static string Secret = "Secret"; // Random secret
            public static string Key = "Key"; // Key - contains SupplierId data
            public static string CreatedDate = "CreatedDate"; // Expiry time
            public static string Expiry = "Expiry"; // Expiry time
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppSupplierAuthToken";
                schema.AddColumn(Columns.AppSupplierAuthTokenId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Secret, typeof(Guid), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Key, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreatedDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.Expiry, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ix_AppSupplierAuthToken_SecretKey", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.Secret, Columns.Key);
                schema.AddIndex("ix_AppSupplierAuthToken_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_AppSupplierAuthToken_Expiry", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.Expiry, SortDirection.ASC);
                schema.AddIndex("ix_AppSupplierAuthToken_CreatedDate", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CreatedDate, SortDirection.ASC);

                schema.AddForeignKey("fk_AppSupplierAuthToken_SupplierId", AppSupplierAuthToken.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AppSupplierAuthTokenId = 0;
        internal Int64 _SupplierId = 0;
        internal Guid _Secret = Guid.Empty;
        internal string _Key = string.Empty;
        internal DateTime _CreatedDate = DateTime.MinValue;
        internal DateTime _Expiry = DateTime.MinValue;
        #endregion

        #region Properties
        public Int64 AppSupplierAuthTokenId
        {
            get { return _AppSupplierAuthTokenId; }
            set { _AppSupplierAuthTokenId = value; }
        }
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
        }
        public Guid Secret
        {
            get { return _Secret; }
            set { _Secret = value; }
        }
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        public DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
        public DateTime Expiry
        {
            get { return _Expiry; }
            set { _Expiry = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return AppSupplierAuthTokenId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.Secret, Secret);
            qry.Insert(Columns.Key, Key);
            qry.Insert(Columns.CreatedDate, CreatedDate);
            qry.Insert(Columns.Expiry, Expiry);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AppSupplierAuthTokenId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.Secret, Secret);
            qry.Update(Columns.Key, Key);
            qry.Update(Columns.CreatedDate, CreatedDate);
            qry.Update(Columns.Expiry, Expiry);
            qry.Where(Columns.AppSupplierAuthTokenId, AppSupplierAuthTokenId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AppSupplierAuthTokenId = Convert.ToInt64(reader[Columns.AppSupplierAuthTokenId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            Secret = GuidFromDb(reader[Columns.Secret]);
            Key = (string)reader[Columns.Key];
            CreatedDate = Convert.ToDateTime(reader[Columns.CreatedDate]);
            Expiry = Convert.ToDateTime(reader[Columns.Expiry]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppSupplierAuthToken FetchByID(Int64 AppSupplierAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppSupplierAuthTokenId, AppSupplierAuthTokenId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppSupplierAuthToken item = new AppSupplierAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AppSupplierAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppSupplierAuthTokenId, AppSupplierAuthTokenId);
            return qry.Execute();
        }
        public static AppSupplierAuthToken FetchByID(ConnectorBase conn, Int64 AppSupplierAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppSupplierAuthTokenId, AppSupplierAuthTokenId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppSupplierAuthToken item = new AppSupplierAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AppSupplierAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppSupplierAuthTokenId, AppSupplierAuthTokenId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class AppSupplierAuthToken
    {

        public static AppSupplierAuthToken FetchByAppSupplierID(Int64 AppSupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, AppSupplierId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppSupplierAuthToken item = new AppSupplierAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}
