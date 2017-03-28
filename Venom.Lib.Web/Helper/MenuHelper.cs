using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text;
using Venom.Lib;

namespace Venom.Web.Helper
{

    /// <summary>
    /// Class responsable to create the menu html.
    /// </summary>
    class MenuHelper
    {
        /// <summary>
        /// A action the execute when the build menu was completed.
        /// </summary>
        Action<TagBuilder> BuilderCompleted;
        /// <summary>
        /// Get or set the main menu container.
        /// </summary>
        TagBuilder Container { get; set; }
        /// <summary>
        /// Get or set the content menu.
        /// </summary>
        TagBuilder Content { get; set; }
        /// <summary>
        /// Get or set the list itens on menu.
        /// </summary>
        IEnumerable<IMenu> MenuItem { get; set; }
        /// <summary>
        /// Get or set the responsable to build the menu url.
        /// </summary>
        UrlHelper Url { get; set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="menu">the itens will rendered.</param>
        /// <param name="urlHelper">the object responsable to resolve the url.</param>
        public MenuHelper(IEnumerable<IMenu> menu, UrlHelper urlHelper)
        {
            Url = urlHelper;
            MenuItem = menu.OrderBy(o => o.Order).ToArray();
        }

        /// <summary>
        /// Start building menu.
        /// </summary>
        /// <returns>the curret instance.</returns>
        private MenuHelper Begin()
        {
            var div = new TagBuilder("div");
            div.AddCssClass("navbar-default sidebar");
            div.Attributes.Add("role", "navigation");
            var divCollapse = new TagBuilder("div");
            divCollapse.AddCssClass("sidebar-nav navbar-collapse");
            BuilderCompleted += (content) =>
            {
                divCollapse.InnerHtml = content.ToString();
                div.InnerHtml = divCollapse.ToString();
                this.Container = div;
            };
            return this;
        }

        /// <summary>
        /// Create a menu html string, bootstrap based on.
        /// </summary>
        /// <returns>bootstrap menu htmlstring.</returns>
        public MvcHtmlString CreateMenu()
        {
            return new MvcHtmlString(
                this.Begin()
                .CreateContent(false)
                .End()
                .Container.ToString()
            );
        }

        /// <summary>
        /// Create the menu content.
        /// </summary>
        /// <param name="hasSearch">define if the menu has a input search.</param>
        /// <returns>the current instânce</returns>
        private MenuHelper CreateContent(bool hasSearch = false)
        {
            var ul = new TagBuilder("ul");
            ul.AddCssClass("nav");
            ul.Attributes.Add("id", "side-menu");
            if (hasSearch)
                ul.InnerHtml = SearchForm().ToString();

            StringBuilder builder = new StringBuilder();
            foreach (var item in MenuItem.Where(w => !w.ParentId.HasValue).OrderBy(o => o.Order).ToArray())
                builder.AppendLine(AddItem(item).ToString().ToString());
            ul.InnerHtml = builder.ToString();
            this.Content = ul;
            return this;
        }

        /// <summary>
        /// Finish the menu build.
        /// </summary>
        /// <returns>the current instance.</returns>
        private MenuHelper End()
        {
            if (BuilderCompleted != null)
                BuilderCompleted(this.Content);
            return this;
        }

        /// <summary>
        /// Add itens on the menu.
        /// </summary>
        /// <param name="item">item to be add.</param>
        /// <returns>a tag represents the item.</returns>
        private TagBuilder AddItem(IMenu item)
        {
            var li = BuilderLiTag(item);
            if (!item.SubItens.Any())
                return li;
            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("nav nav-second-level");
            foreach (var filho in item.SubItens.OrderBy(a => a.Order))
                ul.InnerHtml += AddItem(filho).ToString();
            li.InnerHtml += ul.ToString();
            return li;
        }

        /// <summary>
        /// Build a &lt;li&gt; tag.
        /// </summary>
        /// <param name="item">the menu item.</param>
        /// <returns>a tag to represents &lt;li&gt; tag</returns>
        private TagBuilder BuilderLiTag(IMenu item)
        {
            TagBuilder li = new TagBuilder("li");
            TagBuilder a = new TagBuilder("a");
            TagBuilder i = new TagBuilder("i");
            i.AddCssClass("fa fa-fw");
            a.Attributes.Add("href", Url.Content(item.Url));
            a.InnerHtml = i.ToString() + item.Name + (item.SubItens.Any() ? "<span class=\"fa arrow\"></span>" : string.Empty);
            li.InnerHtml = a.ToString();
            return li;
        }

        /// <summary>
        /// Create the search form.
        /// </summary>
        /// <returns>a tag that contains a search menu.</returns>
        private TagBuilder SearchForm()
        {
            var liSearch = new TagBuilder("li");
            liSearch.AddCssClass("sidebar-search");
            var divSearchForm = new TagBuilder("div");
            divSearchForm.AddCssClass("input-group custom-search-form");
            var input = new TagBuilder("input");
            input.AddCssClass("form-control");
            input.Attributes.Add("placeholder", "search...");
            input.Attributes.Add("type", "text");

            var span = new TagBuilder("span");
            span.AddCssClass("input-group-btn");
            var button = new TagBuilder("button");
            button.AddCssClass("btn btn-default");
            button.Attributes.Add("type", "button");
            button.InnerHtml = "<i class=\"fa fa-search\"></i>";
            span.InnerHtml = button.ToString();
            divSearchForm.InnerHtml = input.ToString() + span.ToString();
            liSearch.InnerHtml = divSearchForm.ToString();

            return liSearch;
        }

        #region Old Code
        
        //private TagBuilder NavBar()
        //{
        //    bool hasSearch = false;

        //    var div = new TagBuilder("div");
        //    div.AddCssClass("navbar-default sidebar");
        //    div.Attributes.Add("role", "navigation");
        //    var divCollapse = new TagBuilder("div");
        //    divCollapse.AddCssClass("sidebar-nav navbar-collapse");
        //    var ul = new TagBuilder("ul");
        //    ul.AddCssClass("nav");
        //    ul.Attributes.Add("id", "side-menu");
        //    if (hasSearch)
        //        ul.InnerHtml = SearchForm().ToString();

        //    StringBuilder builder = new StringBuilder();
        //    foreach (var item in MenuItem.Where(w => !w.ParentId.HasValue).ToList())
        //        builder.AppendLine(AddItem(item).ToString().ToString());
        //    ul.InnerHtml = builder.ToString();
        //    divCollapse.InnerHtml = ul.ToString();
        //    div.InnerHtml = divCollapse.ToString();
        //    return div;
        //}
        #endregion        
    }
}