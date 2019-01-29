
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Snoopi.api
{
     public class PushHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {         
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    streamWriter.Write("Success !! ");
                }
            }
            catch (Exception ex) {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    streamWriter.Write("Failure !! Exception: " + ex.Message + " Stack: " + ex.StackTrace);
                }
            }
        }
    }
}
