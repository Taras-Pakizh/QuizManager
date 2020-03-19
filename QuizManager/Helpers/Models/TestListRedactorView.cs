using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class TestListRedactorView : RedactorView
    {
        public XmlTestList _XmlModel { get; set; }

        public List<int> Ids { get; set; }

        public override XmlBase Model
        {
            get { return _XmlModel; }
            set
            {
                if (value is XmlTestList)
                {
                    _XmlModel = value as XmlTestList;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            }
        }
    }
}