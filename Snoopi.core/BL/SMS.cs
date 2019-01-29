using dg.Utilities;
using Snoopi.core.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Snoopi.core.BL
{
   public class SMSController
    {
       public static void sendSMS(string PhoneNumberList , string message)
        {
            string userName = AppConfig.GetString(@"InforU_SMS_UserName", @""); 
            string password = AppConfig.GetString(@"InforU_SMS_Password", @"");

            string messageText = System.Security.SecurityElement.Escape(message);
            string sender = AppConfig.GetString(@"InforU_SMS_Sender", @"");

            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<Inforu>");
            sbXml.Append("<User>");
            sbXml.Append("<Username>" + userName + "</Username>");
            sbXml.Append("<Password>" + password + "</Password>");
            sbXml.Append("</User>");
            sbXml.Append("<Content Type=\"sms\">");
            sbXml.Append("<Message>" + messageText + "</Message>");
            sbXml.Append("</Content>");
            sbXml.Append("<Recipients>");
            sbXml.Append("<PhoneNumber>" + PhoneNumberList + "</PhoneNumber>");
            sbXml.Append("</Recipients>");
            sbXml.Append("<Settings>");
            sbXml.Append("<Sender>" + sender + "</Sender>");
            sbXml.Append("</Settings>");
            sbXml.Append("</Inforu >");
            string strXML = HttpUtility.UrlEncode(sbXml.ToString(), System.Text.Encoding.UTF8);
            string result = PostDataToURL(AppConfig.GetString(@"InforU_SMS_URL", @""), "InforuXML=" + strXML);

        }

        public static void sendNewBidSMS(string PhoneNumberList)
        {

            SMSController.sendSMS(PhoneNumberList, AppResources.newBidSMSMessage);

        }

        private   static string PostDataToURL(string szUrl, string szData)
        {     //Setup the web request      
            string szResult = string.Empty;
            System.Net.WebRequest Request = WebRequest.Create(szUrl);
            Request.Timeout = 30000;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";

            //Set the POST data in a buffer      
            byte[] PostBuffer;
            try 

    {         // replacing " " with "+" according to Http post RPC          
                szData = szData.Replace(" ", "+");

                //Specify the length of the buffer          
                PostBuffer = Encoding.UTF8.GetBytes(szData);
                Request.ContentLength = PostBuffer.Length;

                //Open up a request stream          
                Stream RequestStream = Request.GetRequestStream();

                //Write the POST data          
                RequestStream.Write(PostBuffer, 0, PostBuffer.Length);

                //Close the stream          
                RequestStream.Close();
                //Create the Response object         
                WebResponse Response;
                Response = Request.GetResponse();

                //Create the reader for the response        
                StreamReader sr = new StreamReader(Response.GetResponseStream(),     Encoding.UTF8);

                //Read the response         
                szResult = sr.ReadToEnd();

                //Close the reader, and response         
                sr.Close();
                Response.Close(); 

                return szResult;
            }
            catch (Exception e)
            {
                return szResult;
            }
        }

    }
}
