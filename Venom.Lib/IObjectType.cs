using System;
namespace Venom.Lib
{
    /// <summary>
    /// Represents an ObjectType on the System
    /// </summary>
    public interface IObjectType
    {
        /// <summary>
        /// Get or set the Name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Get or set the Id.
        /// </summary>
        int ObjectTypeId { get; set; }
    }
}
