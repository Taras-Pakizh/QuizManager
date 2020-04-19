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

        public static bool Validate(XmlPollList model)
        {
            IsValid = true;

            if(model.ListType == null)
            {
                ErrorList.AddError("ListType isn't initialized");
            }

            if(model.Options == null || model.Options.Count == 0)
            {
                ErrorList.AddError("Options isn't initialized");
            }

            if(model.Options.Any(x=>x.Text == null || x.Text == ""))
            {
                ErrorList.AddError("Some Options are not initialized");
            }

            if (!IsValid)
            {
                return false;
            }

            //for(int i = 0; i < model.Options.Count; ++i)
            //{
            //    model.Options[i].Id = i + 1;
            //}

            return true;
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
            
            if (!model.Options.Any(x => x.IsTrue))
            {
                ErrorList.AddError("Should be at least 1 correct option");
            }

            if (model.Options.Any(x => x.Text == null || x.Text == ""))
            {
                ErrorList.AddError("Some Options are not initialized");
            }

            if (!IsValid)
            {
                return false;
            }

            return true;
        }
    }
}