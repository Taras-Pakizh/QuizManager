using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlTFAnswer : XmlAnswer<bool>
    {
        public override void ParseAnswer(List<string> values)
        {
            if(values.Count == 1 && Boolean.TryParse(values[0], out bool val))
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
