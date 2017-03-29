using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib;

namespace Venom.Lego
{
    public static class StartupExtension
    {
        public static Startup MapLego(this Startup startuper)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var itens = allAssemblies.SelectMany(
                            s => s.GetTypes()
                                  .Where(w => w.GetCustomAttributes(typeof(DependencyAttribute), false).Any())
                            );//.GroupBy((e) => 
                                //(e.GetCustomAttributes(typeof(DependencyAttribute), false).FirstOrDefault() as DependencyAttribute).TargetTypeName);
            var _itens = itens.SelectMany(a => a.CustomAttributes);
            foreach (var item in itens)
            {
                foreach (var attr in item.GetCustomAttributes(typeof(DependencyAttribute), false).Cast<DependencyAttribute>())
                {
                    InstanceManager.RegisterType(attr, item);
                }
            }
            //InstanceManager.LoadTypes(itens);
            return startuper;
        }
    }
}