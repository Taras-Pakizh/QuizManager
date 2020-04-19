using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTextAnswer : XmlAnswer<string>
    {
        public override bool IsValid()
        {
            if(Answer == null)
            {
                return false;
            }
            return true;
        }

        public override void ParseAnswer(List<string> values)
        {
            if(values == null)
            {
                return;
            }

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
