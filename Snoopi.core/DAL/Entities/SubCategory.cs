using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * SubCategory
 * SubCategory
 * SubCategoryId:       PRIMARY KEY; INT64; AUTOINCREMENT;
 * SubCategoryName:     FIXEDSTRING(64); Category Name
 * CategoryId:          INT64;
 * SubCategoryImage:    FIXEDSTRING(255); DEFAULT string.Empty;
 * SubCategoryRate:      INT64; DEFAULT 0;
 * @FOREIGNKEY:         NAME(fk_SubCategory_CategoryId); FOREIGNTABLE(Category); COLUMNS[CategoryId]; FOREIGNCOLUMNS[CategoryId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{
    public partial class SubCategoryCollection : AbstractRecordList<SubCategory, SubCategoryCollection>
    {
    }

    public partial class SubCategory : AbstractRecord<SubCategory>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SubCategoryId = "SubCategoryId";
            public static string SubCategoryName = "SubCategoryName"; // Category Name
            public static string CategoryId = "CategoryId";
            public static string SubCategoryImage = "SubCategoryImage";
            public static string SubCategoryRate = "SubCategoryRate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SubCategory";
                schema.AddColumn(Columns.SubCategoryId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SubCategoryName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CategoryId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SubCategoryImage, typeof(string), DataType.Char, 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.SubCategoryRate, typeof(Int64),0,0,0,false,false,false,null);
                _TableSchema = schema;

                schema.AddForeignKey("fk_SubCategory_CategoryId", SubCategory.Columns.CategoryId, Category.TableSchema.SchemaName, Category.Columns.CategoryId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _SubCategoryId = 0;
        internal string _SubCategoryName = string.Empty;
        internal Int64 _CategoryId = 0;
        internal string _SubCategoryImage = string.Empty;
        internal Int64 _SubCategoryRate = 0;
        #endregion

        #region Properties
        public Int64 SubCategoryId
        {
            get { return _SubCategoryId; }
            set { _SubCategoryId = value; }
        }
        public string SubCategoryName
        {
            get { return _SubCategoryName; }
            set { _SubCategoryName = value; }
        }
        public Int64 CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }
        public string SubCategoryImage
        {
            get { return _SubCategoryImage; }
            set { _SubCategoryImage = value; }
        }
        public Int64 SubCategoryRate
        {
            get { return _SubCategoryRate; }
            set { _SubCategoryRate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return SubCategoryId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SubCategoryName, SubCategoryName);
            qry.Insert(Columns.CategoryId, CategoryId);
            qry.Insert(Columns.SubCategoryImage, SubCategoryImage);
            qry.Insert(Columns.SubCategoryRate, SubCategoryRate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                SubCategoryId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SubCategoryName, SubCategoryName);
            qry.Update(Columns.CategoryId, CategoryId);
            qry.Update(Columns.SubCategoryImage, SubCategoryImage);
            qry.Update(Columns.SubCategoryRate, SubCategoryRate);
            qry.Where(Columns.SubCategoryId, SubCategoryId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            SubCategoryId = Convert.ToInt64(reader[Columns.SubCategoryId]);
            SubCategoryName = (string)reader[Columns.SubCategoryName];
            CategoryId = Convert.ToInt64(reader[Columns.CategoryId]);
            SubCategoryImage = (string)reader[Columns.SubCategoryImage];
            SubCategoryRate = Convert.ToInt64(reader[Columns.SubCategoryRate]);
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SubCategory FetchByID(Int64 SubCategoryId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryId, SubCategoryId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static SubCategory FetchByID(Int64 SubCategoryId, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryId, SubCategoryId)
                .AddWhere(Columns.CategoryId, CategoryId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 SubCategoryId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SubCategoryId, SubCategoryId);
            return qry.Execute();
        }
        public static SubCategory FetchByID(ConnectorBase conn, Int64 SubCategoryId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryId, SubCategoryId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 SubCategoryId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SubCategoryId, SubCategoryId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class SubCategory
    {


        public static SubCategory FetchByName(string SubCategoryName, string CategoryName)
        {
            Query qry = new Query(TableSchema)
                .Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(SubCategory.TableSchema.SchemaName, SubCategory.Columns.CategoryId, Category.Columns.CategoryId))
                .Where(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, WhereComparision.EqualsTo, SubCategoryName)
                .AddWhere(Category.TableSchema.SchemaName, Category.Columns.CategoryName, WhereComparision.EqualsTo, CategoryName);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }


        public static SubCategory FetchByName(string SubCategoryName, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryName, SubCategoryName).AddWhere(Columns.CategoryId, CategoryId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }


        public static SubCategory FetchByName(string SubCategoryName)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryName, SubCategoryName);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static SubCategoryCollection FetchAllCategories()
        {
            SubCategoryCollection cc = new SubCategoryCollection();
            Query qry = new Query(TableSchema);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    SubCategory item = new SubCategory();
                    item.Read(reader);
                    cc.Add(item);
                }
            }
            return cc;
        }
    }

}
