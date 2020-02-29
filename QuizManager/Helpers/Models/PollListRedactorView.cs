using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.Helpers.Models
{
    public class PollListRedactorView : RedactorView
    {
        public XmlPollList _XmlModel { get; set; }

        public override XmlBase Model 
        {
            get { return _XmlModel; } 
            set
            {
                if(value is XmlPollList)
                {
                    _XmlModel = value as XmlPollList;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            }
        }

        public override string View 
        {
            get { return this.GetType().Name; } 
        }
    }
}