using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Runtime.Caching;

namespace Venom.Lib.Data
{
    /// <summary>
    /// Represent a repository object handler
    /// </summary>
    /// <typeparam name="T">type will be handled</typeparam>
    public sealed class Repository<T>  where T : new()
    {
        /// <summary>
        /// get or set the current context
        /// </summary>
        public T CurrentContext { get; set; }
        
        /// <summary>
        /// constructor.
        /// </summary>
        public Repository()
        {
            CurrentContext = new T();
        }
    }
}
