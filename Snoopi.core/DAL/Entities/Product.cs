using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * Product
 * Product
 * ProductId:               PRIMARY KEY; INT64; AUTOINCREMENT;
 * ProductNum:              INT64; NULLABLE;
 * ProductName:             FIXEDSTRING(128); DEFAULT string.Empty;
 * ProductCode:             FIXEDSTRING(255); DEFAULT string.Empty;
 * ProductImage:            FIXEDSTRING(255); DEFAULT string.Empty;
 * Amount:                  FIXEDSTRING(128); DEFAULT string.Empty;
 * Description:             FIXEDSTRING(255); DEFAULT string.Empty; 
 * SendSupplier:            BOOL; DEFAULT false;
 * RecomendedPrice:         DECIMAL; DEFAULT 0;
 * CategoryId:              INT64;
 * SubCategoryId:           INT64;
 * IsDeleted:               BOOL; DEFAULT false;
 * CreateDate:              DATETIME; DEFAULT DateTime.UtcNow; Create date/time
 * ProductRate:             INT64; DEFAULT 0;
 * @FOREIGNKEY:             NAME(fk_CategoryId); FOREIGNTABLE(Category); COLUMNS[CategoryId]; FOREIGNCOLUMNS[CategoryId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_SubCategoryId); FOREIGNTABLE(SubCategory); COLUMNS[SubCategoryId]; FOREIGNCOLUMNS[SubCategoryId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ik_ProductId); [ProductId];
 * @INDEX:                  NAME(ix_Product_Date); [CreateDate ASC];
 * */

namespace Snoopi.core.DAL
{
    public partial class ProductCollection : AbstractRecordList<Product, ProductCollection>
    {
    }

    public partial class Product : AbstractRecord<Product>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string ProductId = "ProductId";
            public static string ProductNum = "ProductNum";
            public static string ProductName = "ProductName";
            public static string ProductCode = "ProductCode";
            public static string ProductImage = "ProductImage";
            public static string Amount = "Amount";
            public static string Description = "Description";
            public static string SendSupplier = "SendSupplier";
            public static string RecomendedPrice = "RecomendedPrice";
            public static string CategoryId = "CategoryId";
            public static string SubCategoryId = "SubCategoryId";
            public static string IsDeleted = "IsDeleted";
            public static string CreateDate = "CreateDate"; // Create date/time
            public static string ProductRate = "ProductRate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Product";
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.ProductNum, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ProductName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.ProductCode, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.ProductImage, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Amount, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.Description, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.SendSupplier, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.RecomendedPrice, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.CategoryId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SubCategoryId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.IsDeleted, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.UtcNow);
                schema.AddColumn(Columns.ProductRate, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ik_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);
                schema.AddIndex("ix_Product_Date", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CreateDate, SortDirection.ASC);

                schema.AddForeignKey("fk_CategoryId", Product.Columns.CategoryId, Category.TableSchema.SchemaName, Category.Columns.CategoryId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SubCategoryId", Product.Columns.SubCategoryId, SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _ProductId = 0;
        internal Int64? _ProductNum = null;
        internal string _ProductName = string.Empty;
        internal string _ProductCode = string.Empty;
        internal string _ProductImage = string.Empty;
        internal string _Amount = string.Empty;
        internal string _Description = string.Empty;
        internal bool _SendSupplier = false;
        internal decimal _RecomendedPrice = 0;
        internal Int64 _CategoryId = 0;
        internal Int64 _SubCategoryId = 0;
        internal bool _IsDeleted = false;
        internal DateTime _CreateDate = DateTime.UtcNow;
        internal Int64 _ProductRate = 0;
        #endregion

        #region Properties
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }
        public Int64? ProductNum
        {
            get { return _ProductNum; }
            set { _ProductNum = value; }
        }
        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; }
        }
        public string ProductCode
        {
            get { return _ProductCode; }
            set { _ProductCode = value; }
        }
        public string ProductImage
        {
            get { return _ProductImage; }
            set { _ProductImage = value; }
        }
        public string Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public bool SendSupplier
        {
            get { return _SendSupplier; }
            set { _SendSupplier = value; }
        }
        public decimal RecomendedPrice
        {
            get { return _RecomendedPrice; }
            set { _RecomendedPrice = value; }
        }
        public Int64 CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }
        public Int64 SubCategoryId
        {
            get { return _SubCategoryId; }
            set { _SubCategoryId = value; }
        }
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }

        }
        public Int64 ProductRate
        {
            get { return _ProductRate; }
            set { _ProductRate = value; }
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
            qry.Insert(Columns.ProductNum, ProductNum);
            qry.Insert(Columns.ProductName, ProductName);
            qry.Insert(Columns.ProductCode, ProductCode);
            qry.Insert(Columns.ProductImage, ProductImage);
            qry.Insert(Columns.Amount, Amount);
            qry.Insert(Columns.Description, Description);
            qry.Insert(Columns.SendSupplier, SendSupplier);
            qry.Insert(Columns.RecomendedPrice, RecomendedPrice);
            qry.Insert(Columns.CategoryId, CategoryId);
            qry.Insert(Columns.SubCategoryId, SubCategoryId);
            qry.Insert(Columns.IsDeleted, IsDeleted);
            qry.Insert(Columns.CreateDate, CreateDate);
            qry.Insert(Columns.ProductRate, ProductRate);

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
            qry.Update(Columns.ProductNum, ProductNum);
            qry.Update(Columns.ProductName, ProductName);
            qry.Update(Columns.ProductCode, ProductCode);
            qry.Update(Columns.ProductImage, ProductImage);
            qry.Update(Columns.Amount, Amount);
            qry.Update(Columns.Description, Description);
            qry.Update(Columns.SendSupplier, SendSupplier);
            qry.Update(Columns.RecomendedPrice, RecomendedPrice);
            qry.Update(Columns.CategoryId, CategoryId);
            qry.Update(Columns.SubCategoryId, SubCategoryId);
            qry.Update(Columns.IsDeleted, IsDeleted);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Update(Columns.ProductRate, ProductRate);
            qry.Where(Columns.ProductId, ProductId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            ProductNum = IsNull(reader[Columns.ProductNum]) ? (Int64?)null : Convert.ToInt64(reader[Columns.ProductNum]);
            ProductName = (string)reader[Columns.ProductName];
            ProductCode = (string)reader[Columns.ProductCode];
            ProductImage = (string)reader[Columns.ProductImage];
            Amount = (string)reader[Columns.Amount];
            Description = (string)reader[Columns.Description];
            SendSupplier = Convert.ToBoolean(reader[Columns.SendSupplier]);
            RecomendedPrice = Convert.ToDecimal(reader[Columns.RecomendedPrice]);
            CategoryId = Convert.ToInt64(reader[Columns.CategoryId]);
            SubCategoryId = Convert.ToInt64(reader[Columns.SubCategoryId]);
            IsDeleted = Convert.ToBoolean(reader[Columns.IsDeleted]);
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);
            ProductRate = Convert.ToInt64(reader[Columns.ProductRate]);
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Product FetchByID(Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Product item = new Product();
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
        public static Product FetchByID(ConnectorBase conn, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Product item = new Product();
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
    public partial class Product
    {
        public static Product FetchByCode(string ProductCode)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductCode, ProductCode)
                .AddWhere(Columns.IsDeleted, false);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Product item = new Product();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static Product FetchByProductNum(Int64 ProductNum)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductNum, ProductNum)
                .AddWhere(Columns.IsDeleted, false);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Product item = new Product();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    
    }
}