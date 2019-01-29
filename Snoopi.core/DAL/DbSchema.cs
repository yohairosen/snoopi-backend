using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.BL;

namespace Snoopi.core.DAL
{
    public static class DbSchema
    {
        static TableSchema[] Schemas = new TableSchema[] { 
            ProductYad2Category.TableSchema,
            PriceFilter.TableSchema,
            AppUserCampaign.TableSchema,
            AppUser.TableSchema,
            AppUserAuthToken.TableSchema,
            User.TableSchema,
            UserAuthToken.TableSchema,
            AppUserAPNSToken.TableSchema,
            Category.TableSchema,
            UserProfile.TableSchema,
            Permission.TableSchema,
            UserPermissionMap.TableSchema,
            Setting.TableSchema,
            EmailLog.TableSchema,
            EmailTemplate.TableSchema,
            WebErrorLog.TableSchema,
            AppUserAnimal.TableSchema,
            Animal.TableSchema,
            AppSupplier.TableSchema,
            AppSupplierAuthToken.TableSchema,
            AppUserCard.TableSchema,
            Area.TableSchema,
            Bid.TableSchema,
            BidProduct.TableSchema,
            BidService.TableSchema,
            Campaign.TableSchema,
            Category.TableSchema,
            CategoryYad2.TableSchema,
            City.TableSchema,
            Comment.TableSchema,
            Donation.TableSchema,
            Filter.TableSchema,
            Offer.TableSchema,
            OfferService.TableSchema,
            Order.TableSchema,
            Product.TableSchema,
            ProductAnimal.TableSchema,
            ProductFilter.TableSchema,
            ProductYad2.TableSchema,
            Service.TableSchema,
            Setting.TableSchema,
            SubCategory.TableSchema,
            SubFilter.TableSchema,
            SubCategoryFilter.TableSchema,
            SupplierCity.TableSchema,
            SupplierHomeServiceCity.TableSchema,
            SupplierProduct.TableSchema,
            SupplierService.TableSchema,
            TempAppUser.TableSchema,
            Message.TableSchema,
            AppUserGcmToken.TableSchema,
            AppSupplierGcmToken.TableSchema



        };
        public static void CreateOrUpgrade()
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                conn.BeginTransaction();

                foreach (TableSchema schema in Schemas)
                {
                    if (!conn.CheckIfTableExists(schema.SchemaName))
                    {
                        try { new Query(schema).CreateTable().Execute(conn); }
                        catch { }
                        try
                        {
                            if (schema.Indexes.Count > 0 || schema.ForeignKeys.Count > 0)
                            {
                                conn.ExecuteScript(new Query(schema).CreateIndexes().ToString());
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        foreach (TableSchema.Column col in schema.Columns)
                        {
                            try { new Query(schema).AddColumn(col).Execute(conn); }
                            catch { }
                            try { new Query(schema).ChangeColumn(col).Execute(conn); }
                            catch { }
                        }
                        foreach (TableSchema.Index idx in schema.Indexes)
                        {
                            try { new Query(schema).CreateIndex(idx).Execute(conn); }
                            catch { }
                        }
                        foreach (TableSchema.ForeignKey key in schema.ForeignKeys)
                        {
                            try { new Query(schema).CreateForeignKey(key).Execute(conn); }
                            catch { }
                        }
                    }
                }

                foreach (string key in Permissions.SystemPermissionKeys)
                {
                    try
                    {
                        if (PermissionCollection.Where(Permission.Columns.Key, key).Count == 0)
                        {
                            new Permission(key).Save(conn);
                        }
                    }
                    catch { }
                }
                conn.CommitTransaction();
            }
        }
    }
}
