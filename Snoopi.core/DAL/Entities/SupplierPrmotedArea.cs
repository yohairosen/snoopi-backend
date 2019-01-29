using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL
{

    public partial class SupplierPromotedArea : AbstractRecord<SupplierPromotedArea>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string Id = "id";
            public static string SupplierId = "supplier_id";
            public static string PromotedAreaId = "promoted_area_id";
            public static string StartTime = "start_time";
            public static string EndTime = "end_time";
            public static string ServiceId = "service_id";
            public static string Deleted = "deleted";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"supplierpromotedarea";
                schema.AddColumn(Columns.Id, typeof(int), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.PromotedAreaId, typeof(int), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.StartTime, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.EndTime, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ServiceId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Deleted, typeof(DateTime?), 0, 0, 0, false, false, true, null);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private int _Id = 0;
        internal Int64 _SupplierId = 0;
        internal int _PromotedAreaId = 0;
        internal DateTime _StartTime;
        internal DateTime _EndTime;
        internal Int64 _ServiceId = 0;
        internal DateTime? _Deleted;

        #endregion

        #region Properties

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
        }
        public int PromotedAreaId
        {
            get { return _PromotedAreaId; }
            set { _PromotedAreaId = value; }
        }
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        public Int64 ServiceId
        {
            get { return _ServiceId; }
            set { _ServiceId = value; }
        }
        public DateTime? Deleted
        {
            get { return _Deleted; }
            set { _Deleted = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return Id;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Id, Id);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.PromotedAreaId, PromotedAreaId);
            qry.Insert(Columns.StartTime, StartTime);
            qry.Insert(Columns.EndTime, EndTime);
            qry.Insert(Columns.ServiceId, ServiceId);
            qry.Insert(Columns.Deleted, Deleted);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                Id = Convert.ToInt16((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.PromotedAreaId, PromotedAreaId);
            qry.Update(Columns.StartTime, StartTime);
            qry.Update(Columns.EndTime, EndTime);
            qry.Update(Columns.ServiceId, ServiceId);
            qry.Update(Columns.Deleted, Deleted);
            qry.Where(Columns.Id, Id);


            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Id = Convert.ToInt32(reader[Columns.Id]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            PromotedAreaId = Convert.ToInt16(reader[Columns.PromotedAreaId]);
            StartTime = Convert.ToDateTime(reader[Columns.StartTime]);
            EndTime = Convert.ToDateTime(reader[Columns.EndTime]);
            ServiceId = Convert.ToInt64(reader[Columns.ServiceId]);
            Deleted =reader[Columns.Deleted]!=null && !string.IsNullOrEmpty(reader[Columns.Deleted].ToString()) ? (DateTime?)Convert.ToDateTime(reader[Columns.Deleted]):null;
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SupplierPromotedArea FetchByID(int Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SupplierPromotedArea item = new SupplierPromotedArea();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(int Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Id, Id);
            return qry.Execute();
        }
        public static SupplierPromotedArea FetchByID(ConnectorBase conn, int Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SupplierPromotedArea item = new SupplierPromotedArea();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, int Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Id, Id);
            return qry.Execute(conn);
        }
        #endregion
    }



}

