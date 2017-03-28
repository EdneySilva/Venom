using System;
using System.Collections.Generic;
namespace Venom.Lib
{
    public interface IMenu
    {
        /// <summary>
        /// Get or set the Id.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Get or set the Name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Get or set the Url.
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// Get or set the order of menu
        /// </summary>
        int Order { get; set; }
        /// <summary>
        /// Get or set the Parent Id.
        /// </summary>
        int? ParentId { get; set; }
        /// <summary>
        /// Get or set the Parent Item.
        /// </summary>
        IMenu Parent { get; set; }
        /// <summary>
        /// Return the subitens related to that has this instance as a parent menu
        /// </summary>
        IEnumerable<IMenu> SubItens { get; }
    }
}
