using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib.Util;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Venom.Lib.Data;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.Owin;
using Venom.Lib;
using Venom.Web.Security;
using Venom.Web.Helper;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Venom.Web
{
    /// <summary>
    /// Use to configure the pre-requisits for the application
    /// </summary>
    public class Startup : Lib.Startup
    {
        /// <summary>
        /// Get or set an object responsable to maintain the configurations to BaseController
        /// </summary>
        BaseControllerConfiguration BaseControllerConfiguration { get; set; }

        /// <summary>
        /// Get or set the url to logon view
        /// </summary>
        string LogOnUrl { get; set; }

        /// <summary>
        /// Return a instance of the Startup class
        /// </summary>
        public static Startup Default { get { return new Startup(); } }
        
        /// <summary>
        /// Constructor
        /// </summary>
        private Startup()
        {
            this.LogOnUrl = "/Account/Login";
        }

        /// <summary>
        /// start the project configuration
        /// </summary>
        /// <param name="app">the curret appbuilder</param>
        /// <returns>the current instance</returns>
        public void Configuration(IAppBuilder app)
        {
            TargetAssembly = Assembly.GetCallingAssembly();
            this.Register<Lib.Security.AuthenticationManager, Security.WebAuthenticationManager>();
            this.ConfigureLogOn(app);
            base.Configuration();
        }

        /// <summary>
        /// Configure the source path to defaults views on BaseController [Index, Create, Delete, Edit, CreateOrEdit, Grid].
        /// Default values:
        /// Index="../Views/{ControllerName}/Index"
        /// Create="../Views/{ControllerName}/Create"
        /// Delete=(redirect to index")
        /// Edit="../Views/{ControllerName}/Edit"
        /// CreateOrEdit="../Views/{ControllerName}/CreateOrEdit"
        /// Grid="../Views/{ControllerName}/Grid"
        /// When this configuration are changed, the BaseController will try call the respected view name set here.
        /// </summary>
        /// <param name="newConfiguration">set the configuration must be applied.</param>
        /// <returns>the current instance.</returns>
        public Startup ConfigureDefaultViewsName(BaseControllerConfiguration newConfiguration)
        {
            this.BaseControllerConfiguration = newConfiguration;
            return this;
        }

        /// <summary>
        /// Configure the controller/action will be called with default page logon
        /// default: /Account/Login
        /// </summary>
        /// <param name="url">url to be redirect when necessary</param>
        /// <returns>the current instance</returns>
        public Startup ConfigureLogOnPage(string url)
        {
             this.LogOnUrl = url;
            return this;
        }

        /// <summary>
        /// Configure the datacontext will be used to persist the data
        /// </summary>
        /// <typeparam name="T">Type of the context. this type must extend the AppDbContext</typeparam>
        /// <returns>the current instance value</returns>
        new public Startup ConfigureDataContext<T>() where T : AppDbContext
        {
            base.ConfigureDataContext<T>();
            return this;
        }

        /// <summary>
        /// Configure the authentication provider that must be used
        /// </summary>
        /// <typeparam name="T">type of the authentication provider, that must be implements the IAuthenticationProvider interface</typeparam>
        /// <returns>currentinstance</returns>
        new public Startup ConfigureAuthentication<T>() where T : Lib.Security.IAuthenticationProvider
        {
            base.ConfigureAuthentication<T>();
            return this;
        }

        /// <summary>
        /// Configure the page size to paged collections.
        /// </summary>
        /// <param name="pageSize">the page size.</param>
        /// <returns>the current instance.</returns>
        new public Startup ConfigurePaginationSize(int pageSize)
        {
            base.ConfigurePaginationSize(pageSize);
            return this;
        }

        /// <summary>
        /// Configure the logon features
        /// </summary>
        /// <param name="app">the curret appbuilder</param>
        private void ConfigureLogOn(IAppBuilder app)
        {
            app.CreatePerOwinContext<Lib.Security.AuthenticationManager>(Lib.Security.AuthenticationManager.Create);
            // Configure the authentication manager of the application
            app.CreatePerOwinContext<BaseControllerConfiguration>(() => BaseControllerConfiguration ?? new BaseControllerConfiguration(true));
            // Configure the db context, user manager and signin manager to use a single instance per request            
            app.CreatePerOwinContext(Web.Data.ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // register the provider to authentication and authorization
            this.Register<Lib.Security.IAuthenticationProvider, Web.Security.WebAuthenticationProvider>();
            this.Register<Lib.Security.IAuthorizeProvider, Web.Security.WebAuthorizeProvider>();
            
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(LogOnUrl),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, IdentityUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            
            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }

        /// <summary>
        /// Crete the default object types used on the system
        /// [ Controller, Action, Button, Page ].
        /// </summary>
        protected override void CreateDefaultObjectTypes()
        {
            string[] types = new[] { "Controller", "Action", "Page" };
            for (int i = 0; i < types.Length; i++)
            {
                var objT = new ObjectType() { Name = types[i] };
                if (objT.FindItensAsMe().Any())
                    continue;
                objT.Save();
            }
            base.CreateDefaultObjectTypes();
        }

        // <summary>
        /// Create the default systemobjects on the system.
        /// Basicaly retrieve all of Controllers that inheret of BaseController, and put them on the system.
        /// All of the objects must be on Administrator role
        /// </summary>
        protected override void CreateDefaultSystemObjects()
        {
            const string typeBase = "Venom.Web.BaseController";
            // get the default role e types of objects
            var role = new Role() { Name = "Developer" }.FindItensAsMe().First();
            var controllerType = new ObjectType() { Name = "Controller" }.FindItensAsMe().First();
            var actionType = new ObjectType() { Name = "Action" }.FindItensAsMe().First();
            // get all controllers that extends BaseController
            var controllers = TargetAssembly.DefinedTypes.Where(w =>
                (w.Namespace ?? string.Empty).ToLower().Contains(".controller") &&
                IsAssignableFrom(w, typeBase)
            ).ToArray();
            for (int i = 0; i < controllers.Length; i++)
            {
                var sysObj = CreateDefaultSystemObject(role, null, controllers[i].Name, controllerType.ObjectTypeId);
                CreateDefaultSystemObjectsChildren(role, sysObj, controllers[i], actionType.ObjectTypeId);
            }
            base.CreateDefaultSystemObjects();
        }
    }
}
