using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * City
 * City
 * CityId:        PRIMARY KEY; INT64; AUTOINCREMENT;
 * CityName:      FIXEDSTRING(64);
 * AreaId:        INT64;
 * @INDEX:        NAME(ix_City_CityId); [CityId]; 
 * @FOREIGNKEY:   NAME(fk_City_AreaId); FOREIGNTABLE(Area); COLUMNS[AreaId]; FOREIGNCOLUMNS[AreaId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class CityCollection : AbstractRecordList<City, CityCollection>
    {
    }

    public partial class City : AbstractRecord<City>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CityId = "CityId";
            public static string CityName = "CityName";
            public static string AreaId = "AreaId";
            public static string PromotedAreaId = "PromotedAreaId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"City";
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.CityName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.AreaId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PromotedAreaId, typeof(int), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_City_CityId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CityId);

                schema.AddForeignKey("fk_City_AreaId", City.Columns.AreaId, Area.TableSchema.SchemaName, Area.Columns.AreaId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CityId = 0;
        internal string _CityName = string.Empty;
        internal Int64 _AreaId = 0;
        internal int _PromotedAreaId = 0;

        #endregion

        #region Properties
        public Int64 CityId
        {
            get { return _CityId; }
            set { _CityId = value; }
        }
        public string CityName
        {
            get { return _CityName; }
            set { _CityName = value; }
        }
        public Int64 AreaId
        {
            get { return _AreaId; }
            set { _AreaId = value; }
        }
        public int PromotedAreaId
        {
            get { return _PromotedAreaId; }
            set { _PromotedAreaId = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return CityId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CityName, CityName);
            qry.Insert(Columns.AreaId, AreaId);
            qry.Insert(Columns.PromotedAreaId, PromotedAreaId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CityId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CityName, CityName);
            qry.Update(Columns.AreaId, AreaId);
            qry.Update(Columns.PromotedAreaId, PromotedAreaId);
            qry.Where(Columns.CityId, CityId);


            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            CityName = (string)reader[Columns.CityName];
            AreaId = Convert.ToInt64(reader[Columns.AreaId]);
            PromotedAreaId= Convert.ToInt16(reader[Columns.PromotedAreaId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static City FetchByID(Int64 CityId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CityId, CityId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    City item = new City();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CityId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CityId, CityId);
            return qry.Execute();
        }
        public static City FetchByID(ConnectorBase conn, Int64 CityId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CityId, CityId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    City item = new City();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CityId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CityId, CityId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
