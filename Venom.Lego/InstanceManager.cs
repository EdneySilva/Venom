using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lego
{
    public class InstanceManager
    {
        static DependencyCollection Instances = new DependencyCollection();
        
        internal static void LoadTypes(IEnumerable<IGrouping<string, Type>> types)
        {
            foreach (var item in types)
                Instances.Add(item.Key, item.Select(GetDependencyAttribute).ToArray());
        }

        internal static DependencyAttribute GetDependencyAttribute(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(DependencyAttribute), false).FirstOrDefault() as DependencyAttribute;
            if (attribute == null)
                throw new Exception(string.Format("The type {0} doesn't contains the attribute annotation Venom.Lego.DependencyAttribute", type.FullName));
            return attribute.RegisterInstanceType(type);
        }

        public static T New<T>(params object[] parameters)
        {
            return Instances.GetInstanceFrom<T>(parameters);
        }

        internal static void RegisterType(DependencyAttribute dependency, Type item)
        {
            dependency.RegisterInstanceType(item);
            Instances.Add(dependency.TargetTypeName, dependency);
        }
    }
}
