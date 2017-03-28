using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Venom.Lib
{
    /// <summary>
    /// Represents the userlogin for external logins
    /// </summary>
    [Table("UserLogin")]
    public class UserLogin : VenomObject<UserLogin>
    {
        /// <summary>
        /// Get or set the login provider
        /// </summary>
        [Key]
        [Column(TypeName = "NVARCHAR", Order = 1)]
        [MaxLength(256)]
        public virtual string LoginProvider { get; set; }
        /// <summary>
        /// Get or set the provider key
        /// </summary>
        [Key]
        [Column(TypeName = "NVARCHAR", Order = 2)]
        [MaxLength(256)]
        public virtual string ProviderKey { get; set; }
        /// <summary>
        /// Get or set the id of the associated user
        /// </summary>
        [Key]
        [Column(TypeName = "NVARCHAR", Order = 3)]
        [Index("IX_UserId", IsUnique = false)]
        [MaxLength(128)]        
        public virtual string UserId { get; set; }

        /// <summary>
        /// Get or set the instance of the user
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
