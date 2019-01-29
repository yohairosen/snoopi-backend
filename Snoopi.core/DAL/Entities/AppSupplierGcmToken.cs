using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppSupplierGcmToken
 * AppSupplierGcmToken
 * SupplierId:              INT64; The parent AppUser
 * Token:                   MEDIUMTEXT; Google Cloud Messaging device token
 * @INDEX:                  NAME(ix_AppSupplierGcmToken_PrimaryKey); [SupplierId,Token]; PRIMARYKEY; 
 * @FOREIGNKEY:             NAME(fk_AppSupplierGcmToken_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_AppSupplierGcmToken_SupplierId); [SupplierId];
 * @INDEX:                  NAME(ix_AppSupplierGcmToken_Token); [Token];
 * */

namespace Snoopi.core.DAL
{
    public partial class AppSupplierGcmTokenCollection : AbstractRecordList<AppSupplierGcmToken, AppSupplierGcmTokenCollection>
    {
    }

    public partial class AppSupplierGcmToken : AbstractRecord<AppSupplierGcmToken>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SupplierId = "SupplierId"; // The parent AppUser
            public static string Token = "Token"; // Google Cloud Messaging device token
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppSupplierGcmToken";
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Token, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_AppSupplierGcmToken_PrimaryKey", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.SupplierId, Columns.Token);
                schema.AddIndex("ix_AppSupplierGcmToken_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_AppSupplierGcmToken_Token", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.Token);

                schema.AddForeignKey("fk_AppSupplierGcmToken_SupplierId", AppSupplierGcmToken.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _SupplierId = 0;
        internal string _Token = string.Empty;
        #endregion

        #region Properties
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
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
            qry.Insert(Columns.SupplierId, SupplierId);
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
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.Token, Token);
            qry.Where(Columns.SupplierId, SupplierId);
            qry.AND(Columns.Token, Token);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            Token = (string)reader[Columns.Token];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppSupplierGcmToken FetchByID(Int64 SupplierId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.Token, Token);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppSupplierGcmToken item = new AppSupplierGcmToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 SupplierId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, SupplierId)
                .AND(Columns.Token, Token);
            return qry.Execute();
        }
        public static AppSupplierGcmToken FetchByID(ConnectorBase conn, Int64 SupplierId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.Token, Token);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppSupplierGcmToken item = new AppSupplierGcmToken();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 SupplierId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, SupplierId)
                .AND(Columns.Token, Token);
            return qry.Execute(conn);
        }
        #endregion
    }
}
