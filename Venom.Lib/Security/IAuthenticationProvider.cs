using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Security
{
    /// <summary>
    /// Represents the provide used to authenticate an user
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Return any erros occured on this provider
        /// </summary>
        IEnumerable<IRole> Roles { get; }
        /// <summary>
        /// Retrives the roles of the current user
        /// </summary>
        IEnumerable<string> Errors { get; }
        /// <summary>
        /// SignIn the user
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">user password</param>
        /// <returns>a task to run and create a operation result</returns>
        Task<Result> SignInAsync(string userName, string password);
        /// <summary>
        /// SignIn the user
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">user password</param>
        /// <param name="isPersistent">define if the current login will be persistent on the user browser</param>
        /// <returns>a task to run and create a operation result</returns>
        Task<Result> SignInAsync(string userName, string password, bool isPersistent);
        /// <summary>
        /// Make a logoff of the current user on the sistem
        /// </summary>
        void SignOut();
    }
}
