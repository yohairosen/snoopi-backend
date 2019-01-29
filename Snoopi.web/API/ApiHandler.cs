using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using dg.Utilities.WebApiServices;
using System.Web.SessionState;

namespace Snoopi.web.API
{
    public class ApiHandler : RestHandler, IReadOnlySessionState
    {
        public ApiHandler()
        {
            string pathPrefix = @"/web-api/";

            this.PathPrefix = pathPrefix;
            //this.AutomaticallyHandleEndingSlash = true;

            this.AddRoute(@"image", new ImageUploadHandler(), false, true, true, false, false);
        }
    }
}
