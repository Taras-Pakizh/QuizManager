using QuizManager.XmlModels.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public abstract class XmlSlider : XmlBase, IAnswerName
    {
        public XmlSlider() { }

        public int LeftLimit { get; set; }

        public int RightLimit { get; set; }

        public string LeftText { get; set; }

        public string RightText { get; set; }

        public string GetTypeName()
        {
            return (typeof(XmlSliderAnswer)).Name;
        }
    }
}
