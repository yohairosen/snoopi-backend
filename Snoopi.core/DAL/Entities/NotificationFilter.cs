using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL.Entities
{
    public partial class NotificationFilterCollection : AbstractRecordList<NotificationFilter, NotificationFilterCollection>
    {
    }
         public partial class NotificationFilter : AbstractRecord<NotificationFilter>
        {
     
        public NotificationFilter()
        {
            MaxFrequency = int.MaxValue;
        }
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string Id = "Id";
            public const string Name = "Name"; // Email
            public const string MinFrequency = "MinFrequency"; // Normalized email address, or other ID means
            public const string MaxFrequency = "MaxFrequency"; // Last login date/time
            public const string FromDate = "FromDate"; // The date that the user was created
            public const string ToDate = "ToDate"; // Is locked, due to deliberate locking or to password abuse
            public const string AreaId = "AreaId"; // Is this user verified
            public const string AnimalTypeId = "AnimalTypeId"; // Number of recorded bad logins, since last successful login
            public const string Priority = "Priority"; // Facebook id
            public const string GroupId = "GroupId"; // Facebook token
            public const string Deleted = "Deleted"; // Scrambled password
            public const string Created = "Created"; // Password salt
            public const string AdImageUrl = "AdImageUrl"; // Password recovery key   
            public const string MessageTemplate = "MessageTemplate";
            public const string IsAuto = "IsAuto";
            public const string LastRun = "LastRun";
            public const string RunEvery = "RunEvery";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"notification_filter";
                schema.AddColumn(Columns.Id, typeof(int), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.Name, typeof(string), 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.MinFrequency, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.MaxFrequency, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.FromDate, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ToDate, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.AreaId, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.AnimalTypeId, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.Priority, typeof(int), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.GroupId, typeof(int), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Deleted, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.Created, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.AdImageUrl, typeof(string), 1024, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.MessageTemplate, typeof(string), 256, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.IsAuto, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.LastRun, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.RunEvery, typeof(int), 0, 0, 0, false, false, false, 1);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members

        #endregion

        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int MinFrequency { get; set; }
        public int MaxFrequency { get; set; }
        public int AreaId { get; set; }
        public NotificationGroupsEnum Group { get; set; }
        public int Priority { get; set; }
        public int AnimalTypeId { get; set; }
        public DateTime? Deleted { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string AdImageUrl { get; set; }
        public string MessageTemplate { get; set; }
        public bool IsAuto { get; set; }
        public DateTime? LastRun { get; set; }
        public int RunEvery { get; set; }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return Id;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Name, Name);
            qry.Insert(Columns.FromDate, FromDate);
            qry.Insert(Columns.ToDate, ToDate);
            qry.Insert(Columns.MinFrequency, MinFrequency);
            qry.Insert(Columns.MaxFrequency, MaxFrequency);
            qry.Insert(Columns.AreaId, AreaId);
            qry.Insert(Columns.GroupId, Group);
            qry.Insert(Columns.Priority, Priority);
            qry.Insert(Columns.AnimalTypeId, AnimalTypeId);
            qry.Insert(Columns.Deleted, Deleted);
            qry.Insert(Columns.Created, Created);
            qry.Insert(Columns.AdImageUrl, AdImageUrl);
            qry.Insert(Columns.MessageTemplate, MessageTemplate);
            qry.Insert(Columns.IsAuto, IsAuto);
            qry.Insert(Columns.LastRun, LastRun);
            qry.Insert(Columns.RunEvery, RunEvery);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                Id = Convert.ToInt32((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Name, Name);
            qry.Update(Columns.FromDate, FromDate);
            qry.Update(Columns.ToDate, ToDate);
            qry.Update(Columns.MinFrequency, MinFrequency);
            qry.Update(Columns.MaxFrequency, MaxFrequency);
            qry.Update(Columns.AreaId, AreaId);
            qry.Update(Columns.GroupId, Group);
            qry.Update(Columns.Priority, Priority);
            qry.Update(Columns.AnimalTypeId, AnimalTypeId);
            qry.Update(Columns.Deleted, Deleted);
            qry.Update(Columns.Created, Created);
            qry.Update(Columns.AdImageUrl, AdImageUrl);
            qry.Update(Columns.MessageTemplate, MessageTemplate);
            qry.Update(Columns.IsAuto, IsAuto);
            qry.Update(Columns.LastRun, LastRun);
            qry.Update(Columns.RunEvery, RunEvery);

            qry.Where(Columns.Id, Id);
            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Id = Convert.ToInt32(reader[Columns.Id]);
            Name = (string)reader[Columns.Name];
            FromDate = reader[Columns.FromDate] == DBNull.Value ? (DateTime?) null : Convert.ToDateTime(reader[Columns.FromDate]);
            ToDate = reader[Columns.ToDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.ToDate]);
            MinFrequency = reader[Columns.MinFrequency] == DBNull.Value ? 0 : int.Parse (reader[Columns.MinFrequency].ToString());
            MaxFrequency = reader[Columns.MaxFrequency] == DBNull.Value ? 0 : int.Parse(reader[Columns.MaxFrequency].ToString());
            AreaId = reader[Columns.AreaId] == DBNull.Value ? 0 : int.Parse(reader[Columns.AreaId].ToString());
            Group =(NotificationGroupsEnum)( reader[Columns.GroupId] == DBNull.Value ? 0 : int.Parse(reader[Columns.GroupId].ToString()));
            Priority = reader[Columns.Priority] == DBNull.Value ? 0 : int.Parse(reader[Columns.Priority].ToString());
            AnimalTypeId = reader[Columns.AnimalTypeId] == DBNull.Value ? 0 : int.Parse(reader[Columns.AnimalTypeId].ToString());
            Deleted = reader[Columns.Deleted] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.Deleted]);
            Created = Convert.ToDateTime(reader[Columns.Created]);
            AdImageUrl = reader[Columns.AdImageUrl] == DBNull.Value ? "" : reader[Columns.AdImageUrl].ToString();
            MessageTemplate = reader[Columns.MessageTemplate] == DBNull.Value ? "" : reader[Columns.MessageTemplate].ToString();
            IsAuto = reader[Columns.IsAuto] == DBNull.Value ? false : bool.Parse(reader[Columns.IsAuto].ToString());
            LastRun = reader[Columns.LastRun] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Columns.LastRun]);
            RunEvery = reader[Columns.RunEvery] == DBNull.Value ? 0 : int.Parse(reader[Columns.RunEvery].ToString());

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static NotificationFilter FetchByID(int id)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.Id, id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    var item = new NotificationFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(int id)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.Id, id);
            return qry.Execute();
        }
        public static NotificationFilter FetchByID(ConnectorBase conn, int id)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.Id, id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    var item = new NotificationFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        //public static int Delete(ConnectorBase conn, Int64 AppUserId)
        //{
        //    Query qry = new Query(TableSchema)
        //    .Delete().Where(Columns.AppUserId, AppUserId);
        //    return qry.Execute(conn);
        //}
        #endregion
    }
    
    public enum NotificationGroupsEnum
    {
        All = 0,
        Purchase = 1,
        AddItemToCartWithoutPurchase = 2,
        Registered = 3,
        NeverPurchased = 4,
        Members = 6,
        ApprovedPromotionalContent = 7,
        NotApprovedPromotionalContent = 8,
        CreditStageWithoutPurchase = 9,
        Test = 10,
        None = 11,
        AverageOfLastThree = 12,
        DaysSinceLastPurchase = 13
    }
}
