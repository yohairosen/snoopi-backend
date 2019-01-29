using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppUserAPNSToken
 * AppUserAPNSToken
 * AppUserId:               INT64; The parent AppUser
 * Token:                   FIXEDSTRING(64); APNS device token
 * @INDEX:                  NAME(ix_AppUserAPNSToken_AppUserIdToken); [AppUserId,Token]; PRIMARYKEY; 
 * @FOREIGNKEY:             NAME(fk_AppUserAPNSToken_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_AppUserAPNSToken_AppUserId); [AppUserId];
 * @INDEX:                  NAME(ix_AppUserAPNSToken_Token); [Token];
 * */

namespace Snoopi.core.DAL
{
    public partial class AppUserAPNSTokenCollection : AbstractRecordList<AppUserAPNSToken, AppUserAPNSTokenCollection>
    {
    }

    public partial class AppUserAPNSToken : AbstractRecord<AppUserAPNSToken>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AppUserId = "AppUserId"; // The parent AppUser
            public static string Token = "Token"; // APNS device token
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppUserAPNSToken";
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Token, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_AppUserAPNSToken_AppUserIdToken", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.AppUserId, Columns.Token);
                schema.AddIndex("ix_AppUserAPNSToken_AppUserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.AppUserId);
                schema.AddIndex("ix_AppUserAPNSToken_Token", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.Token);

                schema.AddForeignKey("fk_AppUserAPNSToken_AppUserId", AppUserAPNSToken.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AppUserId = 0;
        internal string _Token = string.Empty;
        #endregion

        #region Properties
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public string Token
        {
            get { return _Token; }
            set { _Token = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.Token, Token);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.Token, Token);
            qry.Where(Columns.AppUserId, AppUserId);
            qry.AND(Columns.Token, Token);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            Token = (string)reader[Columns.Token];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppUserAPNSToken FetchByID(Int64 AppUserId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId)
                .AND(Columns.Token, Token);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserAPNSToken item = new AppUserAPNSToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AppUserId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserId, AppUserId)
                .AND(Columns.Token, Token);
            return qry.Execute();
        }
        public static AppUserAPNSToken FetchByID(ConnectorBase conn, Int64 AppUserId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId)
                .AND(Columns.Token, Token);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppUserAPNSToken item = new AppUserAPNSToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AppUserId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserId, AppUserId)
                .AND(Columns.Token, Token);
            return qry.Execute(conn);
        }
        #endregion
    }
}
