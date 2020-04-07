using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlSliderAnswer : XmlAnswer<double>
    {
        public override void ParseAnswer(List<string> values)
        {
            if(values.Count == 1 && Double.TryParse(values[0], out double val))
            {
                Answer = val;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
