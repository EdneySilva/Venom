using System;
namespace Venom.Lib
{
    /// <summary>
    /// Represents a Role on the System.
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// Get or set the Id.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Get or set the Name.
        /// </summary>
        string Name { get; set; }
    }
}
