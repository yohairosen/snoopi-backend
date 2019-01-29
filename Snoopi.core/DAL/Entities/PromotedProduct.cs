using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL
{
    public partial class PromotedProductCollection : AbstractRecordList<PromotedProduct, PromotedProductCollection>
    {
    }

    public partial class PromotedProduct : AbstractRecord<PromotedProduct>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string Id = "id";
            public static string AreaId = "area_id";
            public static string ProductId = "product_id";
            public static string Section = "section";
            public static string Weight = "weight";
            public static string Deleted = "deleted";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"promotedproduct";
                schema.AddColumn(Columns.AreaId, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ProductId, typeof(long), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Section, typeof(string), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Id, typeof(int), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Weight, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.Deleted, typeof(DateTime), 0, 0, 0, false, false, true, null);

                _TableSchema = schema;            
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal int _areaId;
        internal int _id;
        internal string _section;
        internal int _weight;
        internal long _productId;
        internal DateTime? _deleted;
        #endregion

        #region Properties

        public int AreaId
        {
            get { return _areaId; }
            set { _areaId = value; }
        }
        public Int64 ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }
        public string Section
        {
            get { return _section; }
            set { _section = value; }
        }
        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public DateTime? Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
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
            qry.Insert(Columns.AreaId, AreaId);
            qry.Insert(Columns.Section, Section);
            qry.Insert(Columns.Weight, Weight);
            qry.Insert(Columns.ProductId, ProductId);
            qry.Insert(Columns.Deleted, Deleted);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AreaId, AreaId);
            qry.Update(Columns.Section, Section);
            qry.Update(Columns.Weight, Weight);
            qry.Update(Columns.ProductId, ProductId);
            qry.Update(Columns.Deleted, Deleted);
            qry.Where(Columns.Id, Id);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AreaId = Convert.ToInt16(reader[Columns.AreaId]);
            Section = reader[Columns.Section].ToString();
            Weight = reader[Columns.Weight] == DBNull.Value ? 5 : (int)reader[Columns.Weight];
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            Id = Convert.ToInt32(reader[Columns.Id]);
            Deleted = reader[Columns.Deleted] == DBNull.Value ? (DateTime?)null : (DateTime)reader[Columns.Deleted];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static PromotedProduct FetchByID(int id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    var item = new PromotedProduct();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static PromotedProduct FetchByID(ConnectorBase conn, int id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, id);
            
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    var item = new PromotedProduct();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        #endregion
    }
}
