using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollResSlider :XmlSlider, IXmlTask<int>
    {
        public List<XmlSliderOption> Options { get; set; }

        public object Compare(XmlAnswer<int> answer)
        {
            return Options.Single(x => x.IsEnter(answer.Answer));
        }
    }

    [Serializable]
    public class XmlSliderOption:XmlBase
    {
        public int LeftLim { get; set; }

        public int RightLim { get; set; }

        public int Id { get; set; }

        public int Value { get; set; }

        public bool IsEnter(int value)
        {
            if(LeftLim >= value && value <= RightLim)
            {
                return true;
            }

            return false;
        }
    }
}
