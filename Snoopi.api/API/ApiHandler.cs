using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using dg.Utilities.WebApiServices;

namespace Snoopi.api
{
    public class ApiHandler : RestHandler
    {
        public ApiHandler()
        {
            string pathPrefix = AppConfig.GetString(@"ApiHandler.PathPrefix", @"");

            this.PathPrefix = pathPrefix;
            //this.AutomaticallyHandleEndingSlash = true;
            //this.AddRoute(@"temp/image", new TempImageDownloadHandler(), true, false, false, false, true);
            this.AddRoute(@"auth/signup", new SignupHandler(), false, true, true, false, false);
            this.AddRoute(@"auth/login", new LoginHandler(), false, true, true, false, false);
            this.AddRoute(@"auth/logout", new LogoutHandler(), false, true, true, false, false);
            this.AddRoute(@"me", new AppUserProfileHandler(), true, true, true, false, false);
            this.AddRoute(@"me/receive_advertisement", new AdvertismentHandler(), false, true, true, false, false);
            this.AddRoute(@"me/last_shopping_cart", new ShoppingCartHandler(), true, false, false, false, false);
            this.AddRoute(@"validate/version", new ValidateVersionHandler(), false, true, false, false, false);
            this.AddRoute(@"auth/validate", new AuthValidateHandler(), false, true, true, false, false);
            this.AddRoute(@"auth/forgot", new ForgotPasswordHandler(), false, true, true, false, false);
            this.AddRoute(@"auth/facebook", new LoginFacebookHandler(), false, true, true, false, false);

            this.AddRoute(@"me/missing_details", new AppUserFullProfileDetailsHandler(), true, false, false, false, false);

            this.AddRoute(@"start_page", new StartPageHandler(), true, false, false, false, false);
            
            this.AddRoute(@"categories", new CategoryHandler(), true, false, false, false, false);
            this.AddRoute(@"sub_categories", new SubCategoryHandler(), true, false, false, false, false);
            //this.AddRoute(@"services", new ServicesAllHandler(), true, false, false, false, false);

            this.AddRoute(@"bid/service", new ServiceHandler(), false, true, false, false, false);
            this.AddRoute(@"bid/service/offer/details", new ServiceHandler(), true, false, false, false, false);
            this.AddRoute(@"bid/service/offers", new OffersServiceHandler(), true, false, false, false, false);

            this.AddRoute(@"prom", new AdsHandler(), true, false, false, false, false);
            this.AddRoute(@"prom/image", new AdsImageHandler(), true, false, false, false, false);

            //this.AddRoute(@"filter_product", new FilterHandler(), true, false, false, false, false);
            this.AddRoute(@"products/promoted", new ProductsPromotedHandler(), true, false, false, false, false);
            this.AddRoute(@"products", new ProductsHandler(), true, false, false, false, false);
            this.AddRoute(@"products/search", new SearchProductsHandler(), true, false, false, false, false);
            this.AddRoute(@"product", new ProductHandler(), true, false, false, false, false);
            this.AddRoute(@"product/image/download", new MediaFileHandler(), true, false, false, false, true);

            this.AddRoute(@"me/animal", new MyAnimalHandler(), true, true, true, true, false);
            //this.AddRoute(@"me/image/upload", new MediaFileHandler(), false, true, true, true, false);
            this.AddRoute(@"me/image/download", new MediaFileHandler(), true, false, false, false, true);
            this.AddRoute(@"me/card", new MyCardHandler(), true, false, false, false, false);
            this.AddRoute(@"me/products/yad2", new MyProductsYad2Handler(), true, false, false, false, false);
                       
            this.AddRoute(@"bid/max", new BidCheckSupplierWinningsHandler(),false,true,false,false,false);
            this.AddRoute(@"bid/product", new BidProductHandler(), false , true, true, true, false);
            this.AddRoute(@"compare/prices", new ComparePricesHandler(), false, true, true, true, false);
            this.AddRoute(@"cart/new", new CartsHandler(), false, true, false, false, false);

            this.AddRoute(@"bid/offers", new OffersHandler(), true, false, false, false, false);
            this.AddRoute(@"bid/close", new BidCloseHandler(), true, false, false, false, false);
            this.AddRoute(@"order", new OrderPaymentHandler(), false, true, false, false, false);
            this.AddRoute(@"order/pre", new PreOrderPaymentHandler(), false, true, false, false, false);
            this.AddRoute(@"order/validate", new PriceValidityHandler(), false, true, false, false, false);
            this.AddRoute(@"order/close", new ProductOrderHandler(), false, true, false, false, false);
            this.AddRoute(@"order/history", new OrderHistoryHandler(), true, false, false, false, false);
            this.AddRoute(@"order/details", new OrderHandler(), true, false, false, false, false);
            this.AddRoute(@"order/received", new OrderReceivedHandler(), false, true, false, false, false);

            this.AddRoute(@"processing/preorder", new ProcessingUrlHandler(), false, true, false, false, false);
            this.AddRoute(@"processing/results", new ProcessingResultsHandler(), true, false, false, false, false);
            this.AddRoute(@"processing/card", new SavedCardProcessingHandler(),false, true, false, false, false);

            this.AddRoute(@"push/post-purchace", new PushHandler(), false, true, false, false, false);

            this.AddRoute(@"donation", new DonationHandler(), true, false, false, false, false);
            

            this.AddRoute(@"supplier", new SupplierHandler(), true, false, false, false, false);
            this.AddRoute(@"mail", new MailHandler(), false, true, true, false, false);
            this.AddRoute(@"supplier/rate", new SupplierRateHandler(), false, true, false, false, false);
            this.AddRoute(@"auth/temp", new TempAppUserHandler(), false, true, true, false, false);
            this.AddRoute(@"user/city", new UserCityHandler(),false, true, false, false, false);
            //this.AddRoute(@"city", new CityHandler(), true, false, false, false, false);

            this.AddRoute(@"product/yad2/edit", new ProductYad2Handler(), false, true, false, false, false);
            this.AddRoute(@"product/yad2/delete", new ProductYad2Handler(), false, false, false, true, false);
            this.AddRoute(@"product/yad2/new", new ProductYad2Handler(), false, true, false, false, false);
            this.AddRoute(@"product/yad2/view", new ProductYad2Handler(), true, false, false, false, false);
            this.AddRoute(@"product/yad2/jump", new ProductYad2JumpHandler(), false, true, false, false, false);
            this.AddRoute(@"products/yad2", new ProductsYad2Handler(), true, false, false, false, false);
            //this.AddRoute(@"products/yad2/filter", new ProductsYad2FilterHandler(), true, false, false, false, false);
            //this.AddRoute(@"product/yad2/image/upload", new MediaFileHandler(), false, true, true, true, false);
            this.AddRoute(@"product/yad2/image/download", new MediaFileHandler(), true, false, false, false, true);


            this.AddRoute(@"me/apns/#*", new AppUserAPNSTokenHandler(), true, true, true, true, false);
            this.AddRoute(@"s/me/apns/#*", new SupplierAPNSTokenHandler(), true, true, true, true, false);
            this.AddRoute(@"me/gcm/#*", new AppUserGcmTokenHandler(), true, true, true, true, false);
            this.AddRoute(@"s/me/gcm/#*", new SupplierGcmTokenHandler(), true, true, true, true, false);
            this.AddRoute(@"me/reset/notification", new AppUserResetNotificationHandler(), true, true, false, false, false);
            this.AddRoute(@"crons/#match:^(rematch|clean_tokens)$", new CronJobHandler(), true, true, true, false, false);

            this.AddRoute(@"setting", new SettingHandler(), true, true, true, false, false);
            this.AddRoute(@"image/download", new MediaFileHandler(), true, false, false, false, true);
            this.AddRoute(@"crons/#match:^(bid|offer|service_offer|rate_supplier|test_rate_supplier|order_received|auto_push)$", new CronJobHandler(), true, true, true, false, false);

            this.AddRoute(@"events/registration", new SupplierEventHandler(), false, true, true, false, false);

            this.AddRoute(@"s/auth/login", new SupplierLoginHandler(), false, true, true, false, false);
            this.AddRoute(@"s/auth/validate", new SupplierAuthValidateHandler(), false, true, true, false, false);
            this.AddRoute(@"s/auth/logout", new SupplierLogoutHandler(), false, true, true, false, false);
            this.AddRoute(@"s/me/profile", new SupplierHandler(), false, true, true, false, false);
            this.AddRoute(@"s/me/receive_advertisement", new SupplierAdvertismentHandler(), false, true, true, false, false);
            this.AddRoute(@"s/auth/forgot", new SupplierForgotPasswordHandler(), false, true, true, false, false);
            this.AddRoute(@"s/bid/new", new SupplierNewBidsHandler(), true, false, false, false, false);

           this.AddRoute(@"s/service/results", new ServiceSuppliersHandler(), true, false, false, false, false);

            
            this.AddRoute(@"s/bid/new/details", new SupplierNewBidHandler(), true, false, false, false, false);
            this.AddRoute(@"s/bid/new/join", new SupplierNewBidHandler(), false, true, false, false, false);
            
            this.AddRoute(@"s/bid/active", new SupplierOfferBidsHandler(), true, false, false, false, false);
            this.AddRoute(@"s/bid/active/details", new SupplierOfferBidHandler(), true, false, false, false, false);
            this.AddRoute(@"s/bid/approval", new SupplierBidApprovalHandler(), false, true, false, false, false);
            
            this.AddRoute(@"s/bid/win", new SupplierWinBidsHandler(), true, false, false, false, false);
            this.AddRoute(@"s/bid/win/details", new SupplierWinBidHandler(), true, false, false, false, false);
            this.AddRoute(@"s/bid/win/supplied", new SupplierWinBidHandler(), false, true, false, false, false);

            this.AddRoute(@"s/validate/version", new SupplierValidateVersionHandler(), false, true, false, false, false);
            this.AddRoute(@"s/me/reset/notification", new AppSupplierResetNotification(),true, true, false, false, false);

            
        }
    }
}
