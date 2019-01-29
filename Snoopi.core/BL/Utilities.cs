using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg;
using dg.Sql;
using dg.Utilities;
using System.Web;
using System.Collections;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Globalization;

namespace Snoopi.core.BL
{
    public class Utilities
    {
        public static void QueryPager(Query q, int PageSize = 0, int CurrentPageIndex = 0)
        {
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
        }

        public static void QueryFilter(Query q, string FilterString = null, string FilterType = null, string txtFilterBy = null)
        {
            if (txtFilterBy != null)
            {
                switch (FilterType)
                {
                    case "0": q.AddWhere(FilterString, WhereComparision.EqualsTo, txtFilterBy);
                        break;
                    case "1": q.AddWhere(FilterString, WhereComparision.NotEqualsTo, txtFilterBy);
                        break;
                    case "2":
                        {
                            q.AddWhere(FilterString, WhereComparision.Like, "%" + txtFilterBy + "%");
                            break;
                        }
                }
            }
        }

        public static void QueryMultyFilter(Query q,  List<string> FilterList , string FilterString = null)
        {
            q.AddWhere(FilterString, WhereComparision.In, FilterList);
        }
    }
}