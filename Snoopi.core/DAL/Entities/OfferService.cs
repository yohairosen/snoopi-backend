using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * OfferService
 * OfferService
 * OfferId:           PRIMARY KEY; INT64; AUTOINCREMENT;
 * SupplierId:        INT64;
 * BidId:             INT64;
 * SupplierRemarks:   FIXEDSTRING(255); string.Empty;
 * Price:             DECIMAL; DEFAULT 0;
 * CreateDate:        DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * @INDEX:            NAME(ix_OfferService_OfferId);[OfferId];
 * @FOREIGNKEY:       NAME(fk_OfferService_BidId); FOREIGNTABLE(Bid); COLUMNS[BidId]; FOREIGNCOLUMNS[BidId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:       NAME(fk_OfferService_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:            NAME(ix_OfferService_BidId);[BidId];
 * @INDEX:            NAME(ix_OfferService_SupplierId);[SupplierId];
 * */

namespace Snoopi.core.DAL
{

    public partial class OfferServiceCollection : AbstractRecordList<OfferService, OfferServiceCollection>
    {
    }

    public partial class OfferService : AbstractRecord<OfferService>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string OfferId = "OfferId";
            public static string SupplierId = "SupplierId";
            public static string BidId = "BidId";
            public static string SupplierRemarks = "SupplierRemarks";
            public static string Price = "Price";
            public static string CreateDate = "CreateDate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"OfferService";
                schema.AddColumn(Columns.OfferId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierRemarks, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Price, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ix_OfferService_OfferId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.OfferId);
                schema.AddIndex("ix_OfferService_BidId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.BidId);
                schema.AddIndex("ix_OfferService_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);

                schema.AddForeignKey("fk_OfferService_BidId", OfferService.Columns.BidId, Bid.TableSchema.SchemaName, Bid.Columns.BidId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_OfferService_SupplierId", OfferService.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _OfferId = 0;
        internal Int64 _SupplierId = 0;
        internal Int64 _BidId = 0;
        internal string _SupplierRemarks = string.Empty;
        internal decimal _Price = 0;
        internal DateTime _CreateDate = DateTime.UtcNow;
        #endregion

        #region Properties
        public Int64 OfferId
        {
            get { return _OfferId; }
            set { _OfferId = value; }
        }
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
        }
        public Int64 BidId
        {
            get { return _BidId; }
            set { _BidId = value; }
        }
        public string SupplierRemarks
        {
            get { return _SupplierRemarks; }
            set { _SupplierRemarks = value; }
        }
        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return OfferId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.BidId, BidId);
            qry.Insert(Columns.SupplierRemarks, SupplierRemarks);
            qry.Insert(Columns.Price, Price);
            qry.Insert(Columns.CreateDate, CreateDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                OfferId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.BidId, BidId);
            qry.Update(Columns.SupplierRemarks, SupplierRemarks);
            qry.Update(Columns.Price, Price);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Where(Columns.OfferId, OfferId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            OfferId = Convert.ToInt64(reader[Columns.OfferId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            SupplierRemarks = (string)reader[Columns.SupplierRemarks];
            Price = Convert.ToDecimal(reader[Columns.Price]);
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static OfferService FetchByID(Int64 OfferId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.OfferId, OfferId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    OfferService item = new OfferService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 OfferId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.OfferId, OfferId);
            return qry.Execute();
        }
        public static OfferService FetchByID(ConnectorBase conn, Int64 OfferId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.OfferId, OfferId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    OfferService item = new OfferService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 OfferId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.OfferId, OfferId);
            return qry.Execute(conn);
        }
        #endregion
    }
    
   
        public partial class OfferService
        {
            public static OfferService FetchByBidIdAndSupplierId(Int64 BidId, Int64 SupplierId)
            {
                Query qry = new Query(TableSchema)
                    .Where(Columns.BidId, BidId).AddWhere(Columns.SupplierId, SupplierId);
                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        OfferService item = new OfferService();
                        item.Read(reader);
                        return item;
                    }
                }
                return null;
            }
        }
    

}
