using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;
using dg.Sql.Connector;

namespace Snoopi.core.BL
{
    public static class AppUserActivityController
    {
        static public void UserLoggedIn(Int64 AppUserId, DateTime PreviousLastLogin)
        {
            DateTime dateTime = DateTime.UtcNow;
            if (PreviousLastLogin.Year == dateTime.Year &&
                PreviousLastLogin.Month == dateTime.Month &&
                PreviousLastLogin.Day == dateTime.Day &&
                PreviousLastLogin.Hour == dateTime.Hour)
            {
                return;
            }
            try
            {
                using (ConnectorBase conn = ConnectorBase.NewInstance())
                {
                    //dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond).AddSeconds(-dateTime.Second).AddMinutes(-dateTime.Minute);
                    //Query qryUpdate = Query.New<AppUserHourlyActivity>()
                    //    .Update(AppUserHourlyActivity.Columns.ActiveUsers, conn.EncloseFieldName(AppUserHourlyActivity.Columns.ActiveUsers) + @"+" + 1)
                    //    .Where(AppUserHourlyActivity.Columns.Date, dateTime);

                    //if (qryUpdate.Execute(conn) == 0)
                    //{
                    //    Query qryInsert = Query.New<AppUserHourlyActivity>()
                    //        .Insert(AppUserHourlyActivity.Columns.ActiveUsers, conn.EncloseFieldName(AppUserHourlyActivity.Columns.ActiveUsers) + @"+" + 1)
                    //        .Insert(AppUserHourlyActivity.Columns.Date, dateTime);

                    //    try
                    //    {
                    //        qryInsert.Execute();
                    //    }
                    //    catch
                    //    {
                    //        qryUpdate.Execute();
                    //    }
                    //}
                }
            }
            catch { }
        }
    }
}
