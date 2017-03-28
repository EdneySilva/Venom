using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Represent a Notation to storage the data about property used as a description in a relationship between objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ForeignKeyDescriptionAttribute : Attribute
    {
        /// <summary>
        /// the name of the property used as a description.
        /// </summary>
        readonly string name;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Return the name of the property used as a description.</param>
        public ForeignKeyDescriptionAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Return the name of the property used as a description.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
    }
}
