using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Venom.Lib.Util;

namespace Venom.Lib
{
    /// <summary>
    /// Represent an object type(menu, link, button...)
    /// </summary>
    [Table("ObjectType")]
    public class ObjectType : VenomObject<ObjectType>, Venom.Lib.IObjectType
    {
        /// <summary>
        /// Get or set the id.
        /// </summary>
        [Key]
        public int ObjectTypeId { get; set; }
        /// <summary>
        /// Get or set the Name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Return the SystemObject when their ObjectType as like this current instance.
        /// </summary>
        public virtual ICollection<SystemObject> SystemObjects { get; protected set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectType()
        {
            this.SystemObjects = new HashSet<SystemObject>();
        }
        
        /// <summary>
        /// Parse a IObjectType interface instance to a concrete ObjectType instance
        /// </summary>
        /// <param name="value">the value will be parsed</param>
        /// <returns>an ObjectType instance</returns>
        public static ObjectType Parse(IObjectType value)
        {
            var item = new ObjectType
            {
                Name = value.Name,
                ObjectTypeId = value.ObjectTypeId
            };
            return item.FindItensAsMe().FirstOrDefault() ?? item;
        }
    }
}
