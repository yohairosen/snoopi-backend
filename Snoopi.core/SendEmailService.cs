using Snoopi.core.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Snoopi.core
{
    public class SendEmailService
    {
        private  Timer _timer { get; set; }
        private int _numberOfThreadsInside = 0;

        public SendEmailService()
        {
            _timer = new Timer();
            _timer.Interval = 1000 * 60;
            _timer.AutoReset = true;
            _timer.Elapsed += send_emails;
        }

        public void Start()
        {          
            _timer.Start();
        }

        private void send_emails(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            bool isNotActive = !BIdMessageController.IsSystemActive(now);
            if (isNotActive)
                return;
            
            System.Threading.Interlocked.Increment(ref _numberOfThreadsInside);
            try
            {
                if (_numberOfThreadsInside <= 1)
                {
                    BIdMessageController.SendEmails();
                }
            }
            finally
            {
                System.Threading.Interlocked.Decrement(ref _numberOfThreadsInside);
            }

        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
