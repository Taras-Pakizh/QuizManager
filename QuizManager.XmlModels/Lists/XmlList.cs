using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizManager.DBModels;

namespace QuizManager.XmlModels
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class XmlList<T> : XmlBase, IAnswerName
        where T : XmlBase
    {
        public List<T> Options { get; set; }

        public XmlQuestionType? ListType { get; set; }

        public string GetTypeName()
        {
            if(ListType == null)
            {
                throw new Exception("not initialized");
            }
            if (this.ListType == XmlQuestionType.Checkbox)
            {
                return (typeof(XmlMultyAnswer)).Name;
            }
            else
            {
                return (typeof(XmlSingleAnswer)).Name;
            }
        }
    }
}
