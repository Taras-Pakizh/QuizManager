using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollResMatching : XmlMatching, IXmlTask<int[]>, IXmlTask<int[][]>
    {
        public List<List<XmlPollResOption>> PollOptions { get; set; }

        public object Compare(XmlAnswer<int[]> answer)
        {
            throw new NotImplementedException();
        }

        public object Compare(XmlAnswer<int[][]> answer)
        {
            throw new NotImplementedException();
        }

        public override bool Create(IEnumerable<string> questions, IEnumerable<string> options, bool[][] answers, XmlQuestionType type)
        {
            MatchingType = type;

            var result = _Initialize(questions, options);

            if (!result)
            {
                return false;
            }

            if (answers.Length != Questions.Count || answers[0].Length != Options.Count)
            {
                ErrorList.Add("Index out of range");

                return false;
            }

            foreach (var row in answers)
            {
                var count = row.Count(x => x);

                if (count == 0)
                {
                    ErrorList.Add("All Questions must have answer");

                    return false;
                }

                switch (type)
                {
                    case XmlQuestionType.ComboBox:
                        break;
                    default:
                        if (count != 1)
                        {
                            ErrorList.Add("Questions must have only one answer");

                            return false;
                        }
                        break;
                }
            }

            Answers = answers;

            return true;
        }
    }
}
