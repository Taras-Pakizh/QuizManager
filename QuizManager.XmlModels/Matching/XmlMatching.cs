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
    public abstract class XmlMatching<T>:XmlBase, IAnswerName
        where T : XmlBase
    {
        public XmlQuestionType? MatchingType { get; set; }

        public List<T> Questions { get; set; }

        public string GetTypeName()
        {
            if(MatchingType == null)
            {
                throw new Exception("not initialized");
            }
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
