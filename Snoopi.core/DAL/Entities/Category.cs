using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Category
 * Category
 * CategoryId:       PRIMARY KEY; INT64; AUTOINCREMENT;
 * CategoryName:     FIXEDSTRING(64); Category Name
 * CategoryImage:     FIXEDSTRING(255); DEFAULT string.Empty;
 * CategoryRate:      INT64; DEFAULT 0;
 * @INDEX:           NAME(ix_Category_CategoryName); [CategoryName]; UNIQUE;
 * */

namespace Snoopi.core.DAL
{
    public partial class CategoryCollection : AbstractRecordList<Category, CategoryCollection>
    {
    }

    public partial class Category : AbstractRecord<Category>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string CategoryId = "CategoryId";
            public const string CategoryName = "CategoryName"; // Category Name
            public const string CategoryImage = "CategoryImage";
            public const string CategoryRate = "CategoryRate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Category";
                schema.AddColumn(Columns.CategoryId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.CategoryName, typeof(string), 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CategoryImage, typeof(string), 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CategoryRate, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Category_CategoryName", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.CategoryName);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CategoryId = 0;
        internal string _CategoryName = string.Empty;
        internal string _CategoryImage = string.Empty;
        internal Int64 _CategoryRate = 0;
        #endregion

        #region Properties
        public Int64 CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }
        public string CategoryName
        {
            get { return _CategoryName; }
            set { _CategoryName = value; }
        }
        public string CategoryImage
        {
            get { return _CategoryImage; }
            set { _CategoryImage = value; }
        }
        public Int64 CategoryRate
        {
            get { return _CategoryRate; }
            set { _CategoryRate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return CategoryId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CategoryName, CategoryName);
            qry.Insert(Columns.CategoryImage, CategoryImage);
            qry.Insert(Columns.CategoryRate, CategoryRate);
            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CategoryId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CategoryName, CategoryName);
            qry.Update(Columns.CategoryImage, CategoryImage);
            qry.Update(Columns.CategoryRate, CategoryRate);
            qry.Where(Columns.CategoryId, CategoryId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CategoryId = Convert.ToInt64(reader[Columns.CategoryId]);
            CategoryName = (string)reader[Columns.CategoryName];
            CategoryImage = (string)reader[Columns.CategoryImage];
            CategoryRate = Convert.ToInt64(reader[Columns.CategoryRate]);
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Category FetchByID(Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.CategoryId, CategoryId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Category item = new Category();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.CategoryId, CategoryId);
            return qry.Execute();
        }
        public static Category FetchByID(ConnectorBase conn, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.CategoryId, CategoryId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Category item = new Category();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.CategoryId, CategoryId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Category
    {
        public static Category FetchByName(string CategoryName)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CategoryName, CategoryName);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Category item = new Category();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static CategoryCollection FetchAllCategories()
        {
            CategoryCollection cc = new CategoryCollection();
            Query qry = new Query(TableSchema);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    Category item = new Category();
                    item.Read(reader);
                    cc.Add(item);
                }
            }
            return cc;
        }
    }

}
