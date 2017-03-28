using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Venom.Lib
{
    /// <summary>
    /// Reprent the claim of user
    /// </summary>
    [Table("UserClaim")]
    public class UserClaim : VenomObject<UserClaim>, Venom.Lib.IUserClaim
    {
        /// <summary>
        /// Get or set type of claim
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        public virtual string ClaimType { get; set; }
        /// <summary>
        /// Get or set the claim value
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        [Index("IX_UserId", IsUnique = true, Order = 2)]
        public virtual string ClaimValue { get; set; }
        /// <summary>
        /// Get or set the id
        /// </summary>
        [Key]
        public virtual int Id { get; set; }
        /// <summary>
        /// get or set the user id associed to the claim
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR")]
        [Index("IX_UserId", IsUnique = true, Order = 1)]
        [MaxLength(128)]
        public virtual string UserId { get; set; }
        /// <summary>
        /// get or set a instance of the user associed to the claim
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
