using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * SupplierService
 * SupplierService
 * ServiceId:       INT64;
 * SupplierId:      INT64;
 * @INDEX:          NAME(pk_SupplierService);PRIMARYKEY;[ServiceId,SupplierId]; 
 * @INDEX:          NAME(ix_SupplierService_SupplierId);[SupplierId]; 
 * @INDEX:          NAME(ix_SupplierService_ServiceId);[ServiceId]; 
 * @FOREIGNKEY:     NAME(fk_SupplierService_ServiceId); FOREIGNTABLE(Service); COLUMNS[ServiceId]; FOREIGNCOLUMNS[ServiceId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_SupplierService_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class SupplierServiceCollection : AbstractRecordList<SupplierService, SupplierServiceCollection>
    {
    }

    public partial class SupplierService : AbstractRecord<SupplierService>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string ServiceId = "ServiceId";
            public static string SupplierId = "SupplierId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SupplierService";
                schema.AddColumn(Columns.ServiceId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_SupplierService", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.ServiceId, Columns.SupplierId);
                schema.AddIndex("ix_SupplierService_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_SupplierService_ServiceId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ServiceId);

                schema.AddForeignKey("fk_SupplierService_ServiceId", SupplierService.Columns.ServiceId, Service.TableSchema.SchemaName, Service.Columns.ServiceId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SupplierService_SupplierId", SupplierService.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _ServiceId = 0;
        internal Int64 _SupplierId = 0;
        #endregion

        #region Properties
        public Int64 ServiceId
        {
            get { return _ServiceId; }
            set { _ServiceId = value; }
        }
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
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
            qry.Insert(Columns.ServiceId, ServiceId);
            qry.Insert(Columns.SupplierId, SupplierId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.ServiceId, ServiceId);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Where(Columns.ServiceId, ServiceId);
            qry.AND(Columns.SupplierId, SupplierId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ServiceId = Convert.ToInt64(reader[Columns.ServiceId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SupplierService FetchByID(Int64 ServiceId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ServiceId, ServiceId)
                .AND(Columns.SupplierId, SupplierId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SupplierService item = new SupplierService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 ServiceId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ServiceId, ServiceId)
                .AND(Columns.SupplierId, SupplierId);
            return qry.Execute();
        }
        public static SupplierService FetchByID(ConnectorBase conn, Int64 ServiceId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ServiceId, ServiceId)
                .AND(Columns.SupplierId, SupplierId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SupplierService item = new SupplierService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 ServiceId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ServiceId, ServiceId)
                .AND(Columns.SupplierId, SupplierId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
