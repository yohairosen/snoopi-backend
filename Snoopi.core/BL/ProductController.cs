using System;
using System.Collections.Generic;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.Linq;

namespace Snoopi.core.BL
{
    public class CategoryUI
    {
        public Int64 CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public Int64 CategoryRate { get; set; }
        public int ProductsNum { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryImage { get; set; }
        public Int64 SubCategoryId { get; set; }
        public Int64 SubCategoryRate { get; set; }
        public int SubCategoryProductsNum { get; set; }
        public List<FilterUI> Filters { get; set; }
    }

    public class FilterUI
    {
        public Int64 FilterId { get; set; }
        public string FilterName { get; set; }
        public List<SubFilterUI> LstSubFilter { get; set; }
    }

    public class SubFilterUI
    {
        public Int64 SubFilterId { get; set; }
        public string SubFilterName { get; set; }
        public Int64 FilterId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ProductUI : Product
    {
        public ProductFilterCollection LstFilters { get; set; }
        public AnimalCollection AnimalLst { get; set; }
        public string AnimalName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public bool IsExist { get; set; }
        public decimal ProductPrice { get; set; }
        public string Gift { get; set; }
        public bool IsImage { get; set; }
        public bool Rate { get; set; }
        public static ProductUI ConvertProductToProductUI(Product p)
        {
            ProductUI productUI = new ProductUI();
            productUI.ProductId = p.ProductId;
            productUI.ProductNum = p.ProductNum;
            productUI.ProductCode = p.ProductCode;
            productUI.ProductName = p.ProductName;
            productUI.ProductImage = p.ProductImage;
            productUI.Description = p.Description;
            productUI.Amount = p.Amount;
            productUI.CategoryId = p.CategoryId;
            productUI.SubCategoryId = p.SubCategoryId;
            productUI.SendSupplier = p.SendSupplier;
            productUI.CreateDate = p.CreateDate;
            productUI.RecomendedPrice = p.RecomendedPrice;
            productUI.ProductRate = p.ProductRate;
          
            return productUI;

        }
    }

    public class MaxMinProductUI : ProductUI
    {
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
    }

    public class PromotedProductUI : ProductUI
    {
        public int Id { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public string Section { get; set; }
        public int AreaId { get; set; }
        public int Weight { get; set; }
        public PromotedProductUI() { }
        public PromotedProductUI(DataReaderBase reader)
        {
            Id = Convert.ToInt16(reader[PromotedProduct.Columns.Id]);
            ProductId = Convert.ToInt64(reader[Product.Columns.ProductId]);
            Section = reader[PromotedProduct.Columns.Section].ToString();
            ProductImage = reader[Product.Columns.ProductImage].ToString();
            ProductName = reader[Product.Columns.ProductName].ToString();
            ProductCode = reader[Product.Columns.ProductCode] == DBNull.Value ? "0" : reader[Product.Columns.ProductCode].ToString();
            AreaId = reader[PromotedProduct.Columns.AreaId] == DBNull.Value ? 0 : Convert.ToInt16(reader[PromotedProduct.Columns.AreaId]);
            Weight = reader[PromotedProduct.Columns.Weight] == DBNull.Value ? 10 : Convert.ToInt16(reader[PromotedProduct.Columns.Weight]);
        }
    }

    public class PromotedProductAreaUI
    {
        public string PromotedAreaName { get; set; }
        public int PromotedAreaId { get; set; }
        public List<PromotedProductUI> ProductPromoted { get; set; }

    }
    public static class ProductController
    {
        public static List<ProductUI> GetAllProductBySupplier(Int64 SupplierId, Int64 AnimalId = 0, Int64 CategoryId = 0, Int64 SubCategoryId = 0, string productCode = "", bool IsExist = false)
        {
            Query qry = new Query(Product.TableSchema);
            qry.Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema.SchemaName, Product.Columns.CategoryId, Category.Columns.CategoryId));
            qry.Join(JoinType.InnerJoin, SubCategory.TableSchema, SubCategory.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema.SchemaName, Product.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId));
            if (AnimalId != 0)
            {
                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                          ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
            }
            qry.SelectAllTableColumns();
            if (!IsExist)
            {
                qry.AddSelectLiteral("' ' AS 'Gift'");
                qry.AddSelectLiteral("'0' AS 'Price'");
            }
            else
            {
                qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema.SchemaName, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
                qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, SupplierProduct.Columns.SupplierId);
                qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Gift, SupplierProduct.Columns.Gift);
                qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, SupplierProduct.Columns.Price);
            }
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, SubCategory.Columns.SubCategoryName);
            qry.AddSelectLiteral(" case when " + Product.TableSchema.SchemaName + "." + Product.Columns.CreateDate + " > '" + DateTime.UtcNow.AddMonths(-1) + "' then " + Product.TableSchema.SchemaName + "." + Product.Columns.CreateDate + " else null end as FieldOrder ");
            qry.AddWhere(Product.Columns.IsDeleted, false);
            if (SupplierId != null && IsExist)
                qry.AddWhere(SupplierProduct.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);

            qry.OrderBy("FieldOrder", SortDirection.DESC);
            qry.OrderBy(Product.Columns.ProductId, SortDirection.DESC);
            if (AnimalId != 0) qry.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, AnimalId);
            if (CategoryId != 0) qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.CategoryId, WhereComparision.EqualsTo, CategoryId);
            if (SubCategoryId != 0) qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.SubCategoryId, WhereComparision.EqualsTo, SubCategoryId);

            if (!string.IsNullOrEmpty(productCode)) qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.ProductCode, WhereComparision.Like, "%" + productCode + "%");

            List<ProductUI> LstProduct = new List<ProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    ProductUI p = new ProductUI();
                    ProductUI sup_p = new ProductUI();
                    p.ProductId = reader[Product.Columns.ProductId] != null ? Convert.ToInt64(reader[Product.Columns.ProductId]) : 0;
                    p.ProductNum = (reader[Product.Columns.ProductNum] is DBNull) ? (Int64?)null : Convert.ToInt64(reader[Product.Columns.ProductNum]);
                    p.ProductName = reader[Product.Columns.ProductName] != null ? reader[Product.Columns.ProductName].ToString() : "";
                    p.CategoryName = reader[Category.Columns.CategoryName] != null ? reader[Category.Columns.CategoryName].ToString() : "";
                    p.SubCategoryName = reader[SubCategory.Columns.SubCategoryName] != null ? reader[SubCategory.Columns.SubCategoryName].ToString() : "";
                    p.ProductCode = reader[Product.Columns.ProductCode] != null ? reader[Product.Columns.ProductCode].ToString() : "";
                    p.Amount = reader[Product.Columns.Amount] != null ? reader[Product.Columns.Amount].ToString() : "";
                    p.AnimalLst = GetAnimalByProductId(reader[Product.Columns.ProductId] != null ? Convert.ToInt64(reader[Product.Columns.ProductId]) : 0);
                    sup_p = GetProductBySupplier(p.ProductId, SupplierId);
                    p.IsExist = sup_p != null ? true : false;
                    // p.IsExist = IsSupplierProduct(p.ProductId, SupplierId);
                    if (p.IsExist)
                    {
                        // p.ProductPrice = reader[SupplierProduct.Columns.Price] != null && reader[SupplierProduct.Columns.Price].ToString() != "" ? Convert.ToDecimal(reader[SupplierProduct.Columns.Price]) : 0;
                        p.ProductPrice = sup_p.ProductPrice;
                        p.Gift = sup_p.Gift;
                    }
                    else
                    {
                        p.ProductPrice = reader["Price"] != null && reader["Price"].ToString() != "" ? Convert.ToDecimal(reader["Price"]) : 0;
                        p.Gift = reader["Gift"] != null ? reader["Gift"].ToString() : "";
                    }


                    p.RecomendedPrice = reader[Product.Columns.RecomendedPrice] != null && reader[Product.Columns.RecomendedPrice].ToString() != "" ? Convert.ToDecimal(reader[Product.Columns.RecomendedPrice]) : 0;
                    LstProduct.Add(p);
                }
            }

            return LstProduct;
        }

        public static bool IsSupplierProduct(Int64 ProductId, Int64 supplierID)
        {
            Query q = new Query(SupplierProduct.TableSchema);
            q.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, SupplierProduct.Columns.SupplierId);
            q.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, SupplierProduct.Columns.ProductId);
            q.Where(SupplierProduct.Columns.ProductId, ProductId);
            q.AddWhere(SupplierProduct.Columns.SupplierId, supplierID);
            return Convert.ToBoolean(q.ExecuteScalar());
        }

        public static ProductUI GetProductBySupplier(Int64 productID, Int64 supplierID)
        {
            ProductUI p = null;
            Query q = new Query(SupplierProduct.TableSchema);
            q.Where(SupplierProduct.Columns.SupplierId, supplierID);
            q.AddWhere(SupplierProduct.Columns.ProductId, productID);
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    p = new ProductUI();
                    p.ProductId = reader[SupplierProduct.Columns.ProductId] != null ? Convert.ToInt64(reader[SupplierProduct.Columns.ProductId]) : 0;
                    p.ProductPrice = reader[SupplierProduct.Columns.Price] != null && reader[SupplierProduct.Columns.Price].ToString() != "" ? Convert.ToDecimal(reader[SupplierProduct.Columns.Price]) : 0;
                    p.Gift = reader[SupplierProduct.Columns.Gift] != null ? reader[SupplierProduct.Columns.Gift].ToString() : "";
                }
            }
            return p;
        }


        public static List<ProductUI> GetSupplierProducts(Int64 SupplierId, int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(SupplierProduct.TableSchema);
            qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.TableSchema.SchemaName, new JoinColumnPair(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, Product.Columns.ProductId));
            qry.Where(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);

            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<ProductUI> LstProduct = new List<ProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    ProductUI p = new ProductUI();
                    p.ProductId = reader[Product.Columns.ProductId] != null ? Convert.ToInt64(reader[Product.Columns.ProductId]) : 0;
                    p.ProductName = reader[Product.Columns.ProductName] != null ? reader[Product.Columns.ProductName].ToString() : "";
                    p.ProductCode = reader[Product.Columns.ProductCode] != null ? reader[Product.Columns.ProductCode].ToString() : "";
                    p.Amount = reader[Product.Columns.Amount] != null ? reader[Product.Columns.Amount].ToString() : "";
                    p.ProductPrice = reader[SupplierProduct.Columns.Price] != null && reader[SupplierProduct.Columns.Price].ToString() != "" ? Convert.ToDecimal(reader[SupplierProduct.Columns.Price]) : 0;
                    p.Gift = reader[SupplierProduct.Columns.Gift] != null ? reader[SupplierProduct.Columns.Gift].ToString() : "";
                    LstProduct.Add(p);
                }
            }

            return LstProduct;
        }

        public static List<CategoryUI> GetAllCategory(Int64 AnimalId = 0, int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(Category.TableSchema);
            qry.Join(JoinType.LeftJoin, Category.TableSchema, Category.Columns.CategoryId, Category.TableSchema.SchemaName,
            Product.TableSchema, Product.Columns.CategoryId, Product.TableSchema.SchemaName);
            qry.SelectLiteral(" count(" + Product.TableSchema.SchemaName + "." + Product.Columns.ProductId + ") as CountProduct");
            if (AnimalId != 0)
            {
                //qry.Join(JoinType.InnerJoin, Category.TableSchema, Category.Columns.CategoryId, Category.TableSchema.SchemaName,
                // Product.TableSchema, Product.Columns.CategoryId, Product.TableSchema.SchemaName);
                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                           ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qry.Where(ProductAnimal.Columns.AnimalId, AnimalId);
                //qry.SelectLiteral(" count(" + Product.TableSchema.SchemaName + "." + Product.Columns.ProductId + ") as CountProduct");
            }

            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryId, Category.Columns.CategoryId);
            qry.AddSelect(Category.Columns.CategoryName);
            qry.AddSelect(Category.Columns.CategoryImage);
            qry.AddSelect(Category.Columns.CategoryRate);
            qry.GroupBy(Category.TableSchema.SchemaName, Category.Columns.CategoryId);

            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            qry.OrderBy(Category.Columns.CategoryRate, SortDirection.ASC);
            List<CategoryUI> lstCategory = new List<CategoryUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstCategory.Add(new CategoryUI
                    {
                        CategoryId = (reader[Category.Columns.CategoryId] != null ? (Int64)reader[Category.Columns.CategoryId] : 0),
                        CategoryName = (reader[Category.Columns.CategoryName] != null ? reader[Category.Columns.CategoryName].ToString() : ""),
                        CategoryImage = (reader[Category.Columns.CategoryImage] != null ? reader[Category.Columns.CategoryImage].ToString() : ""),
                        CategoryRate = (reader[Category.Columns.CategoryRate] != null ? (Int64)reader[Category.Columns.CategoryRate] : 0),
                        //ProductsNum = AnimalId > 0 ? ((reader["CountProduct"] != null ? int.Parse(reader["CountProduct"].ToString()) : 0)) : 0
                        ProductsNum = reader["CountProduct"] != null ? int.Parse(reader["CountProduct"].ToString()) : 0
                    });
                }

            }
            return lstCategory;
        }


        public static List<CategoryUI> GetAllCategoriesOfUser(List<int> busketProductIds, Int64 AnimalId = 0, Int64 AppUserId = 0, Int64 TempAppUserId = 0, int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(Product.TableSchema).Select(Product.Columns.ProductId).Distinct();
            qry.SelectAllTableColumns();
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName);
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryImage, Category.Columns.CategoryImage);
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryRate, Category.Columns.CategoryRate);
            qry.AddSelectLiteral("count( distinct `Product`.`ProductId` * `Product`.`CategoryId`) as 'CountProduct'");

            if (AnimalId != 0)
            {
                //qry.Where(Product.Columns.CategoryId, CategoryId);
                //qry.AddWhere(Product.Columns.SubCategoryId, SubCategoryId);
                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                          ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qry.AddWhere(ProductAnimal.Columns.AnimalId, AnimalId);

            }

            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
            qry.Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.CategoryId, Category.Columns.CategoryId));
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.EqualsTo, false);

            var suppliers = GetSuppliersThatSupplyBusketInCity(busketProductIds, 0, AppUserId, TempAppUserId);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);
            qry.GroupBy(Product.TableSchema.SchemaName, Product.Columns.CategoryId);
            qry.OrderBy(Category.TableSchema.SchemaName, Category.Columns.CategoryRate, SortDirection.ASC);

            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<CategoryUI> lstCategory = new List<CategoryUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstCategory.Add(new CategoryUI
                    {
                        CategoryId = (reader[Category.Columns.CategoryId] != null ? (Int64)reader[Category.Columns.CategoryId] : 0),
                        CategoryName = (reader[Category.Columns.CategoryName] != null ? reader[Category.Columns.CategoryName].ToString() : ""),
                        CategoryImage = (reader[Category.Columns.CategoryImage] != null ? reader[Category.Columns.CategoryImage].ToString() : ""),
                        //ProductsNum = AnimalId > 0 ? ((reader["CountProduct"] != null ? int.Parse(reader["CountProduct"].ToString()) : 0)) : 0
                        //CategoryId = (reader[Category.Columns.CategoryId] != null ? (Int64)reader[Category.Columns.CategoryId] : 0),
                        ProductsNum = reader["CountProduct"] != null ? int.Parse(reader["CountProduct"].ToString()) : 0
                    });
                }

            }
            return lstCategory;
        }

        private static List<Int64> GetSuppliersThatSupplyBusketInCity(List<int> busketProductIds, long cityId, long appUserId, long tempAppUserId)
        {
            var suppliers = new List<long>();
            Query innerQuery = new Query(SupplierProduct.TableSchema);
            innerQuery.Join(JoinType.InnerJoin, SupplierCity.TableSchema, SupplierCity.TableSchema.SchemaName,
               new JoinColumnPair(SupplierProduct.TableSchema, SupplierProduct.Columns.SupplierId, SupplierCity.Columns.SupplierId));
            innerQuery.Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName,
                   new JoinColumnPair(SupplierProduct.TableSchema, SupplierProduct.Columns.SupplierId, AppSupplier.Columns.SupplierId));

            innerQuery.Select(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, SupplierProduct.Columns.SupplierId, false).GroupBy(SupplierProduct.Columns.SupplierId);
            innerQuery.AddSelectLiteral(" COUNT(" + SupplierProduct.TableSchema.SchemaName + "." + SupplierProduct.Columns.SupplierId + ") as `suppliercount`");
            innerQuery.AddWhere(SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            if (cityId > 0)
                innerQuery.AddWhere(SupplierCity.TableSchema.SchemaName, SupplierCity.Columns.CityId, WhereComparision.EqualsTo, cityId);

            else if (appUserId > 0)
            {
                innerQuery.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName, new JoinColumnPair(SupplierCity.TableSchema, SupplierCity.Columns.CityId, AppUser.Columns.CityId));
                innerQuery.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, appUserId);
            }
            else if (tempAppUserId > 0)//TempUser
            {
                innerQuery.Join(JoinType.InnerJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName, new JoinColumnPair(SupplierCity.TableSchema, SupplierCity.Columns.CityId, TempAppUser.Columns.CityId));
                innerQuery.AddWhere(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, WhereComparision.EqualsTo, tempAppUserId);
            }

            if (busketProductIds != null && busketProductIds.Count > 0)
                innerQuery.AddWhere(SupplierProduct.Columns.ProductId, WhereComparision.In, busketProductIds);

            innerQuery.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsDeleted, WhereComparision.EqualsTo, 0);
            innerQuery.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsLocked, WhereComparision.EqualsTo, false);
            innerQuery.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Status, WhereComparision.NotEqualsTo, false);

            using (DataReaderBase reader = innerQuery.ExecuteReader())
            {
                if (busketProductIds != null && busketProductIds.Count > 0)
                {
                    while (reader.Read())
                    {

                        if (Convert.ToInt32(reader["suppliercount"]) == busketProductIds.Count)
                        {
                            suppliers.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                        }
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        suppliers.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                    }
                }
            }

            return suppliers;
        }

        public static List<CategoryUI> GetAllSubCategoryOfUser(List<int> busketProductIds, Int64 CategoryId, Int64 AnimalId, Int64 AppUserId = 0, Int64 TempAppUserId = 0)
        {
            var suppliers = GetSuppliersThatSupplyBusketInCity(busketProductIds, 0, AppUserId, TempAppUserId);
            Query qry = new Query(Product.TableSchema).Select(Product.Columns.ProductId).Distinct();
            qry.SelectAllTableColumns();
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName);
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryImage, Category.Columns.CategoryImage);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, SubCategory.Columns.SubCategoryName);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryImage, SubCategory.Columns.SubCategoryImage);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryRate, SubCategory.Columns.SubCategoryRate);
            qry.AddSelectLiteral("count(distinct `Product`.`ProductId` * `Product`.`SubCategoryId`) as 'CountProduct'");
            if (AnimalId != 0)
            {

                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                          ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qry.AddWhere(ProductAnimal.Columns.AnimalId, AnimalId);
                qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.CategoryId, WhereComparision.EqualsTo, CategoryId);
            }

            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
            qry.Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.CategoryId, Category.Columns.CategoryId));
            qry.Join(JoinType.InnerJoin, SubCategory.TableSchema, SubCategory.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.CategoryId, SubCategory.Columns.CategoryId),
                                                                                                  new JoinColumnPair(Product.TableSchema, Product.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId));
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);

            qry.GroupBy(Product.TableSchema.SchemaName, Product.Columns.SubCategoryId);
            qry.OrderBy(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryRate, SortDirection.ASC);
            //  qry.OrderBy(Product.TableSchema.SchemaName, Product.Columns.ProductName, SortDirection.ASC);
            List<CategoryUI> lstSubCategory = new List<CategoryUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstSubCategory.Add(new CategoryUI
                    {
                        CategoryId = (reader[SubCategory.Columns.CategoryId] != null ? (Int64)reader[SubCategory.Columns.CategoryId] : 0),
                        SubCategoryId = (reader[SubCategory.Columns.SubCategoryId] != null ? (Int64)reader[SubCategory.Columns.SubCategoryId] : 0),
                        SubCategoryName = (reader[SubCategory.Columns.SubCategoryName] != null ? reader[SubCategory.Columns.SubCategoryName].ToString() : ""),
                        SubCategoryImage = (reader[SubCategory.Columns.SubCategoryImage] != null ? reader[SubCategory.Columns.SubCategoryImage].ToString() : ""),
                        SubCategoryProductsNum = reader["CountProduct"] != null ? int.Parse(reader["CountProduct"].ToString()) : 0
                    });
                }

            }
            return lstSubCategory;

        }

        public static List<CategoryUI> GetAllSubCategory(Int64 CategoryId, Int64 AnimalId)
        {
            Query qry = new Query(SubCategory.TableSchema);
            qry.AddWhere(SubCategory.TableSchema.SchemaName, SubCategory.Columns.CategoryId, WhereComparision.EqualsTo, CategoryId);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.CategoryId, WhereComparision.EqualsTo, CategoryId);
            qry.Join(JoinType.InnerJoin, SubCategory.TableSchema, SubCategory.Columns.SubCategoryId, SubCategory.TableSchema.SchemaName,
            Product.TableSchema, Product.Columns.SubCategoryId, Product.TableSchema.SchemaName);
            qry.AddWhere(Product.Columns.IsDeleted, false);
            qry.SelectLiteral(" count(" + Product.TableSchema.SchemaName + "." + Product.Columns.ProductId + ") as CountProduct");
            qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                      ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
            qry.AddWhere(ProductAnimal.Columns.AnimalId, AnimalId);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.CategoryId, SubCategory.Columns.CategoryId);
            qry.AddSelect(SubCategory.Columns.SubCategoryName);
            qry.AddSelect(SubCategory.Columns.SubCategoryImage);
            qry.AddSelect(SubCategory.Columns.SubCategoryRate);
            qry.GroupBy(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryId);
            qry.OrderBy(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryRate, SortDirection.ASC);

            List<CategoryUI> lstSubCategory = new List<CategoryUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstSubCategory.Add(new CategoryUI
                    {
                        CategoryId = (reader[SubCategory.Columns.CategoryId] != null ? (Int64)reader[SubCategory.Columns.CategoryId] : 0),
                        SubCategoryId = (reader[SubCategory.Columns.SubCategoryId] != null ? (Int64)reader[SubCategory.Columns.SubCategoryId] : 0),
                        SubCategoryName = (reader[SubCategory.Columns.SubCategoryName] != null ? reader[SubCategory.Columns.SubCategoryName].ToString() : ""),
                        SubCategoryImage = (reader[SubCategory.Columns.SubCategoryImage] != null ? reader[SubCategory.Columns.SubCategoryImage].ToString() : ""),
                        SubCategoryProductsNum = reader["CountProduct"] != null ? int.Parse(reader["CountProduct"].ToString()) : 0
                    });
                }

            }
            return lstSubCategory;
        }

        public static SubCategoryCollection GetAllSubCategory(Int64 CategoryId)
        {
            Query qry = new Query(SubCategory.TableSchema);
            qry.Where(SubCategory.Columns.CategoryId, CategoryId);
            return SubCategoryCollection.FetchByQuery(qry);
        }

        public static List<CategoryUI> GetAllCategoriesAndSubCategories()
        {
            Query qry = new Query(SubCategory.TableSchema);
            qry.Join(JoinType.RightJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(SubCategory.TableSchema, SubCategory.Columns.CategoryId, Category.Columns.CategoryId));
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryId, Category.Columns.CategoryId);
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName);
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryRate, Category.Columns.CategoryRate);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, SubCategory.Columns.SubCategoryName);

            List<CategoryUI> lstAllCategoryAndSubCategory = new List<CategoryUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {

                    lstAllCategoryAndSubCategory.Add(new CategoryUI
                    {
                        CategoryId = (reader[Category.Columns.CategoryId] != null ? (Int64)reader[Category.Columns.CategoryId] : 0),
                        CategoryName = (reader[Category.Columns.CategoryName] != null ? reader[Category.Columns.CategoryName].ToString() : ""),
                        CategoryRate = (reader[Category.Columns.CategoryRate] != null ? (Int64)reader[Category.Columns.CategoryRate] : 0),
                        SubCategoryId = (reader[SubCategory.Columns.SubCategoryId] != DBNull.Value ? (Int64)reader[SubCategory.Columns.SubCategoryId] : 0),
                        SubCategoryName = (reader[SubCategory.Columns.SubCategoryName] != DBNull.Value ? reader[SubCategory.Columns.SubCategoryName].ToString() : "")
                    });
                }

            }
            return lstAllCategoryAndSubCategory;

        }


        public static List<CategoryUI> GetSubCategories(Int64 CategoryId = 0, int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(SubCategory.TableSchema);
            qry.SelectAllTableColumns();
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName)
                .Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(SubCategory.TableSchema, SubCategory.Columns.CategoryId, Category.Columns.CategoryId));
            if (CategoryId > 0)
            {
                qry.Where(Category.TableSchema.SchemaName, Category.Columns.CategoryId, WhereComparision.EqualsTo, CategoryId);
            }
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            qry.OrderBy(SubCategory.Columns.SubCategoryRate, SortDirection.ASC);

            List<CategoryUI> subCategoriesLst = new List<CategoryUI>();

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    subCategoriesLst.Add(new CategoryUI
                    {
                        SubCategoryId = Convert.ToInt64(reader[SubCategory.Columns.SubCategoryId]),
                        SubCategoryName = reader[SubCategory.Columns.SubCategoryName].ToString(),
                        SubCategoryImage = reader[SubCategory.Columns.SubCategoryImage].ToString(),
                        SubCategoryRate = Convert.ToInt64(reader[SubCategory.Columns.SubCategoryRate]),
                        CategoryName = reader[Category.Columns.CategoryName].ToString()
                    });
                }
            }
            return subCategoriesLst;
        }

        public static List<FilterUI> GetAllFilter(int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query qry = new Query(SubFilter.TableSchema);
            qry.Join(JoinType.RightJoin, SubFilter.TableSchema, SubFilter.Columns.FilterId, SubFilter.TableSchema.SchemaName,
                Filter.TableSchema, Filter.Columns.FilterId, Filter.TableSchema.SchemaName);
            qry.Select(Filter.TableSchema.SchemaName, Filter.Columns.FilterId, Filter.Columns.FilterId, true);
            qry.AddSelect(Filter.TableSchema.SchemaName, Filter.Columns.FilterName, Filter.Columns.FilterName);
            qry.AddSelect(SubFilter.TableSchema.SchemaName, SubFilter.Columns.SubFilterId, SubFilter.Columns.SubFilterId);
            qry.AddSelect(SubFilter.TableSchema.SchemaName, SubFilter.Columns.SubFilterName, SubFilter.Columns.SubFilterName);

            //if (PageSize > 0)
            //{
            //    qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            //}

            List<FilterUI> lstFilter = new List<FilterUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {

                    Int64 FilterId = reader[Filter.Columns.FilterId] != null ? (Int64)reader[Filter.Columns.FilterId] : 0;
                    int i = lstFilter.FindIndex(r => r.FilterId == FilterId);
                    if (i > -1)
                    {
                        if (!(reader[SubFilter.Columns.SubFilterId] is DBNull)) lstFilter[i].LstSubFilter.Add(new SubFilterUI { FilterId = FilterId, SubFilterId = (Int64)reader[SubFilter.Columns.SubFilterId], SubFilterName = (reader[SubFilter.Columns.SubFilterName]).ToString(), IsSelected = false });
                    }
                    else
                    {

                        List<SubFilterUI> lst = new List<SubFilterUI>();
                        if (!(reader[SubFilter.Columns.SubFilterId] is DBNull)) lst.Add(new SubFilterUI { FilterId = FilterId, SubFilterId = (Int64)reader[SubFilter.Columns.SubFilterId], SubFilterName = (reader[SubFilter.Columns.SubFilterName]).ToString(), IsSelected = false });
                        lstFilter.Add(new FilterUI { FilterId = FilterId, FilterName = (reader[Filter.Columns.FilterName]).ToString(), LstSubFilter = lst });
                    }
                }
            }

            return lstFilter;
        }

        //public static List<SubFilter> GetSubFilter(Int64 FilterId, Int64 ProductId)
        //{
        //    Query q = new Query(ProductFilter.TableSchema)
        //    .Select(ProductFilter.Columns.SubFilterId)
        //    .AddSelect(SubFilter.TableSchema.SchemaName, SubFilter.Columns.SubFilterName, SubFilter.Columns.SubFilterName)
        //    .Join(JoinType.InnerJoin, SubFilter.TableSchema, SubFilter.TableSchema.SchemaName, new JoinColumnPair(ProductFilter.TableSchema.SchemaName, ProductFilter.Columns.SubFilterId, SubFilter.Columns.SubFilterId))
        //    .Where(ProductFilter.TableSchema.SchemaName, ProductFilter.Columns.FilterId, WhereComparision.EqualsTo, FilterId)
        //    .AddWhere(ProductFilter.Columns.ProductId,ProductId);
        //    List<SubFilter> lst = new List<SubFilter>();
        //    using (DataReaderBase reader = q.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            SubFilter subFilter = new SubFilter();
        //            subFilter.SubFilterId = Convert.ToInt64(reader[ProductFilter.Columns.SubFilterId]);
        //            subFilter.SubFilterName = reader[SubFilter.Columns.SubFilterName].ToString();
        //            lst.Add(subFilter);
        //        }
        //    }
        //    return lst;
        //}

        public static List<SubFilter> GetSubFiltersOfFilter(Int64 FilterId)
        {
            Query q = new Query(SubFilter.TableSchema);
            q.Where(SubFilter.Columns.FilterId, FilterId);
            List<SubFilter> lst = new List<SubFilter>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    SubFilter subFilter = new SubFilter();
                    subFilter.SubFilterId = Convert.ToInt64(reader[ProductFilter.Columns.SubFilterId]);
                    subFilter.SubFilterName = reader[SubFilter.Columns.SubFilterName].ToString();
                    lst.Add(subFilter);
                }
            }
            return lst;
        }

        public static List<FilterUI> GetFilterForProduct(Int64 ProductId)
        {
            Query q = new Query(ProductFilter.TableSchema)
            .Where(ProductFilter.Columns.ProductId, ProductId);
            ProductFilterCollection lstProductFilters = ProductFilterCollection.FetchByQuery(q);

            List<FilterUI> lst = GetAllFilter();
            if (lstProductFilters == null || lstProductFilters.Count == 0) return lst;

            foreach (FilterUI item in lst)
            {
                List<ProductFilter> lh = lstProductFilters.FindAll(m => m.FilterId == item.FilterId);
                if (lh != null && lh.Count != 0)
                {
                    List<Int64> lstHelper = lh.ConvertAll<Int64>(a => a.SubFilterId);
                    item.LstSubFilter.FindAll(r => lstHelper.Contains(r.SubFilterId)).ForEach(r => r.IsSelected = true);
                }
            }
            return lst;
        }

        public static ProductFilterCollection GetProductFilterForProduct(Int64 ProductId)
        {
            Query q = new Query(ProductFilter.TableSchema)
            .Where(ProductFilter.Columns.ProductId, ProductId);
            return ProductFilterCollection.FetchByQuery(q);
        }

        public static List<FilterUI> GetFilterByProductId(Int64 ProductId)
        {
            Query qry = new Query(SubFilter.TableSchema);
            qry.Join(JoinType.InnerJoin, SubFilter.TableSchema, SubFilter.Columns.FilterId, SubFilter.TableSchema.SchemaName,
                Filter.TableSchema, Filter.Columns.FilterId, Filter.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, ProductFilter.TableSchema, ProductFilter.TableSchema.SchemaName, new JoinColumnPair(SubFilter.TableSchema.SchemaName, SubFilter.Columns.SubFilterId, ProductFilter.Columns.SubFilterId).JoinAND(SubFilter.TableSchema.SchemaName, SubFilter.Columns.FilterId, ProductFilter.Columns.FilterId))
                .Where(ProductFilter.Columns.ProductId, ProductId);

            List<FilterUI> lstFilter = new List<FilterUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {

                    Int64 FilterId = reader[SubFilter.Columns.FilterId] != null ? (Int64)reader[SubFilter.Columns.FilterId] : 0;
                    int i = lstFilter.FindIndex(r => r.FilterId == FilterId);
                    if (i > -1)
                    {
                        lstFilter[i].LstSubFilter.Add(new SubFilterUI { FilterId = FilterId, SubFilterId = (Int64)reader[SubFilter.Columns.SubFilterId], SubFilterName = (reader[SubFilter.Columns.SubFilterName]).ToString() });
                    }
                    else
                    {

                        List<SubFilterUI> lst = new List<SubFilterUI>();
                        lst.Add(new SubFilterUI { FilterId = FilterId, SubFilterId = (Int64)reader[SubFilter.Columns.SubFilterId], SubFilterName = (reader[SubFilter.Columns.SubFilterName]).ToString() });
                        lstFilter.Add(new FilterUI { FilterId = FilterId, FilterName = (reader[Filter.Columns.FilterName]).ToString(), LstSubFilter = lst });
                    }
                }
            }

            return lstFilter;

        }
        public static SubFilterCollection GetSubFilters(Int64 FilterId)
        {
            Query q = new Query(SubFilter.TableSchema).Where(SubFilter.Columns.FilterId, FilterId);
            return SubFilterCollection.FetchByQuery(q);
        }


        public static ProductFilterCollection GetAllFilterByProductId(Int64 ProductId)
        {
            Query qry = new Query(ProductFilter.TableSchema);
            qry.Where(ProductFilter.Columns.ProductId, ProductId);

            return ProductFilterCollection.FetchByQuery(qry);
        }

        public static List<FilterUI> GetAllFiltersBySubCategory(Int64 CategoryId = 0, Int64 SubCategoryId = 0)
        {
            Query qry = new Query(SubCategoryFilter.TableSchema);
            qry.Join(JoinType.InnerJoin, Filter.TableSchema, Filter.TableSchema.SchemaName, new JoinColumnPair(SubCategoryFilter.TableSchema.SchemaName, SubCategoryFilter.Columns.FilterId, Filter.Columns.FilterId));
            qry.Where(SubCategoryFilter.Columns.SubCategoryId, SubCategoryId);
            qry.AddWhere(SubCategoryFilter.Columns.CategoryId, CategoryId);

            List<FilterUI> filters = new List<FilterUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    FilterUI filter = new FilterUI();
                    filter.FilterId = Convert.ToInt64(reader[SubCategoryFilter.Columns.FilterId]);
                    filter.FilterName = reader[Filter.Columns.FilterName].ToString();
                    List<SubFilter> subs = GetSubFiltersOfFilter(filter.FilterId);
                    filter.LstSubFilter = new List<SubFilterUI>();
                    foreach (SubFilter item in subs)
                    {
                        filter.LstSubFilter.Add(new SubFilterUI
                        {

                            SubFilterId = item.SubFilterId,
                            SubFilterName = item.SubFilterName,
                            FilterId = filter.FilterId
                        });
                    }

                    filters.Add(filter);
                }
            }
            return filters;
        }

        public static List<ProductUI> GetAllProduct(Int64 CategoryId = 0, Int64 SubCategoryId = 0, Int64 AnimalId = 0)
        {
            Query qry = new Query(Product.TableSchema);
            if (CategoryId != 0 && SubCategoryId != 0 && AnimalId != 0)
            {
                qry.Where(Product.Columns.CategoryId, CategoryId);
                qry.AddWhere(Product.Columns.SubCategoryId, SubCategoryId);
                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                        ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qry.AddWhere(ProductAnimal.Columns.AnimalId, AnimalId);
            }
            qry.AddWhere(Product.Columns.IsDeleted, false);
            qry.OrderBy(Product.Columns.ProductRate, SortDirection.ASC);

            ProductCollection pcol = ProductCollection.FetchByQuery(qry);
            List<ProductUI> lstProduct = new List<ProductUI>();
            foreach (Product item in pcol)
            {
                ProductUI p = ProductUI.ConvertProductToProductUI(item);
                p.LstFilters = GetAllFilterByProductId(p.ProductId);
                lstProduct.Add(p);
            }

            return lstProduct;
        }


        private static List<long> GetAllCitiesOfAreaByCity(long cityId)
        {
            List<long> cities = new List<long>();
            var city = City.FetchByID(cityId);
            Query query = new Query(City.TableSchema);
            query.Select(City.Columns.CityId);
            query.Where(City.Columns.PromotedAreaId, WhereComparision.EqualsTo, city.PromotedAreaId);

            using (DataReaderBase reader = query.ExecuteReader())
            {
                while (reader.Read())
                {
                    cities.Add(Convert.ToInt64(reader[City.Columns.CityId]));
                }
            }
            return cities;
        }

        public static Dictionary<string, IEnumerable<PromotedProductUI>> GetPromotedProducts(List<int> busketProductIds, int nItems, long cityId)
        {
            var suppliers = GetSuppliersThatSupplyBusketInCity(busketProductIds, cityId, 0, 0);
            //var cities = GetAllCitiesOfAreaByCity(cityId);
            Query qry = new Query(Product.TableSchema);
            qry.SelectAllTableColumns();
            qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, SupplierProduct.Columns.Price);
            qry.AddSelect(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Section, PromotedProduct.Columns.Section);
            qry.AddSelect(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Id, PromotedProduct.Columns.Id);
            qry.AddSelect(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.AreaId, PromotedProduct.Columns.AreaId);
            qry.AddSelect(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Weight, PromotedProduct.Columns.Weight);

            qry.Join(JoinType.InnerJoin, PromotedProduct.TableSchema, PromotedProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, PromotedProduct.Columns.ProductId));
            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));

            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            var wl = new WhereList().OR(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Deleted, WhereComparision.EqualsTo, null)
                .OR(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
            qry.AddWhere(WhereCondition.AND, wl);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);
            qry.OrderBy(Product.TableSchema.SchemaName, Product.Columns.ProductRate, SortDirection.ASC);

            var lstProduct = new Dictionary<string, Dictionary<long, PromotedProductUI>>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    decimal price = Convert.ToDecimal(reader[SupplierProduct.Columns.Price]);
                    var promotedProduct = new PromotedProductUI(reader);
                    AddItemToPromotedDictionary(lstProduct, promotedProduct, price);
                }
            }

            var dictToReturn = new Dictionary<string, IEnumerable<PromotedProductUI>>();
            foreach (var section in lstProduct)
                dictToReturn.Add(section.Key, section.Value.Values.Take(nItems));

            return dictToReturn;
        }

        private static void AddItemToPromotedDictionary(Dictionary<string, Dictionary<long, PromotedProductUI>> productDic, PromotedProductUI item, decimal supplierPrice)
        {
            if (!productDic.ContainsKey(item.Section))
                productDic.Add(item.Section, new Dictionary<long, PromotedProductUI>());
            if (!productDic[item.Section].ContainsKey(item.ProductId))
            {
                item.MinPrice = item.MaxPrice = supplierPrice;
                productDic[item.Section].Add(item.ProductId, item);
                return;
            }
            var previousProduct = productDic[item.Section][item.ProductId];
            if (previousProduct.MaxPrice < supplierPrice)
                previousProduct.MaxPrice = supplierPrice;
            if (previousProduct.MinPrice > supplierPrice)
                previousProduct.MinPrice = supplierPrice;
        }

        public static List<PromotedProductAreaUI> GetPromotedAreaProducts(int AreaId = 0, int PageSize = 0, int CurrentPageIndex = 0)
        {
            var listOfPromotedArea = new List<PromotedProductAreaUI>();
            Query qry = new Query(PromotedArea.TableSchema);
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var supplierPromotedUI = new PromotedProductAreaUI();
                    supplierPromotedUI.PromotedAreaName = (string)reader[PromotedArea.Columns.Name];
                    supplierPromotedUI.PromotedAreaId = (int)reader[PromotedArea.Columns.Id];
                    supplierPromotedUI.ProductPromoted = GetPromotedProducts(supplierPromotedUI.PromotedAreaId);
                    listOfPromotedArea.Add(supplierPromotedUI);
                }
            }
            return listOfPromotedArea;
        }

        private static List<PromotedProductUI> GetPromotedProducts(int areaId)
        {
            Query qry = new Query(PromotedProduct.TableSchema);
            qry.Join(JoinType.InnerJoin, PromotedProduct.TableSchema, PromotedProduct.Columns.AreaId, PromotedProduct.TableSchema.SchemaName,
                PromotedArea.TableSchema, PromotedArea.Columns.Id, PromotedArea.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, PromotedProduct.TableSchema, PromotedProduct.Columns.ProductId, PromotedProduct.TableSchema.SchemaName,
               Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);

            qry.SelectAllTableColumns();
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductId, Product.Columns.ProductId);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductCode, Product.Columns.ProductCode);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductName, Product.Columns.ProductName);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductImage, Product.Columns.ProductImage);
            qry.AddSelect(PromotedArea.TableSchema.SchemaName, PromotedArea.Columns.Id, "PromotedAreaId");

            qry.Where(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.AreaId, WhereComparision.EqualsTo, areaId);
            var wl = new WhereList().OR(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Deleted, WhereComparision.EqualsTo, null)
                    .OR(PromotedProduct.TableSchema.SchemaName, PromotedProduct.Columns.Deleted, WhereComparision.GreaterThan, DateTime.Now);
            qry.AddWhere(WhereCondition.AND, wl);
            var wl1 = new WhereList().OR(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.EqualsTo, false).
                OR(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.EqualsTo, null);
            qry.AddWhere(WhereCondition.AND, wl1);
            qry.OrderBy(PromotedProduct.Columns.Weight, SortDirection.ASC);
            List<PromotedProductUI> lstProducts = new List<PromotedProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstProducts.Add(new PromotedProductUI(reader));
                }
            }

            return lstProducts;
        }

        public static List<MaxMinProductUI> GetProductsByUserCity(List<int> busketProductIds, Int64 CategoryId = 0, Int64 SubCategoryId = 0, Int64 AnimalId = 0, Int64 AppUserId = 0, Int64 TempAppUserId = 0)
        {
            var suppliers = GetSuppliersThatSupplyBusketInCity(busketProductIds, 0, AppUserId, TempAppUserId);
            Query qry = new Query(Product.TableSchema).Select(Product.Columns.ProductId).Distinct();
            qry.SelectAllTableColumns();
            if (CategoryId != 0 && SubCategoryId != 0 && AnimalId != 0)
            {
                qry.Where(Product.Columns.CategoryId, CategoryId);
                qry.AddWhere(Product.Columns.SubCategoryId, SubCategoryId);
                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                          ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qry.AddWhere(ProductAnimal.Columns.AnimalId, AnimalId);
            }

            qry.AddSelectLiteral(" MAX(SupplierProduct.Price) as maxPrice ");
            qry.AddSelectLiteral(" MIN(SupplierProduct.Price) as minPrice ");
            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.NotEqualsTo, true);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);

            qry.GroupBy(Product.TableSchema.SchemaName, Product.Columns.ProductId);
            qry.OrderBy(Product.TableSchema.SchemaName, Product.Columns.ProductRate, SortDirection.DESC);

            var lstProduct = new List<MaxMinProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var product = new MaxMinProductUI();
                    product.Read(reader);
                    product.MinPrice = reader["minPrice"] != null ? Convert.ToDecimal(reader["minPrice"]) : 0;
                    product.MaxPrice = reader["maxPrice"] != null ? Convert.ToDecimal(reader["maxPrice"]) : 0;
                    product.LstFilters = GetAllFilterByProductId(product.ProductId);
                    lstProduct.Add(product);
                }
            }

            return lstProduct;
        }

        public static List<ProductUI> GetProductsWithAnimalId()
        {
            var suppliers = GetSuppliersThatSupplyBusketInCity(null, 0, 0, 0);
            Query qry = new Query(Product.TableSchema).Select(Product.Columns.ProductId).Distinct();
            qry.SelectAllTableColumns();
            qry.AddSelectLiteral("productanimal.AnimalId");
            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
            qry.Join(JoinType.InnerJoin, ProductAnimal.TableSchema, ProductAnimal.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, ProductAnimal.Columns.ProductId));
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.NotEqualsTo, true);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);
            qry.GroupBy(Product.TableSchema.SchemaName, Product.Columns.ProductId);
            qry.OrderBy(Product.TableSchema.SchemaName, Product.Columns.ProductRate, SortDirection.DESC);

            var lstProduct = new List<ProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var product = new ProductUI();
                    product.Read(reader);
                    product.LstFilters = GetAllFilterByProductId(product.ProductId);
                    product.AnimalName = reader[ProductAnimal.Columns.AnimalId].ToString() == "1" ? "dog" : "cat";
                    lstProduct.Add(product);
                }
            }

            return lstProduct;
        }

        public static List<MaxMinProductUI> GetProductsByProductName(List<int> busketProductIds, string productName, Int64 CategoryId = 0, Int64 SubCategoryId = 0, Int64 AnimalId = 0, Int64 AppUserId = 0, Int64 TempAppUserId = 0)
        {
            var lstProduct = new List<MaxMinProductUI>();
            if (productName == null || productName.Length < 3)
                return lstProduct;
            var suppliers = GetSuppliersThatSupplyBusketInCity(busketProductIds, 0, AppUserId, TempAppUserId);
            Query qry = new Query(Product.TableSchema).Select(Product.Columns.ProductId).Distinct();
            qry.SelectAllTableColumns();
            if (CategoryId != 0 && SubCategoryId != 0 && AnimalId != 0)
            {
                qry.Where(Product.Columns.CategoryId, CategoryId);
                qry.AddWhere(Product.Columns.SubCategoryId, SubCategoryId);
                qry.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                          ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qry.AddWhere(ProductAnimal.Columns.AnimalId, AnimalId);
            }

            qry.AddSelectLiteral(" MAX(SupplierProduct.Price) as maxPrice ");
            qry.AddSelectLiteral(" MIN(SupplierProduct.Price) as minPrice ");
            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            if (suppliers.Count > 0)
                qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.ProductName, WhereComparision.Like, "%" + productName + "%");

            qry.GroupBy(Product.TableSchema.SchemaName, Product.Columns.ProductId);
            qry.OrderBy(Product.TableSchema.SchemaName, Product.Columns.ProductRate, SortDirection.DESC);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var product = new MaxMinProductUI();
                    product.Read(reader);
                    product.MinPrice = reader["minPrice"] != null ? Convert.ToDecimal(reader["minPrice"]) : 0;
                    product.MaxPrice = reader["maxPrice"] != null ? Convert.ToDecimal(reader["maxPrice"]) : 0;
                    product.LstFilters = GetAllFilterByProductId(product.ProductId);
                    lstProduct.Add(product);
                }
            }

            return lstProduct;
        }

        private static List<Int64> GetSuppliersThatSupplyProductForCity(long productId, long cityId, List<int> busketProductIds)
        {
            if (cityId <= 0)
                return null;

            List<Int64> suppliers = null;

            suppliers = new List<long>();
            Query innerQuery = new Query(SupplierProduct.TableSchema);
            innerQuery.Join(JoinType.InnerJoin, SupplierCity.TableSchema, SupplierCity.TableSchema.SchemaName,
                new JoinColumnPair(SupplierProduct.TableSchema, SupplierProduct.Columns.SupplierId, SupplierProduct.Columns.SupplierId));
            innerQuery.Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName,
                   new JoinColumnPair(SupplierCity.TableSchema, SupplierCity.Columns.SupplierId, AppSupplier.Columns.SupplierId));

            innerQuery.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            innerQuery.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsLocked, WhereComparision.EqualsTo, false);
            innerQuery.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Status, WhereComparision.NotEqualsTo, false);
            innerQuery.AddWhere(SupplierCity.TableSchema.SchemaName, SupplierCity.Columns.CityId, WhereComparision.EqualsTo, cityId);
            innerQuery.AddWhere(SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            innerQuery.Select(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, SupplierProduct.Columns.SupplierId, false).GroupBy(SupplierProduct.Columns.SupplierId);
            innerQuery.AddSelectLiteral(" COUNT(" + SupplierProduct.TableSchema.SchemaName + "." + SupplierProduct.Columns.SupplierId + ") as `suppliercount`");

            int count = 0;
            if (busketProductIds != null && busketProductIds.Count > 0)
            {
                count = busketProductIds.Count;
                if (!busketProductIds.Contains((int)productId))
                    count++;
                var addDefaultProducts = new WhereList().OR(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, WhereComparision.In, busketProductIds)
                   .OR(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, WhereComparision.EqualsTo, productId);
                innerQuery.AddWhere(WhereCondition.AND, addDefaultProducts);
            }

            else
                innerQuery.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, WhereComparision.EqualsTo, productId);

            using (DataReaderBase reader = innerQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (busketProductIds != null && busketProductIds.Count > 0)
                    {
                        if (Convert.ToInt32(reader["suppliercount"]) == count)
                        {
                            suppliers.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                        }
                    }
                    else
                        suppliers.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                }
            }

            return suppliers;
        }



        public static Product GetProductById(Int64 productId, List<int> busketProductIds, out decimal minPrice, out decimal maxPrice, long cityId = 0)
        {
            Query qry = new Query(Product.TableSchema);
            qry.SelectAllTableColumns();
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.ProductId, WhereComparision.EqualsTo, productId);
            maxPrice = 0;
            minPrice = 0;
            var suppliers = GetSuppliersThatSupplyProductForCity(productId, cityId, busketProductIds);
            if (suppliers != null && suppliers.Count > 0)
            {
                qry.AddSelectLiteral(" MAX(SupplierProduct.Price) as maxPrice ");
                qry.AddSelectLiteral(" MIN(SupplierProduct.Price) as minPrice ");
                qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, SupplierProduct.Columns.ProductId));
                qry.Join(JoinType.InnerJoin, SupplierCity.TableSchema, SupplierCity.TableSchema.SchemaName, new JoinColumnPair(SupplierProduct.TableSchema, SupplierProduct.Columns.SupplierId, SupplierCity.Columns.SupplierId));
                qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.In, suppliers);
                qry.GroupBy(Product.TableSchema.SchemaName, Product.Columns.ProductId);
            }

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    var item = new Product();
                    item.Read(reader);
                    if (suppliers != null)
                    {
                        minPrice = reader["minPrice"] != null ? Convert.ToDecimal(reader["minPrice"]) : 0;
                        maxPrice = reader["maxPrice"] != null ? Convert.ToDecimal(reader["maxPrice"]) : 0;
                    }
                    return item;
                }
            }
            return null;
        }

        public static List<ProductUI> GetProductsWithIds(IEnumerable<Int64> productIds)
        {
            Query qry = new Query(Product.TableSchema);
            qry.Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.CategoryId, Category.Columns.CategoryId));
            qry.Join(JoinType.InnerJoin, SubCategory.TableSchema, SubCategory.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId));
            qry.Join(JoinType.InnerJoin, ProductAnimal.TableSchema, ProductAnimal.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, ProductAnimal.Columns.ProductId));
            qry.Join(JoinType.InnerJoin, Animal.TableSchema, Animal.TableSchema.SchemaName, new JoinColumnPair(ProductAnimal.TableSchema, ProductAnimal.Columns.AnimalId, Animal.Columns.AnimalId));
            qry.SelectAllTableColumns();
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, SubCategory.Columns.SubCategoryName);
            qry.AddSelect(Animal.TableSchema.SchemaName, Animal.Columns.AnimalName, Animal.Columns.AnimalName);
            qry.AddWhere(Product.TableSchema.SchemaName, Product.Columns.ProductId, WhereComparision.In, productIds.ToList());
            var products = new List<ProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var item = new ProductUI();
                    item.Read(reader);
                    products.Add(item);
                    item.CategoryName = reader[Category.Columns.CategoryName].ToString();
                    item.SubCategoryName = reader[SubCategory.Columns.SubCategoryName].ToString();
                    item.AnimalName = reader[Animal.Columns.AnimalName].ToString();
                }
            }
            return products;
        }

        public static List<ProductUI> GetProductsByBid(Int64 BidId)
        {
            Query qry = new Query(Product.TableSchema);
            qry.Join(JoinType.InnerJoin,  Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.CategoryId, Category.Columns.CategoryId));
            qry.Join(JoinType.InnerJoin, SubCategory.TableSchema, SubCategory.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId));
            qry.Join(JoinType.InnerJoin, ProductAnimal.TableSchema, ProductAnimal.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema, Product.Columns.ProductId, ProductAnimal.Columns.ProductId));
            qry.Join(JoinType.InnerJoin, Animal.TableSchema, Animal.TableSchema.SchemaName, new JoinColumnPair(ProductAnimal.TableSchema, ProductAnimal.Columns.AnimalId, Animal.Columns.AnimalId));

            qry.SelectAllTableColumns();
            qry.AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName);
            qry.AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, SubCategory.Columns.SubCategoryName);
            qry.AddSelect(Animal.TableSchema.SchemaName, Animal.Columns.AnimalName, Animal.Columns.AnimalName);
            qry.Where(BidProduct.TableSchema.SchemaName, BidProduct.Columns.BidId, WhereComparision.EqualsTo, BidId);
            //qry.GroupBy(BidProduct.TableSchema.SchemaName, BidProduct.Columns.ProductId);

            List<ProductUI> products = new List<ProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var item = new ProductUI();
                    item.Read(reader);
                    products.Add(item);
                    item.CategoryName = reader[Category.Columns.CategoryName].ToString();
                    item.SubCategoryName = reader[SubCategory.Columns.SubCategoryName].ToString();
                    item.AnimalName = reader[Animal.Columns.AnimalName].ToString();
                }
            }

            return products;


        }

        public static bool IsSupplierProduct(Int64 ProductId)
        {
            Query q = new Query(SupplierProduct.TableSchema);
            q.Where(SupplierProduct.Columns.ProductId, ProductId);
            q.Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName, new JoinColumnPair(SupplierProduct.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, SupplierProduct.Columns.SupplierId));
            q.AddWhere(AppSupplier.Columns.IsDeleted, false);
            return Convert.ToBoolean(q.ExecuteScalar());
        }


        public static bool IsCategoryInUse(Int64 CategoryId)
        {
            Query q = new Query(Product.TableSchema);
            q.Where(Product.Columns.CategoryId, CategoryId);
            q.AddWhere(Product.Columns.IsDeleted, false);
            return Convert.ToBoolean(q.ExecuteScalar());
        }

        public static bool IsFilterInUse(Int64 FilterId)
        {
            Query q = new Query(ProductFilter.TableSchema);
            q.Where(ProductFilter.Columns.FilterId, FilterId);
            return Convert.ToBoolean(q.ExecuteScalar());
        }

        public static bool IsSubCategoryInUse(Int64 SubCategoryId)
        {
            Query q = new Query(Product.TableSchema);
            q.Where(Product.Columns.SubCategoryId, SubCategoryId);
            q.AddWhere(Product.Columns.IsDeleted, false);
            return Convert.ToBoolean(q.ExecuteScalar());
        }

        public static AnimalCollection GetAnimalByProductId(Int64 ProductId)
        {
            Query q = new Query(Animal.TableSchema)
            .Join(JoinType.InnerJoin, ProductAnimal.TableSchema, ProductAnimal.TableSchema.SchemaName, new JoinColumnPair(Animal.TableSchema.SchemaName, Animal.Columns.AnimalId, ProductAnimal.Columns.AnimalId))
            .Where(ProductAnimal.Columns.ProductId, ProductId);
            return AnimalCollection.FetchByQuery(q);
        }

        public static List<ProductUI> GetAllProductUI(string ProductCode = "", Int64 category = 0, Int64 subcategory = 0, int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query q = new Query(Product.TableSchema)
            .SelectAllTableColumns()
            .AddSelect(Category.TableSchema.SchemaName, Category.Columns.CategoryName, Category.Columns.CategoryName)
            .AddSelect(SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryName, SubCategory.Columns.SubCategoryName)
            .Join(JoinType.InnerJoin, Category.TableSchema, Category.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema.SchemaName, Product.Columns.CategoryId, Category.Columns.CategoryId))
            .Join(JoinType.InnerJoin, SubCategory.TableSchema, SubCategory.TableSchema.SchemaName, new JoinColumnPair(Product.TableSchema.SchemaName, Product.Columns.SubCategoryId, SubCategory.Columns.SubCategoryId))
            .Where(Product.Columns.IsDeleted, false);

            if (ProductCode != "")
                q.AddWhere(Product.Columns.ProductCode, WhereComparision.Like, ProductCode);

            if (category != 0)
            {
                q.AddWhere(Product.TableSchema.SchemaName, Product.Columns.CategoryId, WhereComparision.EqualsTo, category);
                if (subcategory != 0)
                    q.AddWhere(Product.TableSchema.SchemaName, Product.Columns.SubCategoryId, WhereComparision.EqualsTo, subcategory);
            }
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            q.OrderBy(Product.Columns.ProductRate, SortDirection.ASC);
            List<ProductUI> productLst = new List<ProductUI>();

            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    ProductUI productUI = new ProductUI();
                    productUI.ProductId = Convert.ToInt64(reader[Product.Columns.ProductId]);
                    productUI.ProductNum = (reader[Product.Columns.ProductNum] is DBNull) ? (Int64?)null : Convert.ToInt64(reader[Product.Columns.ProductNum]);
                    productUI.ProductName = reader[Product.Columns.ProductName] == null ? "" : reader[Product.Columns.ProductName].ToString();
                    productUI.ProductCode = reader[Product.Columns.ProductCode] == null ? "" : reader[Product.Columns.ProductCode].ToString();
                    productUI.Amount = reader[Product.Columns.Amount] == null ? "" : reader[Product.Columns.Amount].ToString();
                    productUI.ProductImage = reader[Product.Columns.ProductImage] == null ? "" : reader[Product.Columns.ProductImage].ToString();
                    productUI.Description = reader[Product.Columns.Description] == null ? "" : reader[Product.Columns.Description].ToString();
                    productUI.CreateDate = ((DateTime)reader[Product.Columns.CreateDate]).ToLocalTime();
                    productUI.AnimalLst = GetAnimalByProductId(Convert.ToInt64(reader[Product.Columns.ProductId]));
                    productUI.CategoryName = reader[Category.Columns.CategoryName] == null ? "" : reader[Category.Columns.CategoryName].ToString();
                    productUI.SubCategoryName = reader[SubCategory.Columns.SubCategoryName] == null ? "" : reader[SubCategory.Columns.SubCategoryName].ToString();
                    productUI.ProductRate = (reader[Product.Columns.ProductRate] == null) ? 0 : Convert.ToInt64(reader[Product.Columns.ProductRate]);
                    //
                    productUI.RecomendedPrice = (Decimal)(reader[Product.Columns.RecomendedPrice] == null ? 0 : reader[Product.Columns.RecomendedPrice]);
                    //
                    productLst.Add(productUI);
                    if (productUI.ProductImage.ToLower().Contains(".jpg") || productUI.ProductImage.ToLower().Contains(".jpeg") || productUI.ProductImage.ToLower().Contains(".png"))
                        productUI.IsImage = true;
                }
            }

            return productLst;
        }

        public static Int64 GetFilterIdByProductId(Int64 ProductId)
        {
            Query q = new Query(ProductFilter.TableSchema)
            .Select(ProductFilter.Columns.FilterId)
            .Where(ProductFilter.Columns.ProductId, ProductId);
            return Convert.ToInt64(q.ExecuteScalar());
        }

        public static void DeleteFilterForProduct(Int64 ProductId, Int64 FilterId)
        {
            Query q = new Query(ProductFilter.TableSchema)
            .Where(ProductFilter.Columns.FilterId, FilterId)
            .AddWhere(ProductFilter.Columns.ProductId, ProductId);

            ProductFilterCollection filterCol = ProductFilterCollection.FetchByQuery(q);
            foreach (var item in filterCol)
            {
                ProductFilter.Delete(item.ProductId, item.FilterId, item.SubFilterId);
            }
        }

        public static string ConvertListToString(AnimalCollection lst)
        {
            string str = "";
            foreach (var item in lst)
            {
                str += item.AnimalName + ", ";
            }
            return (str.Length > 0 ? str.Substring(0, str.LastIndexOf(",")) : str);

        }

        public static string ConvertSubFilterListToString(List<SubFilterUI> lst)
        {
            string str = "";
            foreach (var item in lst)
            {
                str += item.SubFilterName + ", ";
            }
            return (str.Length > 0 ? str.Substring(0, str.LastIndexOf(",")) : str);
        }


        public static List<SubCategory> GetSubCategoryByCategoryID(Int64 categoryID)
        {
            Query q = new Query(SubCategory.TableSchema);
            q.Where(SubCategory.TableSchema.SchemaName, SubCategory.Columns.CategoryId, WhereComparision.EqualsTo, categoryID);
            List<SubCategory> subcategories = new List<SubCategory>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    subcategories.Add(new SubCategory
                    {
                        SubCategoryId = Convert.ToInt64(reader[SubCategory.Columns.SubCategoryId]),
                        SubCategoryName = reader[SubCategory.Columns.SubCategoryName].ToString(),
                        SubCategoryImage = reader[SubCategory.Columns.SubCategoryImage].ToString(),
                    });
                }
            }
            return subcategories;
        }

        public static List<Category> getAllCategoriesWithSubCategories()
        {
            Query q1 = new Query(SubCategory.TableSchema).Select(SubCategory.Columns.CategoryId).Distinct();
            Query q2 = new Query(Category.TableSchema).SelectAll()
                      .Where(Category.Columns.CategoryId, WhereComparision.In, q1);

            List<Category> categories = new List<Category>();
            using (DataReaderBase reader = q2.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = Convert.ToInt64(reader[Category.Columns.CategoryId]),
                        CategoryName = reader[Category.Columns.CategoryName].ToString(),
                        CategoryImage = reader[Category.Columns.CategoryImage].ToString(),
                    });
                }
            }
            return categories;
        }
    }
}
