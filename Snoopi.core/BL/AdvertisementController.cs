using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.BL
{
    public static class AdvertisementController
    {
        private static Random rand = new Random();

        public static List<AdCompany> GetAllCompaniesUI(bool IsSearch = false, string SearchName = "", string SearchPhone = "", int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(AdCompany.TableSchema);
            qry.SelectAllTableColumns();
            qry.Distinct();
            var wl1 = new WhereList().OR(AdCompany.TableSchema.SchemaName, AdCompany.Columns.Deleted, WhereComparision.EqualsTo, null)
             .OR(AdCompany.TableSchema.SchemaName, AdCompany.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
            qry.AddWhere(WhereCondition.AND, wl1);

            if (IsSearch == true)
            {
                WhereList wl = new WhereList();
                wl.OR(AdCompany.Columns.ContactPhone, WhereComparision.Like, SearchPhone)
                   .OR(AdCompany.Columns.Phone, WhereComparision.Like, SearchPhone);
                qry.AND(wl);
                qry.AddWhere(AdCompany.Columns.ContactName, WhereComparision.Like, SearchName);
            }
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<AdCompany> list = new List<AdCompany>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new AdCompany
                    {
                        CompanyId = Convert.ToInt64(reader[AdCompany.Columns.CompanyId]),
                        BusinessName = Convert.ToString(reader[AdCompany.Columns.BusinessName]),
                        Email = Convert.ToString(reader[AdCompany.Columns.Email]),
                        ContactPhone = Convert.ToString(reader[AdCompany.Columns.ContactPhone]),
                        Phone = Convert.ToString(reader[AdCompany.Columns.Phone]),
                        ContactName = Convert.ToString(reader[AdCompany.Columns.ContactName]),
                        CreatedDate = reader[AdCompany.Columns.CreatedDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[AdCompany.Columns.CreatedDate]),
                        Description = Convert.ToString(reader[AdCompany.Columns.Description])
                    });
                }
            }
            return list;
        }

        public static Dictionary<long, string> GetAllCompanies()
        {

            Query qry = new Query(AdCompany.TableSchema);
            qry.AddSelect(AdCompany.Columns.CompanyId);
            qry.AddSelect(AdCompany.Columns.BusinessName);
            qry.Distinct();
            var wl1 = new WhereList().OR(AdCompany.TableSchema.SchemaName, AdCompany.Columns.Deleted, WhereComparision.EqualsTo, null)
             .OR(AdCompany.TableSchema.SchemaName, AdCompany.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
            qry.AddWhere(WhereCondition.AND, wl1);

            Dictionary<long, string> list = new Dictionary<long, string>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(Convert.ToInt64(reader[AdCompany.Columns.CompanyId]),
                        Convert.ToString(reader[AdCompany.Columns.BusinessName]));
                }
            }
            return list;
        }

        public static bool DeleteCompany(int companyId)
        {
            try
            {
                Query.New<AdCompany>().Where(AdCompany.Columns.CompanyId, companyId)
                   .Update(AdCompany.Columns.Deleted, DateTime.Now)
                   .Execute();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool DeleteAd(long adId)
        {
            try
            {
                Query.New<Advertisement>().Where(Advertisement.Columns.Id, adId)
                   .Update(AdCompany.Columns.Deleted, DateTime.Now)
                   .Execute();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static Advertisement GetAdvertisementForSite(int bannerId, DateTime date)
        {
            Query qry = new Query(Advertisement.TableSchema);
            qry.SelectAll();
            qry.AddWhere(Advertisement.Columns.FromDate, WhereComparision.LessThan, date);
            qry.AddWhere(Advertisement.Columns.ToDate, WhereComparision.GreaterThan, date.AddDays(1));
            qry.AddWhere(Advertisement.Columns.BunnerId, bannerId);

            var ads = new List<Advertisement>();
            using (DataReaderBase reader = qry.ExecuteReader())
                while (reader.Read())
                    ads.Add(Advertisement.FromReader(reader));

            int num = rand.Next(0, ads.Count);
            return ads[num];
        }

        public static List<AdvertisementUI> GetAllAds(int PageSize = 0, int CurrentPageIndex = 0)
        {

            Query qry = new Query(Advertisement.TableSchema);
            qry.SelectAllTableColumns();
            qry.Distinct();
            qry.Join(JoinType.InnerJoin, Advertisement.TableSchema, Advertisement.Columns.CompanyId, Advertisement.TableSchema.SchemaName,
                AdCompany.TableSchema, AdCompany.Columns.CompanyId, AdCompany.TableSchema.SchemaName);

            var wl1 = new WhereList().OR(Advertisement.TableSchema.SchemaName, Advertisement.Columns.Deleted, WhereComparision.EqualsTo, null)
             .OR(Advertisement.TableSchema.SchemaName, Advertisement.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
            qry.AddWhere(WhereCondition.AND, wl1);
            qry.AddSelect(AdCompany.TableSchema.SchemaName, AdCompany.Columns.BusinessName, AdCompany.Columns.BusinessName);
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<AdvertisementUI> list = new List<AdvertisementUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new AdvertisementUI
                    {
                        Id = Convert.ToInt64(reader[Advertisement.Columns.Id]),
                        CompanyId = Convert.ToInt64(reader[Advertisement.Columns.CompanyId]),
                        CompanyName = (reader[AdCompany.Columns.BusinessName]).ToString(),
                        FromDate = reader[Advertisement.Columns.FromDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Advertisement.Columns.FromDate]),
                        ToDate = reader[Advertisement.Columns.ToDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Advertisement.Columns.ToDate]),
                        CreatedDate = reader[Advertisement.Columns.CreatedDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[Advertisement.Columns.CreatedDate]),
                        FilePath = reader[Advertisement.Columns.FilePath].ToString(),
                        BunnerId = (BunnerType)Convert.ToInt16(reader[Advertisement.Columns.BunnerId]),
                        Href = reader[Advertisement.Columns.Href].ToString()
                    });
                }
            }
            return list;
        }
    }


    public class AdvertisementUI
    {
        public Int64 Id { get; set; }
        public Int64 CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string FilePath { get; set; }
        public BunnerType BunnerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Deleted { get; set; }
        public string Href { get; set; }
    }
}
