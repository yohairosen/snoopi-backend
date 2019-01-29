using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * SupplierHomeServiceCity
 * SupplierHomeServiceCity
 * CityId:          INT64;
 * SupplierId:      INT64;
 * ServiceId:       INT64;
 * @INDEX:          NAME(pk_SupplierHomeServiceCity);PRIMARYKEY;[CityId,SupplierId,ServiceId]; 
 * @INDEX:          NAME(ix_SupplierHomeServiceCity_CityId);[CityId];
 * @INDEX:          NAME(ix_SupplierHomeServiceCity_SupplierId);[SupplierId];
 * @INDEX:          NAME(ix_SupplierHomeServiceCity_ServiceId);[ServiceId];
 * @FOREIGNKEY:     NAME(fk_SupplierHomeServiceCity_CityId); FOREIGNTABLE(City); COLUMNS[CityId]; FOREIGNCOLUMNS[CityId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_SupplierHomeServiceCity_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_SupplierHomeServiceCity_ServiceId); FOREIGNTABLE(Service); COLUMNS[ServiceId]; FOREIGNCOLUMNS[ServiceId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class SupplierHomeServiceCityCollection : AbstractRecordList<SupplierHomeServiceCity, SupplierHomeServiceCityCollection>
    {
    }

    public partial class SupplierHomeServiceCity : AbstractRecord<SupplierHomeServiceCity>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CityId = "CityId";
            public static string SupplierId = "SupplierId";
            public static string ServiceId = "ServiceId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SupplierHomeServiceCity";
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ServiceId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_SupplierHomeServiceCity", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.CityId, Columns.SupplierId, Columns.ServiceId);
                schema.AddIndex("ix_SupplierHomeServiceCity_CityId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CityId);
                schema.AddIndex("ix_SupplierHomeServiceCity_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_SupplierHomeServiceCity_ServiceId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ServiceId);

                schema.AddForeignKey("fk_SupplierHomeServiceCity_CityId", SupplierHomeServiceCity.Columns.CityId, City.TableSchema.SchemaName, City.Columns.CityId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SupplierHomeServiceCity_SupplierId", SupplierHomeServiceCity.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SupplierHomeServiceCity_ServiceId", SupplierHomeServiceCity.Columns.ServiceId, Service.TableSchema.SchemaName, Service.Columns.ServiceId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CityId = 0;
        internal Int64 _SupplierId = 0;
        internal Int64 _ServiceId = 0;
        #endregion

        #region Properties
        public Int64 CityId
        {
            get { return _CityId; }
            set { _CityId = value; }
        }
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
        }
        public Int64 ServiceId
        {
            get { return _ServiceId; }
            set { _ServiceId = value; }
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
            qry.Insert(Columns.CityId, CityId);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.ServiceId, ServiceId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CityId, CityId);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.ServiceId, ServiceId);
            qry.Where(Columns.CityId, CityId);
            qry.AND(Columns.SupplierId, SupplierId);
            qry.AND(Columns.ServiceId, ServiceId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            ServiceId = Convert.ToInt64(reader[Columns.ServiceId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SupplierHomeServiceCity FetchByID(Int64 CityId, Int64 SupplierId, Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId)
                .AND(Columns.ServiceId, ServiceId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SupplierHomeServiceCity item = new SupplierHomeServiceCity();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CityId, Int64 SupplierId, Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId)
                .AND(Columns.ServiceId, ServiceId);
            return qry.Execute();
        }
        public static SupplierHomeServiceCity FetchByID(ConnectorBase conn, Int64 CityId, Int64 SupplierId, Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId)
                .AND(Columns.ServiceId, ServiceId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SupplierHomeServiceCity item = new SupplierHomeServiceCity();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CityId, Int64 SupplierId, Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId)
                .AND(Columns.ServiceId, ServiceId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
