﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Snoopi.core.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AppResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Snoopi.core.Properties.AppResources", typeof(AppResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to הזמנתך נכשלה! אנא צור קשר עם שירות הלקוחות שלנו..
        /// </summary>
        internal static string adminRejected {
            get {
                return ResourceManager.GetString("adminRejected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to עסקה בוטלה עקב סירוב של כרטיס האשראי.
        /// </summary>
        internal static string creditRejected {
            get {
                return ResourceManager.GetString("creditRejected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to לקוח מחכה לתגובתך באפליקציה.
        /// </summary>
        internal static string newBid {
            get {
                return ResourceManager.GetString("newBid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to עסקה בהזדמנות מחכה לאישורך.
        /// </summary>
        internal static string newBidPremium {
            get {
                return ResourceManager.GetString("newBidPremium", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to תודה שחסכת אצלנו 😊 הזמנתך נקלטה במערכת, במידה וביקשת שיצרו איתך קשר, החנות תיצור עמך קשר לתאום המשלוח. לכל שאלה ובקשה אנחנו כאן 03-6161962,שירות לקוחות סנופי.
        /// </summary>
        internal static string newBidSMSMessage {
            get {
                return ResourceManager.GetString("newBidSMSMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to תודה שחסכת אצלנו 😊 הזמנתך נקלטה במערכת, במידה וביקשת שיצרו איתך קשר, החנות תיצור עמך קשר לתאום המשלוח. לכל שאלה ובקשה אנחנו כאן 03-6161962,שירות לקוחות סנופי.
        /// </summary>
        internal static string newBidSMSMessageOd {
            get {
                return ResourceManager.GetString("newBidSMSMessageOd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to פניה חדשה של לקוח מחכה לתגובתך.
        /// </summary>
        internal static string newServiceBid {
            get {
                return ResourceManager.GetString("newServiceBid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to תודה שחסכת אצלנו :)
        ///הזמנתך תתקבל עד 5 ימי עסקים מרגע ההזמנה. במידה וביקשת שיצרו איתך קשר,
        ///החנות תיצור עימך קשר לתאום המשלוח. 
        ///לכל שאלה ובקשה אנחנו כאן
        ///03-6161962,שירות לקוחות סנופי.
        /// </summary>
        internal static string oldMessage {
            get {
                return ResourceManager.GetString("oldMessage", resourceCulture);
            }
        }
    }
}