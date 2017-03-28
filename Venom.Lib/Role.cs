using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Venom.Lib.Util;

namespace Venom.Lib
{
    /// <summary>
    /// Represent the role on the system.
    /// </summary>
    [Table("Role")]
    public class Role : VenomObject<Role>, Venom.Lib.IRole
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Role()
        {
            ApplicationUsers = new HashSet<ApplicationUser>();
            SystemObjects = new HashSet<SystemObject>();
        }

        /// <summary>
        /// Get or set the id.
        /// </summary>
        [Key]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(128)]
        public string Id { get; set; }

        /// <summary>
        /// Get or set the name.
        /// </summary>
        [Required]
        [Index("RoleNameIndex", IsUnique = true)]
        [Column(TypeName = "NVARCHAR")]
        [MaxLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Return the ApplicationUsers in this Role Context.
        /// </summary>
        [IgnoredFilter]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; protected set; }
        /// <summary>
        /// Return the SystemObjects in this Role Context
        /// </summary>
        [IgnoredFilter]
        public virtual ICollection<SystemObject> SystemObjects { get; protected set; }

        /// <summary>
        /// Add an user to this Role
        /// </summary>
        /// <param name="user">the user handled</param>
        /// <returns>true if the user was added with success on Role</returns>
        public bool AddUser(ApplicationUser user)
        {
            var userId = user.Id;
            if (ApplicationUsers.Any(a => a.Id.Equals(userId)))
                return false;
            ApplicationUsers.Add(user);
            using (TransactionScope t = new TransactionScope())
            {
                UserClaim userClaim = new UserClaim();
                userClaim.ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
                userClaim.ClaimValue = this.Name;
                userClaim.UserId = user.Id;
                if (!userClaim.Save() || !this.Save())
                    return false;
                t.Complete();
                return true;
            }
        }

        /// <summary>
        /// Remove an user of this role
        /// </summary>
        /// <param name="user">the user will removed</param>
        /// <returns>true if were removed success</returns>
        public bool RemoveUser(ApplicationUser user)
        {
            var userId = user.Id;
            if (!ApplicationUsers.Any(a => a.Id.Equals(userId)))
                return true;
            ApplicationUsers.Remove(user);
            using (TransactionScope t = new TransactionScope())
            {
                UserClaim userClaim = new UserClaim();
                userClaim.ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
                userClaim.ClaimValue = this.Name;
                userClaim.UserId = user.Id;
                userClaim = userClaim.FindItensAsMe().First();
                if (!userClaim.Delete() || !this.Save())
                    return false;
                t.Complete();
                return true;
            }
        }

        /// <summary>
        /// Add an SystemObject to this Role.
        /// </summary>
        /// <param name="systemObject">the SystemObject handled.</param>
        /// <param name="addChildren">if true the SystemObject's children will be added to the current role</param>
        /// <returns>true if the SystemObject was added with success on Role.</returns>
        public bool AddSystemObject(SystemObject systemObject, bool addChildren = false)
        {
            return AddSystemObject(systemObject, addChildren, false);
        }

        /// <summary>
        /// Remove a system object of this role
        /// </summary>
        /// <param name="systemObject">the system object will removed</param>
        /// <returns>true if were removed success</returns>
        public bool RemoveSystemObject(SystemObject systemObject)
        {
            return RemoveSystemObject(systemObject, false);
        }

        /// <summary>
        /// Add an SystemObject to this Role.
        /// </summary>
        /// <param name="systemObject">the SystemObject handled.</param>
        /// <param name="addChildren">if true the SystemObject's children will be added to the current role</param>
        /// <param name="isChildren">define if the current item is a children object, if true, just will save on the no child item</param>
        /// <returns>true if the SystemObject was added with success on Role.</returns>
        private bool AddSystemObject(SystemObject systemObject, bool addChildren, bool isChildren)
        {
            var id = systemObject.ItemId;
            if (SystemObjects.Any(a => a.ItemId.Equals(id)))
                return true;
            SystemObjects.Add(systemObject);
            if (addChildren && systemObject.Children.Any())
                foreach (var child in systemObject.Children)
                    AddSystemObject(child, addChildren, true);
            if (isChildren)
                return true;
            return this.Save();
        }

        /// <summary>
        /// Remove a system object of this role
        /// </summary>
        /// <param name="systemObject">the system object will removed</param>
        /// <param name="isChildren">define if the current item is a children object, if true, just will save on the no child item</param>
        /// <returns>true if were removed success</returns>
        private bool RemoveSystemObject(SystemObject systemObject, bool isChildren)
        {
            var id = systemObject.ItemId;
            if (!SystemObjects.Any(a => a.ItemId.Equals(id)))
                return true;
            SystemObjects.Remove(systemObject);
            if (systemObject.Children.Any())
                foreach (var child in systemObject.Children)
                    RemoveSystemObject(child, true);
            if (isChildren)
                return true;
            return this.Save();
        }
    }
}
