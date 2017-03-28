using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Util;

namespace Venom.Lib.Extension
{
    /// <summary>
    /// Add behavior for the Type instance objects
    /// </summary>
    public static class TypeExtension
    {
        public static FilterAttribute GetDefaultFilterAttribute(this Type target)
        {
            return FilterManager.Default[target.FullName];
        }
    }
}
