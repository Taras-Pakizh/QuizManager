using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizManager.DBModels;
using QuizManager.XmlModels.Answers;

namespace QuizManager.XmlModels
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class XmlList<T> : XmlBase, IAnswerName
        where T : XmlOption
    {
        public HashSet<T> Options { get; set; }

        public XmlQuestionType ListType { get; set; }

        public List<string> ErrorList { get; set; } = new List<string>();

        public XmlList()
        {
            Options = new HashSet<T>();
        }

        /// <summary>
        /// Set Options, if some option is the SAME or NULL - false
        /// </summary>
        protected bool _Initialize(IEnumerable<T> options)
        {
            Options = new HashSet<T>();

            foreach(var item in options)
            {
                if(item.Text is null || item.Text == "")
                {
                    ErrorList.Add("Option can't be empty");

                    Options = new HashSet<T>();

                    return false;
                }

                if (!Options.Add(item))
                {
                    ErrorList.Add("All Options must be different");

                    Options = new HashSet<T>();

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create XmlList based on type of quiz :
        /// (POLL, TEST, POLLR)
        /// If rule of quiz isn't same as inputed options - exception with text (for error output)
        /// </summary>
        public abstract bool Create(IEnumerable<T> options, XmlQuestionType listType);

        public string GetTypeName()
        {
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
