using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlMatchingSingleAnswer : XmlAnswer<int[]>
    {
        public List<XmlSingleAnswer> Answers
        {
            get
            {
                return Answer.Select(x => new XmlSingleAnswer() { Answer = x }).ToList();
            }
        }
    }
}
