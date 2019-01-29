using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL
{

    public class PromotedArea : AbstractRecord<PromotedArea>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string Id = "id";
            public static string Name = "name";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"promotedarea";
                schema.AddColumn(Columns.Id, typeof(int), 0, 0, 0,false, true, false, null);
                schema.AddColumn(Columns.Name, typeof(string), 0, 0, 0, false, false, false, null);
                
                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private int _Id = 0;
        internal string _Name = string.Empty;


        #endregion

        #region Properties

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return Id;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.Id, Id);
            qry.Insert(Columns.Name, Name);
            
            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                Id = Convert.ToInt16((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.Name, Name);
            qry.Where(Columns.Id, Id);


            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Id = Convert.ToInt32(reader[Columns.Id]);
            Name = (reader[Columns.Name]).ToString();

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static PromotedArea FetchByID(int Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    PromotedArea item = new PromotedArea();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(int Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Id, Id);
            return qry.Execute();
        }
        public static PromotedArea FetchByID(ConnectorBase conn, int Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.Id, Id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    PromotedArea item = new PromotedArea();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, int Id)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.Id, Id);
            return qry.Execute(conn);
        }
        #endregion
    }

}
