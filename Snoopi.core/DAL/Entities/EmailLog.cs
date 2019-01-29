using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
* EmailLog
* EmailLog
* EmailLogId:           PRIMARY KEY; INT64; AUTOINCREMENT;
* ToList:               MEDIUMTEXT;
* CcList:               MEDIUMTEXT;
* BccList:              MEDIUMTEXT;
* FromName:             FIXEDSTRING(128);
* FromEmail:            FIXEDSTRING(128);
* MailPriority:         INT32; ACTUALTYPE System.Net.Mail.MailPriority; ToDb (Int32){0}; FromDb (System.Net.Mail.MailPriority)Convert.ToInt32({0}); DEFAULT 0;
* Subject:              FIXEDSTRING(255);
* Body:                 MEDIUMTEXT;
* DeliveryDate:         DATETIME; DEFAULT DateTime.MinValue;
* Status:               DEFAULT EmailLogStatus.None; EmailLogStatus:
*                               "EmailLogStatus"
*                               - None = 0
*                               - Unknown = 1
*                               - Sent = 2
*                               - Failed = 3
* Exception:            TEXT; NULLABLE;
* */

namespace Snoopi.core.DAL
{
    public partial class EmailLogCollection : AbstractRecordList<EmailLog, EmailLogCollection>
    {
    }

    public enum EmailLogStatus
    {
        None = 0,
        Unknown = 1,
        Sent = 2,
        Failed = 3,
    }

    public partial class EmailLog : AbstractRecord<EmailLog>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string EmailLogId = "EmailLogId";
            public static string ToList = "ToList";
            public static string CcList = "CcList";
            public static string BccList = "BccList";
            public static string FromName = "FromName";
            public static string FromEmail = "FromEmail";
            public static string MailPriority = "MailPriority";
            public static string Subject = "Subject";
            public static string Body = "Body";
            public static string DeliveryDate = "DeliveryDate";
            public static string Status = "Status"; // EmailLogStatus
            public static string Exception = "Exception";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"EmailLog";
                schema.AddColumn(Columns.EmailLogId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.ToList, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CcList, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BccList, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FromName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FromEmail, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.MailPriority, typeof(Int32), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Subject, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Body, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.DeliveryDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.Status, typeof(EmailLogStatus), 0, 0, 0, false, false, false, EmailLogStatus.None);
                schema.AddColumn(Columns.Exception, typeof(string), DataType.Text, 0, 0, 0, false, false, true, null);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _EmailLogId = 0;
        internal string _ToList = string.Empty;
        internal string _CcList = string.Empty;
        internal string _BccList = string.Empty;
        internal string _FromName = string.Empty;
        internal string _FromEmail = string.Empty;
        internal System.Net.Mail.MailPriority _MailPriority = 0;
        internal string _Subject = string.Empty;
        internal string _Body = string.Empty;
        internal DateTime _DeliveryDate = DateTime.MinValue;
        internal EmailLogStatus _Status = EmailLogStatus.None;
        internal string _Exception = null;
        #endregion

        #region Properties
        public Int64 EmailLogId
        {
            get { return _EmailLogId; }
            set { _EmailLogId = value; }
        }
        public string ToList
        {
            get { return _ToList; }
            set { _ToList = value; }
        }
        public string CcList
        {
            get { return _CcList; }
            set { _CcList = value; }
        }
        public string BccList
        {
            get { return _BccList; }
            set { _BccList = value; }
        }
        public string FromName
        {
            get { return _FromName; }
            set { _FromName = value; }
        }
        public string FromEmail
        {
            get { return _FromEmail; }
            set { _FromEmail = value; }
        }
        public System.Net.Mail.MailPriority MailPriority
        {
            get { return _MailPriority; }
            set { _MailPriority = value; }
        }
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }
        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }
        public DateTime DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; }
        }
        public EmailLogStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
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
            return EmailLogId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.ToList, ToList);
            qry.Insert(Columns.CcList, CcList);
            qry.Insert(Columns.BccList, BccList);
            qry.Insert(Columns.FromName, FromName);
            qry.Insert(Columns.FromEmail, FromEmail);
            qry.Insert(Columns.MailPriority, (Int32)MailPriority);
            qry.Insert(Columns.Subject, Subject);
            qry.Insert(Columns.Body, Body);
            qry.Insert(Columns.DeliveryDate, DeliveryDate);
            qry.Insert(Columns.Status, Status);
            qry.Insert(Columns.Exception, Exception);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                EmailLogId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.ToList, ToList);
            qry.Update(Columns.CcList, CcList);
            qry.Update(Columns.BccList, BccList);
            qry.Update(Columns.FromName, FromName);
            qry.Update(Columns.FromEmail, FromEmail);
            qry.Update(Columns.MailPriority, (Int32)MailPriority);
            qry.Update(Columns.Subject, Subject);
            qry.Update(Columns.Body, Body);
            qry.Update(Columns.DeliveryDate, DeliveryDate);
            qry.Update(Columns.Status, Status);
            qry.Update(Columns.Exception, Exception);
            qry.Where(Columns.EmailLogId, EmailLogId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            EmailLogId = Convert.ToInt64(reader[Columns.EmailLogId]);
            ToList = (string)reader[Columns.ToList];
            CcList = (string)reader[Columns.CcList];
            BccList = (string)reader[Columns.BccList];
            FromName = (string)reader[Columns.FromName];
            FromEmail = (string)reader[Columns.FromEmail];
            MailPriority = (System.Net.Mail.MailPriority)Convert.ToInt32(reader[Columns.MailPriority]);
            Subject = (string)reader[Columns.Subject];
            Body = (string)reader[Columns.Body];
            DeliveryDate = Convert.ToDateTime(reader[Columns.DeliveryDate]);
            Status = (EmailLogStatus)Convert.ToInt32(reader[Columns.Status]);
            Exception = StringOrNullFromDb(reader[Columns.Exception]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static EmailLog FetchByID(Int64 EmailLogId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.EmailLogId, EmailLogId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    EmailLog item = new EmailLog();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 EmailLogId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.EmailLogId, EmailLogId);
            return qry.Execute();
        }
        public static EmailLog FetchByID(ConnectorBase conn, Int64 EmailLogId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.EmailLogId, EmailLogId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    EmailLog item = new EmailLog();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 EmailLogId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.EmailLogId, EmailLogId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
