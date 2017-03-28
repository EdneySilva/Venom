using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Enum;

namespace Venom.Web
{
    /// <summary>
    /// Represents a button object on the system
    /// </summary>
    public class Button
    {
        /// <summary>
        /// Get or set the name button
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the text of button
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Get or set if the button will submit the form
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// Get or set tye componenttype of button (danger, primary, succes, default)
        /// </summary>
        public ComponentType Type { get; set; }
    }
}
