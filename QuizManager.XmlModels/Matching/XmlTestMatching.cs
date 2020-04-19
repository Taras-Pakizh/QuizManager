using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestMatching : XmlMatching<XmlTestList>, IXmlTask
    {
        public double Compare(XmlBase answer, double Value)
        {
            if (MatchingType == null)
            {
                throw new Exception("Matching isn't initialized");
            }

            if (MatchingType == XmlQuestionType.Checkbox)
            {
                var converted = answer as XmlMatchingMultyAnswer;

                double sum = 0;

                foreach(var item in converted.Answers.Zip
                    (Questions, (_answer, question) => new
                {
                    Answer = _answer,
                    Question = question
                }))
                {
                    sum += item.Question.Compare(item.Answer, Value / Questions.Count);
                }

                return sum;
            }
            else
            {
                var converted = answer as XmlMatchingSingleAnswer;

                double sum = 0;

                foreach (var item in converted.Answers.Zip
                    (Questions, (_answer, question) => new
                    {
                        Answer = _answer,
                        Question = question
                    }))
                {
                    sum += item.Question.Compare(item.Answer, Value / Questions.Count);
                }

                return sum;
            }
        }
    }
}
