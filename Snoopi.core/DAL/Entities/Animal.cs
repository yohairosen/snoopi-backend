using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Animal
 * Animal
 * AnimalId:      PRIMARY KEY; INT64; AUTOINCREMENT;
 * AnimalName:    FIXEDSTRING(64);
 * @INDEX:        NAME(ix_Animal_AnimalId); [AnimalId]; 
 * */

namespace Snoopi.core.DAL
{

    public partial class AnimalCollection : AbstractRecordList<Animal, AnimalCollection>
    {
    }

    public partial class Animal : AbstractRecord<Animal>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AnimalId = "AnimalId";
            public static string AnimalName = "AnimalName";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Animal";
                schema.AddColumn(Columns.AnimalId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.AnimalName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Animal_AnimalId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.AnimalId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AnimalId = 0;
        internal string _AnimalName = string.Empty;
        #endregion

        #region Properties
        public Int64 AnimalId
        {
            get { return _AnimalId; }
            set { _AnimalId = value; }
        }
        public string AnimalName
        {
            get { return _AnimalName; }
            set { _AnimalName = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return AnimalId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AnimalName, AnimalName);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AnimalId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AnimalName, AnimalName);
            qry.Where(Columns.AnimalId, AnimalId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AnimalId = Convert.ToInt64(reader[Columns.AnimalId]);
            AnimalName = (string)reader[Columns.AnimalName];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Animal FetchByID(Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AnimalId, AnimalId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Animal item = new Animal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AnimalId, AnimalId);
            return qry.Execute();
        }
        public static Animal FetchByID(ConnectorBase conn, Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AnimalId, AnimalId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Animal item = new Animal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AnimalId, AnimalId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Animal
    {

        public static Animal FetchByName(string AnimalName)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AnimalName, AnimalName);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Animal item = new Animal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }

}
