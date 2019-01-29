using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities.Serialization;
using Newtonsoft;
using Newtonsoft.Json;

/*
 * UserProfile
 * UserProfile
 * UserId:                  PRIMARY KEY; INT64;
 * FirstName:               FIXEDSTRING(64); First name
 * LastName:                FIXEDSTRING(64); Last name
 * Phone:                   FIXEDSTRING(64); DEFAULT string.Empty; Contact person's street address
 * DefaultLangCode:         FIXEDSTRING(5); DEFAULT "auto"; The default language code
 * Settings:                MEDIUMTEXT; ACTUALTYPE AttributesContainer; TODB JsonConvert.SerializeObject({0}, Formatting.None); FROMDB JsonConvert.DeserializeObject<AttributesContainer>((string){0}) ?? new AttributesContainer(); DEFAULT new AttributesContainer(); Settings
 * @FOREIGNKEY:             NAME(fk_UserProfile_UserId); FOREIGNTABLE(User); COLUMNS[UserId]; FOREIGNCOLUMNS[UserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                  NAME(ix_UserProfile_UserId);[UserId]; 
 * */

namespace Snoopi.core.DAL
{
    public partial class UserProfileCollection : AbstractRecordList<UserProfile, UserProfileCollection>
    {
    }

    public partial class UserProfile : AbstractRecord<UserProfile>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string UserId = "UserId";
            public static string FirstName = "FirstName"; // First name
            public static string LastName = "LastName"; // Last name
            public static string Phone = "Phone"; // Contact person's street address
            public static string DefaultLangCode = "DefaultLangCode"; // The default language code
            public static string Settings = "Settings"; // Settings
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"UserProfile";
                schema.AddColumn(Columns.UserId, typeof(Int32), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.FirstName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.LastName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Phone, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.DefaultLangCode, typeof(string), DataType.Char, 5, 0, 0, false, false, false, "auto");
                schema.AddColumn(Columns.Settings, typeof(string), DataType.MediumText, 0, 0, 0, false, false, false, new AttributesContainer());

                _TableSchema = schema;

                schema.AddIndex("ix_UserProfile_UserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.UserId);

                schema.AddForeignKey("fk_UserProfile_UserId", UserProfile.Columns.UserId, User.TableSchema.SchemaName, User.Columns.UserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _UserId = 0;
        internal string _FirstName = string.Empty;
        internal string _LastName = string.Empty;
        internal string _Phone = string.Empty;
        internal string _DefaultLangCode = "auto";
        internal AttributesContainer _Settings = new AttributesContainer();
        #endregion

        #region Properties
        public Int64 UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }
        public string DefaultLangCode
        {
            get { return _DefaultLangCode; }
            set { _DefaultLangCode = value; }
        }
        public AttributesContainer Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return UserId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.FirstName, FirstName);
            qry.Insert(Columns.LastName, LastName);
            qry.Insert(Columns.Phone, Phone);
            qry.Insert(Columns.DefaultLangCode, DefaultLangCode);
            qry.Insert(Columns.Settings, JsonConvert.SerializeObject(Settings, Formatting.None));

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                UserId = Convert.ToInt32((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.FirstName, FirstName);
            qry.Update(Columns.LastName, LastName);
            qry.Update(Columns.Phone, Phone);
            qry.Update(Columns.DefaultLangCode, DefaultLangCode);
            qry.Update(Columns.Settings, JsonConvert.SerializeObject(Settings, Formatting.None));
            qry.Where(Columns.UserId, UserId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            UserId = Convert.ToInt32(reader[Columns.UserId]);
            FirstName = (string)reader[Columns.FirstName];
            LastName = (string)reader[Columns.LastName];
            Phone = (string)reader[Columns.Phone];
            DefaultLangCode = (string)reader[Columns.DefaultLangCode];
            Settings = JsonConvert.DeserializeObject<AttributesContainer>((string)reader[Columns.Settings]) ?? new AttributesContainer();

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static UserProfile FetchByID(Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserId, UserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    UserProfile item = new UserProfile();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserId, UserId);
            return qry.Execute();
        }
        public static UserProfile FetchByID(ConnectorBase conn, Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UserId, UserId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    UserProfile item = new UserProfile();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 UserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UserId, UserId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
