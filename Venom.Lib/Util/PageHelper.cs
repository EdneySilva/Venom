using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Helper to pagedlist.
    /// </summary>
    public static class PageHelper
    {
        /// <summary>
        /// Page a iqueryable collection.
        /// </summary>
        /// <typeparam name="T">type of the elements</typeparam>
        /// <param name="query">the current query</param>
        /// <param name="pageNumber">the current number.</param>
        /// <returns>a paged list</returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> query, int pageNumber = 1)
        {
            return new PagedList<T>(query, pageNumber, PageConfiguration.PageSize);
        }
    }
}
