using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Security
{
    public class AuthenticationManager : IDisposable
    {
        public AuthenticationManager()
        {
            AuthenticationProvider = ObjectContainer.New<IAuthenticationProvider>();
        }
        
        protected IAuthenticationProvider AuthenticationProvider { get; private set; }

        public virtual Task<Result> SigInAsync(string userName, string password, bool isPersistent)
        {
            return ObjectContainer.New<IAuthenticationProvider>().SignInAsync(userName, password, isPersistent);
        }

        public virtual void SignOut()
        {
            ObjectContainer.New<IAuthenticationProvider>().SignOut();
        }

        public static AuthenticationManager Create()
        {
            return ObjectContainer.New<AuthenticationManager>();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
