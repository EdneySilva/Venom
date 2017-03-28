using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Venom.Web.Security;
using System.Threading.Tasks;
using Venom.Lib;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;

namespace Venom.Web.Security
{
    /// <summary>
    /// Represents the provide used to authenticate an user
    /// </summary>
    internal class WebAuthenticationProvider : Lib.Security.IAuthenticationProvider
    {
        string[] defaultValue = new string[0];
        /// <summary>
        /// Return any erros occured on this provider
        /// </summary>
        public IEnumerable<string> Errors { get { return defaultValue; } }
        /// <summary>
        /// Retrives the roles of the current user
        /// </summary>
        public IEnumerable<Venom.Lib.IRole> Roles { get; private set; }

        /// <summary>
        /// SignIn the user
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">user password</param>
        /// <returns>a task to run and create a operation result</returns>
        public Task<Lib.Result> SignInAsync(string userName, string password)
        {
            return SignInAsync(userName, password, false);
        }

        /// <summary>
        /// SignIn the user
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">user password</param>
        /// <param name="isPersistent">define if the current login will be persistent on the user browser</param>
        /// <returns>a task to run and create a operation result</returns>
        public async Task<Lib.Result> SignInAsync(string userName, string password, bool isPersistent)
        {
            var result = await System.Web.HttpContext.Current.GetOwinContext()
                .Get<Web.Security.ApplicationSignInManager>()
                .PasswordSignInAsync(userName, password, isPersistent, false);
            if (result != SignInStatus.Success)
                defaultValue = new[] { Lib.Util.ApplicationResource.FailedLogOn };
            return new Result(defaultValue);
        }

        /// <summary>
        /// Make a logoff of the current user on the sistem
        /// </summary>
        public void SignOut()
        {
            //HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.Current.GetOwinContext().Authentication.SignOut();
        }
    }
}