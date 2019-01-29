using dg.Sql;
using dg.Sql.Connector;
using System;

namespace Snoopi.core.DAL.Entities
{
   public partial class AdCompany: AbstractRecord<AdCompany>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CompanyId = "CompanyId";
            public static string Email = "Email";
            public static string ContactName = "ContactName";
            public static string BusinessName = "BusinessName";
            public static string ContactPhone = "ContactPhone";
            public static string Phone = "Phone";
            public static string CreatedDate = "CreatedDate";
            public static string Description = "Description";
            public static string Deleted = "Deleted";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"adCompanies";
                schema.AddColumn(Columns.CompanyId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Email, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ContactName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BusinessName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ContactPhone, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Phone, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreatedDate, typeof(DateTime),0, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Description, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Deleted, typeof(DateTime),0, 0, 0, 0, false, false, false, null);
                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CompanyId = 0;
        internal string _Email = string.Empty;
        internal string _ContactName = string.Empty;
        internal string _BusinessName = string.Empty;
        internal string _ContactPhone = string.Empty;
        internal string _Phone = string.Empty;
        internal DateTime? _CreatedDate = DateTime.Now;
        internal string _Description = string.Empty;
        internal DateTime? _Deleted = null;
        #endregion

        #region Properties
        public Int64 CompanyId
        {
            get { return _CompanyId; }
            set { _CompanyId = value; }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        public string ContactName
        {
            get { return _ContactName; }
            set { _ContactName = value; }
        }
            public string BusinessName
        {
            get { return _BusinessName; }
            set { _BusinessName = value; }
        }
            public string ContactPhone
        {
            get { return _ContactPhone; }
            set { _ContactPhone = value; }
        }
            public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }
            public DateTime? CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }
            public string Description
        {
            get { return _Description; }
            set { _Description = value; }
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
            return CompanyId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.BusinessName, BusinessName);
            qry.Insert(Columns.ContactName, ContactName);
            qry.Insert(Columns.ContactPhone, ContactPhone);
            qry.Insert(Columns.CreatedDate, CreatedDate);
            qry.Insert(Columns.Deleted, Deleted);
            qry.Insert(Columns.Description, Description);
            qry.Insert(Columns.Email, Email);
            qry.Insert(Columns.Phone, Phone);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CompanyId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.BusinessName, BusinessName);
            qry.Update(Columns.ContactName, ContactName);
            qry.Update(Columns.ContactPhone, ContactPhone);
            qry.Update(Columns.CreatedDate, CreatedDate);
            qry.Update(Columns.Deleted, Deleted);
            qry.Update(Columns.Description, Description);
            qry.Update(Columns.Email, Email);
            qry.Update(Columns.Phone, Phone);
            qry.Where(Columns.CompanyId, CompanyId);
            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CompanyId = Convert.ToInt64(reader[Columns.CompanyId]);
            BusinessName = (string)reader[Columns.BusinessName];
            ContactName = (string)reader[Columns.ContactName];
            ContactPhone = (string)reader[Columns.ContactPhone];
            CreatedDate = reader[Columns.CreatedDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.CreatedDate]);
            Deleted = reader[Columns.Deleted] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.Deleted]);
            Description = (string)reader[Columns.Description];
            Email = (string)reader[Columns.Email];
            Phone = (string)reader[Columns.Phone];
            IsThisANewRecord = false; 
        }
        #endregion

        #region Helpers
        public static AdCompany FetchByID(Int64 CompanyId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CompanyId, CompanyId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AdCompany item = new AdCompany();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CompanyId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CompanyId, CompanyId);
            return qry.Execute();
        }
        public static AdCompany FetchByID(ConnectorBase conn, Int64 CompanyId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CompanyId, CompanyId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AdCompany item = new AdCompany();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CompanyId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CompanyId, CompanyId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class AdCompany
    {

        public static AdCompany FetchByEmail(string CompanyEmail)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Email, CompanyEmail);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AdCompany item = new AdCompany();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}
