using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlTextAnswer : XmlAnswer<string>
    {
        public override void ParseAnswer(List<string> values)
        {
            if(values.Count == 1)
            {
                Answer = values[0];
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
