using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class TestTextInputRedactorView : RedactorView
    {
        public XmlTextInput _XmlModel { get; set; }

        public override XmlBase Model 
        { 
            get { return _XmlModel; }
            set
            {
                if (value is XmlTextInput)
                {
                    _XmlModel = value as XmlTextInput;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            } 
        }
    }
}