using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Venom.Lib;
using Venom.Lib.Data;
using Venom.Lib.Enum;
using Venom.Lib.Util;
using Venom.Web.Helper;

namespace Venom.Web
{
    /// <summary>
    /// Helper for htmlhelper object, to create custom components
    /// </summary>
    public static class VenomHtmlHelper
    {
        /// <summary>
        /// Create an ajax html string for a pagination control.
        /// </summary>
        /// <param name="helper">the current html helper</param>
        /// <param name="list">the pagedcollection</param>
        /// <param name="generatePageUrl">action will generate the url that contains the page number and return the url</param>
        /// <param name="targetId">id of the container will be updated</param>
        /// <param name="uiDataLoadId">id from the dataload element</param>
        /// <returns>Pagination Control</returns>
        public static MvcHtmlString AjaxPagedListPager(this System.Web.Mvc.HtmlHelper helper, IPagedList list, Func<int, string> generatePageUrl, string targetId, string uiDataLoadId = "")
        {
            return PagedList.Mvc.HtmlHelper.PagedListPager(
                helper,
                list,
                generatePageUrl,
                PagedListRenderOptions.EnableUnobtrusiveAjaxReplacing(
                new PagedListRenderOptions
                {
                    LinkToFirstPageFormat = ApplicationResource.First,
                    LinkToLastPageFormat = ApplicationResource.Last,
                    LinkToNextPageFormat = ApplicationResource.Next,
                    LinkToPreviousPageFormat = ApplicationResource.Previous

                }, new System.Web.Mvc.Ajax.AjaxOptions
                {
                    LoadingElementId = uiDataLoadId,
                    UpdateTargetId = targetId,
                    HttpMethod = "post"
                })
            );
        }

        /// <summary>
        /// Create an ajax html string for a pagination control.
        /// </summary>
        /// <typeparam name="T">type of the model</typeparam>
        /// <param name="helper">current helper</param>
        /// <param name="itens">the itens of the menu</param>
        /// <param name="htmlAttributes">the html attributes of the component</param>
        /// <returns>A MvcHtmlString represents a menu html element</returns>
        public static MvcHtmlString Menu<T>(this HtmlHelper<T> helper, IEnumerable<IMenu> itens, object htmlAttributes = null)
        {
            var urlhelper = new UrlHelper(helper.ViewContext.RequestContext); 
            var menuManager = new Helper.MenuHelper(itens, urlhelper);
            return menuManager.CreateMenu();
        }

        /// <summary>
        /// Create a dropdownlist based on a nested complexy property, that has a target property as a foreignKey
        /// </summary>
        /// <typeparam name="T">type of the model</typeparam>
        /// <typeparam name="TTarget">type of the target property that will receive the value from the select item</typeparam>
        /// <param name="helper">current helper</param>
        /// <param name="expression">expression to return the nested property</param>
        /// <param name="target">selector from the property that will receive the value from the select item</param>
        /// <param name="htmlAttributes"> the select attributes</param>
        /// <returns>A MvcHtmlString represents a select html element</returns>
        public static MvcHtmlString DropDownListFrom<T, TTarget>(this HtmlHelper<T> helper, Expression<Func<T, TTarget>> target, object htmlAttributes = null)
        {
            var propertyName = target.ToString().Split('.').Last();
            string cacheKey = "dropdonw." + target.ToString();//+ typeof(T).Name + "." + propertyName;
            DropDown<T> ddl = CacheManager.Get<DropDown<T>>(cacheKey);
            if (ddl == null)
            {
                var property = GetProperty(helper.ViewData.Model, target.ToString().Split('.').Skip(1).ToArray());
                    //= helper.ViewData.Model.GetType().GetProperties()
                    //.Where(w => w.GetCustomAttributes(typeof(ForeignKeyAttribute)).Cast<ForeignKeyAttribute>()
                    //.Any(a => a.Name.Equals(propertyName)
                    //)).FirstOrDefault();

                ddl = new DropDown<T>(property.PropertyType.Assembly.CreateInstance(property.PropertyType.FullName));
                CacheManager.AddItem<DropDown<T>>(cacheKey, ddl);
            }
            return helper.DropDownListFor(target, ddl.ToList().OrderBy(s => s.Text).ToArray(), ApplicationResource.SelectUpper, htmlAttributes ?? new { @class = "form-control" });
        }

        /// <summary>
        /// Get the property will be used to find the select list itens
        /// </summary>
        /// <param name="item">the current value</param>
        /// <param name="propertyName">the navigation properties</param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(object item, string[] propertyName)
        {
            var propertiesName = propertyName;
            PropertyInfo property = null;
            // when there are more than one property, means thats just the last one is the property to be used, anothers are the navigation properties(complexy property) will be used to call the them
            if (propertiesName.Length > 1)
            {
                property = item.GetType().GetProperty(propertiesName.First());
                property = GetProperty(property.PropertyType.Assembly.CreateInstance(property.PropertyType.FullName), propertiesName.Skip(1).ToArray());
                return property;
            }
            
            property = item.GetType().GetProperties()
             .Where(w => w.GetCustomAttributes(typeof(ForeignKeyAttribute)).Cast<ForeignKeyAttribute>()
             .Any(a => a.Name.Equals(propertyName.First())
             )).FirstOrDefault();

            return property;
        }

        /// <summary>
        /// Create a dropdownlist based on a nested complexy property, that has a target property as a foreignKey
        /// </summary>
        /// <typeparam name="T">type of the model</typeparam>
        /// <typeparam name="TTarget">type of the target property that will receive the value from the select item</typeparam>
        /// <param name="helper">current helper</param>
        /// <param name="expression">expression to return the nested property</param>
        /// <param name="target">selector from the property that will receive the value from the select item</param>
        /// <param name="filter">Object the type of expecte on the query, with the property populated, to filter a Source using the FindAsMe query</param>
        /// <param name="htmlAttributes"> the select attributes</param>
        /// <returns>A MvcHtmlString represents a select html element</returns>
        public static MvcHtmlString DropDownListFrom<T, TTarget>(this HtmlHelper<T> helper, Expression<Func<T, TTarget>> target, IVenomObject filter, object htmlAttributes = null)
        {
            var propertyName = target.ToString().Split('.').Last();
            string cacheKey = "dropdonw." + typeof(T).Name + "." + propertyName;
            DropDown<T> ddl = CacheManager.Get<DropDown<T>>(cacheKey);
            if (ddl == null)
            {
                var property = helper.ViewData.Model.GetType().GetProperties()
                    .Where(w => w.GetCustomAttributes(typeof(ForeignKeyAttribute)).Cast<ForeignKeyAttribute>()
                    .Any(a => a.Name.Equals(propertyName)
                    )).FirstOrDefault();
                ddl = new DropDown<T>(filter);
                CacheManager.AddItem<DropDown<T>>(cacheKey, ddl);
            }
            return helper.DropDownListFor(target, ddl.ToList(filter), ApplicationResource.SelectUpper, htmlAttributes ?? new { @class = "form-control" });
        }

        /// <summary>
        /// Create a dropdownlist based on a nested complexy property
        /// </summary>
        /// <typeparam name="T">type of the model</typeparam>
        /// <typeparam name="TProperty">type of the nested property</typeparam>
        /// <typeparam name="TTarget">type of the target property that will receive the value from the select item</typeparam>
        /// <param name="helper">current helper</param>
        /// <param name="expression">expression to return the nested property</param>
        /// <param name="target">selector from the property that will receive the value from the select item</param>
        /// <param name="htmlAttributes"> the select attributes</param>
        /// <returns>A MvcHtmlString represents a select html element</returns>
        public static MvcHtmlString DropDownListFrom<T, TProperty, TTarget>(this HtmlHelper<T> helper, Expression<Func<T, TProperty>> expression, Expression<Func<T, TTarget>> target, object htmlAttributes = null)
            where TProperty : VenomObject<TProperty>,
            new ()
        {
            var property = expression.Compile()(helper.ViewData.Model) as TProperty ?? new TProperty();
            var ddl = new DropDownFactory<TProperty>(property);
            return helper.DropDownListFor(target, ddl.ToList(), ApplicationResource.SelectUpper, htmlAttributes ?? new { @class = "form-control" });            
        }

        /// <summary>
        /// Create a dropdownlist based on a nested complexy property
        /// </summary>
        /// <typeparam name="T">type of the model</typeparam>
        /// <typeparam name="TProperty">type of the nested property</typeparam>
        /// <typeparam name="TTarget">type of the target property that will receive the value from the select item</typeparam>
        /// <param name="helper">current helper</param>
        /// <param name="expression">expression to return the nested property</param>
        /// <param name="target">selector from the property that will receive the value from the select item</param>
        /// <param name="predicate">the filter to apply on the search</param>
        /// <param name="htmlAttributes"> the select attributes</param>
        /// <returns>A MvcHtmlString represents a select html element</returns>
        public static MvcHtmlString DropDownListFrom<T, TProperty, TTarget>(this HtmlHelper<T> helper, Expression<Func<T, TProperty>> expression, Expression<Func<T, TTarget>> target, Expression<Func<TProperty, bool>> predicate, object htmlAttributes = null)
            where TProperty : VenomObject<TProperty>,
            new()
        {
            var property = expression.Compile()(helper.ViewData.Model) as TProperty ?? new TProperty();
            var ddl = new DropDownFactory<TProperty>(property);
            return helper.DropDownListFor(target, ddl.ToList(), ApplicationResource.SelectUpper, htmlAttributes ?? new { @class = "form-control" });
        }

        /// <summary>
        /// Create a mvchtmlstring to a systembutton
        /// </summary>
        /// <typeparam name="T">type of the model</typeparam>
        /// <param name="helper">current helper</param>
        /// <param name="target">selector from the property that will receive the value from the select item</param>
        /// <returns>A MvcHtmlString represents a button html element</returns>
        public static MvcHtmlString SystemButton<T>(this HtmlHelper<T> helper, Button target)
        {
            var button = "<input type=\"{0}\" name=\"btnVenom_{1}\" id=\"btnVenom.{1}\" class=\"btn btn-outline btn-{2} btn-lg btn-block\" value=\"{3}\" />";
            button = string.Format(button, (target.IsSubmit ? "submit" : "button"), target.Name, System.Enum.GetName(typeof(ComponentType), target.Type).ToLower(), target.Text);
            return MvcHtmlString.Create(button);
        }
    }
}
