using System;
namespace Venom.Lib
{
    public interface ISystemObject : IVenomObject<ISystemObject>
    {
        /// <summary>
        /// Get or set the system object instance
        /// </summary>
        ISystemObject ParentItem { get; set; }
        /// <summary>
        /// Get or set the parent item id
        /// </summary>
        int? ParentItemId { get; set; }
        /// <summary>
        /// Get or set the status of the systemobject.
        /// </summary>
        bool IsEnable { get; set; }
        /// <summary>
        /// Get o set the id of the systemobject.
        /// </summary>
        int ItemId { get; set; }
        /// <summary>
        /// Get or set the name of the systemobject.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Get or set a object type instance about systemobject
        /// </summary>
        IObjectType ObjectType { get; set; }
        /// <summary>
        /// Get or set the type of the systemobject.
        /// </summary>
        int ObjectTypeId { get; set; }
        /// <summary>
        /// Check if the systemobject is on the role group
        /// </summary>
        /// <param name="roles">the rules</param>
        /// <returns>true if is on role</returns>
        bool IsInRoles(string[] roles);
    }
}
