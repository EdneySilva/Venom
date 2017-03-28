using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Web.Helper
{
    /// <summary>
    /// Helper to extend functions to IdentityUser instances
    /// </summary>
    internal static class IdentityUserHelper
    {
        /// <summary>
        /// Generate a claimidentity to an useridentity instance
        /// </summary>
        /// <param name="identity"> the current identity instance</param>
        /// <param name="manager">the user manager</param>
        /// <returns>a task to run and create a claimsidentity</returns>
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this IdentityUser identity, UserManager<IdentityUser> manager)
        {
            try
            {
                // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
                var userIdentity = await manager.CreateIdentityAsync(identity, DefaultAuthenticationTypes.ApplicationCookie);
                // Add custom user claims here            
                return userIdentity;
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
