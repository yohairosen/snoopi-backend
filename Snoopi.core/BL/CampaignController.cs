using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dg.Utilities;
using dg.Sql;
using Snoopi.core.DAL;

namespace Snoopi.core.BL
{
    public class CampaignController
    {

        public static CampaignCollection GetAllCampaign(int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(Campaign.TableSchema);

            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            return CampaignCollection.FetchByQuery(qry);
        }
    }
}
