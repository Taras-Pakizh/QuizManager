using QuizManager.XmlModels.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    /// <summary>
    /// Не потрібно - буду використовувати масив лістів
    /// </summary>
    [Serializable]
    public abstract class XmlMatching:XmlBase, IAnswerName
    {
        public XmlQuestionType MatchingType { get; set; }

        public List<string> ErrorList { get; set; } = new List<string>();

        public HashSet<string> Questions { get; set; }

        public HashSet<string> Options { get; set; }

        public bool[][] Answers { get; set; }

        protected bool _Initialize(IEnumerable<string> questions, 
                                   IEnumerable<string> options)
        {
            Questions = new HashSet<string>();

            Options = new HashSet<string>();

            foreach (var item in options)
            {
                if (item is null || item == "")
                {
                    ErrorList.Add("Option can't be empty");

                    return false;
                }

                if (!Options.Add(item))
                {
                    ErrorList.Add("All Options must be different");

                    return false;
                }
            }

            foreach(var item in questions)
            {
                if (item is null || item == "")
                {
                    ErrorList.Add("Question can't be empty");

                    return false;
                }

                if (!Questions.Add(item))
                {
                    ErrorList.Add("All Questions must be different");

                    return false;
                }
            }

            return true;
        }

        public abstract bool Create(IEnumerable<string> questions,
                                    IEnumerable<string> options,
                                    bool[][] answers, 
                                    XmlQuestionType type);

        public string GetTypeName()
        {
            if (this.MatchingType == XmlQuestionType.ComboBox)
            {
                return (typeof(XmlMatchingSingleAnswer)).Name;
            }
            else
            {
                return (typeof(XmlMatchingMultyAnswer)).Name;
            }
        }
    }
}
