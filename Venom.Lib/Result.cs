using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib
{
    /// <summary>
    /// Define a result operation.
    /// </summary>
    public class Result
    {
        static Result _result;
        /// <summary>
        /// Returns a default success Result value.
        /// </summary>
        public static Result SuccessResult { get { return _result = _result ?? new Result(new string[0]); } }

        /// <summary>
        /// Return all of errors occurs on the operation.
        /// </summary>
        public IEnumerable<string> Errors { get; private set; }
        /// <summary>
        /// Return if the result is successed.
        /// </summary>
        public bool Successed { get { return !Errors.Any(); } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="errors">error occurs on the operation.</param>
        public Result(IEnumerable<string> errors)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Constructors.
        /// </summary>
        /// <param name="errors">errors occurs on the operation.</param>
        public Result(params string[] errors)
        {
            this.Errors = errors;
        }
    }
}
