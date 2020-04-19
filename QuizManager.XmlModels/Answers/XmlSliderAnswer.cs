using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlSliderAnswer : XmlAnswer<double>
    {
        public override bool IsValid()
        {
            if(Answer == -1)
            {
                return false;
            }
            return true;
        }

        public override void ParseAnswer(List<string> values)
        {
            if(values == null)
            {
                Answer = -1;
                return;
            }

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
