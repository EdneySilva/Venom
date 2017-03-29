using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lego
{
    public class Object<T>
    {        
        public static T New(params object[] parameters)
        {
            return InstanceManager.New<T>();            
        }
    }
}
