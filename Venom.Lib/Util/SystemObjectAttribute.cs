using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Set a class as an System Object.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class SystemObjectAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SystemObjectAttribute()
        {
        }
    }
}
