using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Util;

namespace Venom.Lib
{
    /// <summary>
    /// Represente all object systems, like, menus, butons, actions, controllers and wherever
    /// </summary>
    [Table("SystemObject")]
    public class SystemObject : VenomObject<SystemObject>, Venom.Lib.ISystemObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SystemObject()
        {
            Roles = new HashSet<Role>();
            Children = new HashSet<SystemObject>();
        }

        /// <summary>
        /// Get o set the id of the systemobject.
        /// </summary>
        [Key]
        public int ItemId { get; set; }

        /// <summary>
        /// Get or set the name of the systemobject.
        /// </summary>
        [Required]
        [MaxLength(150)]
        [Index("SystemObjectName")]
        [Display(Name = "NameLabel", ResourceType = typeof(ApplicationResource))]
        public string Name { get; set; }

        /// <summary>
        /// Get or set the type of the systemobject.
        /// </summary>
        [Display(Name = "ObjectTypeLabel", ResourceType = typeof(ApplicationResource))]
        public int ObjectTypeId { get; set; }

        /// <summary>
        /// Get or set the status of the systemobject.
        /// </summary>
        [Display(Name = "StatusLabel", ResourceType = typeof(ApplicationResource))]
        public bool IsEnable { get; set; }

        /// <summary>
        /// Get or set the parent item id
        /// </summary>
        [Display(Name = "FatherLabel", ResourceType = typeof(ApplicationResource))]
        public int? ParentItemId { get; set; }

        /// <summary>
        /// Get or set the system object instance
        /// </summary>
        [ForeignKey("ParentItemId")]
        public virtual SystemObject ParentItem { get; set; }

        /// <summary>
        /// Get or set a object type instance about systemobject
        /// </summary>
        [ForeignKey("ObjectTypeId")]
        public virtual ObjectType ObjectType { get; set; }

        /// <summary>
        /// Return all rules about the current systemobject instance
        /// </summary>
        public virtual ICollection<Role> Roles { get; protected set; }

        public virtual ICollection<SystemObject> Children { get; protected set; }
 
        /// <summary>
        /// Get or set the system object instance
        /// </summary>
        [NotMapped]
        ISystemObject ISystemObject.ParentItem
        {
            get { return this.ParentItem; }
            set { this.ParentItem = SystemObject.Parse(value); }
        }

        /// <summary>
        /// Get or set a object type instance about systemobject
        /// </summary>
        [NotMapped]
        IObjectType ISystemObject.ObjectType
        {
            get { return this.ObjectType; }
            set { this.ObjectType = Lib.ObjectType.Parse(value); }
        }

        /// <summary>
        /// Check if the systemobject is on the role group
        /// </summary>
        /// <param name="roles">the rules</param>
        /// <returns>true if is on role</returns>
        public bool IsInRoles(string[] roles)
        {
            return Roles.Any(a => roles.Contains(a.Name));
        }

        /// <summary>
        /// Get the item by the id.
        /// </summary>
        /// <returns>an instance of ISystemObject.</returns>
        ISystemObject IVenomObject<ISystemObject>.GetById()
        {
            return this.GetById();
        }

        /// <summary>
        /// Find the objects look is like the current instance.
        /// </summary>
        /// <returns>A collection of systemobjects</returns>
        IEnumerable<ISystemObject> IVenomObject<ISystemObject>.FindItensAsMe()
        {
            return this.FindItensAsMe().Cast<ISystemObject>();
        }

        //IQueryable<ISystemObject> IVenomObject<ISystemObject>.CreateQuery()
        //{
        //    return this.CreateQuery();
        //}

        //IEnumerable<ISystemObject> IVenomObject<ISystemObject>.FindItensAsMe(FilterRules<ISystemObject> filter)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<ISystemObject> IVenomObject<ISystemObject>.ToList(Expression<Func<ISystemObject, bool>> query)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Convert a ISystemObject to a SystemObject.
        /// </summary>
        /// <param name="value">the current value of isystemobject.</param>
        /// <returns>an instance of systemobject.</returns>
        public static SystemObject Parse(ISystemObject value)
        {
            return Parse(value, 0);
        }

        /// <summary>
        /// Convert a ISystemObject to a SystemObject.
        /// </summary>
        /// <param name="value">the current value of isystemobject.</param>
        /// <param name="depth">the current level that the cursos are posistioned, use it to avoid stackoverflow</param>
        /// <returns>an instance of systemobject.</returns>
        internal static SystemObject Parse(ISystemObject value, int depth)
        {
            if(depth > 30)
                return null;
            var item = new SystemObject
            {
                ParentItemId = value.ParentItemId,
                Name = value.Name,
                ObjectTypeId = value.ObjectTypeId,
            };
            return item.FindItensAsMe().FirstOrDefault() ?? new SystemObject
            {
                ParentItem = SystemObject.Parse(value.ParentItem, depth + 1),
                ParentItemId = value.ParentItemId,
                IsEnable = value.IsEnable,
                ItemId = value.ItemId,
                Name = value.Name,
                ObjectType = Lib.ObjectType.Parse(value.ObjectType),
                ObjectTypeId = value.ObjectTypeId,
            };
        }
    }
}
