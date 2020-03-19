using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class TestMatchingRedactorView : RedactorView
    {
        public XmlTestMatching _XmlModel { get; set; }

        public override XmlBase Model
        {
            get { return _XmlModel; }
            set
            {
                if (value is XmlTestMatching)
                {
                    _XmlModel = value as XmlTestMatching;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            }
        }
    }
}