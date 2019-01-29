using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities.Serialization;
using Newtonsoft;
using Newtonsoft.Json;

/*
 * Permission
 * Permission
 * PermissionId:                  PRIMARY KEY; AUTOINCREMENT; INT32;
 * Key:                           FIXEDSTRING(64); First name
 * @INDEX:                        NAME(ix_Permission_Key);UNIQUE;[Key]; 
*/

namespace Snoopi.core.DAL
{
    public partial class PermissionCollection : AbstractRecordList<Permission, PermissionCollection>
    {
    }

    public partial class Permission : AbstractRecord<Permission>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string PermissionId = "PermissionId";
            public static string Key = "Key"; // First name
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Permission";
                schema.AddColumn(Columns.PermissionId, typeof(Int32), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Key, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Permission_Key", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.Key);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int32 _PermissionId = 0;
        internal string _Key = string.Empty;
        #endregion

        #region Properties
        public Int32 PermissionId
        {
            get { return _PermissionId; }
            set { _PermissionId = value; }
        }
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return PermissionId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Key, Key);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                PermissionId = Convert.ToInt32((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Key, Key);
            qry.Where(Columns.PermissionId, PermissionId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            PermissionId = Convert.ToInt32(reader[Columns.PermissionId]);
            Key = (string)reader[Columns.Key];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Permission FetchByID(Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.PermissionId, PermissionId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Permission item = new Permission();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.PermissionId, PermissionId);
            return qry.Execute();
        }
        public static Permission FetchByID(ConnectorBase conn, Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.PermissionId, PermissionId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Permission item = new Permission();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.PermissionId, PermissionId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Permission : AbstractRecord<Permission>
    {
        public Permission() { }
        public Permission(string Key) : base()
        {
            this.Key = Key;
        }
    }
}
