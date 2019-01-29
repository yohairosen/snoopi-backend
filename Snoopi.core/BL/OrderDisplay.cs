using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using dg.Sql;
using dg.Utilities;
using Snoopi.core.DAL;
using Snoopi.core.Caching.Manager;
using System.Globalization;
using dg.Utilities.Apns;

namespace Snoopi.core.BL
{
    //public class OrderDisplay
    //{
    //    public int OrderDisplay { get; set; }
    //    public Int64 itemId { get; set; }
    //    public string itemName { get; set; }

    //    public static List<OrderDisplay> GetAllItemsOrdered()
    //    {
    //        Query qry = new Query(AppUser.TableSchema);
    //        qry.Select(AppUser.TableSchema.SchemaName, AppUser.Columns.OrderDisplay, AppUser.Columns.OrderDisplay, true);
    //        qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, "itemId");
    //        qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Email, "itemName");
    //        qry.OrderBy(AppUser.Columns.OrderDisplay, SortDirection.ASC);

    //        List<OrderDisplay> list = new List<OrderDisplay>();
    //        using (DataReaderBase reader = qry.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                list.Add(new OrderDisplay
    //                {
    //                    OrderDisplay = reader.GetInt32(0),
    //                    itemId = reader.GetInt64OrZero(1),
    //                    itemName = reader.GetStringOrEmpty(2),
    //                });
    //            }
    //        }
    //        return list;
    //    }

    //    public static int GetLastOrder()
    //    {
    //        Query qry = new Query(AppUser.TableSchema);
    //        qry.Select(AppUser.TableSchema.SchemaName, AppUser.Columns.OrderDisplay, AppUser.Columns.OrderDisplay, true);

    //        return (int)qry.GetMax(AppUser.Columns.OrderDisplay); ;
    //    }

    //}
}
