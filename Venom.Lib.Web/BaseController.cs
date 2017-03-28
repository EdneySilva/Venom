using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Venom.Lib;
using Venom.Lib.Security;
using Venom.Lib.Util;
using Venom.Web.Security;
using Microsoft.Owin.Host;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Venom.Web
{
    [VenomAuthorize]
    /// <summary>
    /// All controller, that have a VenomObject as a default model, should inherit this controller.
    /// Here default actions are implemented, and can be overrided by the custom user behavior
    /// </summary>
    /// <typeparam name="T">A type that preferably inherit VenomObject, this model must have a default constructor whitout parameters</typeparam>
    public class BaseController<T> : System.Web.Mvc.Controller
        where T : class, new()
    {
        /// <summary>
        /// Encrypt an item used as a password;
        /// </summary>
        /// <param name="value">the original value</param>
        /// <returns>the new value.</returns>
        protected string EncryptAsPassword(string value)
        {
            return HttpContext.GetOwinContext().Get<Web.Security.ApplicationUserManager>().PasswordHasher.HashPassword(value);
        }

        /// <summary>
        /// the authenticationmanager instance.
        /// </summary>
        private Lib.Security.AuthenticationManager _authenticationManager;
        /// <summary>
        /// the default configurations.
        /// </summary>
        private BaseControllerConfiguration _baseControllerConfiguration;

        //private ApplicationUserManager _userManager;
        /// <summary>
        /// Get or Set the model used on the controller
        /// </summary>
        protected T Model { get; set; }
        /// <summary>
        /// Return a instace of the Application AuthenticaionManager.
        /// </summary>
        protected Lib.Security.AuthenticationManager AutenticationManager
        {
            get
            {
                return _authenticationManager = _authenticationManager ?? HttpContext.GetOwinContext().Get<Lib.Security.AuthenticationManager>();
            }
        }
        /// <summary>
        /// Return a instace of the BaseControllerConfiguration.
        /// </summary>
        protected BaseControllerConfiguration Configuration
        {
            get
            {
                return _baseControllerConfiguration = _baseControllerConfiguration ?? HttpContext.GetOwinContext().Get<BaseControllerConfiguration>();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseController()
        {
            this.Model = new T();
        }

        /// <summary>
        /// Find a item based on the primarykey value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual VenomObject<T> FindItemToEdit(int id)
        {
            // retrive the propertykey o the type
            var type = typeof(T);
            var properties = type.GetProperties().Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Any()).ToArray();
            // the object must be only one PrimaryKey to use this method
            if (properties.Count() > 1)
                throw new InvalidOperationException("This itens have more one property key, may be you need override or create another edit or delete action");
            if (properties.Count() == 0)
                throw new InvalidOperationException("Key property not found!");
            // set de value on primary key and return the object
            properties.First().SetValue(this.Model, id);
            return this.Model as VenomObject<T>;
        }

        /// <summary>
        /// GET: /{Controller}/
        /// The index page of controller
        /// </summary>
        /// <returns>ActionResult represents the Index View</returns>
        [SystemObject]
        //[VenomAuthorize]
        public virtual ActionResult Index()
        {
            return Configuration.IsDefault ? View(Model) : View(Configuration.IndexViewName, Model);
        }

        /// <summary>
        /// POST: /{Controller}/
        /// The post index action
        /// </summary>
        /// <param name="model">a object retivied from the data posted</param>
        /// <returns>ActionResult represents the Index View</returns>
        [HttpPost]
        [ValidateInput(false)]
        [SystemObject]
        //[VenomAuthorize]
        public virtual ActionResult Index(T model)
        {
            // clear to doesn't return error on searchs
            this.ModelState.Clear();
            return Configuration.IsDefault ? View(model) : View(Configuration.IndexViewName, model);
        }

        /// <summary>
        /// GET: /{Controller}/Create
        /// The Create page of controller
        /// </summary>
        /// <returns>ActionResult represents the Create View</returns>
        [SystemObject]
        //[VenomAuthorize]
        public virtual ActionResult Create()
        {
            return Configuration.IsDefault ? View(Model) : View(Configuration.CreateViewName, Model);
        }

        /// <summary>
        /// POST: /{Controller}/Create
        /// The post Create action
        /// </summary>
        /// <param name="model">a object retivied from the data posted</param>
        /// <returns>ActionResult represents the Create View</returns>
        [HttpPost]
        [SystemObject]
        //[VenomAuthorize]
        public virtual ActionResult Create(T model)
        {
            bool result = this.Cast(model).Save();
            if (result)
                return RedirectToAction("Index");
            return Configuration.IsDefault ? View(model) : View(Configuration.CreateViewName, model);
        }

        /// <summary>
        /// POST: /{Controller}/Create
        /// The post Create action
        /// </summary>
        /// <param name="model">a object retivied from the data posted</param>
        /// <returns>ActionResult represents the Create View</returns>
        [HttpPost]
        [SystemObject]
        //[VenomAuthorize]
        public virtual JsonResult CreateAsync(T model)
        {
            try
            {               
                bool result = this.Cast(model).Save();
                return Json(new Result(result ? new string[0] : new string[] { Lib.Util.ApplicationResource.FailSave }));
            }
            catch (Exception ex)
            {
                return Json(new Result(new string[] { ex.Message }));
            }
        }

        /// <summary>
        /// GET: /{Controller}/Edit
        /// The Edit page of controller
        /// </summary>
        /// <param name="id">id of the model</param>
        /// <returns>ActionResult represents the Edit View</returns>
        [SystemObject]
        //[VenomAuthorize]
        public virtual ActionResult Edit(int id)
        {
            return Configuration.IsDefault ? View(FindItemToEdit(id).GetById()) : View(Configuration.EditViewName, FindItemToEdit(id).GetById());
        }

        /// <summary>
        /// POST: /{Controller}/Edit
        /// The post Edit action
        /// </summary>
        /// <param name="model">a object retivied from the data posted</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [SystemObject]
        //[VenomAuthorize]
        public virtual JsonResult EditAsync(T model)
        {
            try
            {
                bool result = this.Cast(model).Save();
                return Json(new Result(result ? new string[0] : new string[] { Lib.Util.ApplicationResource.FailSave }));
            }
            catch (Exception ex)
            {
                return Json(new Result(new string[] { ex.Message }));
            }
        }

        /// <summary>
        /// POST: /{Controller}/Edit
        /// The post Edit action
        /// </summary>
        /// <param name="model">a object retivied from the data posted</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [SystemObject]
        //[VenomAuthorize]
        public virtual ActionResult Edit(T model)
        {
            bool result = this.Cast(model).Save();
            if (result)
                return RedirectToAction("Index");
            return Configuration.IsDefault ? View(model) : View(Configuration.EditViewName, model);
        }

        /// <summary>
        /// GET: /{Controller}/Delete
        /// The Delete page of controller
        /// </summary>
        /// <param name="id">id of the model</param>
        /// <returns>Redirect to Index</returns>
        [SystemObject]
        //[VenomAuthorize]
        public ActionResult Delete(int id)
        {
            var item = FindItemToEdit(id).GetById() as VenomObject<T>;
            item.Delete();
            return Configuration.IsDefault ? RedirectToAction("Index") : RedirectToAction(Configuration.DeleteViewName);
        }

        /// <summary>
        /// GET: /{Controller}/Grid
        /// The Grid PartialView
        /// </summary>
        /// <returns>PartialViewResult that represents a Grid View</returns>
        [SystemObject]
        //[VenomAuthorize]
        public virtual PartialViewResult Grid()
        {
            return Configuration.IsDefault ? PartialView() : PartialView(Configuration.GridViewName);
        }

        /// <summary>
        /// POST: /{Controller}/Grid
        /// The Grid PartialView action used to search an filter
        /// </summary>
        /// <returns>PartialViewResult that represents a Grid View</returns>
        [HttpPost]
        [SystemObject]
        //[VenomAuthorize]
        public virtual PartialViewResult Grid(T model)
        {
            // clear to doesn't return error on searchs
            this.ModelState.Clear();
            var modelResult = (model as VenomObject<T>).FindItensAsMe();
            return Configuration.IsDefault ? PartialView(modelResult) : PartialView(Configuration.GridViewName, modelResult);
        }


        /// <summary>
        /// GET: /{Controller}/PagedGrid
        /// The Grid PartialView
        /// </summary>
        /// <returns>PartialViewResult that represents a Grid View</returns>
        [SystemObject]
        //[VenomAuthorize]
        public virtual PartialViewResult PagedGrid()
        {
            return Configuration.IsDefault ? PartialView() : PartialView(Configuration.PagedGridViewName);
        }

        /// <summary>
        /// POST: /{Controller}/PagedGrid
        /// The Grid PartialView action used to search an filter
        /// </summary>
        /// <returns>PartialViewResult that represents a Grid View</returns>
        [HttpPost]
        [SystemObject]
        //[VenomAuthorize]
        public virtual PartialViewResult PagedGrid(T model, int? pageNumber)
        {
            // clear to doesn't return error on searchs
            this.ModelState.Clear();
            var modelResult = (model as VenomObject<T>).ToPagedList(pageNumber ?? 1);
            return Configuration.IsDefault ? PartialView(modelResult) : PartialView(Configuration.PagedGridViewName, modelResult);
        }

        /// <summary>
        /// GET: /{Controller}/CreateOrEdit
        /// The CreateOrEdit PartialView
        /// </summary>
        /// <returns>PartialViewResult that represents a CreateOrEdit View</returns>
        [SystemObject]
        //[VenomAuthorize]
        public virtual PartialViewResult CreateOrEdit()
        {
            return Configuration.IsDefault ? PartialView() : PartialView(Configuration.CreateOrEditViewName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [SystemObject]
        public virtual JsonResult List(T item)
        {
            return Json((item as VenomObject<T>).FindItensAsMe());
        }

        /// <summary>
        /// Cast a model from a Unknown type to a VenomObject
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A instance of VenomObject</returns>
        private VenomObject<T> Cast(T model)
        {
            return model as VenomObject<T>;
        }
    }
}
