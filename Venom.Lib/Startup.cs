using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Data;
using Venom.Lib.Util;

namespace Venom.Lib
{
    /// <summary>
    /// Use to configure the pre-requisits for the application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Get or set the assembly that call the Venom.Lib.
        /// </summary>
        protected Assembly TargetAssembly { get; set; }

        /// <summary>
        /// Configure the Venom.Lib.
        /// </summary>
        public virtual void Configuration()
        {
            TargetAssembly = TargetAssembly ?? Assembly.GetCallingAssembly();
            this.RegisterTypes();
            this.CreateDefaultRoles();
            this.CreateDefaultObjectTypes();
            this.CreateDefaultSystemObjects();
        }

        /// <summary>
        /// Register the required tipes used on the System.
        /// </summary>
        private void RegisterTypes()
        {
            Register<Lib.IObjectType, Lib.ObjectType>();
            Register<Lib.ISystemObject, Lib.SystemObject>();
            Register<Lib.IUserClaim, Lib.UserClaim>();
            Register<Lib.Security.AuthenticationManager, Lib.Security.AuthenticationManager>();
        }

        /// <summary>
        /// Register a Type.
        /// </summary>
        /// <typeparam name="TKey">The target type</typeparam>
        /// <typeparam name="TValue">The instance value type</typeparam>
        protected void Register<TKey, TValue>()
        {
            // check if the type was registered
            if (ObjectContainer.IsRegistered<TKey>())
                return;
            // register type
            ObjectContainer.RegisterType<TKey, TValue>();
        }

        /// <summary>
        /// Create the default roles on the database
        /// [ Developer, Administrator ]
        /// </summary>
        private void CreateDefaultRoles()
        {
            string[] roles = new[] { "Developer", "Administrator" };
            for (int i = 0; i < roles.Length; i++)
            {
                var role = new Role()
                {                    
                    Name = roles[i]
                };
                if (role.FindItensAsMe().Any())
                    continue;
                role.Id = Guid.NewGuid().ToString();
                role.Save();
            }
        }

        /// <summary>
        /// Crete the default object types used on the system
        /// [ Controller, Action, Button, Page ].
        /// </summary>
        protected virtual void CreateDefaultObjectTypes()
        {
            string[] types = new[] { "Button" };
            for (int i = 0; i < types.Length; i++)
            {
                var objT = new ObjectType() { Name = types[i] };
                if (objT.FindItensAsMe().Any())
                    continue;
                objT.Save();
            }
        }

        /// <summary>
        /// Create the default systemobjects on the system.
        /// Basicaly retrieve all of Controllers that inheret of BaseController, and put them on the system.
        /// All of the objects must be on Administrator role
        /// </summary>
        protected virtual void CreateDefaultSystemObjects()
        {
        }

        /// <summary>
        /// Check if the item extends base controller
        /// </summary>
        /// <param name="type">type of the target type</param>
        /// <param name="typeBase">the type base to compare</param>
        /// <returns>return if the type extends typebase</returns>
        protected bool IsAssignableFrom(Type type, string typeBase)
        {
            return type.BaseType.FullName.Contains(typeBase) || (type.BaseType.BaseType != null && IsAssignableFrom(type.BaseType, typeBase));
        }

        /// <summary>
        /// Create the childrens of each default system object
        /// </summary>
        /// <param name="role">the role to applied for each element</param>
        /// <param name="parent">the parent object</param>
        /// <param name="value">the parent type</param>
        /// <param name="objectTypeId">the id of the type of the system object</param>
        protected virtual void CreateDefaultSystemObjectsChildren(Role role, SystemObject parent, Type value, int objectTypeId)
        {
            var methods = value.GetMethods().Where(w => w.GetCustomAttributes(typeof(SystemObjectAttribute), false).Any()).ToArray();
            for (int i = 0; i < methods.Length; i++)
                CreateDefaultSystemObject(role, parent, methods[i].Name, objectTypeId);
        }

        /// <summary>
        /// Configure the application data context with the caller context
        /// </summary>
        /// <typeparam name="T">type of the context</typeparam>
        /// <returns>the current instance</returns>
        public Startup ConfigureDataContext<T>() where T : AppDbContext
        {
            ObjectContainer.RegisterType<AppDbContext, T>();
            return this;
        }

        /// <summary>
        /// Configure the application provider authentication with the caller authentication implementation.
        /// </summary>
        /// <typeparam name="T">type of the provider, that most implement Venom.Lib.Security.IAuthenticationProvider.</typeparam>
        /// <returns>the current instance.</returns>
        public Startup ConfigureAuthentication<T>() where T : Security.IAuthenticationProvider
        {
            ObjectContainer.RegisterType<Security.IAuthenticationProvider, T>();
            return this;
        }

        /// <summary>
        /// Configure the page size to paged collections.
        /// </summary>
        /// <param name="pageSize">the page size.</param>
        /// <returns>the current instance.</returns>
        public virtual Startup ConfigurePaginationSize(int pageSize)
        {
            PageConfiguration.Create(pageSize);
            return this;
        }

        /// <summary>
        /// Create and persist a SystemObject on the system.
        /// </summary>
        /// <param name="role">Role that have access to the SystemObject.</param>
        /// <param name="parent">the object parent of this SystemObject, if null, this object won't have a parent.</param>
        /// <param name="name">name of the SystemObject.</param>
        /// <param name="objectTypeId">type of the SystemObject.</param>
        /// <returns></returns>
        protected virtual SystemObject CreateDefaultSystemObject(Role role, SystemObject parent, string name, int objectTypeId)
        {
            var sysObj = new SystemObject() { Name = name, ParentItem = parent, ObjectTypeId = objectTypeId, IsEnable = true };
            var query = sysObj.FindItensAsMe();
            if (query.Any())
                return query.First();
            role.AddSystemObject(sysObj);
            return sysObj;
        }
    }
}
