using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlOrder : XmlBase, IAnswerName, IXmlTask
    {
        public List<XmlTestOption> Options { get; set; }

        public double Compare(XmlBase answer, double Value)
        {
            var order = answer as XmlMultyAnswer;

            if(order == null)
            {
                throw new Exception("Wrong answer type");
            }

            if(order.Answer.Length != Options.Count())
            {
                throw new Exception("Answer is initialized wrong");
            }

            var correctAnswer = Options.Select(x => x.Id).ToList();

            double eCount = 0;

            for(int i = 0; i < order.Answer.Length; ++i)
            {
                if(order.Answer[i] != correctAnswer[i])
                {
                    eCount++;
                }
            }

            return Value * (1 - (eCount / order.Answer.Length));
        }

        public string GetTypeName()
        {
            return typeof(XmlMultyAnswer).Name;
        }
    }
}
