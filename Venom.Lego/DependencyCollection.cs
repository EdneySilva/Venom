using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Venom.Lego
{
    class DependencyCollection
    {
        Dictionary<string, DependencyAttribute[]> types = new Dictionary<string, DependencyAttribute[]>();

        public DependencyAttribute[] this[string key]
        {
            get { return types.ContainsKey(key) ? types[key] : null; }
        }

        internal void Add(string key, params DependencyAttribute[] itens)
        {
            if (types.ContainsKey(key))
                types[key] = types[key].Union(itens).ToArray();
            else
                types.Add(key, itens);
        }

        public T GetInstanceFrom<T>(params object[] parameters)
        {
            var itens = this[typeof(T).FullName];
            if (itens == null)
                return default(T);
            for (int i = 0; i < itens.Length; i++)
            {
                var instance = itens[i].New<T>(parameters);
                if (instance != null)
                    return instance;
            }
            throw new Exception(string.Format("Could not possible to create an instance to {0}, no dependency match to create value", typeof(T).FullName));

        }
    }
}
