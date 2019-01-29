using Snoopi.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snoopi.web
{
    public class ApplicationGlobal : System.Web.HttpApplication
    {
        SendMessagesService _messagesService = new SendMessagesService();
        SendEmailService _emailService = new SendEmailService();

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            _messagesService.Start();
            _emailService.Start();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            _messagesService.Stop();
            _emailService.Stop();
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }
    }
}
