using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Represent a Rule can be configured.
    /// </summary>
    public sealed class Rule
    {
        /// <summary>
        /// Get or set the rule identifier.
        /// </summary>
        public FilterIdentifier RuleIdetifier { get; set; }
        /// <summary>
        /// Get or Set the Property name target.
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// Get or Set the expected value on the expression.
        /// </summary>
        public object ExpectedValue { get; set; }
        /// <summary>
        /// Get or Set the logical operator must be used.
        /// </summary>
        internal Operator Operator { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterRules<T>
    {
        /// <summary>
        /// Return the default seperator string.
        /// </summary>
        const char SEPARATOR = '.';
        /// <summary>
        /// Storage all rules added.
        /// </summary>
        private List<Rule> rules;
        /// <summary>
        /// Return if the filter are empty.
        /// </summary>
        private bool IsEmpty { get { return rules == null || rules.Count == 0; } }
        /// <summary>
        /// Return all rules Added.
        /// </summary>
        public IEnumerable<Rule> Rules { get { return rules; } }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">the target object used to build an expression</param>
        public FilterRules (T target)
	    {
            rules = new List<Rule>();
	    }

        /// <summary>
        /// Add a rule.
        /// </summary>
        /// <typeparam name="TKey">the type property target.</typeparam>
        /// <param name="property">the property target.</param>
        /// <param name="rule">the identifier will be used.</param>
        /// <returns>the current instance.</returns>
        public FilterRules<T> AddRule<TKey>(Expression<Func<T, TKey>> property, FilterIdentifier rule)
        {
            rules.Add(new Rule { Target = property.Body.ToString().Split(SEPARATOR).Last(), RuleIdetifier = rule, ExpectedValue = null, Operator = Operator.Or });
            return this;
        }

        /// <summary>
        /// Add a rule.
        /// </summary>
        /// <typeparam name="TKey">the type property target.</typeparam>
        /// <param name="property">the property target.</param>
        /// <param name="rule">the identifier will be used.</param>
        /// <param name="expectedValue">The value expected on the filter.</param>
        /// <returns>the current instance.</returns>
        public FilterRules<T> AddRule<TKey>(Expression<Func<T, TKey>> property, FilterIdentifier rule, TKey expectedValue)
        {
            rules.Add(new Rule { Target = property.Body.ToString().Split(SEPARATOR).Last(), RuleIdetifier = rule, ExpectedValue = expectedValue, Operator = Operator.Or });
            return this;
        }

        /// <summary>
        /// Add an Rule join with the last expression using the OR operator.
        /// </summary>
        /// <typeparam name="TKey">the type property target.</typeparam>
        /// <param name="property">the property target.</param>
        /// <param name="rule">the identifier will be used.</param>
        /// <returns>the current instance.</returns>
        public FilterRules<T> Or<TKey>(Expression<Func<T, TKey>> property, FilterIdentifier rule)
        {
            if (IsEmpty)
                throw new InvalidOperationException("Try add some rule first!");
            rules.Add(new Rule { Target = property.Body.ToString().Split(SEPARATOR).Last(), RuleIdetifier = rule, ExpectedValue = null, Operator = Operator.Or });
            return this;
        }

        /// <summary>
        /// Add an Rule join with the last expression using the OR operator.
        /// </summary>
        /// <typeparam name="TKey">the type property target.</typeparam>
        /// <param name="property">the property target.</param>
        /// <param name="rule">the identifier will be used.</param>
        /// <param name="expectedValue">The value expected on the filter.</param>
        /// <returns>the current instance.</returns>
        public FilterRules<T> Or<TKey>(Expression<Func<T, TKey>> property, FilterIdentifier rule, TKey expectedValue)
        {
            if (IsEmpty)
                throw new InvalidOperationException("Try add some rule first!");
            rules.Add(new Rule { Target = property.Body.ToString().Split('.').Last(), RuleIdetifier = rule, ExpectedValue = expectedValue, Operator = Operator.Or });
            return this;
        }

        /// <summary>
        /// Add an Rule join with the last expression using the AND operator.
        /// </summary>
        /// <typeparam name="TKey">the type property target.</typeparam>
        /// <param name="property">the property target.</param>
        /// <param name="rule">the identifier will be used.</param>
        /// <returns>the current instance.</returns>
        public FilterRules<T> And<TKey>(Expression<Func<T, TKey>> property, FilterIdentifier rule)
        {
            if (IsEmpty)
                throw new InvalidOperationException("Try add some rule first!");
            rules.Add(new Rule { Target = property.Body.ToString().Split(SEPARATOR).Last(), RuleIdetifier = rule, ExpectedValue = null, Operator = Operator.And });
            return this;
        }

        /// <summary>
        /// Add an Rule join with the last expression using the AND operator.
        /// </summary>
        /// <typeparam name="TKey">the type property target.</typeparam>
        /// <param name="property">the property target.</param>
        /// <param name="rule">the identifier will be used.</param>
        /// <param name="expectedValue">The value expected on the filter.</param>
        /// <returns>the current instance.</returns>
        public FilterRules<T> And<TKey>(Expression<Func<T, TKey>> property, FilterIdentifier rule, TKey expectedValue)
        {
            if (IsEmpty)
                throw new InvalidOperationException("Try add some rule first!");
            rules.Add(new Rule { Target = property.Body.ToString().Split(SEPARATOR).Last(), RuleIdetifier = rule, ExpectedValue = expectedValue, Operator = Operator.And });
            return this;
        }

    }
}
