using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.IO
{
    public class Template
    {
        public string Path { get; protected set; }
        
        public Template(string path)
        {
            this.Path = path;
            if (!File.Exists(path))
                throw new FileNotFoundException();
        }

        public string Fill(params object[] values)
        {
            var lines = File.ReadAllText(this.Path, Encoding.Default);
            return string.Format(lines, values);
        }
    }
}
