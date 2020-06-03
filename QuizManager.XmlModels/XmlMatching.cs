using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuizManager.XmlModels
{
    /// <summary>
    /// Не потрібно - буду використовувати масив лістів
    /// </summary>
    [Serializable]
    public class XmlMatching :XmlBase, IAnswerName, IXmlTask
    {
        public XmlMatchingType? MatchingType { get; set; }

        public List<XmlMatchingOption> Rows { get; set; }

        public List<XmlMatchingOption> Columns { get; set; }

        [XmlIgnore]
        //Id of Row to Id of Column
        public List<XmlKeyValuePair<int, int>> Answers
        {
            get
            {
                return _RowIds.Zip(_ColIds,
                    (row, col) => new XmlKeyValuePair<int, int>(row, col)).ToList();
            }
            set
            {
                _RowIds = value.Select(x => x.Key).ToList();

                _ColIds = value.Select(x => x.Value).ToList();
            }
        }

        public List<int> _RowIds { get; set; }

        public List<int> _ColIds { get; set; }

        public string GetTypeName()
        {
            if(MatchingType == null)
            {
                throw new Exception("not initialized");
            }
            if (this.MatchingType == XmlMatchingType.Multy)
            {
                return (typeof(XmlMatchingMultyAnswer)).Name;
            }
            else
            {
                return (typeof(XmlMatchingSingleAnswer)).Name;
            }
        }

        public double Compare(XmlBase answer, double Value)
        {
            if (MatchingType == XmlMatchingType.Multy)
            {
                var _answer = answer as XmlMatchingMultyAnswer;

                double eCount = 0;

                foreach(var item in Answers)
                {
                    if (!_answer.Answer.Contains(item))
                    {
                        ++eCount;
                    }
                }

                foreach(var item in _answer.Answer)
                {
                    if (!Answers.Contains(item))
                    {
                        ++eCount;
                    }
                }

                if (eCount >= Answers.Count)
                    eCount = Answers.Count;

                return Value * (1 - (eCount / Answers.Count));
            }
            else if(MatchingType == XmlMatchingType.Single)
            {
                var _answer = answer as XmlMatchingSingleAnswer;

                var correctAnswers = Answers.OrderBy(x => x.Key).
                    Select(y => y.Value).ToList();

                var input = _answer.Answer.ToList().Take(Rows.Count).ToList();

                double eCount = 0;

                if(correctAnswers.Count != input.Count())
                {
                    throw new Exception("Matching - wrong option count");
                }
                
                for(int i = 0; i < correctAnswers.Count; ++i)
                {
                    if(correctAnswers[i] != input[i])
                    {
                        eCount++;
                    }
                }

                return Value * (1 - (eCount / correctAnswers.Count));
            }
            else
            {
                throw new Exception("Matching isn't initialized");
            }
        }
    }
}
