using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib
{
    /// <summary>
    /// Represents a menu item.
    /// </summary>
    [Table("Menu")]
    public class Menu : VenomObject<Menu>, Venom.Lib.IMenu
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Menu()
        {
            SubItens = new HashSet<Menu>();
        }

        /// <summary>
        /// Get or set the Id.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Get or set the Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the Url.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Get or set the order of menu
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Get or set the Parent Id.
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// Get or set the Parent Item.
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual Menu Parent { get; set; }
        /// <summary>
        /// Return the subitens related to that has this instance as a parent menu
        /// </summary>
        public virtual ICollection<Menu> SubItens { get; protected set; }
        /// <summary>
        /// Get or set the Parent Item.
        /// </summary>
        IMenu IMenu.Parent { get { return this.Parent; } set { this.Parent = value as Menu; } }
        /// <summary>
        /// Return the subitens related to that has this instance as a parent menu
        /// </summary>
        IEnumerable<IMenu> IMenu.SubItens { get { return SubItens.Cast<IMenu>().AsQueryable(); } }
    }
}
