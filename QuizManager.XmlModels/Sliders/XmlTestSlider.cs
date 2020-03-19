using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizManager.XmlModels.Answers;


namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestSlider : XmlPollSlider, IXmlTask
    {
        public double TrueValue { get; set; }

        public double Compare(XmlBase answer, double Value)
        {
            var _answer = answer as XmlSliderAnswer;

            if(_answer.Answer == TrueValue)
            {
                return Value;
            }

            return 0;
        }
    }
}
