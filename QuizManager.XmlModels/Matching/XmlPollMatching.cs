using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollMatching : XmlMatching
    {
        public override bool Create(IEnumerable<string> questions, IEnumerable<string> options, bool[][] answers, XmlQuestionType type)
        {
            MatchingType = type;

            return _Initialize(questions, options);
        }
    }
}
