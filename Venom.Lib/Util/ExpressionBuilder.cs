using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Util
{
    /// <summary>
    /// Builder expressions based on the object properties.
    /// </summary>
    class ExpressionBuilder
    {
        /// <summary>
        /// Get if the expressions are empty.
        /// </summary>
        public bool IsEmpty { get { return !Expressions.Any(); } }
        /// <summary>
        /// Get or Set the collection to Stored the expressions before compile there.
        /// </summary>
        List<Expression> Expressions { get; set; }
        /// <summary>
        /// Get o Set the parameter used on the exprssion.
        /// </summary>
        ParameterExpression Parameter { get; set; }
        /// <summary>
        /// Get or Set the value of the target object.
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// Get or Set the Type fo the target object.
        /// </summary>
        Type TargetType { get; set; }
        /// <summary>
        /// Get or set if the current instance is a Nested instance of another ExpressionBuilder.
        /// </summary>
        bool IsNested { get; set; }
        /// <summary>
        /// Get or set the last property used, this property is parent and contains the nested current property.
        /// </summary>
        public PropertyInfo LastProperty { get; set; }
        /// <summary>
        /// Get or set the las builder used, this builder is parend of the current builder instance.
        /// </summary>
        public ExpressionBuilder LastBuilder { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">type of tagert object.</param>
        /// <param name="value">value of target object.</param>
        public ExpressionBuilder(Type type, object value)
        {
            Expressions = new List<Expression>();
            // create the paramenter will used on the expressino (parameter) => ....
            Parameter = Expression.Parameter(type, "a");
            Value = value;
            TargetType = type;
        }

        /// <summary>
        /// Constructor.
        /// Use this constructor when you are building a expression used as nested expression for a parent property.
        /// </summary>
        /// <param name="type">type of tagert object.</param>
        /// <param name="value">value of target object.</param>
        /// <param name="lastExpression">the expression that contains the parent property.</param>
        internal ExpressionBuilder(Type type, object value, ExpressionBuilder lastExpression)
        {
            Expressions = new List<Expression>();
            LastBuilder = lastExpression;
            Parameter = lastExpression.Parameter;
            Value = value;
            TargetType = type;
            IsNested = true;
        }

        /// <summary>
        /// Create an expression based on the property target.
        /// </summary>
        /// <param name="property">target property info.</param>
        /// <param name="filter">filter used to the target property.</param>
        /// <returns>the current instance.</returns>
        public ExpressionBuilder Add(PropertyInfo property, FilterAttribute filter)
        {
            // crete the expression result
            var result = Result(property, filter);
            // if the result is valid add on the container
            if (result != null)
                Expressions.Add(result);
            return this;
        }

        /// <summary>
        /// Join two expressions with the AND operator based on the property target.
        /// </summary>
        /// <param name="property">target property info.</param>
        /// <param name="filter">filter used to the target property.</param>
        /// <returns>the current instance.</returns>
        public ExpressionBuilder And(PropertyInfo property, FilterAttribute filter)
        {
            // create an AND expression Join the last expression the expression result to anhoter property expression: ((Expression.Last()) AND Result(property, filter))
            var andExpression = Expression.AndAlso(Expressions.Last(), Result(property, filter));
            // replace the last expression value
            Expressions[Expressions.IndexOf(Expressions.Last())] = andExpression;
            return this;
        }

        /// <summary>
        /// Join two expressions with the AND operator based on the property target.
        /// </summary>
        /// <param name="property">target property info.</param>
        /// <param name="filter">filter used to the target property.</param>
        /// <returns>the current instance.</returns>
        public ExpressionBuilder Or(PropertyInfo property, FilterAttribute filter)
        {
            // create an OR expression Join the last expression the expression result to anhoter property expression: ((Expression.Last()) OR Result(property, filter))
            var orExpression = Expression.OrElse(Expressions.Last(), Result(property, filter));
            // replace the last expression value
            Expressions[Expressions.IndexOf(Expressions.Last())] = orExpression;
            return this;
        }

        /// <summary>
        /// Convert all expressions created on a Expression&lt;Func&lt;T, TResult&gt;&gt;.
        /// </summary>
        /// <typeparam name="T">target type.</typeparam>
        /// <typeparam name="TResult">type of the result expected.</typeparam>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToLinqExpression<T, TResult>()
        {
            // no expression created, return default expression (parameter) => return true
            if (Expressions.Count < 1)
                Expressions.Add(Expression.Equal(Expression.Constant(true), Expression.Constant(true)));
            // just one expression created, than return just it
            if (Expressions.Count < 2)
                return Expression.Lambda<Func<T, bool>>(Expressions.First(), Parameter);
            // more than one join all the expressions with the AND operator
            Expression baseExp = Expressions.First();
            for (int i = 1; i < Expressions.Count; i++)
            {
                baseExp = Expression.AndAlso(baseExp, Expressions[i]);
            }
            // return the expression result
            return Expression.Lambda<Func<T, bool>>(baseExp, Parameter);
        }

        /// <summary>
        /// Build a parent expression, use it when create an expression to NestedProperties.
        /// </summary>
        /// <returns>An Expression.</returns>
        public Expression ToParentExpression()
        {
            // no expression created, return default expression (parameter) => return true
            if (Expressions.Count < 1)
                Expressions.Add(Expression.Equal(Expression.Constant(true), Expression.Constant(true)));
            // just one expression created, than return just it
            if (Expressions.Count < 2)
                return Expressions.First();
            // more than one join all the expressions with the AND operator
            Expression baseExp = Expressions.First();
            for (int i = 1; i < Expressions.Count; i++)
            {
                baseExp = Expression.AndAlso(baseExp, Expressions[i]);
            }
            // return the expression result
            return baseExp;
        }

        /// <summary>
        /// Build the expression.
        /// </summary>
        /// <param name="property">the current target property.</param>
        /// <param name="filter">the filter related to target proeprty.</param>
        /// <returns>An Expression.</returns>
        private Expression Result(PropertyInfo property, FilterAttribute filter)
        {
            // if expected value is null and the value property isn't valid return null
            if (filter.ExpectedValue == null && !PropertyIsValid(filter, property))
                return null;
            // if the current propery is a complexy object(class) and isn't a string, return an Expression based onn the ComplexyType
            if ((property.PropertyType.IsClass || property.PropertyType.IsInterface) && property.PropertyType != typeof(string))
                return this.CreateExpressionFromComplexyType(property);

            // build the left part on the expression (parameter.property ==) or (parameter.property.[space to a method calling])
            var left = LeftPart(property);
            // build the right part on the expression (.MyMethodCall) or ( == constant value)
            var right = RightPart(property, filter, left);
            // if is a call method than use just the left part else make a binary expression
            var result = filter.Method != null ? right : Expression.MakeBinary(filter.Expression, left, right);
            // create a sentece that result must be false
            if (filter.NotSentence)
                result = Expression.Not(result);
            return result;
        }

        /// <summary>
        /// Create a left part expression, that represent the logical expression using the left party to do it
        /// </summary>
        /// <param name="property">the current property</param>
        /// <param name="filter">the filter to be applied</param>
        /// <param name="left">the left expression part</param>
        /// <returns>An expreession that represents a logical expression join the left and right part</returns>
        private Expression RightPart(PropertyInfo property, FilterAttribute filter, Expression left) 
        {
            // check if the filter is a call method,  in this case create a call method expression, in another case just create a constant with the property value
            return filter.Method == null ?
                Expression.Constant(filter.ExpectedValue ?? property.GetValue(Value, null), property.PropertyType) :
                CreateCallMethodExpression(property.GetValue(Value, null), filter, left);
        }

        /// <summary>
        /// Create a left part expression, here, are create a call to the property
        /// </summary>
        /// <param name="property">the current property</param>
        /// <returns>An Expressino to the current property</returns>
        private Expression LeftPart(PropertyInfo property)
        {
            // if the current instance has a parent, than it build the expression based on the parent left part, in anhoter cases create a call to the property
            return this.IsNested ? Expression.Property(this.LastBuilder.LeftPart(this.LastBuilder.LastProperty), property) :
                Expression.Property(Parameter, TargetType.GetProperty(property.Name));
        }

        /// <summary>
        /// Create an expression that represents a method call.
        /// </summary>
        /// <param name="objeto">the current object value.</param>
        /// <param name="filter">the filter to be applied.</param>
        /// <param name="left">the left expression used on this call.</param>
        /// <returns>Expression to a method calling.</returns>
        private Expression CreateCallMethodExpression(object objeto, FilterAttribute filter, Expression left)
        {
            // get the method indicated on the filter
            var method = objeto.GetType().GetMethods().Where(f => f.Name == filter.Method && f.GetParameters().Count() == 1).First();
            // get the parameter used on the method
            var param = method.GetParameters()[0].ParameterType;
            // create the constante for the parameter value
            var right = Expression.Constant(Convert.ChangeType(objeto, param));
            // create the calling expression to method
            return Expression.Call(left, method, right);
        }

        /// <summary>
        /// Check if the property value is valid.
        /// </summary>
        /// <param name="filter">filter applied on the proeprty.</param>
        /// <param name="property">the current property.</param>
        /// <returns>true | false.</returns>
        private bool PropertyIsValid(FilterAttribute filter, PropertyInfo property)
        {
            if (filter.DefaultValue == null && property.GetValue(Value, null) == filter.DefaultValue)
                return false;
            else if (property.PropertyType.IsEnum && property.GetValue(Value, null).Equals(filter.DefaultValue))
                return false;
            // just used with primitive properties
            else if (property.PropertyType.IsPrimitive && Convert.ChangeType(property.GetValue(Value, null), property.PropertyType).Equals(Convert.ChangeType(filter.DefaultValue, property.PropertyType)))
                return false;
            return true;
        }

        /// <summary>
        /// Create an expression from a nested property.
        /// </summary>
        /// <param name="property">the current nested property.</param>
        /// <returns>Expression created based on a nested property.</returns>
        private Expression CreateExpressionFromComplexyType(PropertyInfo property)
        {
            // get the method toexpression to create an expression based on the nested current property
            var method = property.PropertyType.GetMethod("ToExpression", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null)
                return null;
            this.LastProperty = property;
            // invoke the method if the property contains that
            return method.Invoke(property.GetValue(Value), new object[] { this }) as Expression;
        }
    }
}
