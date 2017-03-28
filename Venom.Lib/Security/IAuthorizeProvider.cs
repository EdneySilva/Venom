using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Venom.Lib.Security
{
    public interface IAuthorizeProvider
    {
        IEnumerable<IUserClaim> UserClaim { get; }
        bool Authorized(ISystemObject systemObject);
    }
}
