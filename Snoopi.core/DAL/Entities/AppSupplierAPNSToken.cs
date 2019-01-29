using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppSupplierAPNSToken
 * AppSupplierAPNSToken
 * SupplierId:           INT64; The parent AppSupplier
 * Token:                   FIXEDSTRING(64); APNS device token
 * @INDEX:                  NAME(ix_AppSupplierAPNSToken_AppSupplierIdToken); [SupplierId,Token]; PRIMARYKEY; 
 * @FOREIGNKEY:             NAME(fk_AppSupplierAPNSToken_AppSupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_AppSupplierAPNSToken_AppSupplierId); [SupplierId];
 * @INDEX:                  NAME(ix_AppSupplierAPNSToken_Token); [Token];
 * */

namespace Snoopi.core.DAL
{
    public partial class AppSupplierAPNSTokenCollection : AbstractRecordList<AppSupplierAPNSToken, AppSupplierAPNSTokenCollection>
    {
    }

    public partial class AppSupplierAPNSToken : AbstractRecord<AppSupplierAPNSToken>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SupplierId = "SupplierId"; // The parent AppSupplier
            public static string Token = "Token"; // APNS device token
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppSupplierAPNSToken";
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Token, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_AppSupplierAPNSToken_AppSupplierIdToken", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.SupplierId, Columns.Token);
                schema.AddIndex("ix_AppSupplierAPNSToken_AppSupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_AppSupplierAPNSToken_Token", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.Token);

                schema.AddForeignKey("fk_AppSupplierAPNSToken_AppSupplierId", AppSupplierAPNSToken.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

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
        public static AppSupplierAPNSToken FetchByID(Int64 SupplierId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.Token, Token);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppSupplierAPNSToken item = new AppSupplierAPNSToken();
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
        public static AppSupplierAPNSToken FetchByID(ConnectorBase conn, Int64 SupplierId, string Token)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.Token, Token);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppSupplierAPNSToken item = new AppSupplierAPNSToken();
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
