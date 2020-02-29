using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizManager.DBModels;
using QuizManager.Helpers.Models;
using QuizManager.ModelViews;
using QuizManager.XmlModels;

namespace QuizManager.Helpers
{
    public static class RedactorHelper
    {
        //check a type of question - (Poll, PollRes, Test)
        //gets type name and XmlBase
        //for diffenert types

        public static MvcHtmlString Redactor(this HtmlHelper html, RedactorView view)
        {
            return view.View();
        }

    }
}