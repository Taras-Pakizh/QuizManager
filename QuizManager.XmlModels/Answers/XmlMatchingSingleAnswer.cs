using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlMatchingSingleAnswer : XmlAnswer<int[]>
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

            var result = new List<int>();

            foreach(var item in values)
            {
                if(Int32.TryParse(item, out int val))
                {
                    result.Add(val);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            Answer = result.ToArray();
        }
    }
}
