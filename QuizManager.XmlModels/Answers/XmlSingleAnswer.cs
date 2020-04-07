using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlSingleAnswer : XmlAnswer<int>
    {
        public override void ParseAnswer(List<string> values)
        {
            if(values.Count == 1 && Int32.TryParse(values[0], out int val))
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
