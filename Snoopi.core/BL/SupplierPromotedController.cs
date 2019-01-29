using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.BL
{
    public static class SupplierPromotedController
    {
        public static List<SupplierPromotedUI> GetPromotedAreaSuppliers(string[] servicesIds,int AreaId = 0, int PageSize = 0, int CurrentPageIndex = 0)
        {
            var listOfPromotedArea = new List<SupplierPromotedUI>();
            Query qry = new Query(Service.TableSchema);
            qry.AddWhere(Service.TableSchema.SchemaName, Service.Columns.ServiceId, WhereComparision.In, servicesIds);

            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var supplierPromotedUI = new SupplierPromotedUI();
                    //supplierPromotedUI.PromotedAreaName = (string)reader[PromotedArea.Columns.Name];
                    //supplierPromotedUI.PromotedAreaId = (int)reader[PromotedArea.Columns.Id];
                    supplierPromotedUI.ServiceName = (string)reader[Service.Columns.ServiceName];
                    supplierPromotedUI.ServiceId = (Int64)reader[Service.Columns.ServiceId];
                    supplierPromotedUI.SupplierPromoted = GetSuppliersPromoted(AreaId, supplierPromotedUI.ServiceId);
                    listOfPromotedArea.Add(supplierPromotedUI);
                }
            }
            return listOfPromotedArea;
        }

        private static List<SupplierPromotedAreaUI> GetSuppliersPromoted(int areaId,Int64 servicesId)
        {
            List<SupplierPromotedAreaUI> lstSuppliers = new List<SupplierPromotedAreaUI>();

                Query qry = new Query(SupplierPromotedArea.TableSchema);
                qry.Join(JoinType.InnerJoin, SupplierPromotedArea.TableSchema, SupplierPromotedArea.Columns.PromotedAreaId, SupplierPromotedArea.TableSchema.SchemaName,
                    PromotedArea.TableSchema, PromotedArea.Columns.Id, PromotedArea.TableSchema.SchemaName);
                qry.Join(JoinType.InnerJoin, SupplierPromotedArea.TableSchema, SupplierPromotedArea.Columns.SupplierId, SupplierPromotedArea.TableSchema.SchemaName,
                   AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
                qry.Join(JoinType.LeftJoin, SupplierPromotedArea.TableSchema, SupplierPromotedArea.Columns.ServiceId, SupplierPromotedArea.TableSchema.SchemaName,
                   Service.TableSchema, Service.Columns.ServiceId, Service.TableSchema.SchemaName);


                qry.SelectAllTableColumns();
                qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
                qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.ContactName, AppSupplier.Columns.ContactName);
                qry.AddSelect(PromotedArea.TableSchema.SchemaName, PromotedArea.Columns.Id, "PromotedAreaId");
                qry.AddSelect(Service.TableSchema.SchemaName, Service.Columns.ServiceName, Service.Columns.ServiceName);

                qry.Where(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.PromotedAreaId, WhereComparision.EqualsTo, areaId);
                qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsService, WhereComparision.EqualsTo, true);
                qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsLocked, WhereComparision.EqualsTo, false);
                qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsDeleted, WhereComparision.EqualsTo, false);
                qry.AddWhere(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.ServiceId, WhereComparision.EqualsTo, servicesId);
                var wl1 = new WhereList().OR(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.Deleted, WhereComparision.EqualsTo, null)
                         .OR(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
                qry.AddWhere(WhereCondition.AND, wl1);


            using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lstSuppliers.Add(new SupplierPromotedAreaUI
                        {
                            Id = (reader[SupplierPromotedArea.Columns.Id] != null ? (int)reader[SupplierPromotedArea.Columns.Id] : 0),
                            SupplierId = (reader[SupplierPromotedArea.Columns.SupplierId] != null ? (Int64)reader[SupplierPromotedArea.Columns.SupplierId] : 0),
                            SupplierBuisnesName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : ""),
                            SupplierContactName = (reader[AppSupplier.Columns.ContactName] != null ? reader[AppSupplier.Columns.ContactName].ToString() : ""),
                            PromotedAreaId = (reader["PromotedAreaId"] != null ? (int)reader["PromotedAreaId"] : 0),
                            StartTime = (reader[SupplierPromotedArea.Columns.StartTime] != null ? (DateTime)reader[SupplierPromotedArea.Columns.StartTime] : new DateTime()),
                            EndTime = (reader[SupplierPromotedArea.Columns.EndTime] != null ? (DateTime)reader[SupplierPromotedArea.Columns.EndTime] : new DateTime()),
                            ServiceId = (reader[SupplierPromotedArea.Columns.ServiceId] != null ? (Int64)reader[SupplierPromotedArea.Columns.ServiceId] : 0),

                        });
                    }
                }
            
            return lstSuppliers;
        }

        public static List<SupplierServiceUI> GetSuppliersPromotedOfCity(long cityId,int serviceId ,int limit)
        {
            Query qry = new Query(SupplierPromotedArea.TableSchema);
            qry.Join(JoinType.InnerJoin, SupplierPromotedArea.TableSchema, SupplierPromotedArea.Columns.PromotedAreaId, SupplierPromotedArea.TableSchema.SchemaName,
                PromotedArea.TableSchema, PromotedArea.Columns.Id, PromotedArea.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, SupplierPromotedArea.TableSchema, SupplierPromotedArea.Columns.SupplierId, SupplierPromotedArea.TableSchema.SchemaName,
               AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, SupplierPromotedArea.TableSchema, SupplierPromotedArea.Columns.PromotedAreaId, SupplierPromotedArea.TableSchema.SchemaName,
               City.TableSchema, City.Columns.PromotedAreaId, City.TableSchema.SchemaName);

            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.HouseNum, AppSupplier.Columns.HouseNum);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Street, AppSupplier.Columns.Street);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Description, AppSupplier.Columns.Description);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.ProfileImage, AppSupplier.Columns.ProfileImage);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Phone, AppSupplier.Columns.Phone);
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.ContactName, AppSupplier.Columns.ContactName);
            qry.AddSelect(PromotedArea.TableSchema.SchemaName, PromotedArea.Columns.Id, "PromotedAreaId");

            qry.AddSelectLiteral("(SELECT avg(" + Comment.Columns.Rate + ") from " + Comment.TableSchema.SchemaName + " where "
              + Comment.TableSchema.SchemaName + "." + Comment.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                " AND " + Comment.TableSchema.SchemaName + "." + Comment.Columns.Status + "=" + (int)CommentStatus.Approved + ")", "AvgRate");
            qry.AddSelectLiteral("(SELECT Count(" + Comment.Columns.Rate + ") from " + Comment.TableSchema.SchemaName + " where " +
                Comment.TableSchema.SchemaName + "." + Comment.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                 " AND " + Comment.TableSchema.SchemaName + "." + Comment.Columns.Status + "=" + (int)CommentStatus.Approved + ")", "numberOfComments");

            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsService, WhereComparision.EqualsTo, true);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsLocked, WhereComparision.EqualsTo, false);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qry.AddWhere(City.TableSchema.SchemaName, City.Columns.CityId, WhereComparision.EqualsTo, cityId);
            qry.AddWhere(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.ServiceId, WhereComparision.EqualsTo, serviceId);
            qry.AddWhere(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.Deleted, WhereComparision.EqualsTo, null);

            var wl1 = new WhereList().OR(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.Deleted, WhereComparision.EqualsTo, null)
                                     .OR(SupplierPromotedArea.TableSchema.SchemaName, SupplierPromotedArea.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
            qry.AddWhere(WhereCondition.AND, wl1);


            qry.LimitRows(limit);
            qry.Randomize();
            var lstSuppliers = new List<SupplierServiceUI>();

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var supplier = new SupplierServiceUI();
                    supplier.SupplierId = (reader[AppSupplier.Columns.SupplierId] != null ? (long)reader[AppSupplier.Columns.SupplierId] : 0);
                    supplier.BusinessName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    supplier.Phone = (reader[AppSupplier.Columns.Phone] != null ? reader[AppSupplier.Columns.Phone].ToString() : "");
                    supplier.CityName = (reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "");
                    supplier.Street = (reader[AppSupplier.Columns.Street] != null ? reader[AppSupplier.Columns.Street].ToString() : "");
                    supplier.HouseNum = (reader[AppSupplier.Columns.HouseNum] != null ? reader[AppSupplier.Columns.HouseNum].ToString() : "");
                    supplier.AvgRate = string.IsNullOrEmpty(reader["AvgRate"].ToString()) ? 0 :
                        Convert.ToDouble(reader["AvgRate"]);
                    supplier.Description = Convert.ToString(reader[AppSupplier.Columns.Description]);
                    supplier.NumberOfComments = Convert.ToInt32(reader["numberOfComments"]);
                    supplier.ProfileImage = Convert.ToString(reader[AppSupplier.Columns.ProfileImage]);

                    lstSuppliers.Add(supplier);
                }
            }

            return lstSuppliers;
        }

        public static List<PromotedArea> GetPromotedArea()
        {
            var listOfPromotedArea = new List<PromotedArea>();
            Query qry = new Query(PromotedArea.TableSchema);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var promotedArea = new PromotedArea();
                    promotedArea.Name = (string)reader[PromotedArea.Columns.Name];
                    promotedArea.Id = (int)reader[PromotedArea.Columns.Id];
                    listOfPromotedArea.Add(promotedArea);
                }
            }
            return listOfPromotedArea;
        }

        public static bool Delete(int promotedSupplierAreaId)
        {
            try
            {
                SupplierPromotedArea supplierPromotedArea = SupplierPromotedArea.FetchByID(promotedSupplierAreaId);
                supplierPromotedArea.Deleted = DateTime.Now;
                supplierPromotedArea.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        
                }
    }

    public class SupplierPromotedUI
    {
    //    public string PromotedAreaName { get; set; }
   //     public int PromotedAreaId { get; set; }

        public string ServiceName { get; set; }
        public Int64 ServiceId { get; set; }
        public List<SupplierPromotedAreaUI> SupplierPromoted { get; set; }

    }
    public class SupplierPromotedAreaUI : SupplierPromotedArea
    {
        public string SupplierBuisnesName { get; set; }
        public string SupplierContactName { get; set; }

    }
}
