using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lego
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
        protected readonly static object[] EmptyConstructor = new object[0];
        private Type TargetType { get; set; }
        private ConstructorInfo[] Constructors { get; set; }
        public string TargetTypeName { get; private set; }
        public Type InstanceType { get; private set; }
        

        public DependencyAttribute(Type target)
        {
            this.TargetType = target;
            this.TargetTypeName = target.FullName;
        }

        protected virtual bool InstanceAllowed(params object[] parameters)
        {
            return true;
        }

        protected virtual object[] GetInstanceParameters(params object[] parameters)
        {
            return parameters;
        }

        public T New<T>(params object[] parameters)
        {
            if (!this.InstanceAllowed(parameters))
                return default(T);
            var instanceParameters = this.GetInstanceParameters(parameters);
            if (instanceParameters.Length > this.Constructors.Length)
                throw new Exception(string.Format("No constructors was found to {0} parameters", parameters.Length));
            return (T)this.Constructors[instanceParameters.Length].Invoke(instanceParameters);
        }

        internal DependencyAttribute RegisterInstanceType(Type type)
        {
            if(this.InstanceType != null)
                throw new Exception("An instance type has already gone set");
            this.InstanceType = type;
            this.Constructors = type.GetConstructors().OrderBy(o => o.GetParameters().Count()).ToArray();
            return this;
        }
    }
}