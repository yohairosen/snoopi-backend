using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppUserCard
 * AppUserCard
 * CardId:                      INT64; PRIMARY KEY; AUTOINCREMENT;
 * AppUserId:                   INT64;
 * Last4Digit:                  FIXEDSTRING(4); DEFAULT string.Empty;
 * IdNumber:                    FIXEDSTRING(10); DEFAULT string.Empty;
 * CardToken:               FIXEDSTRING(255); DEFAULT string.Empty;
 * ExpiryDate:                  FIXEDSTRING(4); DEFAULT string.Empty;
 * CreateDate:                  DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * @INDEX:                      NAME(ix_AppUserCard_CardId);[CardId]; 
 * @FOREIGNKEY:                 NAME(fk_AppUserCard_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{
    public partial class AppUserCardCollection : AbstractRecordList<AppUserCard, AppUserCardCollection>
    {
    }

    public partial class AppUserCard : AbstractRecord<AppUserCard>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string CardId = "CardId";
            public const string AppUserId = "AppUserId";
            public const string Last4Digit = "Last4Digit";
            public const string IdNumber = "IdNumber";
            public const string CardToken = "CardToken";
            public const string ExpiryDate = "ExpiryDate";
            public const string CreateDate = "CreateDate";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppUserCard";
                schema.AddColumn(Columns.CardId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Last4Digit, typeof(string), 4, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.IdNumber, typeof(string), 10, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CardToken, typeof(string), 255, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.ExpiryDate, typeof(string), 4, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("ix_AppUserCard_CardId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CardId);

                schema.AddForeignKey("fk_AppUserCard_AppUserId", AppUserCard.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CardId = 0;
        internal Int64 _AppUserId = 0;
        internal string _Last4Digit = string.Empty;
        internal string _IdNumber = string.Empty;
        internal string _CardToken = string.Empty;
        internal string _ExpiryDate = string.Empty;
        internal DateTime _CreateDate = DateTime.UtcNow;
        #endregion

        #region Properties
        public Int64 CardId
        {
            get { return _CardId; }
            set { _CardId = value; }
        }
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public string Last4Digit
        {
            get { return _Last4Digit; }
            set { _Last4Digit = value; }
        }
        public string IdNumber
        {
            get { return _IdNumber; }
            set { _IdNumber = value; }
        }
        public string CardToken
        {
            get { return _CardToken; }
            set { _CardToken = value; }
        }
        public string ExpiryDate
        {
            get { return _ExpiryDate; }
            set { _ExpiryDate = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return CardId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.Last4Digit, Last4Digit);
            qry.Insert(Columns.IdNumber, IdNumber);
            qry.Insert(Columns.CardToken, CardToken);
            qry.Insert(Columns.ExpiryDate, ExpiryDate);
            qry.Insert(Columns.CreateDate, CreateDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                CardId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.Last4Digit, Last4Digit);
            qry.Update(Columns.IdNumber, IdNumber);
            qry.Update(Columns.CardToken, CardToken);
            qry.Update(Columns.ExpiryDate, ExpiryDate);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Where(Columns.CardId, CardId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CardId = Convert.ToInt64(reader[Columns.CardId]);
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            Last4Digit = (string)reader[Columns.Last4Digit];
            IdNumber = (string)reader[Columns.IdNumber];
            CardToken = (string)reader[Columns.CardToken];
            ExpiryDate = (string)reader[Columns.ExpiryDate];
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppUserCard FetchByID(Int64 CardId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.CardId, CardId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserCard item = new AppUserCard();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CardId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.CardId, CardId);
            return qry.Execute();
        }
        public static AppUserCard FetchByID(ConnectorBase conn, Int64 CardId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.CardId, CardId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppUserCard item = new AppUserCard();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CardId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.CardId, CardId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class AppUserCard {

        public static AppUserCard FetchByAppUserId(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserCard item = new AppUserCard();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}
