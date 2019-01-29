using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
* WebErrorLog
* WebErrorLog
* WebErrorLogId:           PRIMARY KEY; INT64; AUTOINCREMENT;
* Referrer:             MEDIUMTEXT;
* Url:                  MEDIUMTEXT;
* StatusCode:           INT32;
* UserId:               INT32; NULLABLE;
* UserEmail:            FIXEDSTRING(64); NULLABLE;
* AccountId:            INT32; NULLABLE;
* AccountName:          FIXEDSTRING(64); NULLABLE;
* When:                 DATETIME; DEFAULT DateTime.MinValue;
* Exception:            MEDIUMTEXT; NULLABLE;
* */

namespace Snoopi.core.DAL
{
    public partial class WebErrorLogCollection : AbstractRecordList<WebErrorLog, WebErrorLogCollection>
    {
    }

    public partial class WebErrorLog : AbstractRecord<WebErrorLog>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string WebErrorLogId = "WebErrorLogId";
            public static string Referrer = "Referrer";
            public static string Url = "Url";
            public static string StatusCode = "StatusCode";
            public static string UserId = "UserId";
            public static string UserEmail = "UserEmail";
            public static string AccountId = "AccountId";
            public static string AccountName = "AccountName";
            public static string When = "When";
            public static string Exception = "Exception";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"WebErrorLog";
                schema.AddColumn(Columns.WebErrorLogId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Referrer, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Url, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.StatusCode, typeof(Int32), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UserId, typeof(Int32), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.UserEmail, typeof(string), DataType.Char, 64, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.AccountId, typeof(Int32), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.AccountName, typeof(string), DataType.Char, 64, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.When, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.Exception, typeof(string), DataType.MediumText, 0, 0, 0, false, false, true, null);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _WebErrorLogId = 0;
        internal string _Referrer = string.Empty;
        internal string _Url = string.Empty;
        internal Int32 _StatusCode = 0;
        internal Int32? _UserId = null;
        internal string _UserEmail = null;
        internal Int32? _AccountId = null;
        internal string _AccountName = null;
        internal DateTime _When = DateTime.MinValue;
        internal string _Exception = null;
        #endregion

        #region Properties
        public Int64 WebErrorLogId
        {
            get { return _WebErrorLogId; }
            set { _WebErrorLogId = value; }
        }
        public string Referrer
        {
            get { return _Referrer; }
            set { _Referrer = value; }
        }
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
        public Int32 StatusCode
        {
            get { return _StatusCode; }
            set { _StatusCode = value; }
        }
        public Int32? UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public string UserEmail
        {
            get { return _UserEmail; }
            set { _UserEmail = value; }
        }
        public Int32? AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; }
        }
        public string AccountName
        {
            get { return _AccountName; }
            set { _AccountName = value; }
        }
        public DateTime When
        {
            get { return _When; }
            set { _When = value; }
        }
        public string Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return WebErrorLogId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Referrer, Referrer);
            qry.Insert(Columns.Url, Url);
            qry.Insert(Columns.StatusCode, StatusCode);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.UserEmail, UserEmail);
            qry.Insert(Columns.AccountId, AccountId);
            qry.Insert(Columns.AccountName, AccountName);
            qry.Insert(Columns.When, When);
            qry.Insert(Columns.Exception, Exception);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                WebErrorLogId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Referrer, Referrer);
            qry.Update(Columns.Url, Url);
            qry.Update(Columns.StatusCode, StatusCode);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.UserEmail, UserEmail);
            qry.Update(Columns.AccountId, AccountId);
            qry.Update(Columns.AccountName, AccountName);
            qry.Update(Columns.When, When);
            qry.Update(Columns.Exception, Exception);
            qry.Where(Columns.WebErrorLogId, WebErrorLogId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            WebErrorLogId = Convert.ToInt64(reader[Columns.WebErrorLogId]);
            Referrer = (string)reader[Columns.Referrer];
            Url = (string)reader[Columns.Url];
            StatusCode = Convert.ToInt32(reader[Columns.StatusCode]);
            UserId = Int32OrNullFromDb(reader[Columns.UserId]);
            UserEmail = StringOrNullFromDb(reader[Columns.UserEmail]);
            AccountId = Int32OrNullFromDb(reader[Columns.AccountId]);
            AccountName = StringOrNullFromDb(reader[Columns.AccountName]);
            When = Convert.ToDateTime(reader[Columns.When]);
            Exception = StringOrNullFromDb(reader[Columns.Exception]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static WebErrorLog FetchByID(Int64 WebErrorLogId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.WebErrorLogId, WebErrorLogId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    WebErrorLog item = new WebErrorLog();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 WebErrorLogId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.WebErrorLogId, WebErrorLogId);
            return qry.Execute();
        }
        public static WebErrorLog FetchByID(ConnectorBase conn, Int64 WebErrorLogId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.WebErrorLogId, WebErrorLogId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    WebErrorLog item = new WebErrorLog();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 WebErrorLogId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.WebErrorLogId, WebErrorLogId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
