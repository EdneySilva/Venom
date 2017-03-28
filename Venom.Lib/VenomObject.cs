using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Venom.Lib.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Venom.Lib.Util;
using System.Diagnostics;
using System.Collections;
using PagedList;

namespace Venom.Lib
{
    /// <summary>
    /// A Global base class that must be inherited by itens that wish a complete behavior about handle EntityFramework object
    /// without need implement every thing all the time
    /// </summary>
    /// <typeparam name="T">the target object type inheriting the venomobject</typeparam>
    public class VenomObject<T> : Venom.Lib.IVenomObject<T> where T : class
    {

        /// <summary>
        /// A implicit conversion from a VenomObject Templated Source to an Expression Func Templated thas return true, to use in a query
        /// </summary>
        /// <param name="source">source object</param>
        /// <returns>a expression for a linq query</returns>
        public static implicit operator Expression<Func<T, bool>>(VenomObject<T> source)
        {
            return new Where<T>(source as T, source.exceptionRule).Compile();            
        }

        /// <summary>
        /// A implicit conversion of a venomobject to a custom type
        /// </summary>
        /// <param name="source"></param>
        /// <returns>An instance of T</returns>
        public static implicit operator T(VenomObject<T> source)
        {
            return source as T;
        }
        /// <summary>
        /// a collection of rules that must replace the default rules of each property
        /// </summary>
        private IEnumerable<Rule> exceptionRule = null;
        /// <summary>
        /// Get o set the handler repository
        /// </summary>
        AppRepositoryManager<AppDbContext> RepositoryManager { get; set; }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public VenomObject()
        {
            RepositoryManager = AppRepositoryManager<AppDbContext>.Default;
        }

        /// <summary>
        /// Insert or update a item on database
        /// </summary>
        /// <returns>true | false</returns>
        public virtual bool Save()
        {
            RepositoryManager.Context.Set<T>().AddOrUpdate(this);
            return this.SaveContext();
        }

        /// <summary>
        /// Delete a object of the database
        /// </summary>
        /// <returns>true | false</returns>
        public virtual bool Delete()
        {
            //if (RepositoryManager.Context.Entry(this).State == EntityState.Detached)
            //    RepositoryManager.Context.Set<T>().Attach(this);
            RepositoryManager.Context.Set<T>().Remove(this);
            return this.SaveContext();
        }

        /// <summary>
        /// Save the data context
        /// </summary>
        /// <returns>true | false</returns>
        private bool SaveContext()
        {
            try
            {
                RepositoryManager.Context.SaveChanges();
                return  true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get a instance object base on the Primary keys of object
        /// </summary>
        /// <returns>An instance of a T object based on the PrimaryKey filter</returns>
        public T GetById()
        {
            return this.CreateQuery().FirstOrDefault(new PkWhere<T>(this).Compile());
        }

        /// <summary>
        /// Create a query to handle directly the repository item
        /// </summary>
        /// <returns>An IQueryable item of T type, used to build complexy querys by the consumer</returns>
        public IQueryable<T> CreateQuery()
        {
            return RepositoryManager.Context.Set<T>().AsQueryable();
        }

        /// <summary>
        /// Looking for itens that's look is like with the current reference(for while just primitive properties are working)
        /// </summary>
        /// <returns>An IEnumerable of T</returns>
        public IEnumerable<T> FindItensAsMe()
        {
            //var ctx = RepositoryManager.Context;            
            return RepositoryManager.Context.Set<T>().Where(this).ToArray();
        }

        /// <summary>
        /// Get itens looks like the current instance
        /// </summary>
        /// <returns>return a IEnumerable itens</returns>
        IEnumerable IVenomObject.FindItensAsMe()
        {
            return this.FindItensAsMe();
        }

        /// <summary>
        /// Looking for itens that's look is like with the current reference(for while just primitive properties are working), with a pre-defined rules
        /// </summary>
        /// <param name="filter">rules that must be applyed in each property on the rule list</param>
        /// <returns>An IEnumerable of T</returns>
        public IEnumerable<T> FindItensAsMe(FilterRules<T> filter)
        {
            this.exceptionRule = filter.Rules;
            // in the where method we can pass this object, because venom class contains a implicit operator to convert him self in an expression to be used from EntityFramework
            return RepositoryManager.Context.Set<T>().Where(this).ToArray();
        }

        /// <summary>
        /// Create paged list from a query search, using the current properties with filter
        /// </summary>
        /// <param name="pageNumber">the current page number, default is 0</param>
        /// <returns>PagedList&lt;T&gt;</returns>
        public IPagedList<T> ToPagedList(int pageNumber = 1, int pageSize = 0)
        {
            var query = this.CreateQuery().Where(this).OrderBy(o => 1);
            return pageSize > 0 ? query.ToPagedList(pageNumber, pageSize) : query.ToPagedList(pageNumber);
            //return this.CreateQuery().Where(this).OrderBy(o => 1).ToPagedList(pageNumber);
        }

        /// <summary>
        /// List instens stored on the repository
        /// </summary>
        /// <param name="query">custom filter to apply on the query</param>
        /// <returns></returns>
        public IEnumerable<T> ToList(Expression<Func<T, bool>> query)
        {
            return RepositoryManager.Context.Set<T>().Where(query).ToArray();
        }

        /// <summary>
        /// Create an expression based on a nested property
        /// </summary>
        /// <param name="expression">the last expression builder was executed</param>
        /// <returns>a expression based on nested property</returns>
        internal Expression ToExpression(ExpressionBuilder expression)
        {
            var predicate = new Where<T>(this, expression, this.exceptionRule).NestedPropertyCompile();
            return predicate;
        }

    }
}
