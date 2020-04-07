using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlMatchingMultyAnswer : XmlAnswer<int[][]>
    {
        public List<XmlMultyAnswer> Answers
        {
            get
            {
                return Answer.Select(x => new XmlMultyAnswer() 
                {
                    Answer = x 
                }).ToList();
            }
        }

        /// <summary>
        /// if next val is equal or samller than previous, next question
        /// </summary>
        public override void ParseAnswer(List<string> values)
        {
            var result = new List<List<int>>();
            result.Add(new List<int>());

            foreach(var item in values)
            {
                if(Int32.TryParse(item, out int val))
                {
                    if(result.Last().Count != 0 && result.Last().Last() >= val)
                    {
                        result.Add(new List<int>());
                    }
                    result.Last().Add(val);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            Answer = result.Select(x => x.ToArray()).ToArray();
        }
    }
}
