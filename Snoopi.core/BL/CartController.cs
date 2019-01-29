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
    public class CartUi
    {
        public int CartId { get; set; }
        public List<BidProductUI> Products { get; set; }
        public AppUserUI User { get; set; }
        public Int64 SupplierId { get; set; }
        public string SupplierBusinessName { get; set; }
        public decimal TotalSum { get; set; }
    }

    public class CartController
    {
        public static List<CartUi> GetLastNDaysCarts(int nDays)
        {
            var listOfCarts = new List<CartUi>();
            Query qry = new Query(Cart.TableSchema);
            qry.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName, new JoinColumnPair(Cart.TableSchema.SchemaName, Cart.Columns.UserId, AppUser.Columns.AppUserId));
            qry.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName, new JoinColumnPair(Cart.TableSchema.SchemaName, Cart.Columns.TempUserId, TempAppUser.Columns.TempAppUserId));
            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName, new JoinColumnPair(Cart.TableSchema.SchemaName, Cart.Columns.SupplierId, AppSupplier.Columns.SupplierId));
            qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName, new JoinColumnPair(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId)
                .JoinOR(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.CityId, TempAppUser.Columns.CityId));

            qry.AddSelect(Cart.TableSchema.SchemaName, Cart.Columns.CartId, Cart.Columns.CartId);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Email, AppUser.Columns.Email);
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Street, AppUser.Columns.Street);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.HouseNum, AppUser.Columns.HouseNum);
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.ApartmentNumber, AppUser.Columns.ApartmentNumber);
            qry.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId);
            qry.AddSelect(Cart.TableSchema.SchemaName, Cart.Columns.CartTotalPrice, Cart.Columns.CartTotalPrice);

            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
                     
            qry.AddWhere(Cart.Columns.CreatedDate, WhereComparision.GreaterThan, DateTime.Now.AddDays(-nDays));
            qry.OrderBy(Cart.Columns.CartId, SortDirection.DESC);
            
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    var cartUi = new CartUi();
                    cartUi.User = new AppUserUI();
                    cartUi.CartId = (int)reader[CartProduct.Columns.CartId];
                    cartUi.User.FirstName = reader[AppUser.Columns.FirstName] != null ? reader[AppUser.Columns.FirstName].ToString() : "";
                    cartUi.User.LastName = reader[AppUser.Columns.LastName] != null ? reader[AppUser.Columns.LastName].ToString() : "";
                    cartUi.User.Phone = reader[AppUser.Columns.Phone] != null ? reader[AppUser.Columns.Phone].ToString() : "";
                    cartUi.User.UniqueEmail = reader[AppUser.Columns.Email] != null ? reader[AppUser.Columns.Email].ToString() : "";
                    cartUi.User.CityName = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    cartUi.User.Street = reader[AppUser.Columns.Street] != null ? reader[AppUser.Columns.Street].ToString() : "";
                    cartUi.User.HouseNum = reader[AppUser.Columns.HouseNum] != null ? reader[AppUser.Columns.HouseNum].ToString() : "";
                    cartUi.User.ApartmentNumber = reader[AppUser.Columns.ApartmentNumber] != null ? reader[AppUser.Columns.ApartmentNumber].ToString() : "";
                    cartUi.SupplierId = Convert.ToInt64(reader[AppSupplier.Columns.SupplierId]);
                    cartUi.SupplierBusinessName =  reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "";
                    cartUi.Products = GetProductsByCartId(cartUi.CartId);
                    cartUi.TotalSum = reader[Cart.Columns.CartTotalPrice] != null ? (decimal)reader[Cart.Columns.CartTotalPrice] : 0;
                    listOfCarts.Add(cartUi);
                }
            }
            return listOfCarts;
        }

        private static List<BidProductUI> GetProductsByCartId(int cartId)
        {
            Query qry = new Query(CartProduct.TableSchema);
            qry.Join(JoinType.InnerJoin, CartProduct.TableSchema, CartProduct.Columns.CartId, CartProduct.TableSchema.SchemaName,
                Cart.TableSchema, Cart.Columns.CartId, Cart.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, CartProduct.TableSchema, CartProduct.Columns.ProductId, CartProduct.TableSchema.SchemaName,
               Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);

            qry.SelectAllTableColumns();
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.RecomendedPrice, Product.Columns.RecomendedPrice);
            qry.AddSelect(CartProduct.TableSchema.SchemaName, CartProduct.Columns.ProductAmount, CartProduct.Columns.ProductAmount);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductName, Product.Columns.ProductName);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductId, Product.Columns.ProductId);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.Amount, Product.Columns.Amount);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductImage, Product.Columns.ProductImage);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductCode, Product.Columns.ProductCode);
            qry.Where(CartProduct.TableSchema.SchemaName, CartProduct.Columns.CartId, WhereComparision.EqualsTo, cartId);

            List<BidProductUI> lstProduct = new List<BidProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstProduct.Add(new BidProductUI
                    {
                        Amount = (reader[CartProduct.Columns.ProductAmount] != null ? (int)reader[CartProduct.Columns.ProductAmount] : 1),
                        ProductId = (reader[Product.Columns.ProductId] != null ? (Int64)reader[Product.Columns.ProductId] : 0),
                        ProductName = (reader[Product.Columns.ProductName] != null ? reader[Product.Columns.ProductName].ToString() : ""),
                        ProductAmount = (reader[CartProduct.Columns.ProductAmount] != null ? reader[CartProduct.Columns.ProductAmount].ToString() : ""),
                        ProductImage = (reader[Product.Columns.ProductImage] != null ? reader[Product.Columns.ProductImage].ToString() : ""),
                        ProductCode = (reader[Product.Columns.ProductCode] != null ? reader[Product.Columns.ProductCode].ToString() : ""),
                        RecomendedPrice = reader[Product.Columns.RecomendedPrice] != null ? Convert.ToInt32(reader[Product.Columns.RecomendedPrice]) : 0
                    });
                }
            }

            return lstProduct;
        }

        public static int CreateCart(Int64 AppUserId, Int64 TempAppUserId, Int64 supplierId, decimal totalPrice, Dictionary<Int64, int> LstProduct)
        {
            var cart = new Cart
            {
                UserId = AppUserId > 0 ? AppUserId : (long?)null,
                CartTotalPrice = totalPrice,
                SupplierId = supplierId,
                CreatedDate = DateTime.Now,
                TempUserId = TempAppUserId > 0 ? TempAppUserId : (long?)null
            };
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                cart.Save(conn);
            }
            foreach (KeyValuePair<Int64, int> item in LstProduct)
            {
                CartProduct cartProduct = new CartProduct();
                cartProduct.CartId = cart.CartId;
                cartProduct.ProductAmount = item.Value;
                cartProduct.ProductId = item.Key;
                cartProduct.Save();
            }

            return cart.CartId;
        }

    }
}
