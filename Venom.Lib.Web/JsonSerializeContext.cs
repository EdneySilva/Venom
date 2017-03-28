using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Data;

namespace Venom.Web
{
    /// <summary>
    /// Responsable to manage the context
    /// </summary>
    public class JsonSerializeContext : IDisposable
    {
        AppRepositoryManager<AppDbContext> Repository;

        public JsonSerializeContext()
        {
            Repository.Context.Configuration.LazyLoadingEnabled = false;
            Repository.Context.Configuration.ProxyCreationEnabled = true;
        }

        public void Dispose()
        {
            Repository.Context.Configuration.LazyLoadingEnabled = true;
            Repository.Context.Configuration.ProxyCreationEnabled = true;
        }
    }
}
