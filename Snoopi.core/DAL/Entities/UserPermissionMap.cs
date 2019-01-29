using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * UserPermissionMap
 * UserPermissionMap
 * UserId:                INT64;
 * PermissionId:          INT32;
 * @INDEX:                NAME(pk_UserPermissionMap_UserIdPermissionId);PRIMARYKEY;[UserId,PermissionId]; 
 * @FOREIGNKEY:           NAME(fk_UserPermissionMap_UserId); FOREIGNTABLE(User); COLUMNS[UserId]; FOREIGNCOLUMNS[UserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:           NAME(fk_UserPermissionMap_PermissionId); FOREIGNTABLE(Permission); COLUMNS[PermissionId]; FOREIGNCOLUMNS[PermissionId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                NAME(ix_UserPermissionMap_UserId);[UserId]; 
 * @INDEX:                NAME(ix_UserPermissionMap_PermissionId);[PermissionId]; 
 * */

namespace Snoopi.core.DAL
{
    public partial class UserPermissionMapCollection : AbstractRecordList<UserPermissionMap, UserPermissionMapCollection>
    {
    }

    public partial class UserPermissionMap : AbstractRecord<UserPermissionMap>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string UserId = "UserId";
            public static string PermissionId = "PermissionId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"UserPermissionMap";
                schema.AddColumn(Columns.UserId, typeof(Int32), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PermissionId, typeof(Int32), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_UserPermissionMap_UserIdPermissionId", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.UserId, Columns.PermissionId);
                schema.AddIndex("ix_UserPermissionMap_UserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.UserId);
                schema.AddIndex("ix_UserPermissionMap_PermissionId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.PermissionId);

                schema.AddForeignKey("fk_UserPermissionMap_UserId", UserPermissionMap.Columns.UserId, User.TableSchema.SchemaName, User.Columns.UserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_UserPermissionMap_PermissionId", UserPermissionMap.Columns.PermissionId, Permission.TableSchema.SchemaName, Permission.Columns.PermissionId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _UserId = 0;
        internal Int32 _PermissionId = 0;
        #endregion

        #region Properties
        public Int64 UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public Int32 PermissionId
        {
            get { return _PermissionId; }
            set { _PermissionId = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.PermissionId, PermissionId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.PermissionId, PermissionId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            UserId = Convert.ToInt32(reader[Columns.UserId]);
            PermissionId = Convert.ToInt32(reader[Columns.PermissionId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static UserPermissionMap FetchByID(Int64 UserId, Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserId, UserId)
                .AND(Columns.PermissionId, PermissionId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    UserPermissionMap item = new UserPermissionMap();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 UserId, Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserId, UserId)
                .AND(Columns.PermissionId, PermissionId);
            return qry.Execute();
        }
        public static UserPermissionMap FetchByID(ConnectorBase conn, Int64 UserId, Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserId, UserId)
                .AND(Columns.PermissionId, PermissionId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    UserPermissionMap item = new UserPermissionMap();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 UserId, Int32 PermissionId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserId, UserId)
                .AND(Columns.PermissionId, PermissionId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
