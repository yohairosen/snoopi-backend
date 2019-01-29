using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * ProductYad2
 * ProductYad2
 * ProductId:               PRIMARY KEY; INT64; AUTOINCREMENT;
 * ProductName:             FIXEDSTRING(128); DEFAULT string.Empty;
 * ProductImage:            FIXEDSTRING(255); DEFAULT string.Empty;
 * Price:                   DECIMAL; DEFAULT null;
 * ContactName:             FIXEDSTRING(128); DEFAULT string.Empty;
 * Details:                 FIXEDSTRING(128); DEFAULT string.Empty;
 * Status:                  DEFAULT StatusType.Wait; StatusType:
 *                               "StatusType"
 *                               - Wait = 0
 *                               - Approved = 1
 *                               - Denied = 2
 * StatusRemarks:           FIXEDSTRING(128); DEFAULT string.Empty;
 * PriceId:                 INT64;
 * Phone:                   FIXEDSTRING(128); DEFAULT string.Empty;
 * AppUserId:               INT64;
 * CityId:                  INT64;
 * UpdateDate:              DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * CreateDate:              DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * @FOREIGNKEY:             NAME(fk_PriceId); FOREIGNTABLE(PriceFilter); COLUMNS[PriceId]; FOREIGNCOLUMNS[PriceId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_ProductYad2_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_ProductYad2_CityId); FOREIGNTABLE(City); COLUMNS[CityId]; FOREIGNCOLUMNS[CityId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ik_ProductYad2_ProductId); [ProductId];
 * @INDEX:                  NAME(ik_ProductYad2_CityId); [CityId];
 * @INDEX:                  NAME(ik_ProductYad2_PriceId); [PriceId];
 * @INDEX:                  NAME(ix_Product_UpdateDate); [UpdateDate DESC];
 * */

namespace Snoopi.core.DAL
{
    public partial class ProductYad2Collection : AbstractRecordList<ProductYad2, ProductYad2Collection>
    {
    }

    public enum StatusType
    {
        Wait = 0,
        Approved = 1,
        Denied = 2,
    }

    public partial class ProductYad2 : AbstractRecord<ProductYad2>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string ProductId = "ProductId";
            public static string ProductName = "ProductName";
            public static string ProductImage = "ProductImage";
            public static string Price = "Price";
            public static string ContactName = "ContactName";
            public static string Details = "Details";
            public static string Status = "Status"; // StatusType
            public static string StatusRemarks = "StatusRemarks";
            public static string PriceId = "PriceId";
            public static string Phone = "Phone";
            public static string AppUserId = "AppUserId";
            public static string CityId = "CityId";
            public static string UpdateDate = "UpdateDate";
            public static string CreateDate = "CreateDate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"ProductYad2";
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.ProductName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.ProductImage, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Price, typeof(decimal), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ContactName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Details, typeof(string), DataType.Text, 0, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Status, typeof(StatusType), 0, 0, 0, false, false, false, StatusType.Wait);
                schema.AddColumn(Columns.StatusRemarks, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.PriceId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Phone, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UpdateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ik_ProductYad2_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);
                schema.AddIndex("ik_ProductYad2_CityId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CityId);
                schema.AddIndex("ik_ProductYad2_PriceId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.PriceId);
                schema.AddIndex("ix_Product_UpdateDate", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.UpdateDate, SortDirection.DESC);

                schema.AddForeignKey("fk_PriceId", ProductYad2.Columns.PriceId, PriceFilter.TableSchema.SchemaName, PriceFilter.Columns.PriceId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_ProductYad2_AppUserId", ProductYad2.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_ProductYad2_CityId", ProductYad2.Columns.CityId, City.TableSchema.SchemaName, City.Columns.CityId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _ProductId = 0;
        internal string _ProductName = string.Empty;
        internal string _ProductImage = string.Empty;
        internal decimal _Price = 0m;
        internal string _ContactName = string.Empty;
        internal string _Details = string.Empty;
        internal StatusType _Status = StatusType.Wait;
        internal string _StatusRemarks = string.Empty;
        internal Int64 _PriceId = 0;
        internal string _Phone = string.Empty;
        internal Int64 _AppUserId = 0;
        internal Int64 _CityId = 0;
        internal DateTime _UpdateDate = DateTime.UtcNow;
        internal DateTime _CreateDate = DateTime.UtcNow;
        #endregion

        #region Properties
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }
        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; }
        }
        public string ProductImage
        {
            get { return _ProductImage; }
            set { _ProductImage = value; }
        }
        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; }
        }
        public string ContactName
        {
            get { return _ContactName; }
            set { _ContactName = value; }
        }
        public string Details
        {
            get { return _Details; }
            set { _Details = value; }
        }
        public StatusType Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public string StatusRemarks
        {
            get { return _StatusRemarks; }
            set { _StatusRemarks = value; }
        }
        public Int64 PriceId
        {
            get { return _PriceId; }
            set { _PriceId = value; }
        }
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public Int64 CityId
        {
            get { return _CityId; }
            set { _CityId = value; }
        }
        public DateTime UpdateDate
        {
            get { return _UpdateDate; }
            set { _UpdateDate = value; }
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
            return ProductId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.ProductName, ProductName);
            qry.Insert(Columns.ProductImage, ProductImage);
            qry.Insert(Columns.Price, Price);
            qry.Insert(Columns.ContactName, ContactName);
            qry.Insert(Columns.Details, Details);
            qry.Insert(Columns.Status, Status);
            qry.Insert(Columns.StatusRemarks, StatusRemarks);
            qry.Insert(Columns.PriceId, PriceId);
            qry.Insert(Columns.Phone, Phone);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.CityId, CityId);
            qry.Insert(Columns.UpdateDate, UpdateDate);
            qry.Insert(Columns.CreateDate, CreateDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                ProductId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.ProductName, ProductName);
            qry.Update(Columns.ProductImage, ProductImage);
            qry.Update(Columns.Price, Price);
            qry.Update(Columns.ContactName, ContactName);
            qry.Update(Columns.Details, Details);
            qry.Update(Columns.Status, Status);
            qry.Update(Columns.StatusRemarks, StatusRemarks);
            qry.Update(Columns.PriceId, PriceId);
            qry.Update(Columns.Phone, Phone);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.CityId, CityId);
            qry.Update(Columns.UpdateDate, UpdateDate);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Where(Columns.ProductId, ProductId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            ProductName = (string)reader[Columns.ProductName];
            ProductImage = (string)reader[Columns.ProductImage];
            Price = Convert.ToDecimal(reader[Columns.Price]);
            ContactName = (string)reader[Columns.ContactName];
            Details = (string)reader[Columns.Details];
            Status = (StatusType)Convert.ToInt32(reader[Columns.Status]);
            StatusRemarks = (string)reader[Columns.StatusRemarks];
            PriceId = Convert.ToInt64(reader[Columns.PriceId]);
            Phone = (string)reader[Columns.Phone];
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            UpdateDate = Convert.ToDateTime(reader[Columns.UpdateDate]);
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static ProductYad2 FetchByID(Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    ProductYad2 item = new ProductYad2();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ProductId, ProductId);
            return qry.Execute();
        }
        public static ProductYad2 FetchByID(ConnectorBase conn, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    ProductYad2 item = new ProductYad2();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ProductId, ProductId);
            return qry.Execute(conn);
        }
        #endregion
    }
}