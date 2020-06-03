using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Helpers
{
    public static class EnumExtention
    {
        public static IEnumerable<SelectListItem> GetSelectList(this HtmlHelper html, Type enumType, Enum value = null)
        {
            var enumValues = Enum.GetValues(enumType);

            return enumValues.OfType<Enum>().ToList().
                Select(x =>
                    new SelectListItem()
                    {
                        Selected = x.Equals(value),
                        Text = x.ToDescription(),
                        Value = x.ToString()
                    });
        }

        public static IEnumerable<SelectListItem> GetSelectList(this Enum _enum, Type enumType)
        {
            var enumValues = Enum.GetValues(enumType);

            return enumValues.OfType<Enum>().ToList().
                Select(x =>
                    new SelectListItem()
                    {
                        Selected = x.Equals(_enum),
                        Text = x.ToDescription(),
                        Value = x.ToString()
                    });
        }

        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.
                GetType().
                GetField(value.ToString()).
                GetCustomAttributes(
                    typeof(DescriptionAttribute), 
                    false
                );
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}