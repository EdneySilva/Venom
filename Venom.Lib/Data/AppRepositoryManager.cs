using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Venom.Lib.Data
{
    public class AppRepositoryManager<T> where T : class, new ()
    {
        /// <summary>
        /// Retorna a instância default para o gerenciador de repositório de contexto
        /// </summary>
        public static AppRepositoryManager<T> Default
        {
            get { return new AppRepositoryManager<T>(); }
        }

        /// <summary>
        /// Instância local para o contexto requerido
        /// </summary>
        private T context;

        /// <summary>
        /// Retorna o contexto da thread
        /// </summary>
        public T Context
        {
            get
            {
                return context = context ??
                    CacheManager.Get<T>(Thread.CurrentThread.ManagedThreadId.ToString()) ??
                    CacheManager.AddItem(Thread.CurrentThread.ManagedThreadId.ToString(), ObjectContainer.IsRegistered<T>() ? ObjectContainer.New<T>() : new T());
            }
        }
        
        /// <summary>
        /// Construtor default
        /// </summary>
        private AppRepositoryManager()
        {

        }
    }
}
