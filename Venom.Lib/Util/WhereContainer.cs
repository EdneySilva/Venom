using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Represent a container based in a Where Object.
    /// </summary>
    class WhereContainer
    {
        /// <summary>
        /// Get or set the properties used on the Where Object.
        /// </summary>
        public PropertyInfo Property { get; set; }
        /// <summary>
        /// Get or set the left expression on the Where object.
        /// </summary>
        public Expression LeftExpression { get; set; }
        /// <summary>
        /// Get or set the filtres used on the Where Object.
        /// </summary>
        public FilterAttribute[] Filter { get; set; }
    }

}
