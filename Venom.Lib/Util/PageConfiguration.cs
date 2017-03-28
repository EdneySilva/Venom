using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Data;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Keep and handle de configuration to pagination collections.
    /// </summary>
    class PageConfiguration
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        PageConfiguration()
        {

        }

        /// <summary>
        /// Return the page size of a collection.
        /// Default page size is 5.
        /// </summary>
        public static int PageSize
        {
            get
            {
                var pageSize = CacheManager.Get<int>("pageConfig");
                return pageSize > 0 ? pageSize : 5;
            }
        }

        /// <summary>
        /// Create the default configuration.
        /// </summary>
        /// <param name="pageSize">the page size of each collection</param>
        public static void Create(int pageSize)
        {
            CacheManager.AddItem<int>("pageConfig", pageSize);
        }
    }
}
