using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizManager.XmlModels.Answers;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollList : XmlList<XmlOption>
    {
        public override bool Create(IEnumerable<XmlOption> options, XmlQuestionType listType)
        {
            ListType = listType;

            return _Initialize(options);
        }
    }
}
