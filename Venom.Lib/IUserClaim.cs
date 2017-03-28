using System;
namespace Venom.Lib
{
    /// <summary>
    /// Represent IUserClaim on the System.
    /// </summary>
    public interface IUserClaim
    {
        /// <summary>
        /// Get or set the Type ex: role.
        /// </summary>
        string ClaimType { get; set; }
        /// <summary>
        /// Get or set the value ex: rolename, or roleid.
        /// </summary>
        string ClaimValue { get; set; }
        /// <summary>
        /// Get or set the id.
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Get or set the User Id related with this claim.
        /// </summary>
        string UserId { get; set; }

        //ApplicationUser User { get; set; }
    }
}
