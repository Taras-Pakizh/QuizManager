using QuizManager.XmlModels.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlText : XmlBase, IXmlTask<string>, IAnswerName
    {
        public string Text { get; set; }

        public object Compare(XmlAnswer<string> answer)
        {
            if(Text == answer.Answer)
            {
                return true;
            }

            return false;
        }

        public string GetTypeName()
        {
            return (typeof(XmlTextAnswer)).Name;
        }
    }
}
