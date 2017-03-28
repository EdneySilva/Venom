using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Handle queryes to page their results, paged then.
    /// </summary>
    /// <typeparam name="T">Type of the query itens</typeparam>
    public class PagedList<T> : PagedList.BasePagedList<T>
    {
        /// <summary>
        /// Create a paged list.
        /// </summary>
        /// <param name="query">the current query(datasource) to be paged</param>
        /// <param name="pageNumber">the current page number</param>
        /// <param name="pageSize">the Length of the page</param>
        public PagedList(IQueryable<T> query,
            int pageNumber, 
            int pageSize
            //Func<TR, T> selector
         )
            : base(pageNumber, pageSize, query == null ? 0 : query.Count())
        {
            if (query == null || TotalItemCount == 0)
                return;
            // add all itens on list
            Subset.AddRange(
                // when list count is one
                pageNumber == 1 ? 
                    query.Skip(0)
                    .Take(pageSize)
                    .ToList() : 
                    // else make next page
                    query.Skip((pageNumber - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
            );
        }
    }
}
