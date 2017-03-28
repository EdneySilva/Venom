using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib
{
    public class ModelResult<T> : Result where T : new ()
    {
        public ModelResult(T model, IEnumerable<string> errors)
            : base(errors)
        {
            this.Model = model;
        }

        public ModelResult(T model, params string[] errors)
            : base(errors)
        {
            this.Model = model;
        }

        public T Model { get; private set; }

        public static ModelResult<T> SuccessResult(T model)
        {
            return new ModelResult<T>(model, new string[0]);
        }
    }
}
