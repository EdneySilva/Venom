using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using Microsoft.Owin.Host;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Venom.Web
{
    /// <summary>
    /// Represente the base configurations to BaseController
    /// </summary>
    public class BaseControllerConfiguration : IDisposable
    {
        public static BaseControllerConfiguration Default
        {
            get { return System.Web.HttpContext.Current.GetOwinContext().Get<BaseControllerConfiguration>(); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseControllerConfiguration()
        {
            IndexViewName = "Index";
            CreateOrEditViewName = "CreateOrEdit";
            CreateViewName = "Create";
            EditViewName = "Edit";
            GridViewName = "Grid";
            PagedGridViewName = "PagedGrid";
            DeleteViewName = "Index";
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isDefault">define if this instance is an default instance</param>
        public BaseControllerConfiguration(bool isDefault)
            : this()
        {
            this.IsDefault = isDefault;
        }

        /// <summary>
        /// Return if the configuration is default
        /// </summary>
        public bool IsDefault { get; private set; }
        /// <summary>
        /// Get or set the IndexView name
        /// </summary>
        public string IndexViewName { get; set; }
        /// <summary>
        /// Get or set the CreateViewName name
        /// </summary>
        public string CreateViewName { get; set; }
        /// <summary>
        /// Get or set the EditViewName name
        /// </summary>
        public string EditViewName { get; set; }
        /// <summary>
        /// Get or set the CreateOrEditViewName name
        /// </summary>
        public string CreateOrEditViewName { get; set; }
        /// <summary>
        /// Get or set the GridViewName name
        /// </summary>
        public string GridViewName { get; set; }
        /// <summary>
        /// Get or set the PagedGridViewName name
        /// </summary>
        public string PagedGridViewName { get; set; }
        /// <summary>
        /// Get or set the DeleteViewName name. If you change it, use a valid Action name in your controller
        /// </summary>
        public string DeleteViewName { get; set; }

        /// <summary>
        /// Dispose this instance
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
