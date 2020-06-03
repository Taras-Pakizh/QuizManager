using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTextInput : XmlBase, IXmlTask, IAnswerName
    {
        public string Text { get; set; }

        public string Input { get; set; }

        public string Comment { get; set; }

        public double Compare(XmlBase answer, double Value)
        {
            var _answer = answer as XmlTextInputAnswer;

            if(_answer == null)
            {
                throw new Exception("Wrong answer type");
            }

            if(Input == _answer.Answer)
            {
                return Value;
            }

            return 0;
        }

        public string GetTypeName()
        {
            return (typeof(XmlTextInputAnswer)).Name;
        }
    }
}
