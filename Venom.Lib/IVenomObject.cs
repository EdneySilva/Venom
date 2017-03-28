using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Venom.Lib.Util;
namespace Venom.Lib
{
    /// <summary>
    /// Represent a IVenomObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IVenomObject<T> : IVenomObject
     where T : class
    {
        /// <summary>
        /// Find an instance on the repository based on the KeyProperty
        /// </summary>
        /// <returns>a Instance of T</returns>
        T GetById();
        IEnumerable<T> FindItensAsMe();
        //IEnumerable<T> FindItensAsMe(FilterRules<T> filter);
        //IEnumerable<T> ToList(Expression<Func<T, bool>> query);
    }

    public interface IVenomObject
    {
        /// <summary>
        /// Delete the current instance in his repository
        /// </summary>
        /// <returns>return true with was deleted with success</returns>
        bool Delete();
        /// <summary>
        /// Save the current instance in his repository
        /// </summary>
        /// <returns>return true with was saved with success</returns>
        bool Save();
        /// <summary>
        /// Get itens looks like the current instance
        /// </summary>
        /// <returns>return a IEnumerable itens</returns>
        System.Collections.IEnumerable FindItensAsMe();
    }
}
