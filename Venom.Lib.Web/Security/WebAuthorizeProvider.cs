using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib;

namespace Venom.Web.Security
{
    /// <summary>
    /// Define a authorize provider to be used on the Venom.Web
    /// </summary>
    internal class WebAuthorizeProvider : Lib.Security.IAuthorizeProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="claims">the user claims</param>
        public WebAuthorizeProvider(IEnumerable<IUserClaim> claims)
        {
            if(claims != null)
                UserClaim = claims.ToArray();
        }

        /// <summary>
        /// Return all user claims
        /// </summary>
        public IEnumerable<IUserClaim> UserClaim { get; private set; }

        /// <summary>
        /// Check if the current claims have permission for a systemobject
        /// </summary>
        /// <param name="systemObject">the system object to be available</param>
        /// <returns>true when the systemoject is on the claims</returns>
        public bool Authorized(ISystemObject systemObject)
        {
            if(UserClaim == null || systemObject == null)
                return false;
            return systemObject.IsInRoles(UserClaim.Where(a => a.ClaimType.Equals(System.Security.Claims.ClaimTypes.Role)).Select(s => s.ClaimValue).ToArray());
        }
    }
}
