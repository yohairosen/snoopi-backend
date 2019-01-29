using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * SupplierCity
 * SupplierCity
 * CityId:          INT64;
 * SupplierId:      INT64;
 * @INDEX:          NAME(pk_SupplierCity);PRIMARYKEY;[CityId,SupplierId]; 
 * @INDEX:          NAME(ix_SupplierCity_SupplierId);[SupplierId];
 * @INDEX:          NAME(ix_SupplierCity_CityId);[CityId];
 * @FOREIGNKEY:     NAME(fk_SupplierCity_CityId); FOREIGNTABLE(City); COLUMNS[CityId]; FOREIGNCOLUMNS[CityId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_SupplierCity_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class SupplierCityCollection : AbstractRecordList<SupplierCity, SupplierCityCollection>
    {
    }

    public partial class SupplierCity : AbstractRecord<SupplierCity>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CityId = "CityId";
            public static string SupplierId = "SupplierId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SupplierCity";
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_SupplierCity", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.CityId, Columns.SupplierId);
                schema.AddIndex("ix_SupplierCity_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_SupplierCity_CityId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CityId);

                schema.AddForeignKey("fk_SupplierCity_CityId", SupplierCity.Columns.CityId, City.TableSchema.SchemaName, City.Columns.CityId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SupplierCity_SupplierId", SupplierCity.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CityId = 0;
        internal Int64 _SupplierId = 0;
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
            qry.Where(Columns.CityId, CityId);
            qry.AND(Columns.SupplierId, SupplierId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SupplierCity FetchByID(Int64 CityId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SupplierCity item = new SupplierCity();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CityId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId);
            return qry.Execute();
        }
        public static SupplierCity FetchByID(ConnectorBase conn, Int64 CityId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SupplierCity item = new SupplierCity();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CityId, Int64 SupplierId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CityId, CityId)
                .AND(Columns.SupplierId, SupplierId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
