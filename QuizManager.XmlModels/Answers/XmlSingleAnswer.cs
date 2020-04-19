using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlSingleAnswer : XmlAnswer<int>
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
            if(values == null || values[0] == "")
            {
                Answer = -1;
                return;
            }
            if(values.Count != 1)
            {
                throw new InvalidCastException();
            }

            Int32.TryParse(values[0], out int val);

            Answer = val;
        }
    }
}
