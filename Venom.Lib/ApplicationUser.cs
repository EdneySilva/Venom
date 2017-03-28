using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib
{
    /// <summary>
    /// Represent an account user login.
    /// </summary>
    [Table("ApplicationUser")]
    public class ApplicationUser : VenomObject<ApplicationUser>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ApplicationUser()
        {
            Roles = new HashSet<Role>();
            Logins = new HashSet<UserLogin>();
            Claims = new HashSet<UserClaim>();
        }

        /// <summary>
        /// Get or set Id.
        /// </summary>
        [Key]
        [Column(TypeName="NVARCHAR")]
        [MaxLength(128)]
        public string Id { get; set; }
        /// <summary>
        /// Get or set Email account.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(256)]
        public virtual string Email { get; set; }
        /// <summary>
        /// Get or set if the email is confirmed.
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }
        /// <summary>
        /// Get or set password.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        public virtual string PasswordHash { get; set; }
        /// <summary>
        /// Get or set SecurityStamp.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        public virtual string SecurityStamp { get; set; }
        /// <summary>
        /// Get or set PhoneNumber.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        public virtual string PhoneNumber { get; set; }
        /// <summary>
        /// Get or set if the PhoneNumber was confirmed.
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }
        /// <summary>
        /// Get or set TwoFactor is enabled.
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }
        /// <summary>
        /// Get or set LockoutEndDateUtc.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        /// <summary>
        /// Get or set Lockout is enabled.
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }
        /// <summary>
        /// Get or set how many times the login was failed.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }
        /// <summary>
        /// Get or set User Name.
        /// </summary>
        [Required]
        [Column(TypeName = "NVARCHAR")]
        [Index("UserNameIndex", IsUnique = true)]
        [MaxLength(256)]
        public virtual string UserName { get; set; }
        /// <summary>
        /// Return all roles of this user.
        /// </summary>
        public virtual ICollection<Role> Roles { get; protected set; }
        /// <summary>
        /// Return all logins of this user.
        /// </summary>
        public virtual ICollection<UserLogin> Logins { get; protected set; }
        /// <summary>
        /// Return all claims of this user.
        /// </summary>
        public virtual ICollection<UserClaim> Claims { get; protected set; }
    }
}