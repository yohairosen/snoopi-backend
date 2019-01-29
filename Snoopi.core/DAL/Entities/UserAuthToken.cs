using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * UserAuthToken
 * UserAuthToken
 * UserAuthTokenId:         PRIMARY KEY; INT64; AUTOINCREMENT;
 * UserId:                  INT64; The parent User
 * Secret:                  GUID; Random secret
 * Key:                     FIXEDSTRING(128); Key - contains UserId data
 * CreatedOn:               DATETIME; DEFAULT DateTime.MinValue; Expiry time
 * Expiry:                  DATETIME; DEFAULT DateTime.MinValue; Expiry time
 * @INDEX:                  NAME(ix_UserAuthToken_SecretKey); [Secret,Key]; UNIQUE; 
 * @FOREIGNKEY:             NAME(fk_UserAuthToken_UserId); FOREIGNTABLE(User); COLUMNS[UserId]; FOREIGNCOLUMNS[UserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_UserAuthToken_UserId); [UserId];
 * @INDEX:                  NAME(ix_UserAuthToken_Expiry); [Expiry ASC];
 * @INDEX:                  NAME(ix_UserAuthToken_CreatedOn); [CreatedOn ASC];
 * */

namespace Snoopi.core.DAL
{
    public partial class UserAuthTokenCollection : AbstractRecordList<UserAuthToken, UserAuthTokenCollection>
    {
    }

    public partial class UserAuthToken : AbstractRecord<UserAuthToken>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string UserAuthTokenId = "UserAuthTokenId";
            public static string UserId = "UserId"; // The parent User
            public static string Secret = "Secret"; // Random secret
            public static string Key = "Key"; // Key - contains UserId data
            public static string CreatedOn = "CreatedOn"; // Expiry time
            public static string Expiry = "Expiry"; // Expiry time
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"UserAuthToken";
                schema.AddColumn(Columns.UserAuthTokenId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.UserId, typeof(Int32), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Secret, typeof(Guid), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Key, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreatedOn, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.Expiry, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ix_UserAuthToken_SecretKey", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.Secret, Columns.Key);
                schema.AddIndex("ix_UserAuthToken_UserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.UserId);
                schema.AddIndex("ix_UserAuthToken_Expiry", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.Expiry, SortDirection.ASC);
                schema.AddIndex("ix_UserAuthToken_CreatedOn", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CreatedOn, SortDirection.ASC);

                schema.AddForeignKey("fk_UserAuthToken_UserId", UserAuthToken.Columns.UserId, User.TableSchema.SchemaName, User.Columns.UserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _UserAuthTokenId = 0;
        internal Int64 _UserId = 0;
        internal Guid _Secret = Guid.Empty;
        internal string _Key = string.Empty;
        internal DateTime _CreatedOn = DateTime.MinValue;
        internal DateTime _Expiry = DateTime.MinValue;
        #endregion

        #region Properties
        public Int64 UserAuthTokenId
        {
            get { return _UserAuthTokenId; }
            set { _UserAuthTokenId = value; }
        }
        public Int64 UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
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
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; }
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
            return UserAuthTokenId;
        }

        public override void Insert(ConnectorBase conn)
        {
            CreatedOn = DateTime.UtcNow;

            Query qry = new Query(TableSchema);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.Secret, Secret);
            qry.Insert(Columns.Key, Key);
            qry.Insert(Columns.CreatedOn, CreatedOn);
            qry.Insert(Columns.Expiry, Expiry);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                UserAuthTokenId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.Secret, Secret);
            qry.Update(Columns.Key, Key);
            qry.Update(Columns.CreatedOn, CreatedOn);
            qry.Update(Columns.Expiry, Expiry);
            qry.Where(Columns.UserAuthTokenId, UserAuthTokenId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            UserAuthTokenId = Convert.ToInt64(reader[Columns.UserAuthTokenId]);
            UserId = Convert.ToInt32(reader[Columns.UserId]);
            Secret = GuidFromDb(reader[Columns.Secret]);
            Key = (string)reader[Columns.Key];
            CreatedOn = Convert.ToDateTime(reader[Columns.CreatedOn]);
            Expiry = Convert.ToDateTime(reader[Columns.Expiry]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static UserAuthToken FetchByID(Int64 UserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserAuthTokenId, UserAuthTokenId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    UserAuthToken item = new UserAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 UserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserAuthTokenId, UserAuthTokenId);
            return qry.Execute();
        }
        public static UserAuthToken FetchByID(ConnectorBase conn, Int64 UserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserAuthTokenId, UserAuthTokenId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    UserAuthToken item = new UserAuthToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 UserAuthTokenId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserAuthTokenId, UserAuthTokenId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
