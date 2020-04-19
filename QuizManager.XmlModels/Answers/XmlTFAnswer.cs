using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTFAnswer : XmlAnswer<bool?>
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
