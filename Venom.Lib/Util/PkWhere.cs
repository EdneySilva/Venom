using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Represents a Where Expression Builder based on the primary key property.
    /// </summary>
    /// <typeparam name="T">type of the object about query is based.</typeparam>
    internal class PkWhere<T> : Where<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">query is built based on the object source.</param>
        public PkWhere(T source)
            : base(source)
        {
            Properties = typeof(T).GetProperties().Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Any() && w.GetCustomAttributes(typeof(KeyAttribute), false).Any()).ToArray();
        }
    }

}
