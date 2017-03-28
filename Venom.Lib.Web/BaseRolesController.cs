using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Venom.Web.Security;
using Venom.Web;
using Venom.Lib;
using Venom.Lib.Util;
using System.Linq.Expressions;
using PagedList;

namespace Venom.Web
{

    [VenomAuthorize]
    public class BaseRolesController : BaseController<Role>
    {
        protected bool AllowInsertSystemObjectChilds { get; set; }
        [HttpPost]
        [SystemObject]
        public virtual JsonResult AddRole(Role role)
        {
            try
            {
                role.Id = Guid.NewGuid().ToString();
                role.Save();
                return Json(Result.SuccessResult);
            }
            catch (Exception ex)
            {
                return Json(new Result(ex.Message));
            }
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult SearchUsers(string termo)
        {
            using (JsonSerializerContext ctx = new JsonSerializerContext(1))
            {
                return ctx.Json(new ApplicationUser { UserName = termo }.FindItensAsMe().ToArray());
            }
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult AddUser(string roleId, ApplicationUser usuario)
        {
            try
            {
                var role = new Role { Id = roleId };
                if (!role.FindItensAsMe().Any())
                    return Json(new Result("Role inválida"));
                if (!(usuario.Id ?? string.Empty).Any() || !usuario.FindItensAsMe().Any())
                    return Json(new Result("Usuário inválido"));
                role = role.FindItensAsMe().FirstOrDefault();
                role.AddUser(usuario.FindItensAsMe().First());
                return Json(Result.SuccessResult);
            }
            catch (Exception ex)
            {
                return Json(new Result(ex.Message));
            }
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult RemoveUser(string roleId, ApplicationUser usuario)
        {
            try
            {
                var role = new Role { Id = roleId };
                if (!(roleId ?? string.Empty).Any() || !role.FindItensAsMe().Any())
                    return Json(new Result("Role inválida"));
                if (!(usuario.Id ?? string.Empty).Any() || !usuario.FindItensAsMe().Any())
                    return Json(new Result("Usuário inválido"));
                role = role.FindItensAsMe().FirstOrDefault();
                role.RemoveUser(usuario.FindItensAsMe().First());
                return Json(Result.SuccessResult);
            }
            catch (Exception ex)
            {
                return Json(new Result(ex.Message));
            }
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult AddItem(string roleId, SystemObject systemObject)
        {
            try
            {
                var role = new Role { Id = roleId };
                if (!role.FindItensAsMe().Any())
                    return Json(new Result("Role inválida"));
                if (systemObject.ItemId.Equals(0) || !systemObject.FindItensAsMe().Any())
                    return Json(new Result("Item inválido"));
                role = role.FindItensAsMe().FirstOrDefault();
                role.AddSystemObject(systemObject.FindItensAsMe().First(), AllowInsertSystemObjectChilds);
                return Json(Result.SuccessResult);
            }
            catch (Exception ex)
            {
                return Json(new Result(ex.Message));
            }
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult RemoveItem(string roleId, SystemObject systemObject)
        {
            try
            {
                var role = new Role { Id = roleId };
                if (!(roleId ?? string.Empty).Any() || !role.FindItensAsMe().Any())
                    return Json(new Result("Role inválida"));
                if (systemObject.ItemId.Equals(0) || !systemObject.FindItensAsMe().Any())
                    return Json(new Result("Usuário inválido"));
                role = role.FindItensAsMe().FirstOrDefault();
                role.RemoveSystemObject(systemObject.FindItensAsMe().First());
                return Json(Result.SuccessResult);
            }
            catch (Exception ex)
            {
                return Json(new Result(ex.Message));
            }
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult SearchItens(string termo)
        {
            using (JsonSerializerContext ctx = new JsonSerializerContext(2))
            {
                return ctx.Json(new SystemObject { Name = termo }.FindItensAsMe().ToArray());
            }
        }

        [SystemObject]
        public virtual ActionResult EditRole(string id)
        {
            var item = new Role { Id = id }.FindItensAsMe().FirstOrDefault();
            if (item == null)
                return new HttpNotFoundResult("Role não encontrada");
            return View("Edit", item);
        }

        [HttpPost]
        [SystemObject]
        public virtual JsonResult Delete(string id)
        {
            try
            {
                this.Model.Id = id;
                var item = this.Model.FindItensAsMe().FirstOrDefault();
                if (item == null)
                    return Json(new Result("Role inválida"));
                if (item.Delete())
                    return Json(new Result());
                throw new Exception("Falha ao excluir a role. Tente novamente!");
            }
            catch (Exception ex)
            {
                return Json(new Result(ex.Message));
            }
        }
    }
}
