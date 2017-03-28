using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Data;

namespace Venom.Web.Data
{
    /// <summary>
    /// Manage the default asp.net authentication
    /// </summary>
    internal class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">name of the connection</param>
        public ApplicationDbContext(string name)
            : base(name, throwIfV1Schema: false)
        {

        }

        /// <summary>
        /// Execute when the model start be created
        /// </summary>
        /// <param name="modelBuilder">the model builder used in this context</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // create the default tables used by the system
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().ToTable("ApplicationUser");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
        }

        /// <summary>
        /// Factory a instance of ApplicationDbContext
        /// </summary>
        /// <returns>ApplicationDbContext</returns>
        public static ApplicationDbContext Create()
        {
            return new Web.Data.ApplicationDbContext(AppRepositoryManager<AppDbContext>.Default.Context.ConnectionName);
        }
    }
}