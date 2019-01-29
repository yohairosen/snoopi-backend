using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using Snoopi.core.DAL;
using dg.Sql;
using System.Security.Cryptography;
using System.Collections;
using dg.Sql.Connector;
using System.Web;
using System.Web.Caching;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;

namespace Snoopi.core.BL
{

    public class ProductYad2UI : ProductYad2
    {
        public ProductYad2 ProductYad2 { get; set; }
        public List<CategoryYad2> LstCategory { get; set; }
        public string CityName { get; set; }

        public ProductYad2UI(ProductYad2 productYad2, List<CategoryYad2> lstCategory)
        {
            ProductYad2 = productYad2;
            LstCategory = lstCategory;
        }

        public ProductYad2UI()
        {
            // TODO: Complete member initialization
        }

    }

    public static class ProductYad2Controller
    {


        public static CategoryYad2Collection GetAllCategory()
        {
            return CategoryYad2Collection.FetchAll();
        }

        public static List<ProductYad2UI> GetAllProduct()
        {
            Query qry = new Query(ProductYad2.TableSchema);
            qry.Where(ProductYad2.Columns.Status, StatusType.Approved);
            qry.AddWhere(ProductYad2.TableSchema.SchemaName, ProductYad2.Columns.UpdateDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow.AddDays(-(Convert.ToDouble(Settings.GetSetting(Settings.Keys.YAD_2_EXPIRY_DAY)))));
            qry.OrderBy(ProductYad2.Columns.UpdateDate, SortDirection.DESC);
            ProductYad2Collection pcol = ProductYad2Collection.FetchByQuery(qry);
            List<ProductYad2UI> LstProductYad2UI = new List<ProductYad2UI>();
            foreach (ProductYad2 item in pcol)
            {
                LstProductYad2UI.Add(new ProductYad2UI(item, GetAllCatagoriesOfProduct(item._ProductId)));
            }
            return LstProductYad2UI;
        }

        public static List<CategoryYad2> GetAllCatagoriesOfProduct(Int64 ProductId)
        {
            Query qry = new Query(CategoryYad2.TableSchema);
            qry.Join(JoinType.InnerJoin, CategoryYad2.TableSchema,
            CategoryYad2.Columns.CategoryYad2Id, CategoryYad2.TableSchema.SchemaName,
            ProductYad2Category.TableSchema, ProductYad2Category.Columns.CategoryYad2Id, ProductYad2Category.TableSchema.SchemaName);
            qry.Where(ProductYad2Category.TableSchema.SchemaName, ProductYad2Category.Columns.ProductId, WhereComparision.EqualsTo,ProductId);
            List<CategoryYad2> list = new List<CategoryYad2>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new CategoryYad2
                    {
                        CategoryYad2Name = Convert.ToString(reader["CategoryYad2Name"]),
                        CategoryYad2Id = Convert.ToInt64(reader["CategoryYad2Id"]),
                    });
                }
            }
            return list;
        }

        public static List<ProductYad2UI> GetAllProductsYad2(List<int> StatusIds = null, int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(ProductYad2.TableSchema);
            qry.Join(JoinType.InnerJoin, ProductYad2.TableSchema, ProductYad2.Columns.CityId, ProductYad2.TableSchema.SchemaName,
                City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.SelectAll();
            qry.SelectAllTableColumns();
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            if (StatusIds != null && StatusIds.Count != 0)
            {
                qry.AddWhere(ProductYad2.TableSchema.SchemaName, ProductYad2.Columns.Status, WhereComparision.In, StatusIds);
            }
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            List<ProductYad2UI> list = new List<ProductYad2UI>();

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new ProductYad2UI
                    {
                        ProductId = Convert.ToInt64(reader["ProductId"]),
                        ProductName = Convert.ToString(reader["ProductName"]),
                        ProductImage = Convert.ToString(reader["ProductImage"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        AppUserId = Convert.ToInt64(reader["AppUserId"]),
                        Status = (StatusType)Enum.Parse(typeof(StatusType), reader["Status"].ToString()),
                        CreateDate = Convert.ToDateTime(reader["CreateDate"]).ToLocalTime(),
                        UpdateDate = Convert.ToDateTime(reader["UpdateDate"]).ToLocalTime(),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        CityName = Convert.ToString(reader["CityName"]),
                        StatusRemarks = Convert.ToString(reader["StatusRemarks"]),
                        Price = Convert.ToDecimal(reader["Price"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        Details = Convert.ToString(reader["Details"]),
                        LstCategory = GetAllCatagoriesOfProduct(Convert.ToInt64(reader["ProductId"])),
                    });
                }
            }
            return list;
        }

        public static ProductYad2Collection GetAllProductByAppUserId(Int64 AppUserId)
        {
            Query qry = new Query(ProductYad2.TableSchema);
            qry.Where(ProductYad2.Columns.AppUserId, AppUserId);
            qry.OrderBy(ProductYad2.Columns.UpdateDate, SortDirection.DESC);
            ProductYad2Collection pcol = ProductYad2Collection.FetchByQuery(qry);

            return pcol;
        }

        public static void ApproveProduct(Int64 ProductId, String StatusRemarks, String message)
        {
            ProductYad2 product = ProductYad2.FetchByID(ProductId);
            if (product != null)
            {
                product.Status = StatusType.Approved;
                product.StatusRemarks = StatusRemarks;
                product.UpdateDate = DateTime.UtcNow;
                product.Save();
                Notification.SendNotificationAppUserApprovedProduct(product.AppUserId, product.ProductId, message);

            }
        }
        public static void DenyProduct(Int64 ProductId, String StatusRemarks, String message)
        {
            ProductYad2 product = ProductYad2.FetchByID(ProductId);
            if (product != null)
            {
                product.Status = StatusType.Denied;
                product.StatusRemarks = StatusRemarks;
                product.UpdateDate = DateTime.UtcNow;
                product.Save();
                Notification.SendNotificationDenyProduct(product.AppUserId, product.ProductId, message);

            }
        }

    }
}
