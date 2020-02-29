using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestSlider : XmlSlider, IXmlTask<int>
    {
        public int TrueValue { get; set; }

        public object Compare(XmlAnswer<int> answer)
        {
            if(answer.Answer == TrueValue)
            {
                return true;
            }

            return false;
        }
    }
}
