using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * Message
 * tbl_Message
 * MessageId:               PRIMARY KEY; INT64; AUTOINCREMENT;
 * Description:             FIXEDSTRING(1000); DEFAULT string.Empty;
 * SendingDate:             DATETIME; DEFAULT DateTime.Now; SendingDate/time
 * @INDEX:                  NAME(ik_Message_Id); [MessageId];
 * @INDEX:                  NAME(ix_Message_Date); [SendingDate ASC];
 * */

namespace Snoopi.core.DAL
{
    public partial class MessageCollection : AbstractRecordList<Message, MessageCollection>
    {
    }

    public partial class Message : AbstractRecord<Message>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string MessageId = "MessageId";
            public static string Description = "Description";
            public static string SendingDate = "SendingDate"; // SendingDate/time
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"tbl_Message";
                schema.AddColumn(Columns.MessageId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Description, typeof(string), DataType.Char, 1000, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.SendingDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.Now);

                _TableSchema = schema;

                schema.AddIndex("ik_Message_Id", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.MessageId);
                schema.AddIndex("ix_Message_Date", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SendingDate, SortDirection.ASC);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _MessageId = 0;
        internal string _Description = string.Empty;
        internal DateTime _SendingDate = DateTime.Now;
        #endregion

        #region Properties
        public Int64 MessageId
        {
            get { return _MessageId; }
            set { _MessageId = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public DateTime SendingDate
        {
            get { return _SendingDate; }
            set { _SendingDate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return MessageId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Description, Description);
            qry.Insert(Columns.SendingDate, SendingDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MessageId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Description, Description);
            qry.Update(Columns.SendingDate, SendingDate);
            qry.Where(Columns.MessageId, MessageId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            MessageId = Convert.ToInt64(reader[Columns.MessageId]);
            Description = (string)reader[Columns.Description];
            SendingDate = Convert.ToDateTime(reader[Columns.SendingDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Message FetchByID(Int64 MessageId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.MessageId, MessageId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Message item = new Message();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 MessageId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.MessageId, MessageId);
            return qry.Execute();
        }
        public static Message FetchByID(ConnectorBase conn, Int64 MessageId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.MessageId, MessageId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Message item = new Message();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 MessageId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.MessageId, MessageId);
            return qry.Execute(conn);
        }
        #endregion
    }
}