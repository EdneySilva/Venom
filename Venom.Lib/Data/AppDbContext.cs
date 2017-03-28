using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Data
{
    /// <summary>
    /// Represente the base appdbcontext used on the framework
    /// </summary>
    public partial class AppDbContext : System.Data.Entity.DbContext
    {
        private string connectionName = "name=DefaultConnection";
        /// <summary>
        /// return default connection string name
        /// </summary>
        public string ConnectionName { get { return connectionName; } }
        /// <summary>
        /// Get o set the application user
        /// </summary>
        protected DbSet<ApplicationUser> ApplicationUser { get; set; }
        /// <summary>
        /// Get or set the menus.
        /// </summary>
        protected DbSet<Menu> Menu { get; set; }
        /// <summary>
        /// Get or set the objecttype
        /// </summary>
        protected DbSet<ObjectType> ObjectType { get; set; }
        /// <summary>
        /// get or set the systemobject
        /// </summary>
        protected DbSet<SystemObject> SystemObject { get; set; }
        /// <summary>
        /// get or set the roles
        /// </summary>
        protected DbSet<Role> Role { get; set; }
        
        /// <summary>
        /// Constructor
        /// default connection name: "name=DefaultConnection"
        /// </summary>
        public AppDbContext()
            : this("name=DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">the connection "key=value"</param>
        public AppDbContext(string name)
            : base(name)
        {
            connectionName = name;
        }

        /// <summary>
        /// Occurs when the model is in creation
        /// </summary>
        /// <param name="modelBuilder">the current model builder</param>
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(s => s.Roles)
                .WithMany(s => s.ApplicationUsers)
                .Map(c =>
                {
                    c.MapLeftKey("UserId").MapRightKey("RoleId").ToTable("UserRole");
                });

            //modelBuilder.Entity<Occurrence>()
            //    .HasRequired(c => c.User)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<SystemObject>()
                .HasMany(s => s.Roles)
                .WithMany(s => s.SystemObjects)
                .Map(c => c.MapLeftKey("ObjectId").MapRightKey("RoleId").ToTable("SystemObjectRoles"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
