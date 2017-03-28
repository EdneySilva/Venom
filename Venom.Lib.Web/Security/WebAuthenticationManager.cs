using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Venom.Web.Security;
using System.Threading.Tasks;
using Venom.Lib;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Host.SystemWeb;

namespace Venom.Web.Security
{
    /// <summary>
    /// Manager the authentication job on the Venom.Web
    /// </summary>
    internal class WebAuthenticationManager : Lib.Security.AuthenticationManager
    {
        /// <summary>
        /// Release a async SignIn
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">password</param>
        /// <param name="isPersistent">define if the signIn will be persisted</param>
        /// <returns>a task to run and create the signIn result</returns>
        public override async System.Threading.Tasks.Task<Lib.Result> SigInAsync(string userName, string password, bool isPersistent)
        {
            var result = await base.SigInAsync(userName, password, isPersistent);
            if(result.Successed)
                CheckResponseGranted(userName, isPersistent);
            return result;
        }

        /// <summary>
        /// Release the application signOut
        /// </summary>
        public override void SignOut()
        {
            base.SignOut();
            this.CheckResponseRevoked();
        }
            
        /// <summary>
        /// Check if the authentication provider revoked the login, if not, create a responserevoke to do it.
        /// </summary>
        private void CheckResponseRevoked()
        {
            if (System.Web.HttpContext.Current.GetOwinContext().Authentication.AuthenticationResponseRevoke != null)
                return;
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut();
        }
        
        /// <summary>
        /// Check if the authentication provider granted for the response an identity and his respective claims, if not, create the responsegrant based on the username, and the roles
        /// provided by the provider.
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <param name="isPersistent">define if the signIn will be persisted</param>
        private void CheckResponseGranted(string userName, bool isPersistent)
        {
            // if the provider create the response greate then do nothing
            if (System.Web.HttpContext.Current.GetOwinContext().Authentication.AuthenticationResponseGrant != null)
                return;
            // create the claimsidentity to the context
            var claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);
            claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Guid.NewGuid().ToString(), "http://www.w3.org/2001/XMLSchema#string"));
            claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userName, "http://www.w3.org/2001/XMLSchema#string"));
            foreach (var item in this.AuthenticationProvider.Roles)
                claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, item.Name, "http://www.w3.org/2001/XMLSchema#string"));
            // authentic the claimsidentity created
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7),
                IsPersistent = isPersistent
            }, claimsIdentity);
            //var claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier, ClaimTypes.Role);
            //claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userName, "http://www.w3.org/2001/XMLSchema#string"));
            //claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "role", "http://www.w3.org/2001/XMLSchema#string"));
            //claimsIdentity.AddClaim(new System.Security.Claims.Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Custom Identity", "http://www.w3.org/2001/XMLSchema#string"));
        }
    }
}
