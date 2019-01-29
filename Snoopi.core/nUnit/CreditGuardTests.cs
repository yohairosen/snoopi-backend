using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Snoopi.core.BL;
using Snoopi.core.DAL;

namespace Snoopi.core
{
    [TestFixture]
    public class CreditGuardTests
    {
        [Test]
        public void runCreditGuard()
        {
            var paymentDetails = new PaymentDetails
            {
                Amount = (float)12,
                CreditId = "1061724217431708",
                Exp = "0917",
                AuthNumber = "1919572",
                NumOfPayments =1 
            };
            var response = CreditGuardManager.CreateMPITransaction(paymentDetails);
        }

        [Test]
        public void sendEmail ()
        {
            var bid = new BidMessage { BidId = 123};
            BIdMessageController.send_message_to_admin(bid);
        }
    }

}
