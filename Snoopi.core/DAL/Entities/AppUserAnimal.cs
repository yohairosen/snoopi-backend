using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * AppUserAnimal
 * AppUserAnimal
 * AppUserId:               INT64; AppUser Id
 * AnimalName:              FIXEDSTRING(128); DEFAULT string.Empty;
 * AnimalType:              FIXEDSTRING(128); DEFAULT string.Empty;
 * AnimagAge:               FIXEDSTRING(128); DEFAULT string.Empty;
 * AnimalImg:               FIXEDSTRING(128); DEFAULT string.Empty;
 * CreatedOn:               DATETIME; DEFAULT DateTime.MinValue; The date that the connection was created
 * @INDEX:                  NAME(pk_AppUserAnimal_AppUserId);PRIMARYKEY;[AppUserId]; 
 * @FOREIGNKEY:             NAME(fk_AppUserAnimal_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{
    public partial class AppUserAnimalCollection : AbstractRecordList<AppUserAnimal, AppUserAnimalCollection>
    {
    }

    public partial class AppUserAnimal : AbstractRecord<AppUserAnimal>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AppUserId = "AppUserId"; // AppUser Id
            public static string AnimalName = "AnimalName";
            public static string AnimalType = "AnimalType";
            public static string AnimagAge = "AnimagAge";
            public static string AnimalImg = "AnimalImg";
            public static string CreatedOn = "CreatedOn"; // The date that the connection was created
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"AppUserAnimal";
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.AnimalName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.AnimalType, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.AnimagAge, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.AnimalImg, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CreatedOn, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("pk_AppUserAnimal_AppUserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.AppUserId);

                schema.AddForeignKey("fk_AppUserAnimal_AppUserId", AppUserAnimal.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AppUserId = 0;
        internal string _AnimalName = string.Empty;
        internal string _AnimalType = string.Empty;
        internal string _AnimagAge = string.Empty;
        internal string _AnimalImg = string.Empty;
        internal DateTime _CreatedOn = DateTime.MinValue;
        #endregion

        #region Properties
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public string AnimalName
        {
            get { return _AnimalName; }
            set { _AnimalName = value; }
        }
        public string AnimalType
        {
            get { return _AnimalType; }
            set { _AnimalType = value; }
        }
        public string AnimagAge
        {
            get { return _AnimagAge; }
            set { _AnimagAge = value; }
        }
        public string AnimalImg
        {
            get { return _AnimalImg; }
            set { _AnimalImg = value; }
        }
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set { _CreatedOn = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return AppUserId;
        }

        public override void Insert(ConnectorBase conn)
        {
            CreatedOn = DateTime.UtcNow;

            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.AnimalName, AnimalName);
            qry.Insert(Columns.AnimalType, AnimalType);
            qry.Insert(Columns.AnimagAge, AnimagAge);
            qry.Insert(Columns.AnimalImg, AnimalImg);
            qry.Insert(Columns.CreatedOn, CreatedOn);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AppUserId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.AnimalName, AnimalName);
            qry.Update(Columns.AnimalType, AnimalType);
            qry.Update(Columns.AnimagAge, AnimagAge);
            qry.Update(Columns.AnimalImg, AnimalImg);
            qry.Update(Columns.CreatedOn, CreatedOn);
            qry.Where(Columns.AppUserId, AppUserId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            AnimalName = (string)reader[Columns.AnimalName];
            AnimalType = (string)reader[Columns.AnimalType];
            AnimagAge = (string)reader[Columns.AnimagAge];
            AnimalImg = (string)reader[Columns.AnimalImg];
            CreatedOn = Convert.ToDateTime(reader[Columns.CreatedOn]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static AppUserAnimal FetchByID(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    AppUserAnimal item = new AppUserAnimal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserId, AppUserId);
            return qry.Execute();
        }
        public static AppUserAnimal FetchByID(ConnectorBase conn, Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AppUserId, AppUserId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    AppUserAnimal item = new AppUserAnimal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AppUserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AppUserId, AppUserId);
            return qry.Execute(conn);
        }
        #endregion
    }
}