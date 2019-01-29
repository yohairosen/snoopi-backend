using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Donation
 * Donation
 * DonationId:        PRIMARY KEY; INT64; AUTOINCREMENT;
 * DonationName:      FIXEDSTRING(128); DEFAULT string.Empty; 
 * DonationPrice:     DECIMAL; DEFAULT 0;
 * DonationItem:      FIXEDSTRING(128); DEFAULT string.Empty;        
 * @INDEX:            NAME(ix_Donation);[DonationId];
 * */

namespace Snoopi.core.DAL
{

    public partial class DonationCollection : AbstractRecordList<Donation, DonationCollection>
    {
    }

    public partial class Donation : AbstractRecord<Donation>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string DonationId = "DonationId";
            public static string DonationName = "DonationName";
            public static string DonationPrice = "DonationPrice";
            public static string DonationItem = "DonationItem";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Donation";
                schema.AddColumn(Columns.DonationId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.DonationName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.DonationPrice, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.DonationItem, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);

                _TableSchema = schema;

                schema.AddIndex("ix_Donation", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.DonationId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _DonationId = 0;
        internal string _DonationName = string.Empty;
        internal decimal _DonationPrice = 0;
        internal string _DonationItem = string.Empty;
        #endregion

        #region Properties
        public Int64 DonationId
        {
            get { return _DonationId; }
            set { _DonationId = value; }
        }
        public string DonationName
        {
            get { return _DonationName; }
            set { _DonationName = value; }
        }
        public decimal DonationPrice
        {
            get { return _DonationPrice; }
            set { _DonationPrice = value; }
        }
        public string DonationItem
        {
            get { return _DonationItem; }
            set { _DonationItem = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return DonationId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.DonationName, DonationName);
            qry.Insert(Columns.DonationPrice, DonationPrice);
            qry.Insert(Columns.DonationItem, DonationItem);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                DonationId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.DonationName, DonationName);
            qry.Update(Columns.DonationPrice, DonationPrice);
            qry.Update(Columns.DonationItem, DonationItem);
            qry.Where(Columns.DonationId, DonationId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            DonationId = Convert.ToInt64(reader[Columns.DonationId]);
            DonationName = (string)reader[Columns.DonationName];
            DonationPrice = Convert.ToDecimal(reader[Columns.DonationPrice]);
            DonationItem = (string)reader[Columns.DonationItem];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Donation FetchByID(Int64 DonationId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.DonationId, DonationId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Donation item = new Donation();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 DonationId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.DonationId, DonationId);
            return qry.Execute();
        }
        public static Donation FetchByID(ConnectorBase conn, Int64 DonationId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.DonationId, DonationId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Donation item = new Donation();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 DonationId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.DonationId, DonationId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class Donation 
    {

        public static Donation GetLastDonation()
        {
            Query qry = new Query(TableSchema)
                .OrderBy(Columns.DonationId, SortDirection.DESC).LimitRows(1);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Donation item = new Donation();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}
