using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Define the logical operators choose.
    /// </summary>
    enum Operator
    {
        Or,
        And
    }

    /// <summary>
    /// Define an Identifiel for each Filter.
    /// It's only nicks for the user filters, used to identifier multiple filter per properties,
    /// allowing caller choose which filter he will use on the Source Methods
    /// </summary>
    public enum FilterIdentifier
    {
        /// <summary>
        /// Define as a default filter to be used.
        /// </summary>
        Default,
        /// <summary>
        /// Define as a filter that should configure a call for a Contains Method.
        /// </summary>
        Contains,
        /// <summary>
        /// Define as a filter that should configure a call for a Contains Method
        /// that returns false
        /// </summary>
        NotContains,
        /// <summary>
        /// Define as a filter that should configure a call for a Equal Method.
        /// </summary>
        Equal,
        /// <summary>
        /// Define as a filter that should configure the operator Greater Than.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Define as a filter that should configure the operator Greater Than or
        /// Equal.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// Define as a filter that should configure the operator Less Than.
        /// </summary>
        LessThan,
        /// <summary>
        /// Define as a filter that should configure the operator Less Than or
        /// Equal.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Define as a filter that should configure a call for a Equal Method
        /// that returns false.
        /// </summary>
        NotEqual,
        /// <summary>
        /// Define as a filter that should configure a call for a StartsWith
        /// Method.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Define as a filter that should configure a call for a StartsWith
        /// Method that return false.
        /// </summary>
        NotStartsWith,
        /// <summary>
        /// Define as a filter that should configure a call for a EndsWith Method.
        /// </summary>
        EndsWith,
        /// <summary>
        /// Define as a filter that should configure a call for a EndsWith Method
        /// that return false.
        /// </summary>
        NotEndsWith,
        /// <summary>
        /// Define as a filter that should configure the operator Greater Than
        /// that return false.
        /// </summary>
        NotGreaterThan,
        /// <summary>
        /// Define as a filter that should configure the operator Greater Than
        /// or Equal that return false.
        /// </summary>
        NotGreaterThanOrEqual,
        /// <summary>
        /// Define as a filter that should configure the operator Less Than
        /// that return false.
        /// </summary>
        NotLessThan,
        /// <summary>
        /// Define as a filter that should configure the operator Less Than
        /// or Equal that return false.
        /// </summary>
        NotLessThanOrEqual,
    }

    /// <summary>
    /// Objects Maked with this Notation will ignored on the filter builders
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class IgnoredFilterAttribute : Attribute
    {
    }

    /// <summary>
    /// Represents a filter configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class FilterAttribute : Attribute
    {
        /// <summary>
        /// Get or set the default value of the filter. If the property equal DefaultValue
        /// it will ignored on the filter.
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// Get or set Type of the expression to be used to Make the expression.
        /// </summary>
        public ExpressionType Expression { get; set; }
        /// <summary>
        /// Get or set the value expected for the filter, use it for custom filter add on Runtime.
        /// </summary>
        internal object ExpectedValue { get; set; }
        /// <summary>
        /// Get or set the filter identifier, must be unique
        /// </summary>
        public FilterIdentifier FilterIdentifier { get; set; }
        /// <summary>
        /// Get or set the method name, that must be used on the expression builder.
        /// </summary>
        /// <example>
        /// "Equal" or "Contains" or "StartsWith" etc.
        /// </example>
        public string Method { get; set; }
        /// <summary>
        /// Get or set if the sentence must return a false value.
        /// </summary>
        public bool NotSentence { get; set; }
        /// <summary>
        /// Get or set the operator must be used to Concat expressions, this is used a lot for expressions defined in runtime.
        /// </summary>
        internal Operator Operator { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filterIdentifier">the filter identifier to be used</param>
        public FilterAttribute(FilterIdentifier filterIdentifier = FilterIdentifier.Default)
        {
            FilterIdentifier = filterIdentifier;
        }

    }

    /// <summary>
    /// Manager the default filter values by type.
    /// This class create default filter for some primitive types, that will be used for a property that isn't Marked with
    /// the FilterAttribute.
    /// </summary>
    class FilterManager
    {
        /// <summary>
        /// Constructor.
        /// Create default filter to follow types:
        /// string, short, int, long, float, decimal, double, bool, short?, int?, long?, float?, decimal?, double?, bool? and object.
        /// </summary>
        static FilterManager()
        {
            DefaultTypesFilter = new Dictionary<string, FilterAttribute>();
            DefineFilterTo<string>(ExpressionType.Call, "Contains");
            DefineFilterTo<short>(ExpressionType.Equal);
            DefineFilterTo<int>(ExpressionType.Equal);
            DefineFilterTo<long>(ExpressionType.Equal);
            DefineFilterTo<float>(ExpressionType.Equal);
            DefineFilterTo<decimal>(ExpressionType.Equal);
            DefineFilterTo<double>(ExpressionType.Equal);
            DefineFilterTo<bool>(ExpressionType.Equal);
            DefineFilterTo<bool?>(ExpressionType.Equal);
            DefineFilterTo<short?>(ExpressionType.Equal);
            DefineFilterTo<int?>(ExpressionType.Equal);
            DefineFilterTo<long?>(ExpressionType.Equal);
            DefineFilterTo<float?>(ExpressionType.Equal);
            DefineFilterTo<decimal?>(ExpressionType.Equal);
            DefineFilterTo<double?>(ExpressionType.Equal);
            DefineFilterTo<object>(ExpressionType.Equal);
            Default = new FilterManager();
        }

        /// <summary>
        /// Returns an instance of the FilterManager.
        /// </summary>
        public static FilterManager Default { get; private set; }

        /// <summary>
        /// Storage all default filter configuration.
        /// </summary>
        private static Dictionary<string, FilterAttribute> DefaultTypesFilter { get; set; }

        /// <summary>
        /// Return an instance of the FilterAttribute.
        /// </summary>
        /// <param name="key">name of the key used to storage the filter.</param>
        /// <example>typeof(int).FullName.</example>
        /// <returns>an instance of FilterAttribute.</returns>
        public FilterAttribute this[string key]
        {
            get { return DefaultTypesFilter.ContainsKey(key) ? DefaultTypesFilter[key] : this[typeof(object)]; }
        }

        /// <summary>
        /// Return an instance of the FilterAttribute.
        /// </summary>
        /// <param name="type">type used.</param>
        /// <example>typeof(int)</example>
        /// <returns>an instance of FilterAttribute.</returns>
        public FilterAttribute this[Type type]
        {
            get { return DefaultTypesFilter[type.FullName]; }
        }

        /// <summary>
        /// Storage a default filter.
        /// </summary>
        /// <typeparam name="T">type in use</typeparam>
        /// <param name="expression">expression must be used</param>
        /// <param name="method">method name must be used </param>
        /// <param name="notSentence">define if the expression must return true or false</param>
        private static void DefineFilterTo<T>(ExpressionType expression, string method = null, bool notSentence = false)
        {
            var type = typeof(T);
            DefaultTypesFilter.Add(type.FullName, new FilterAttribute(FilterIdentifier.Default)
            {
                DefaultValue = default(T),
                Expression = expression,
                Method = method,
                NotSentence = notSentence
            });
        }
    }

}
