using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * CategoryYad2
 * CategoryYad2
 * CategoryYad2Id:          PRIMARY KEY; INT64; AUTOINCREMENT; 
 * CategoryYad2Name:        FIXEDSTRING(128);
 * @INDEX:                  NAME(ik_CategoryYad2_CategoryYad2Id); [CategoryYad2Id];
 * */

namespace Snoopi.core.DAL
{
    public partial class CategoryYad2Collection : AbstractRecordList<CategoryYad2, CategoryYad2Collection>
    {
    }

    public partial class CategoryYad2 : AbstractRecord<CategoryYad2>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CategoryYad2Id = "CategoryYad2Id";
            public static string CategoryYad2Name = "CategoryYad2Name";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"CategoryYad2";
                schema.AddColumn(Columns.CategoryYad2Id, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.CategoryYad2Name, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ik_CategoryYad2_CategoryYad2Id", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CategoryYad2Id);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CategoryYad2Id = 0;
        internal string _CategoryYad2Name = string.Empty;
        #endregion

        #region Properties
        public Int64 CategoryYad2Id
        {
            get { return _CategoryYad2Id; }
            set { _CategoryYad2Id = value; }
        }
        public string CategoryYad2Name
        {
            get { return _CategoryYad2Name; }
            set { _CategoryYad2Name = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return CategoryYad2Id;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CategoryYad2Name, CategoryYad2Name);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CategoryYad2Id = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CategoryYad2Name, CategoryYad2Name);
            qry.Where(Columns.CategoryYad2Id, CategoryYad2Id);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CategoryYad2Id = Convert.ToInt64(reader[Columns.CategoryYad2Id]);
            CategoryYad2Name = (string)reader[Columns.CategoryYad2Name];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static CategoryYad2 FetchByID(Int64 CategoryYad2Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CategoryYad2Id, CategoryYad2Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    CategoryYad2 item = new CategoryYad2();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CategoryYad2Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CategoryYad2Id, CategoryYad2Id);
            return qry.Execute();
        }
        public static CategoryYad2 FetchByID(ConnectorBase conn, Int64 CategoryYad2Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CategoryYad2Id, CategoryYad2Id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    CategoryYad2 item = new CategoryYad2();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CategoryYad2Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CategoryYad2Id, CategoryYad2Id);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class CategoryYad2
    {
        static public CategoryYad2 FetchByName(string name)
        {
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.CategoryYad2Name, name).ExecuteReader())
            {
                if (reader.Read())
                {
                    CategoryYad2 c = new CategoryYad2();
                    c.Read(reader);
                    return c;
                }
            }
            return null;
        }
    }
}