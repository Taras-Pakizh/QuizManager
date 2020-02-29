using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizManager.DBModels;
using QuizManager.ModelViews;
using QuizManager.XmlModels;

namespace QuizManager.Helpers
{
    public static class SquereItemHelper
    {
        public static MvcHtmlString CreateList(this HtmlHelper html, string[] items)
        {
            TagBuilder ul = new TagBuilder("ul");
            foreach (string item in items)
            {
                TagBuilder li = new TagBuilder("li");
                li.SetInnerText(item);
                ul.InnerHtml += li.ToString();
            }
            return new MvcHtmlString(ul.ToString());
        }

        private static IEnumerable<string> _aClasses = new List<string>()
        {
            "btn",
            "btn-squared-default",
            "btn-squared-default-plain:hover",
            "text-center",
            "dropdown-toggle"
        };

        private static IList<string> _button_color = new List<string>()
        {
            "btn-primary", "btn-danger", "btn-success", "btn-info", "btn-warning"
        };

        private static int _colorCounter = 0;

        private static string _GetColor
        {
            get
            {
                ++_colorCounter;

                if(_colorCounter >= _button_color.Count())
                {
                    _colorCounter = 0;
                }

                return _button_color[_colorCounter];
            }
        }

        private static TagBuilder _DrowListElement(SquereData data, int id)
        {
            var element = new TagBuilder("a");

            element.SetInnerText(data.Name);

            element.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(data.Attributes));

            element.MergeAttribute("href", data.Link + "?id=" + id);

            return element;
        }

        /// <summary>
        /// Link must be HttpGet method which has parameter int? (id)
        /// </summary>
        public static MvcHtmlString CreateSquere(this HtmlHelper helper, string name, int id, IEnumerable<SquereData> data)
        {
            var tag_a = new TagBuilder("a");

            //---------Change to colors - not classes
            tag_a.AddCssClass(_GetColor);

            foreach(var item in _aClasses)
            {
                tag_a.AddCssClass(item);
            }

            tag_a.MergeAttributes(new Dictionary<string, string>()
            {
                { "href", "#" },
                { "data-toggle", "dropdown"}
            });

            var tag_i = new TagBuilder("i");
            tag_i.AddCssClass("fa");
            tag_i.AddCssClass("fa-key");
            tag_i.AddCssClass("fa-5x");

            tag_a.InnerHtml += tag_i.ToString();
            tag_a.InnerHtml += new TagBuilder("br").ToString(TagRenderMode.SelfClosing);
            tag_a.SetInnerText(name);

            var tag_ul = new TagBuilder("ul");

            tag_ul.AddCssClass("dropdown-menu");

            foreach(var item in data)
            {
                var li = new TagBuilder("li");

                var element = _DrowListElement(item, id);

                li.InnerHtml += element.ToString();

                tag_ul.InnerHtml += li.ToString();
            }

            TagBuilder tag_div = new TagBuilder("div");

            tag_div.AddCssClass("dropdown");

            tag_div.AddCssClass("div_squere");

            tag_div.InnerHtml += tag_a.ToString();

            tag_div.InnerHtml += tag_ul.ToString();

            return new MvcHtmlString(tag_div.ToString());
        }
    }
}