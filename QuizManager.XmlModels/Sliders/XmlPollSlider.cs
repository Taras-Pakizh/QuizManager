using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public abstract class XmlPollSlider : XmlBase, IAnswerName
    {
        public XmlPollSlider() { }

        public double LeftLimit { get; set; }

        public double RightLimit { get; set; }

        public string LeftText { get; set; }

        public string RightText { get; set; }

        public string GetTypeName()
        {
            return (typeof(XmlSliderAnswer)).Name;
        }
    }
}
