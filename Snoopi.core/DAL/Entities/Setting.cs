using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
* Setting
* Setting
* Key:             PRIMARY KEY; FIXEDSTRING(64);
* Value:           FIXEDSTRING(255);
* */

namespace Snoopi.core.DAL
{
    public partial class SettingCollection : AbstractRecordList<Setting, SettingCollection>
    {
    }

    public partial class Setting : AbstractRecord<Setting>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string Key = "Key";
            public static string Value = "Value";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Setting";
                schema.AddColumn(Columns.Key, typeof(string), DataType.Char, 64, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.Value, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);

                _TableSchema = schema;

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal string _Key = string.Empty;
        internal string _Value = string.Empty;
        #endregion

        #region Properties
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return Key;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Key, Key);
            qry.Insert(Columns.Value, Value);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                Key = (string)(lastInsert);
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Key, Key);
            qry.Update(Columns.Value, Value);
            qry.Where(Columns.Key, Key);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Key = (string)reader[Columns.Key];
            Value = (string)reader[Columns.Value];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Setting FetchByID(string Key)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Key, Key);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Setting item = new Setting();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(string Key)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Key, Key);
            return qry.Execute();
        }
        public static Setting FetchByID(ConnectorBase conn, string Key)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Key, Key);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Setting item = new Setting();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, string Key)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Key, Key);
            return qry.Execute(conn);
        }
        #endregion
    }
}
