using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;
using Snoopi.infrastructure;
using Snoopi.infrastructure.Loggers;
using System.Diagnostics;

/*
 * Order
 * tbl_Order
 * OrderId:                 PRIMARY KEY; INT64; AUTOINCREMENT;
 * SpecialInstructions:     FIXEDSTRING(255); string.Empty;
 * OfferId:                 INT64;
 * AppUserId:               INT64;
 * DonationId:              INT64; NULLABLE;
 * BidId:                   INT64;
 * CampaignId:              INT64; NULLABLE;
 * IsSendRecived:           BOOL; DEFAULT false;
 * IsSendRateSupplier:      BOOL; DEFAULT false;
 * Last4Digits:             FIXEDSTRING(4); string.Empty;
 * ExpiryDate:              FIXEDSTRING(4); string.Empty;        
 * SupplierRemarks:         FIXEDSTRING(255); string.Empty;
 * Transaction:             FIXEDSTRING(255); string.Empty;
 * TransactionErrorMessage: FIXEDSTRING(255); string.Empty;
 * TransactionResponseCode: FIXEDSTRING(128); string.Empty;
 * TransactionStatus:       DEFAULT OrderStatus.NotPayed; OrderStatus:
 *                              "OrderStatus"
 *                              - NotPayed = 0
 *                              - Payed = 1
 *                              - Canceled = 2
 * PaySupplierStatus:       DEFAULT PaymentStatus.NotPayed; PaymentStatus:
 *                              "PaymentStatus"
 *                              - NotPayed = 0
 *                              - Payed = 1
 *                              - Canceled = 2
 * UserPaySupplierStatus:    DEFAULT UserPaymentStatus.NotPayed; UserPaymentStatus:
 *                              "UserPaymentStatus"
 *                              - NotPayed = 0
 *                              - Payed = 1
 *                              - Canceled = 2
 * SuppliedDate:            DATETIME; NULLABLE;
 * ReceivedDate:            DATETIME; NULLABLE;
 * TotalPrice:              DECIMAL; DEFAULT 0;
 * PriceAfterDiscount:      DECIMAL; DEFAULT 0;
 * PrecentDiscount:         DECIMAL; DEFAULT 0;
 * Remarks:                 FIXEDSTRING(255); string.Empty;
 * CreateDate:              DATETIME; DEFAULT DateTime.MinValue; ACTUALDEFAULT DateTime.UtcNow;
 * @FOREIGNKEY:             NAME(fk_Order_BidId); FOREIGNTABLE(Bid); COLUMNS[BidId]; FOREIGNCOLUMNS[BidId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_Order_OfferId); FOREIGNTABLE(Offer); COLUMNS[OfferId]; FOREIGNCOLUMNS[OfferId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_Order_AppUserId); FOREIGNTABLE(AppUser); COLUMNS[AppUserId]; FOREIGNCOLUMNS[AppUserId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_Order_CampaignId); FOREIGNTABLE(Campaign); COLUMNS[CampaignId]; FOREIGNCOLUMNS[CampaignId];
 * @INDEX:                  NAME(ix_Order_OrderId);[OrderId];
 * @INDEX:                  NAME(ix_Order_AppUserId);[AppUserId];
 * @INDEX:                  NAME(ix_Order_OfferId);[OfferId];
 * @INDEX:                  NAME(ix_Order_BidId);[BidId];
 * */

namespace Snoopi.core.DAL
{
    public partial class OrderCollection : AbstractRecordList<Order, OrderCollection>
    {
    }

    public enum OrderStatus
    {
        NotPayed = 0,
        Payed = 1,
        Canceled = 2,
    }

    public enum PaymentStatus
    {
        NotPayed = 0,
        Payed = 1,
        Canceled = 2,
    }

    public enum UserPaymentStatus
    {
        NotPayed = 0,
        Payed = 1,
        Canceled = 2,
    }
    public enum Source
    {
        Unknown=0,
        Application = 1,
        WebSite = 2,
    }
    public partial class Order : AbstractRecord<Order>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string OrderId = "OrderId";
            public const string SpecialInstructions = "SpecialInstructions";
            public const string AppUserId = "AppUserId";
            public const string DonationId = "DonationId";
            public const string BidId = "BidId";
            public const string CampaignId = "CampaignId";
            public const string IsSendRecived = "IsSendRecived";
            public const string IsSendRateSupplier = "IsSendRateSupplier";
            public const string Last4Digits = "Last4Digits";
            public const string ExpiryDate = "ExpiryDate";
            public const string SupplierRemarks = "SupplierRemarks";
            public const string Transaction = "Transaction";
            public const string TransactionErrorMessage = "TransactionErrorMessage";
            public const string TransactionResponseCode = "TransactionResponseCode";
            public const string TransactionStatus = "TransactionStatus"; // OrderStatus
            public const string PaySupplierStatus = "PaySupplierStatus"; // PaymentStatus
            public const string UserPaySupplierStatus = "UserPaySupplierStatus"; // UserPaymentStatus
            public const string SuppliedDate = "SuppliedDate";
            public const string ReceivedDate = "ReceivedDate";
            public const string TotalPrice = "TotalPrice";
            public const string PriceAfterDiscount = "PriceAfterDiscount";
            public const string PrecentDiscount = "PrecentDiscount";
            public const string Remarks = "Remarks";
            public const string CreateDate = "CreateDate";
            public const string SupplierId = "SupplierId";
            public const string Gifts = "Gifts";
            public const string AuthNumber = "AuthNumber";
            public const string NumOfPayments = "NumOfPayments";
            public const string Source = "Source";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"tbl_Order";
                schema.AddColumn(Columns.OrderId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SpecialInstructions, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.AppUserId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.DonationId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CampaignId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.IsSendRecived, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.IsSendRateSupplier, typeof(bool), 0, 0, 0, false, false, false, false);
                schema.AddColumn(Columns.Last4Digits, typeof(string), DataType.Char, 4, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ExpiryDate, typeof(string), DataType.Char, 4, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierRemarks, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Transaction, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.TransactionErrorMessage, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.TransactionResponseCode, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.TransactionStatus, typeof(OrderStatus), 0, 0, 0, false, false, false, OrderStatus.NotPayed);
                schema.AddColumn(Columns.PaySupplierStatus, typeof(PaymentStatus), 0, 0, 0, false, false, false, PaymentStatus.NotPayed);
                schema.AddColumn(Columns.UserPaySupplierStatus, typeof(UserPaymentStatus), 0, 0, 0, false, false, false, UserPaymentStatus.NotPayed);
                schema.AddColumn(Columns.SuppliedDate, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.ReceivedDate, typeof(DateTime), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.TotalPrice, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.PriceAfterDiscount, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.PrecentDiscount, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Remarks, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.Gifts, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.AuthNumber, typeof(string), DataType.VarChar, 20, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.NumOfPayments, typeof(int), 0, 0, 0, false, false, false, null);
               schema.AddColumn(Columns.Source, typeof(int), 0, 0, 0, false, false, false, 0);

                _TableSchema = schema;

                schema.AddIndex("ix_Order_OrderId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.OrderId);
                schema.AddIndex("ix_Order_AppUserId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.AppUserId);
                schema.AddIndex("ix_Order_BidId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.BidId);

                schema.AddForeignKey("fk_Order_BidId", Order.Columns.BidId, Bid.TableSchema.SchemaName, Bid.Columns.BidId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_Order_SupplierId", Order.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.None, TableSchema.ForeignKeyReference.None);
                schema.AddForeignKey("fk_Order_AppUserId", Order.Columns.AppUserId, AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_Order_CampaignId", Order.Columns.CampaignId, Campaign.TableSchema.SchemaName, Campaign.Columns.CampaignId, TableSchema.ForeignKeyReference.None, TableSchema.ForeignKeyReference.None);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _OrderId = 0;
        internal string _SpecialInstructions = string.Empty;
        internal Int64 _AppUserId = 0;
        internal Int64? _DonationId = null;
        internal Int64 _BidId = 0;
        internal Int64? _CampaignId = null;
        internal bool _IsSendRecived = false;
        internal bool _IsSendRateSupplier = false;
        internal string _Last4Digits = string.Empty;
        internal string _ExpiryDate = string.Empty;
        internal string _SupplierRemarks = string.Empty;
        internal string _Transaction = string.Empty;
        internal string _TransactionErrorMessage = string.Empty;
        internal string _TransactionResponseCode = string.Empty;
        internal OrderStatus _TransactionStatus = OrderStatus.NotPayed;
        internal PaymentStatus _PaySupplierStatus = PaymentStatus.NotPayed;
        internal UserPaymentStatus _UserPaySupplierStatus = UserPaymentStatus.NotPayed;
        internal DateTime? _SuppliedDate = null;
        internal DateTime? _ReceivedDate = null;
        internal decimal _TotalPrice = 0;
        internal decimal _PriceAfterDiscount = 0;
        internal decimal _PrecentDiscount = 0;
        internal string _Remarks = string.Empty;
        internal DateTime _CreateDate = DateTime.UtcNow;
        internal Int64? _supplierId = null;
        internal string _gifts = string.Empty;
        internal string _authNumber = string.Empty;
        internal int _numOfPayments = 1;
        internal int? _source =0;
        #endregion

        #region Properties
        public Int64 OrderId
        {
            get { return _OrderId; }
            set { _OrderId = value; }
        }
        public string SpecialInstructions
        {
            get { return _SpecialInstructions; }
            set { _SpecialInstructions = value; }
        }
       
        public Int64 AppUserId
        {
            get { return _AppUserId; }
            set { _AppUserId = value; }
        }
        public Int64? DonationId
        {
            get { return _DonationId; }
            set { _DonationId = value; }
        }
        public Int64 BidId
        {
            get { return _BidId; }
            set { _BidId = value; }
        }
        public Int64? SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }

        public Int64? CampaignId
        {
            get { return _CampaignId; }
            set { _CampaignId = value; }
        }
        public bool IsSendRecived
        {
            get { return _IsSendRecived; }
            set { _IsSendRecived = value; }
        }
        public bool IsSendRateSupplier
        {
            get { return _IsSendRateSupplier; }
            set { _IsSendRateSupplier = value; }
        }
        public string Last4Digits
        {
            get { return _Last4Digits; }
            set { _Last4Digits = value; }
        }
        public string ExpiryDate
        {
            get { return _ExpiryDate; }
            set { _ExpiryDate = value; }
        }
        public string SupplierRemarks
        {
            get { return _SupplierRemarks; }
            set { _SupplierRemarks = value; }
        }
        public string Transaction
        {
            get { return _Transaction; }
            set { _Transaction = value; }
        }
        public string TransactionErrorMessage
        {
            get { return _TransactionErrorMessage; }
            set { _TransactionErrorMessage = value; }
        }
        public string TransactionResponseCode
        {
            get { return _TransactionResponseCode; }
            set { _TransactionResponseCode = value; }
        }
        public OrderStatus TransactionStatus
        {
            get { return _TransactionStatus; }
            set { _TransactionStatus = value; }
        }
        public PaymentStatus PaySupplierStatus
        {
            get { return _PaySupplierStatus; }
            set { _PaySupplierStatus = value; }
        }
        public UserPaymentStatus UserPaySupplierStatus
        {
            get { return _UserPaySupplierStatus; }
            set { _UserPaySupplierStatus = value; }
        }
        public DateTime? SuppliedDate
        {
            get { return _SuppliedDate; }
            set { _SuppliedDate = value; }
        }
        public DateTime? ReceivedDate
        {
            get { return _ReceivedDate; }
            set { _ReceivedDate = value; }
        }
        public decimal TotalPrice
        {
            get { return _TotalPrice; }
            set { _TotalPrice = value; }
        }
        public decimal PriceAfterDiscount
        {
            get { return _PriceAfterDiscount; }
            set { _PriceAfterDiscount = value; }
        }
        public decimal PrecentDiscount
        {
            get { return _PrecentDiscount; }
            set { _PrecentDiscount = value; }
        }
        public string Remarks
        {
            get { return _Remarks; }
            set { _Remarks = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }
        public string Gifts
        {
            get { return _gifts; }
            set { _gifts = value; }
        }

        public string AuthNumber
        {
            get { return _authNumber; }
            set { _authNumber = value; }
        }

        public int NumOfPayments
        {
            get { return _numOfPayments;}
            set { _numOfPayments = value; }
        }
        public int? Source
        {
            get { return _source;}
            set { _source = value; }
        }

        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return OrderId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SpecialInstructions, SpecialInstructions);
            qry.Insert(Columns.AppUserId, AppUserId);
            qry.Insert(Columns.DonationId, DonationId);
            qry.Insert(Columns.BidId, BidId);
            qry.Insert(Columns.CampaignId, CampaignId);
            qry.Insert(Columns.IsSendRecived, IsSendRecived);
            qry.Insert(Columns.IsSendRateSupplier, IsSendRateSupplier);
            qry.Insert(Columns.Last4Digits, Last4Digits);
            qry.Insert(Columns.ExpiryDate, ExpiryDate);
            qry.Insert(Columns.SupplierRemarks, SupplierRemarks);
            qry.Insert(Columns.Transaction, Transaction);
            qry.Insert(Columns.TransactionErrorMessage, TransactionErrorMessage);
            qry.Insert(Columns.TransactionResponseCode, TransactionResponseCode);
            qry.Insert(Columns.TransactionStatus, TransactionStatus);
            qry.Insert(Columns.PaySupplierStatus, PaySupplierStatus);
            qry.Insert(Columns.UserPaySupplierStatus, UserPaySupplierStatus);
            qry.Insert(Columns.SuppliedDate, SuppliedDate);
            qry.Insert(Columns.ReceivedDate, ReceivedDate);
            qry.Insert(Columns.TotalPrice, TotalPrice);
            qry.Insert(Columns.PriceAfterDiscount, PriceAfterDiscount);
            qry.Insert(Columns.PrecentDiscount, PrecentDiscount);
            qry.Insert(Columns.Remarks, Remarks);
            qry.Insert(Columns.CreateDate, CreateDate);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.Gifts, Gifts);
            qry.Insert(Columns.AuthNumber, AuthNumber);
            qry.Insert(Columns.NumOfPayments, NumOfPayments);
            qry.Insert(Columns.Source, Source);
            Helpers.LogState(this);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                OrderId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SpecialInstructions, SpecialInstructions);
            qry.Update(Columns.AppUserId, AppUserId);
            qry.Update(Columns.DonationId, DonationId);
            qry.Update(Columns.BidId, BidId);
            qry.Update(Columns.CampaignId, CampaignId);
            qry.Update(Columns.IsSendRecived, IsSendRecived);
            qry.Update(Columns.IsSendRateSupplier, IsSendRateSupplier);
            qry.Update(Columns.Last4Digits, Last4Digits);
            qry.Update(Columns.ExpiryDate, ExpiryDate);
            qry.Update(Columns.SupplierRemarks, SupplierRemarks);
            qry.Update(Columns.Transaction, Transaction);
            qry.Update(Columns.TransactionErrorMessage, TransactionErrorMessage);
            qry.Update(Columns.TransactionResponseCode, TransactionResponseCode);
            qry.Update(Columns.TransactionStatus, TransactionStatus);
            qry.Update(Columns.PaySupplierStatus, PaySupplierStatus);
            qry.Update(Columns.UserPaySupplierStatus, UserPaySupplierStatus);
            qry.Update(Columns.SuppliedDate, SuppliedDate);
            qry.Update(Columns.ReceivedDate, ReceivedDate);
            qry.Update(Columns.TotalPrice, TotalPrice);
            qry.Update(Columns.PriceAfterDiscount, PriceAfterDiscount);
            qry.Update(Columns.PrecentDiscount, PrecentDiscount);
            qry.Update(Columns.Remarks, Remarks);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.Gifts, Gifts);
            qry.Update(Columns.AuthNumber, AuthNumber);
            qry.Update(Columns.NumOfPayments, NumOfPayments);
            qry.Where(Columns.OrderId, OrderId);

            Helpers.LogState(this);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            OrderId = Convert.ToInt64(reader[Columns.OrderId]);
            SpecialInstructions = (string)reader[Columns.SpecialInstructions];
            AppUserId = Convert.ToInt64(reader[Columns.AppUserId]);
            DonationId = IsNull(reader[Columns.DonationId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.DonationId]);
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            CampaignId = IsNull(reader[Columns.CampaignId]) ? (Int64?)null : Convert.ToInt64(reader[Columns.CampaignId]);
            IsSendRecived = Convert.ToBoolean(reader[Columns.IsSendRecived]);
            IsSendRateSupplier = Convert.ToBoolean(reader[Columns.IsSendRateSupplier]);
            Last4Digits = (string)reader[Columns.Last4Digits];
            ExpiryDate = (string)reader[Columns.ExpiryDate];
            SupplierRemarks = (string)reader[Columns.SupplierRemarks];
            Transaction = (string)reader[Columns.Transaction];
            TransactionErrorMessage = (string)reader[Columns.TransactionErrorMessage];
            TransactionResponseCode = (string)reader[Columns.TransactionResponseCode];
            TransactionStatus = (OrderStatus)Convert.ToInt32(reader[Columns.TransactionStatus]);
            PaySupplierStatus = (PaymentStatus)Convert.ToInt32(reader[Columns.PaySupplierStatus]);
            UserPaySupplierStatus = (UserPaymentStatus)Convert.ToInt32(reader[Columns.UserPaySupplierStatus]);
            SuppliedDate = DateTimeOrNullFromDb(reader[Columns.SuppliedDate]);
            ReceivedDate = DateTimeOrNullFromDb(reader[Columns.ReceivedDate]);
            TotalPrice = Convert.ToDecimal(reader[Columns.TotalPrice]);
            PriceAfterDiscount = Convert.ToDecimal(reader[Columns.PriceAfterDiscount]);
            PrecentDiscount = Convert.ToDecimal(reader[Columns.PrecentDiscount]);
            Remarks = (string)reader[Columns.Remarks];
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);
            SupplierId = IsNull(reader[Columns.SupplierId]) ? (long?)null  : Convert.ToInt64(reader[Columns.SupplierId]);
            Gifts = IsNull(reader[Columns.Gifts]) ? "" : (string)reader[Columns.Gifts];
            AuthNumber = IsNull(reader[Columns.AuthNumber]) ? "" : (string)reader[Columns.AuthNumber];
            NumOfPayments = (int)Convert.ToInt32(reader[Columns.NumOfPayments]);
            Source = (int)Convert.ToInt16(reader[Columns.Source]);
            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Order FetchByID(Int64 OrderId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.OrderId, OrderId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Order item = new Order();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 OrderId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.OrderId, OrderId);

            return qry.Execute();
        }
        public static Order FetchByID(ConnectorBase conn, Int64 OrderId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.OrderId, OrderId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Order item = new Order();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 OrderId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.OrderId, OrderId);

            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Order
    {
        public static Order FetchByBidId(Int64 BidId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.BidId, BidId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Order item = new Order();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
        public static Order FetchByOrderId(Int64 OrderId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.OrderId, OrderId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Order item = new Order();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }



}
