using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class TestOrderRedactorView : RedactorView
    {
        public XmlOrder _XmlModel { get; set; }

        public override XmlBase Model 
        {
            get { return _XmlModel; }
            set
            {
                if (value is XmlOrder)
                {
                    _XmlModel = value as XmlOrder;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            }
        }
    }
}