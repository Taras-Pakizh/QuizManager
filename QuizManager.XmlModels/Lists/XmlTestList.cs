using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizManager.XmlModels.Answers;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestList : XmlList<XmlTestOption>, IXmlTask
    {
        public double Compare(XmlBase answer, double Value)
        {
            if(ListType == null)
            {
                throw new Exception("list isn't initialized");
            }

            if(this.ListType == XmlQuestionType.ComboBox)
            {
                var converted = answer as XmlMultyAnswer;

                var trueAnswers = Options.Where(x => x.IsTrue).Select(y => y.Id).ToList();

                double trueCount = 0;

                foreach(var item in converted.Answer)
                {
                    if (trueAnswers.Contains(item))
                    {
                        trueCount++;
                    }
                }

                return (Value / trueAnswers.Count) * trueCount;
            }
            else
            {
                var converted = answer as XmlSingleAnswer;

                if(converted.Answer == Options.Single(x => x.IsTrue).Id)
                {
                    return Value;
                }

                return 0;
            }
        }
    }
}
