using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppUserAuthToken
 * AppUserAuthToken
 * AppUserAuthTokenId:      PRIMARY KEY; INT64; AUTOINCREMENT;
 * AppUserId:               INT64; The parent AppUser
 * Secret:                  GUID; Random secret
 * Key:                     FIXEDSTRING(128); Key - contains AppUserId data
 * CreatedDate:             DATETIME; DEFAULT DateTime.MinValue; Expiry time
 * Expiry:                  DATETIME; DEFAULT DateTime.MinValue; Expiry time
 * @INDEX:                  NAME(ix_AppUserAuthToken_SecretKey); [Secret,Key]; UNIQUE; 
 * @FOREIGNKEY:             NAME(fk_AppUserAuthToken_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_AppUserAuthToken_AppUserId); [AppUserId];
 * @INDEX:                  NAME(ix_AppUserAuthToken_Expiry); [Expiry ASC];
 * @INDEX:                  NAME(ix_AppUserAuthToken_CreatedDate); [CreatedDate ASC];
 * */

namespace Snoopi.core.DAL
{
    public partial class AppUserAuthTokenCollection : AbstractRecordList<AppUserAuthToken, AppUserAuthTokenCollection>
    {
    }

    public partial class AppUserAuthToken : AbstractRecord<AppUserAuthToken>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AppUserAuthTokenId = "AppUserAuthTokenId";
            public static string AppUserId = "AppUserId"; // The parent AppUser
            public static string Secret = "Secret"; // Random secret
            public static string Key = "Key"; // Key - contains AppUserId data
            public static string CreatedDate = "CreatedDate"; // Expiry time
            public static string Expiry = "Expiry"; // Expiry time
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppUserAuthToken";
                schema.AddColumn(Columns.AppUserAuthTokenId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Secret, typeof(Guid), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Key, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreatedDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.Expiry, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ix_AppUserAuthToken_SecretKey", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.Secret, Columns.Key);
                schema.AddIndex("ix_AppUserAuthToken_AppUserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.AppUserId);
                schema.AddIndex("ix_AppUserAuthToken_Expiry", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.Expiry, SortDirection.ASC);
                schema.AddIndex("ix_AppUserAuthToken_CreatedDate", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CreatedDate, SortDirection.ASC);

                schema.AddForeignKey("fk_AppUserAuthToken_AppUserId", AppUserAuthToken.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AppUserAuthTokenId = 0;
        internal Int64 _AppUserId = 0;
        internal Guid _Secret = Guid.Empty;
        internal string _Key = string.Empty;
        internal DateTime _CreatedDate = DateTime.MinValue;
        internal DateTime _Expiry = DateTime.MinValue;
        #endregion

        #region Properties
        public Int64 AppUserAuthTokenId
        {
            get { return _AppUserAuthTokenId; }
            set { _AppUserAuthTokenId = value; }
        }
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
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
            return AppUserAuthTokenId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.Secret, Secret);
            qry.Insert(Columns.Key, Key);
            qry.Insert(Columns.CreatedDate, CreatedDate);
            qry.Insert(Columns.Expiry, Expiry);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AppUserAuthTokenId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.Secret, Secret);
            qry.Update(Columns.Key, Key);
            qry.Update(Columns.CreatedDate, CreatedDate);
            qry.Update(Columns.Expiry, Expiry);
            qry.Where(Columns.AppUserAuthTokenId, AppUserAuthTokenId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AppUserAuthTokenId = Convert.ToInt64(reader[Columns.AppUserAuthTokenId]);
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            Secret = GuidFromDb(reader[Columns.Secret]);
            Key = (string)reader[Columns.Key];
            CreatedDate = Convert.ToDateTime(reader[Columns.CreatedDate]);
            Expiry = Convert.ToDateTime(reader[Columns.Expiry]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppUserAuthToken FetchByID(Int64 AppUserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserAuthTokenId, AppUserAuthTokenId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserAuthToken item = new AppUserAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AppUserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserAuthTokenId, AppUserAuthTokenId);
            return qry.Execute();
        }
        public static AppUserAuthToken FetchByID(ConnectorBase conn, Int64 AppUserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserAuthTokenId, AppUserAuthTokenId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppUserAuthToken item = new AppUserAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AppUserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserAuthTokenId, AppUserAuthTokenId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class AppUserAuthToken
    {

        public static AppUserAuthToken FetchByAppUserID(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserAuthToken item = new AppUserAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}
