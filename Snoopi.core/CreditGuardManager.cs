using dg.Utilities;
using Snoopi.infrastructure.Loggers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;

namespace Snoopi.core
{
   
    public class PaymentDetails
    {
        public float Amount { get; set; }
        public string CreditId { get; set; }
        public string Cvv { get; set; }
        public string Exp { get; set; }
        public string HolderId { get; set; }
        public string AuthNumber { get; set; }
        public string SupplierToken { get; set; }
        public int NumOfPayments { get; set; }
    }
    public static class CreditGuardManager
    {
        static string _terminalId { get; set; }
        static string _snoopyTerminalId { get; set; }
        static string _newCardTerminalId { get; set; }
        static string _userId { get; set; }
        static string _password { get; set; }
        static string _gatewayUrl { get; set; }
        static string _mid { get; set; }
        static string _returnUrl { get; set; }
        static string _suppliersForSnoopyTerminal { get; set; }


        static CreditGuardManager()
        {
            _terminalId = AppConfig.GetString(@"cgTerminal", @"");
            _newCardTerminalId = AppConfig.GetString(@"cgNewCardTerminalId", @"");
            _userId = AppConfig.GetString(@"cgUser", @"");
            _password = AppConfig.GetString(@"cgPassword", @"");
            _gatewayUrl = AppConfig.GetString(@"cgGateway", @"");
            _mid = AppConfig.GetString(@"cgMid", @"");
            _returnUrl = AppConfig.GetString(@"cgReturnUrl", @"");
            _snoopyTerminalId = AppConfig.GetString(@"cgTerminalSnoopy", @"");
            _suppliersForSnoopyTerminal = AppConfig.GetString(@"suppliersForSnoopyTerminal", @"");
        }

        public static ProcessingResults QueryMPITransaction(string transactionId)
        {
            if ((transactionId == "") || (transactionId == null))
                return null;
          
            var rand = new Random();
            string uniqueID = DateTime.Now.ToString("yyyyddMM") + rand.Next(0, 1000);
            string result = "";
            string poststring = "user=" + _userId +
                                  "&password=" + _password +
                                  "&int_in=<ashrait>" +
                                  "<request>" +
                                   "<language>HEB</language>" +
                                   "<command>inquireTransactions</command>" +
                                   "<inquireTransactions>" +
                                    "<terminalNumber>" + _newCardTerminalId + "</terminalNumber>" +
                                    "<mainTerminalNumber/>" +
                                    "<queryName>mpiTransaction</queryName>" +
                                    "<mid>" + _mid + "</mid>" +
                                    "<mpiTransactionId>" + transactionId + "</mpiTransactionId>" +
                                    "<userData1/>" +
                                    "<userData2/>" +
                                    "<userData3/>" +
                                    "<userData4/>" +
                                    "<userData5/>" +
                                   "</inquireTransactions>" +
                                  "</request>" +
                                 "</ashrait>";


            StreamWriter myWriter = null;

            var objRequest = (HttpWebRequest)WebRequest.Create(_gatewayUrl);
            objRequest.Method = "POST";
            objRequest.ContentLength = poststring.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            var processingResults = new ProcessingResults();
            Helpers.LogProcessing("QueryMPITransaction - " + transactionId + " - request", poststring, false);         
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(poststring);
            }
            catch (Exception e)
            {
                //log e.Message;
                Helpers.LogProcessing("QueryMPITransaction - ex -" + transactionId + " - ", result + "\n exception: " + e.ToString(), true);

                processingResults.ErrorMessage = e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            var objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                Helpers.LogProcessing("QueryMPITransaction - " + transactionId + " - response", result, false);

                // Close and clean up the StreamReader
                sr.Close();
            }


            var doc = new XmlDocument();
            doc.LoadXml(result);

            string cardToken = doc.GetElementsByTagName("cardId")[0].InnerText;
            processingResults.ResultCode =  doc.GetElementsByTagName("result")[1].InnerText;
            processingResults.UniqueId = Convert.ToInt64(doc.GetElementsByTagName("uniqueid")[0].InnerText);
            processingResults.AuthNumber = doc.GetElementsByTagName("authNumber")[0].InnerText;
            processingResults.CardToken = doc.GetElementsByTagName("cardId")[0].InnerText;
            processingResults.CardExpiration = doc.GetElementsByTagName("cardExpiration")[0].InnerText;
            processingResults.PersonalId = doc.GetElementsByTagName("personalId")[0].InnerText;
            processingResults.TransactionID = doc.GetElementsByTagName("tranId")[0].InnerText;
            processingResults.Last4Digits = cardToken.Substring(cardToken.Length - 4);
            if (doc.GetElementsByTagName("statusText") != null && doc.GetElementsByTagName("statusText").Count > 1)
                processingResults.ErrorMessage = doc.GetElementsByTagName("statusText")[1].InnerText;

            return processingResults;
        }

        public static string GetCgUrl(long appUserId, decimal amount, long uniqueId, int numberOfPayments, string mastercardId,
           string specialInstructions, out string transactionId)
        {
            string result = "";
            int firstPayment = 0;
            int periodicalPaymentNum = 0;
            int periodicalPayment = 0;
            string ampTag = "%26amp;";
            string numOfPaymentsText = "<numberOfPayments/>";
            string creditType  = "RegularCredit";
            transactionId = null; 
            if (numberOfPayments > 1)
            {
                periodicalPayment = (100 / numberOfPayments);
                periodicalPaymentNum = numberOfPayments - 1;
                creditType = "Payments";
                firstPayment = (100 - periodicalPayment * periodicalPaymentNum);
                numOfPaymentsText = "<numberOfPayments>" + periodicalPaymentNum + "</numberOfPayments>" +
                                                    "<firstPayment>" + firstPayment + "</firstPayment>" +
                                                    "<periodicalPayment>" + periodicalPayment + "</periodicalPayment>";
            }

            string poststring = "user=" + _userId +
                                  "&password=" + _password +
                                  "&int_in=<ashrait>" +
                                     "<request>" +
                                      "<version>1000</version>" +
                                      "<language>HEB</language>" +
                                      "<dateTime/>" +
                                      "<command>doDeal</command>" +
                                      "<doDeal>" +
                                           "<terminalNumber>" + _newCardTerminalId + "</terminalNumber>" +
                                           "<mainTerminalNumber/>" +
                                           "<cardNo>CGMPI</cardNo>" +
                                           "<successUrl>" + _returnUrl +"</successUrl>" +
                                           "<cancelUrl>" + _returnUrl + "</cancelUrl>" +
                                           "<errorUrl>" + _returnUrl + "</errorUrl>" +
                                           "<total>100</total>" +
                                           "<transactionType>Debit</transactionType>" +
                                           "<currency>ILS</currency>" +
                                           "<transactionCode>Phone</transactionCode>" +
                                           "<authNumber/>" +
                                           numOfPaymentsText +
                                           "<creditType>" + creditType + "</creditType>"  +
                                           "<validation>TxnSetup</validation>" +
                                           "<dealerNumber>" + mastercardId + "</dealerNumber>" +

                                           "<user>" + _userId + "</user>" +
                                           "<mid>" + _mid + "</mid>" +
                                           "<uniqueid>" + uniqueId + "</uniqueid>" +
                                           "<mpiValidation>verify</mpiValidation>" +
                                           "<email>snoopy@creditguard.co.il</email>" +
                                           "<clientIP/>" +
                                           "<description>"  + amount + "</description>" +

                                           "<customerData>" +
                                            "<userData2>" + numberOfPayments + "</userData2>" +
                                            "<userData3>1</userData3>" +
                                            "<userData4>" + appUserId + "</userData4>" +
                                            "<userData5/>" +
                                            "<userData6/>" +
                                            "<userData7/>" +
                                            "<userData8/>" +
                                            "<userData9/>" +
                                            "<userData10/>" +
                                           "</customerData>" +
                                      "</doDeal>" +
                                     "</request>" +
                                    "</ashrait>";


            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(_gatewayUrl);
            objRequest.Method = "POST";
            objRequest.ContentLength = poststring.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            //ServicePointManager.ServerCertificateValidationCallback =
            //delegate(object s, X509Certificate certificate,
            //         X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //{ return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(poststring);
            }
            catch (Exception e)
            {
                Helpers.LogProcessing("GetCgUrl - ex -" + transactionId + " -  ", result + "\n exception: " + e.ToString(), true);

                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();

                // Close and clean up the StreamReader
                sr.Close();
            }

            result = result.Replace("&", ampTag);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);

            XmlNodeList Nodes = doc.GetElementsByTagName("mpiHostedPageUrl");
            transactionId = doc.GetElementsByTagName("tranId")[0].InnerText;
            string response = Nodes[0].InnerText;
            // return "<script>window.location='" + response + "';</" + "script>";
            return response;
        }

        public static ProcessingResults ProcessSavedCard(long appUserId, decimal amount, int numberOfPayments, string mastercardId,
           string specialInstructions, string cardToken, string cardExpiration, out string transactionId)
        {
            string result = "";
            int firstPayment = 0;
            int periodicalPaymentNum = 0;
            int periodicalPayment = 0;
            string numOfPaymentsText = "<numberOfPayments/>";
            string creditType = "RegularCredit";
            transactionId = null;
            if (numberOfPayments > 1)
            {
                periodicalPayment = (100 / numberOfPayments);
                periodicalPaymentNum = numberOfPayments - 1;
                creditType = "Payments";
                firstPayment = (100 - periodicalPayment * periodicalPaymentNum);
                numOfPaymentsText = "<numberOfPayments>" + periodicalPaymentNum + "</numberOfPayments>" +
                                                    "<firstPayment>" + firstPayment + "</firstPayment>" +
                                                    "<periodicalPayment>" + periodicalPayment + "</periodicalPayment>";
            }
            
            bool isProduction = Convert.ToBoolean(AppConfig.GetString(@"IsProduction", @"false"));
            String poststring = "user=" + _userId +
                    "&password=" + _password +
                    "&int_in=" +
                    "<ashrait>" +
                    "<request>" +
                    "<command>doDeal</command>" +
                    "<version>1001</version>" +
                    "<language>HEB</language>" +
                    "<mayBeDuplicate>0</mayBeDuplicate>" +
                    "<doDeal>" +
                        "<terminalNumber>" + _terminalId + "</terminalNumber>" +
                        "<cardId>" + cardToken + "</cardId>" +
                        "<cardExpiration>" + cardExpiration + "</cardExpiration>" +
                        "<transactionType>Debit</transactionType>" +
                        "<currency>ILS</currency>" +
                        "<transactionCode>Phone</transactionCode>" +
                        "<total>" + 100 + "</total>" +
                        "<validation>Verify</validation>";
            if (isProduction)
                poststring += "<dealerNumber>" + mastercardId + "</dealerNumber>";
                      poststring +=  "<authNumber/>" +
                        numOfPaymentsText +
                        "<creditType>" + creditType + "</creditType>" +
                        "<user>" + _userId + "</user>" +
                         "<customerData>" +
                        "<userData2>" + numberOfPayments + "</userData2>" +
                        "<userData3>" + specialInstructions + "</userData3>" +
                        "<userData4>" + appUserId + "</userData4>" +
                        "<userData5/>" +
                        "<userData6/>" +
                        "<userData7/>" +
                        "<userData8/>" +
                        "<userData9/>" +
                        "<userData10/>" +
                        "</customerData>" +
                    "</doDeal>" +
                    "</request>" +
                    "</ashrait>";

            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(_gatewayUrl);
            objRequest.Method = "POST";
            objRequest.ContentType = "application/x-www-form-urlencoded";
            //ServicePointManager.ServerCertificateValidationCallback =
            //delegate(object s, X509Certificate certificate,
            //         X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //{ return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var processingResults = new ProcessingResults();
            try
            {
                Helpers.LogProcessing("ProcessSavedCard- " + cardToken + "- request", poststring, false);
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(poststring);
            }
            catch (Exception e)
            {
                Helpers.LogProcessing("ProcessSavedCard - ex -" + cardToken +  " - ", result + "\n exception: " + e.ToString(), true);
                processingResults.ErrorMessage = e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            var objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                Helpers.LogProcessing("ProcessSavedCard- " + cardToken + "- request", result,false);

                // Close and clean up the StreamReader
                sr.Close();
            }


            var doc = new XmlDocument();
            doc.LoadXml(result);

            processingResults.ResultCode = doc.GetElementsByTagName("result")[0].InnerText;
            processingResults.AuthNumber = doc.GetElementsByTagName("authNumber")[0].InnerText;
            processingResults.CardToken = cardToken;
            processingResults.Last4Digits = cardToken.Substring(cardToken.Length - 4);
            processingResults.CardExpiration = doc.GetElementsByTagName("cardExpiration")[0].InnerText;
            processingResults.TransactionID = doc.GetElementsByTagName("tranId")[0].InnerText;

            return processingResults;
        }

        public static string CreateMPITransaction(PaymentDetails paymentDetails)
        {         
            var suppliers = _suppliersForSnoopyTerminal.Split(',');
            bool isSnoopySupplier = suppliers.Any(x => x == paymentDetails.SupplierToken);
            string terminalId = isSnoopySupplier ? _snoopyTerminalId : _terminalId;

            ILogger logger = new FileLogger();
            int periodicalPayment = (int)paymentDetails.Amount / paymentDetails.NumOfPayments;
            float firstPayment = paymentDetails.Amount - ((paymentDetails.NumOfPayments - 1) * periodicalPayment);
            String result = "";
            var req = new StringBuilder();
            req.AppendFormat("user={0}&password={1}&int_in=<ashrait><request>", _userId, _password);
            req.Append("<language>ENG</language><command>doDeal</command><requestId/><version>1000</version>");
            req.AppendFormat("<doDeal><terminalNumber>{0}</terminalNumber><transactionCode>Phone</transactionCode>", terminalId);
            req.AppendFormat("<transactionType>Debit</transactionType><total>{0}</total>", paymentDetails.Amount);
            req.AppendFormat("<creditType>{0}</creditType>", paymentDetails.NumOfPayments > 1 ? "Payments" : "RegularCredit");
            if (paymentDetails.NumOfPayments > 1)
            {
                req.AppendFormat("<numberOfPayments>{0}</numberOfPayments>", paymentDetails.NumOfPayments - 1);
                req.AppendFormat("<firstPayment>{0}</firstPayment>", firstPayment);
                req.AppendFormat("<periodicalPayment>{0}</periodicalPayment>", periodicalPayment);
            }
            req.AppendFormat("<cardId>{0}</cardId><authNumber>{1}</authNumber><cvv>{2}</cvv>", paymentDetails.CreditId, paymentDetails.AuthNumber, paymentDetails.Cvv);
            req.AppendFormat("<cardExpiration>{0}</cardExpiration>", paymentDetails.Exp);
            if (!isSnoopySupplier)
                req.AppendFormat("<dealerNumber>{0}</dealerNumber>", paymentDetails.SupplierToken);
            req.Append("<validation>AutoComm</validation><customerData><userData1/><userData2/><userData3/><userData4/>" +
                                          "<userData5/></customerData><currency>ILS</currency>");
            req.AppendFormat("<id>{0}</id></doDeal></request></ashrait>", paymentDetails.HolderId);

            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(_gatewayUrl);
            objRequest.Method = "POST";
            objRequest.ContentLength = req.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            //ServicePointManager.Expect100Continue = true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
                                                   | (SecurityProtocolType)768
                                                   | (SecurityProtocolType)3072;

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);
            objRequest.Timeout = 50000;
            try
            {
                Helpers.LogProcessing("CreateMPITransaction- " + paymentDetails.CreditId + " - ", req.ToString(),false);

                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(req);
            }
            catch (Exception e)
            {
                Helpers.LogProcessing("CreateMPITransaction - ex -" + paymentDetails.CreditId + " - ",  "\n exception: " + e.ToString(), true);
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                Helpers.LogProcessing("CreateMPITransaction- " + paymentDetails.CreditId + "- response", result, false);

                // Close and clean up the StreamReader
                sr.Close();
            }


            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);

            string response = doc.GetElementsByTagName("status")[0].InnerText;
            return response;
        }

        private static bool AllwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        
    }

    public class ProcessingResults
    {
        public string ResultCode { get; set; }
        public long UniqueId { get; set; }
        public string ErrorMessage { get; set; }
        public string CardToken { get; set; }
        public string AuthNumber { get; set; }
        public string CardExpiration { get; set; }
        public string TransactionID { get; set; }
        public string Last4Digits { get; set; }
        public string SpecialInstructions { get; set; }
        public int NumOfPayments { get; set; }
        public string PersonalId { get; set; }
    }
}
