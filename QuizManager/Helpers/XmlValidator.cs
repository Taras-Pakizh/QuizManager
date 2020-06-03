using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizManager.XmlModels;
using QuizManager.Helpers.Models;

namespace QuizManager.Helpers
{
    public static class XmlValidator
    {
        public static List<string> ErrorList { get; private set; } = new List<string>();

        private static bool _IsValid = true;

        private static bool IsValid
        {
            get { return _IsValid; }
            set
            {
                if (value)
                {
                    ErrorList.Clear();
                }
                _IsValid = value;
            }
        }

        private static void AddError(this List<string> list, string item)
        {
            XmlValidator.IsValid = false;

            list.Add(item);
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var r = new Random();

            return list.OrderBy(x => r.Next()).ToList();
        }

        public static bool Validate(XmlTestList model)
        {
            IsValid = true;

            if (model.ListType == null)
            {
                ErrorList.AddError("ListType isn't initialized");
            }
            
            if (model.Options == null || model.Options.Count == 0)
            {
                ErrorList.AddError("Options isn't initialized");
            }
            else
            {
                if (!model.Options.Any(x => x.IsTrue))
                {
                    ErrorList.AddError("Should be at least 1 correct option");
                }

                if (model.Options.Any(x => x.Text == null || x.Text == ""))
                {
                    ErrorList.AddError("Some Options are not initialized");
                }
            }

            return IsValid;
        }

        public static bool Validate(XmlOrder model)
        {
            IsValid = true;

            if(model.Options == null || 
                model.Options.Count() == 0 || 
                model.Options.Count() < 2)
            {
                ErrorList.AddError("Should be at least 2 options to order");
            }
            else
            {
                if(model.Options.Any(x=>x.Text == null || x.Text == ""))
                {
                    ErrorList.AddError("Some Options are not initialized");
                }
            }

            return IsValid;
        }

        public static bool Validate(XmlMatching model)
        {
            IsValid = true;

            if (model.MatchingType == null)
            {
                ErrorList.AddError("MatchingType isn't initialized");
            }

            if (model.Rows == null || model.Rows.Count == 0 ||
                model.Columns == null || model.Columns.Count == 0)
            {
                ErrorList.AddError("Should be at least 1 row and 1 column");
            }
            else
            {
                if (model.MatchingType == XmlMatchingType.Multy && 
                    model.Answers.Count < model.Rows.Count)
                {
                    ErrorList.AddError("Not correct count of answers");
                }

                if(model.MatchingType == XmlMatchingType.Single && 
                    model.Rows.Count > model.Columns.Count)
                {
                    ErrorList.AddError("Not correct count of options");
                }
            }

            return IsValid;
        }

        public static bool Validate(XmlTextInput model)
        {
            IsValid = true;

            if(model.Text == null || model.Input == null ||
                model.Text == "" || model.Input == "")
            {
                ErrorList.AddError("Question is not initialized");
            }
            
            return IsValid;
        }
    }
}