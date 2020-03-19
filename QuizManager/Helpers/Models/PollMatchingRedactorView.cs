using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class PollMatchingRedactorView : RedactorView
    {
        public XmlPollMatching _XmlModel { get; set; }

        public override XmlBase Model
        {
            get { return _XmlModel; }
            set
            {
                if (value is XmlPollMatching)
                {
                    _XmlModel = value as XmlPollMatching;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            }
        }
    }
}