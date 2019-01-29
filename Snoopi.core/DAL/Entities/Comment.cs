using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Comment
 * Comment
 * CommentId:               PRIMARY KEY; INT64; AUTOINCREMENT;
 * Name:                    FIXEDSTRING(255); string.Empty;
 * Rate:                    DOUBLE;
 * Content:                 FIXEDSTRING(255); string.Empty;
 * AppUserId:               INT64;
 * SupplierId:              INT64;
 * BidId:                   INT64; NULLABLE; DEFAULT null;
 * Status:                  DEFAULT CommentStatus.Wait; CommentStatus:
 *                              "CommentStatus"
 *                              - Wait = 0
 *                              - Approved = 1
 *                              - Denied = 2
 * CreateDate:              DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * ApproveDate:              DATETIME; NULLABLE; DEFAULT null;
 * @INDEX:                  NAME(ix_Comment_CommentId);[CommentId];
 * @INDEX:                  NAME(ix_Comment_CreateDate);[CreateDate ASC];
 * @FOREIGNKEY:             NAME(fk_Comment_SupplierId); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; 
 * @FOREIGNKEY:             NAME(fk_Comment_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId];
 * */

namespace Snoopi.core.DAL
{
    public partial class CommentCollection : AbstractRecordList<Comment, CommentCollection>
    {
    }

    public enum CommentStatus
    {
        Wait = 0,
        Approved = 1,
        Denied = 2,
    }

    public partial class Comment : AbstractRecord<Comment>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CommentId = "CommentId";
            public static string Name = "Name";
            public static string Rate = "Rate";
            public static string Content = "Content";
            public static string AppUserId = "AppUserId";
            public static string SupplierId = "SupplierId";
            public static string BidId = "BidId";
            public static string Status = "Status"; // CommentStatus
            public static string CreateDate = "CreateDate";
            public static string ApproveDate = "ApproveDate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Comment";
                schema.AddColumn(Columns.CommentId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Name, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Rate, typeof(double), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Content, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.Status, typeof(CommentStatus), 0, 0, 0, false, false, false, CommentStatus.Wait);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.ApproveDate, typeof(DateTime), 0, 0, 0, false, false, true, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Comment_CommentId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CommentId);
                schema.AddIndex("ix_Comment_CreateDate", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CreateDate, SortDirection.ASC);

                schema.AddForeignKey("fk_Comment_SupplierId", Comment.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.None, TableSchema.ForeignKeyReference.None);
                schema.AddForeignKey("fk_Comment_AppUserId", Comment.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.None, TableSchema.ForeignKeyReference.None);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CommentId = 0;
        internal string _Name = string.Empty;
        internal double _Rate = 0d;
        internal string _Content = string.Empty;
        internal Int64 _AppUserId = 0;
        internal Int64 _SupplierId = 0;
        internal Int64? _BidId = null;
        internal CommentStatus _Status = CommentStatus.Wait;
        internal DateTime _CreateDate = DateTime.UtcNow;
        internal DateTime? _ApproveDate = null;
        #endregion

        #region Properties
        public Int64 CommentId
        {
            get { return _CommentId; }
            set { _CommentId = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public double Rate
        {
            get { return _Rate; }
            set { _Rate = value; }
        }
        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
        }
        public Int64? BidId
        {
            get { return _BidId; }
            set { _BidId = value; }
        }
        public CommentStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }
        public DateTime? ApproveDate
        {
            get { return _ApproveDate; }
            set { _ApproveDate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return CommentId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Name, Name);
            qry.Insert(Columns.Rate, Rate);
            qry.Insert(Columns.Content, Content);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.BidId, BidId);
            qry.Insert(Columns.Status, Status);
            qry.Insert(Columns.CreateDate, CreateDate);
            qry.Insert(Columns.ApproveDate, ApproveDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CommentId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Name, Name);
            qry.Update(Columns.Rate, Rate);
            qry.Update(Columns.Content, Content);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.BidId, BidId);
            qry.Update(Columns.Status, Status);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Update(Columns.ApproveDate, ApproveDate);
            qry.Where(Columns.CommentId, CommentId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CommentId = Convert.ToInt64(reader[Columns.CommentId]);
            Name = (string)reader[Columns.Name];
            Rate = Convert.ToDouble(reader[Columns.Rate]);
            Content = (string)reader[Columns.Content];
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            BidId = IsNull(reader[Columns.BidId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.BidId]);
            Status = (CommentStatus)Convert.ToInt32(reader[Columns.Status]);
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);
            ApproveDate = DateTimeOrNullFromDb(reader[Columns.ApproveDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Comment FetchByID(Int64 CommentId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CommentId, CommentId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Comment item = new Comment();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CommentId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CommentId, CommentId);
            return qry.Execute();
        }
        public static Comment FetchByID(ConnectorBase conn, Int64 CommentId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CommentId, CommentId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Comment item = new Comment();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CommentId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CommentId, CommentId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
