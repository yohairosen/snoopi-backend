using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL.Entities
{
    public enum BunnerType
    {
        home_page_bottom=1,
        home_page_left=2,
        home_page_top=3,
        page_bottom=4,
        page_left=5,
        page_top=6,
    }
    partial class Advertisement:  AbstractRecord<Advertisement>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string Id = "Id";
            public static string CompanyId = "CompanyId";
            public static string FromDate = "FromDate";
            public static string ToDate = "ToDate";
            public static string FilePath = "FilePath";
            public static string BunnerId = "BunnerId"; //BunnerType
            public static string CreatedDate = "CreatedDate";
            public static string Deleted = "Deleted";
            public static string Href = "Href";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"advertisement";
                schema.AddColumn(Columns.Id, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.CompanyId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FilePath, typeof(string), 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BunnerId, typeof(BunnerType), 0, 0, 0, false, false, false, BunnerType.home_page_top);
                schema.AddColumn(Columns.FromDate, typeof(DateTime), 0, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ToDate, typeof(DateTime), 0, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreatedDate, typeof(DateTime), 0, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Deleted, typeof(DateTime), 0, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Href, typeof(string), 0, 0, 0, 0, false, false, true, null);
                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _Id;
        internal Int64 _CompanyId;
        internal string _FilePath;
        internal BunnerType _BunnerId = BunnerType.home_page_top;
        internal DateTime? _FromDate;
        internal DateTime? _ToDate;
        internal DateTime? _CreatedDate = DateTime.Now;
        internal DateTime? _Deleted;
        internal string _Href;
        #endregion

        #region Properties
        public Int64 Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public Int64 CompanyId
        {
            get { return _CompanyId; }
            set { _CompanyId = value; }
        }
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }
        public BunnerType BunnerId
        {
            get { return _BunnerId; }
            set { _BunnerId = value; }
        }
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set { _FromDate = value; }
        }

        public DateTime? ToDate
        {
            get { return _ToDate; }
            set { _ToDate = value; }
        }
        public DateTime? CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }

        public DateTime? Deleted
        {
            get { return _Deleted; }
            set { _Deleted = value; }
        }

        public string Href
        {
            get { return _Href; }
            set { _Href = value; }
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
            qry.Insert(Columns.CompanyId, CompanyId);
            qry.Insert(Columns.BunnerId, (int)BunnerId);
            qry.Insert(Columns.FilePath, FilePath);
            qry.Insert(Columns.FromDate, FromDate);
            qry.Insert(Columns.ToDate, ToDate);
            qry.Insert(Columns.CreatedDate, CreatedDate);
            qry.Insert(Columns.Deleted, Deleted);
            qry.Insert(Columns.Href, Href);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                Id = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CompanyId, CompanyId);
            qry.Update(Columns.BunnerId, (int)BunnerId);
            qry.Update(Columns.FilePath, FilePath);
            qry.Update(Columns.FromDate, FromDate);
            qry.Update(Columns.ToDate, ToDate);
            qry.Update(Columns.CreatedDate, CreatedDate);
            qry.Update(Columns.Href, Href);

            qry.Update(Columns.Deleted, Deleted);
            qry.Where(Columns.Id, Id);
            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Id = Convert.ToInt64(reader[Columns.Id]);
            CompanyId = Convert.ToInt64(reader[Columns.CompanyId]);
            FilePath = reader[Columns.FilePath].ToString();
            BunnerId =(BunnerType) Convert.ToInt16(reader[Columns.BunnerId]);
            FromDate = reader[Columns.FromDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.FromDate]);
            ToDate = reader[Columns.ToDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.ToDate]);
            CreatedDate = reader[Columns.CreatedDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.CreatedDate]);
            Deleted = reader[Columns.Deleted] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.Deleted]);
            Href = reader[Columns.Href].ToString();
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Advertisement FetchByID(Int64 Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Advertisement item = new Advertisement();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Id, Id);
            return qry.Execute();
        }
        public static Advertisement FetchByID(ConnectorBase conn, Int64 Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Advertisement item = new Advertisement();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Id, Id);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Advertisement
    {

        public static Advertisement FetchById(string Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Advertisement item = new Advertisement();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}

