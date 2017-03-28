using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Data;
using Venom.Lib.Extension;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Represents a Where Expression Builder.
    /// </summary>
    /// <typeparam name="T">type of the object about query is based.</typeparam>
    internal class Where<T>
    {
        #region Properties
        /// <summary>
        /// Get or set the expression builder
        /// </summary>
        private ExpressionBuilder Builder { get; set; }
        /// <summary>
        /// Return the cachekey used to retrieve data on cache
        /// </summary>
        private string CacheKey { get { return typeof(T).Name + "." + this.GetType().Name; } }
        /// <summary>
        /// get or set all properties of the target object
        /// </summary>
        protected PropertyInfo[] Properties { get; set; } 

        #endregion

        #region Fields
        
        /// <summary>
        /// Storage the WhereContainer used to make a cache.
        /// </summary>
        protected List<WhereContainer> container;

        /// <summary>
        /// Storage the exceptions that doens't make a default part of the property.
        /// </summary>
        private IEnumerable<Rule> Exceptions; 

        #endregion

        #region Constructors
        
        /// <summary>
        /// constructor.
        /// Use this constructor, when you have an ExpressionBuilder parent, and you are build a NestedPropertie expression.
        /// </summary>
        /// <param name="source">the object that contains the properties used to build an expression.</param>
        /// <param name="lastExpression">a parent expression.</param>
        /// <param name="exceptions">the rules that are exceptions, add in runtime.</param>
        public Where(T source, ExpressionBuilder lastExpression, IEnumerable<Rule> exceptions = null)
            : this(source, exceptions)
        {
            Builder = new ExpressionBuilder(typeof(T), source, lastExpression);
        }

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="source">the object that contains the properties used to build an expression.</param>
        /// <param name="exceptions">the rules that are exceptions, add in runtime.</param>
        public Where(T source, IEnumerable<Rule> exceptions = null)
        {
            Builder = new ExpressionBuilder(typeof(T), source);
            Exceptions = exceptions ?? new List<Rule>();
            container = CacheManager.Get<List<WhereContainer>>(CacheKey);
            RestoreCache();
            if (container == null)
                container = new List<WhereContainer>();
            Initialize();
        } 

        #endregion

        #region Public Methods

        /// <summary>
        /// Build and Compile an expression based on all object properties.
        /// </summary>
        /// <returns>A Lambda Expression instance.</returns>
        public Expression<Func<T, bool>> Compile()
        {
            CreateExpressions();
            return Builder.ToLinqExpression<T, bool>();
        }

        /// <summary>
        /// Create the expression for NestedProperties.
        /// </summary>
        /// <returns>an instace of Expression.</returns>
        public Expression NestedPropertyCompile()
        {
            CreateExpressions();
            return Builder.ToParentExpression();
        }

        #endregion

        #region Private / Protected Methods

        /// <summary>
        /// Restore the properties and filters based on cache.
        /// </summary>
        private void RestoreCache()
        {
            Properties = container == null ?
                typeof(T).GetProperties().Where(w => !w.GetCustomAttributes(typeof(IgnoredFilterAttribute), false).Any()).ToArray() :
                container.Select(s => s.Property).ToArray();
        }

        /// <summary>
        /// Create the expression based on all filters
        /// </summary>
        private void CreateExpressions()
        {
            for (int i = 0; i < container.Count; i++)
            {
                var first = container[i].Filter.First();
                // add the first filter on the builder
                Builder.Add(container[i].Property, container[i].Filter.First());
                // if the property contais more than one filter, add in runtime, builder append them using the logical operators, && or ||
                for (int f = 1; f < container[i].Filter.Length; f++)
                {
                    if(container[i].Filter[f].Operator == Operator.Or)
                        Builder.Or(container[i].Property, container[i].Filter[f]);
                    else
                        Builder.And(container[i].Property, container[i].Filter[f]);
                }
            }
        }

        /// <summary>
        /// Initialize the where container.
        /// </summary>
        protected virtual void Initialize()
        {
            if (container.Count > 0)
                return;
            // add properties and filters on the cache
            for (int i = 0; i < Properties.Length; i++)
                container.Add(
                    new WhereContainer
                    {
                        Filter = PrepareFilters(Properties[i]),
                        Property = Properties[i],
                    }
                );
            CacheManager.AddItem(CacheKey, container);
        }

        /// <summary>
        /// Prepere the filters to buil the where expression.
        /// </summary>
        /// <param name="property">the current property.</param>
        /// <returns>Array of FilterAttributes configured in the property.</returns>
        private FilterAttribute[] PrepareFilters(PropertyInfo property)
        {
            // get the rules on the exceptions rule list, if empty use a default filter
            var rule = Exceptions.DefaultIfEmpty(new Rule { RuleIdetifier = FilterIdentifier.Default, Target = property.Name }).Where(p => p.Target.Equals(property.Name)).ToArray();
            // get the filters in the property, if more than one was set as Default, an exception is threw
            var filterArray = property.GetCustomAttributes(typeof(FilterAttribute), false).Cast<FilterAttribute>().ToArray();
            if (filterArray.Length > 1 && rule.All(a => a.RuleIdetifier == FilterIdentifier.Default) && filterArray.All(a => a.FilterIdentifier != FilterIdentifier.Default))
                throw new Exception("Define a default filter");
            // create the rules
            var filter = new FilterAttribute[rule.Length];
            bool hasFilter = false;
            for (int i = 0; i < rule.Length; i++)
            {
                var element = CreateFilterByRule(rule[i], filterArray);
                if (element == null)
                    continue;
                filter[i] = element;
                hasFilter = true;
            }
            // if no rules was created, return the default filter by propertytype
            return hasFilter ? filter.Where(w => w != null).ToArray() : new FilterAttribute[] { property.PropertyType.GetDefaultFilterAttribute() };
        }

        /// <summary>
        /// Create a filter based on the custom rule
        /// </summary>
        /// <param name="rule">rule reference to be used</param>
        /// <param name="filterArray">filter in usage</param>
        /// <returns>the filter that match with the rule</returns>
        private FilterAttribute CreateFilterByRule(Rule rule, FilterAttribute[] filterArray)
        {
            // get the filter based on the identifier
            FilterAttribute baseObject = filterArray.FirstOrDefault(f => rule.RuleIdetifier == f.FilterIdentifier);
            if (baseObject != null)
            {
                baseObject.ExpectedValue = rule.ExpectedValue;
                baseObject.Operator = rule.Operator;
            }
            return baseObject;
        }

        #endregion

    }
}
