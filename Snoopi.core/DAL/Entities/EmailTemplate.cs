using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
* EmailTemplate
* EmailTemplate
* EmailTemplateId:          PRIMARY KEY; INT64; AUTOINCREMENT;
* Name:                     FIXEDSTRING(128);
* ToList:                   MEDIUMTEXT;
* CcList:                   MEDIUMTEXT;
* BccList:                  MEDIUMTEXT;
* FromName:                 FIXEDSTRING(128);
* FromEmail:                FIXEDSTRING(128);
* ReplyToName:              FIXEDSTRING(128);
* ReplyToEmail:             FIXEDSTRING(128);
* MailPriority:             INT32; ACTUALTYPE System.Net.Mail.MailPriority; ToDb (Int32){0}; FromDb (System.Net.Mail.MailPriority)Convert.ToInt32({0}); DEFAULT 0;
* Subject:                  FIXEDSTRING(255);
* Body:                     MEDIUMTEXT;
* CreatedBy:                FIXEDSTRING(128); NULLABLE;
* CreatedOn:                DATETIME; NULLABLE;
* ModifiedBy:               FIXEDSTRING(128); NULLABLE;
* ModifiedOn:               DATETIME; NULLABLE;
* */

namespace Snoopi.core.DAL
{
    public partial class EmailTemplateCollection : AbstractRecordList<EmailTemplate, EmailTemplateCollection>
    {
    }

    public partial class EmailTemplate : AbstractRecord<EmailTemplate>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string EmailTemplateId = "EmailTemplateId";
            public static string Name = "Name";
            public static string ToList = "ToList";
            public static string CcList = "CcList";
            public static string BccList = "BccList";
            public static string FromName = "FromName";
            public static string FromEmail = "FromEmail";
            public static string ReplyToName = "ReplyToName";
            public static string ReplyToEmail = "ReplyToEmail";
            public static string MailPriority = "MailPriority";
            public static string Subject = "Subject";
            public static string Body = "Body";
            public static string CreatedBy = "CreatedBy";
            public static string CreatedOn = "CreatedOn";
            public static string ModifiedBy = "ModifiedBy";
            public static string ModifiedOn = "ModifiedOn";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"EmailTemplate";
                schema.AddColumn(Columns.EmailTemplateId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Name, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ToList, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CcList, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BccList, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FromName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FromEmail, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ReplyToName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ReplyToEmail, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.MailPriority, typeof(Int32), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Subject, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Body, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreatedBy, typeof(string), DataType.Char, 128, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.CreatedOn, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ModifiedBy, typeof(string), DataType.Char, 128, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ModifiedOn, typeof(DateTime), 0, 0, 0, false, false, true, null);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _EmailTemplateId = 0;
        internal string _Name = string.Empty;
        internal string _ToList = string.Empty;
        internal string _CcList = string.Empty;
        internal string _BccList = string.Empty;
        internal string _FromName = string.Empty;
        internal string _FromEmail = string.Empty;
        internal string _ReplyToName = string.Empty;
        internal string _ReplyToEmail = string.Empty;
        internal System.Net.Mail.MailPriority _MailPriority = 0;
        internal string _Subject = string.Empty;
        internal string _Body = string.Empty;
        internal string _CreatedBy = null;
        internal DateTime? _CreatedOn = null;
        internal string _ModifiedBy = null;
        internal DateTime? _ModifiedOn = null;
        #endregion

        #region Properties
        public Int64 EmailTemplateId
        {
            get { return _EmailTemplateId; }
            set { _EmailTemplateId = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
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
        public string ReplyToName
        {
            get { return _ReplyToName; }
            set { _ReplyToName = value; }
        }
        public string ReplyToEmail
        {
            get { return _ReplyToEmail; }
            set { _ReplyToEmail = value; }
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
        public string CreatedBy
        {
            get { return _CreatedBy; }
            set { _CreatedBy = value; }
        }
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; }
        }
        public string ModifiedBy
        {
            get { return _ModifiedBy; }
            set { _ModifiedBy = value; }
        }
        public DateTime? ModifiedOn
        {
            get { return _ModifiedOn; }
            set { _ModifiedOn = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return EmailTemplateId;
        }

        public override void Insert(ConnectorBase conn)
        {
            CreatedBy = base.CurrentSessionUserName;
            CreatedOn = DateTime.UtcNow;

            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Name, Name);
            qry.Insert(Columns.ToList, ToList);
            qry.Insert(Columns.CcList, CcList);
            qry.Insert(Columns.BccList, BccList);
            qry.Insert(Columns.FromName, FromName);
            qry.Insert(Columns.FromEmail, FromEmail);
            qry.Insert(Columns.ReplyToName, ReplyToName);
            qry.Insert(Columns.ReplyToEmail, ReplyToEmail);
            qry.Insert(Columns.MailPriority, (Int32)MailPriority);
            qry.Insert(Columns.Subject, Subject);
            qry.Insert(Columns.Body, Body);
            qry.Insert(Columns.CreatedBy, CreatedBy);
            qry.Insert(Columns.CreatedOn, CreatedOn);
            qry.Insert(Columns.ModifiedBy, ModifiedBy);
            qry.Insert(Columns.ModifiedOn, ModifiedOn);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                EmailTemplateId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            ModifiedBy = base.CurrentSessionUserName;
            ModifiedOn = DateTime.UtcNow;

            Query qry = new Query(TableSchema);
            qry.Update(Columns.Name, Name);
            qry.Update(Columns.ToList, ToList);
            qry.Update(Columns.CcList, CcList);
            qry.Update(Columns.BccList, BccList);
            qry.Update(Columns.FromName, FromName);
            qry.Update(Columns.FromEmail, FromEmail);
            qry.Update(Columns.ReplyToName, ReplyToName);
            qry.Update(Columns.ReplyToEmail, ReplyToEmail);
            qry.Update(Columns.MailPriority, (Int32)MailPriority);
            qry.Update(Columns.Subject, Subject);
            qry.Update(Columns.Body, Body);
            qry.Update(Columns.CreatedBy, CreatedBy);
            qry.Update(Columns.CreatedOn, CreatedOn);
            qry.Update(Columns.ModifiedBy, ModifiedBy);
            qry.Update(Columns.ModifiedOn, ModifiedOn);
            qry.Where(Columns.EmailTemplateId, EmailTemplateId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            EmailTemplateId = Convert.ToInt64(reader[Columns.EmailTemplateId]);
            Name = (string)reader[Columns.Name];
            ToList = (string)reader[Columns.ToList];
            CcList = (string)reader[Columns.CcList];
            BccList = (string)reader[Columns.BccList];
            FromName = (string)reader[Columns.FromName];
            FromEmail = (string)reader[Columns.FromEmail];
            ReplyToName = (string)reader[Columns.ReplyToName];
            ReplyToEmail = (string)reader[Columns.ReplyToEmail];
            MailPriority = (System.Net.Mail.MailPriority)Convert.ToInt32(reader[Columns.MailPriority]);
            Subject = (string)reader[Columns.Subject];
            Body = (string)reader[Columns.Body];
            CreatedBy = StringOrNullFromDb(reader[Columns.CreatedBy]);
            CreatedOn = DateTimeOrNullFromDb(reader[Columns.CreatedOn]);
            ModifiedBy = StringOrNullFromDb(reader[Columns.ModifiedBy]);
            ModifiedOn = DateTimeOrNullFromDb(reader[Columns.ModifiedOn]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static EmailTemplate FetchByID(Int64 EmailTemplateId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.EmailTemplateId, EmailTemplateId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    EmailTemplate item = new EmailTemplate();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 EmailTemplateId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.EmailTemplateId, EmailTemplateId);
            return qry.Execute();
        }
        public static EmailTemplate FetchByID(ConnectorBase conn, Int64 EmailTemplateId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.EmailTemplateId, EmailTemplateId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    EmailTemplate item = new EmailTemplate();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 EmailTemplateId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.EmailTemplateId, EmailTemplateId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
