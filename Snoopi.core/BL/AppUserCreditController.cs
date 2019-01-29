using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using AnyGym.core.DAL;
using dg.Sql;
using dg.Utilities;

namespace AnyGym.core.BL
{
    public static class AppUserCreditController
    {
        public static bool AddCreditToAppUser(Int64 AppUserId, decimal Credit)
        {
            if (Credit == 0) return true;
            using (ConnectorBase connector = ConnectorBase.NewInstance())
            {
                Query qry = new Query(AppUserBalance.TableSchema)
                    .Update(AppUserBalance.Columns.Credit, connector.encloseFieldName(AppUserBalance.Columns.Credit) + (Credit > 0 ? (@"+" + Credit) : (@"-" + -Credit)), true)
                    .Where(AppUserBalance.Columns.AppUserId, AppUserId);
                if (Credit < 0)
                {
                    qry.AddWhere(WhereCondition.AND, connector.encloseFieldName(AppUserBalance.Columns.Credit) + (Credit > 0 ? (@"+" + Credit) : (@"-" + -Credit)), ValueObjectType.Literal, WhereComparision.GreaterThanOrEqual, 0, ValueObjectType.Value);
                }
                int changes = qry.Execute(connector);
                return changes > 0;
            }
        }
        public static decimal CreditForAppUser(Int64 AppUserId)
        {
            Query qry = new Query(AppUserBalance.TableSchema).Select(AppUserBalance.Columns.Credit).Where(AppUserBalance.Columns.AppUserId, AppUserId).LimitRows(1);
            object ret = qry.ExecuteScalar();
            if (ret.IsNull()) return 0;
            return (decimal)ret;
        }
    }
}
