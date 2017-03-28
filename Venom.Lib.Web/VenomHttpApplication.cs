using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Venom.Lib.Data;

namespace Venom.Web
{
    /// <summary>
    /// All the Global.asax webapplications must be inherit of this type, because here, ensure a unique instance by session of the 
    /// EntityDataAccess handler
    /// </summary>
    public class VenomHttpApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Occurs when a request is started
        /// </summary>
        protected virtual void Application_BeginRequest()
        {
        }

        /// <summary>
        /// Occurs when the request ends, here, the cache session is killed
        /// </summary>
        protected virtual void Application_EndRequest()
        {
            Venom.Lib.Data.CacheManager.Remove(Thread.CurrentThread.ManagedThreadId.ToString());
        }

        /// <summary>
        /// Occurs when the application is stopped, here, the cache is killed
        /// </summary>
        protected virtual void Application_End()//Application_Disposed()
        {
            Venom.Lib.Data.CacheManager.Destroy();
        }

    }
}
