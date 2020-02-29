using QuizManager.XmlModels.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTF:XmlBase, IXmlTask<bool>, IAnswerName
    {
        public bool Answer { get; set; }

        public object Compare(XmlAnswer<bool> answer)
        {
            if(Answer == answer.Answer)
            {
                return true;
            }

            return false;
        }

        public string GetTypeName()
        {
            return (typeof(XmlTFAnswer)).Name;
        }
    }
}
