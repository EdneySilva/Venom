using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Venom.Lib;
using Venom.Lib.Util;

namespace Venom.Web.Helper
{
    /// <summary>
    /// Class tha factory a dropdown itens needed to build the dropdownlist
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DropDown<T>
    {
        /// <summary>
        /// get or set the object used to search
        /// </summary>
        object Object { get; set; }
        /// <summary>
        /// get or set the key property used to return the value
        /// </summary>
        PropertyInfo KeyProperty { get; set; }
        /// <summary>
        /// get or set the text property used to return the text
        /// </summary>
        PropertyInfo TextProperty { get; set; }
        /// <summary>
        /// get or set the text property used to return the text, when the object contains more than one string item
        /// </summary>
        ForeignKeyDescriptionAttribute DescriptionAttribute { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="value">target object</param>
        public DropDown(object value)
        {
            Object = value;
            // retrive the key property
            KeyProperty = Object.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(KeyAttribute), false).Any()).FirstOrDefault();
            // check if the object contains more than one string property, if true, get the property marked on the ForeignKeyDescriptionAttribute
            value.GetType().GetProperties().Where(w => w.PropertyType.IsGenericType);
            var stringProperties = value.GetType().GetProperties().Where(w => w.PropertyType == typeof(string));
            if (stringProperties.Count() == 1)
                TextProperty = stringProperties.First();
            else
                DescriptionAttribute = value.GetType().GetCustomAttributes(typeof(ForeignKeyDescriptionAttribute), false).Cast<ForeignKeyDescriptionAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Return a list of SelectListItem
        /// </summary>
        /// <returns>IEnumerable of SelectListItem</returns>
        public IEnumerable<SelectListItem> ToList()
        {
            foreach (var item in ((IVenomObject)this.Object).FindItensAsMe())
            {
                yield return CreateSelectListItem(item);
            }
        }

        /// <summary>
        /// Return a list of SelectListItem with a search filter
        /// </summary>
        /// <param name="datasource">filter</param>
        /// <returns>IEnumerable of SelectListItem</returns>
        public IEnumerable<SelectListItem> ToList<T>(T datasource)
        {
            foreach (var item in ((IVenomObject)datasource).FindItensAsMe())
            {
                yield return CreateSelectListItem(item);
            }
        }

        /// <summary>
        /// Create a selectlistitem
        /// </summary>
        /// <typeparam name="TProperty">the type of the item</typeparam>
        /// <param name="item">the current item on search</param>
        /// <returns>a instance of SelectListItem</returns>
        private SelectListItem CreateSelectListItem<TProperty>(TProperty item)
        {
            return new SelectListItem
            {
                Value = GetValue(item),
                Text = GetText(item)
            };
        }

        /// <summary>
        /// Get the text based on the only one string property, or, the property marked on the ForeignKeyDescriptionAttribute
        /// </summary>
        /// <param name="value">the current object search</param>
        /// <returns>the text value</returns>
        private string GetText(object value)
        {
            if (TextProperty == null && DescriptionAttribute == null)
                throw new InvalidOperationException("The property class type doesn't have a ForeignKeyDescriptionAttribute to define a description");
            return (TextProperty == null ? Object.GetType().GetProperty(DescriptionAttribute.Name).GetValue(value) : TextProperty.GetValue(value)).ToString();
        }

        /// <summary>
        /// Get the text based on the key property
        /// </summary>
        /// <param name="value">the current object search</param>
        /// <returns>the string value</returns>
        private string GetValue(object item)
        {
            return (KeyProperty.GetValue(item) ?? string.Empty).ToString();
        }
    }
}
