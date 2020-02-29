using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollSlider : XmlSlider
    {
        public XmlPollSlider(string leftS, int leftI, string rightS, int rightI)
        {
            LeftLimit = leftI;

            LeftText = leftS;

            RightLimit = rightI;

            RightText = rightS;
        }
    }
}
