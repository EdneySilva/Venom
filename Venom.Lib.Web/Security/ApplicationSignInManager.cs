using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Venom.Lib;
using Venom.Lib.Security;
using Venom.Web.Helper;

namespace Venom.Web.Security
{    
    /// <summary>
    /// Configure the application sign-in manager which is used in this application.
    /// this class implement the default asp.net authentication created as a asp.net template,
    /// use that when you don't have a custom provider to authention
    /// </summary>
    public class ApplicationSignInManager : SignInManager<IdentityUser, string>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userManager">a instance used to manager the application users</param>
        /// <param name="authenticationManager">a instace used to manager the application authentication</param>
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
                
        /// <summary>
        /// Called to generate the ClaimsIdentity for the user, override to add additional
        /// claims before SignIn
        /// </summary>
        /// <param name="user">the identity user</param>
        /// <returns>task to create a claimsidentity</returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(IdentityUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        /// <summary>
        /// Creates a user identity and then signs the identity using the AuthenticationManager
        /// </summary>
        /// <param name="user">the current user identity</param>
        /// <param name="isPersistent">define if the authentication will be persistent</param>
        /// <param name="rememberBrowser">flag to define if the authentication will be persisted on the browser</param>
        /// <returns></returns>
        public override Task SignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser)
        {            
            return base.SignInAsync(user, isPersistent, rememberBrowser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            // força a autenticação para salvar no browser
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //ClaimsIdentity claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.NameIdentifier, ClaimTypes.Role);
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "MyCustomID", "http://www.w3.org/2001/XMLSchema#string"));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userName, "http://www.w3.org/2001/XMLSchema#string"));
            //claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Custom Identity", "http://www.w3.org/2001/XMLSchema#string"));
            //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimsIdentity);
            //'return SignInStatus.Success;
            return base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }

        /// <summary>
        /// Create a instance of the applicationSignInManager
        /// </summary>
        /// <param name="options">the options to create that</param>
        /// <param name="context">the owin context in use</param>
        /// <returns></returns>
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
