using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlMultyAnswer : XmlAnswer<int[]>
    {
        public override void ParseAnswer(List<string> values)
        {
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
