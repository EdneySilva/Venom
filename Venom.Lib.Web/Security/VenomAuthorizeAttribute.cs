
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Venom.Lib;
using Venom.Lib.Security;

namespace Venom.Web.Security
{
    /// <summary>
    /// Represent an object responsable to hanlder de Asp.Net MVC authorization
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class VenomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        IAuthorizeProvider AuthorizeProvider { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public VenomAuthorizeAttribute()
        {
        }

        /// <summary>
        /// Called when a process requests authorization.
        /// </summary>
        /// <param name="filterContext">The filter context, which encapsulates information for using System.Web.Mvc.AuthorizeAttribute.</param>
        /// <exception cref="System.ArgumentNullException">The filterContext parameter is null.</exception>
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            // check if the Annonymous Authentication is allowed in this request
            if (filterContext.ActionDescriptor.GetCustomAttributes(true).Any(a => a.GetType().Name.Contains("AllowAnonymous")))
                return;
            // apply the base authorization, if base response is unauthorized or not found  claims, break the operation
            base.OnAuthorization(filterContext);
            if (filterContext.Result is System.Web.Mvc.HttpUnauthorizedResult || (System.Web.HttpContext.Current.User as ClaimsPrincipal) == null)
                return;
            // Create the provide based on the userclaims
            AuthorizeProvider = ObjectContainer.New<IAuthorizeProvider>(
                (System.Web.HttpContext.Current.User as ClaimsPrincipal).Claims
                .Select(
                    s => ObjectContainer.New<IUserClaim>((obj) => {
                        obj.ClaimType = s.Type;
                        obj.ClaimValue = s.Value;
                    }
                 )
             ).AsEnumerable());
            // check the user permissions
            if (!AuthorizeProvider.Authorized(CreateSystemObject(filterContext))) 
                filterContext.Result = new System.Web.Mvc.HttpUnauthorizedResult("Sem acesso ao recurso!");
        }

        /// <summary>
        /// Create the system object based on the filterContext(action/controller)
        /// </summary>
        /// <param name="filterContext">the current filtercontext</param>
        /// <returns>an instance to ISystemObject</returns>
        private ISystemObject CreateSystemObject(System.Web.Mvc.AuthorizationContext filterContext)
        {
            var systemObject = ObjectContainer.New<ISystemObject>();
            systemObject.Name = filterContext.ActionDescriptor.ActionName;
            systemObject.ParentItem = ObjectContainer.New<ISystemObject>((obj) =>
            {
                obj.Name = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "Controller";
            });
            return systemObject.FindItensAsMe().FirstOrDefault();
        }
    }
}
