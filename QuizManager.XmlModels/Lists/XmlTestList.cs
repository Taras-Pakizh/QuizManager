using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestList : XmlList<XmlTestOption>, IXmlTask<int>, IXmlTask<int[]>
    {
        public object Compare(XmlAnswer<int> answer)
        {
            return Options.Single(x => x.Id == answer.Answer).IsTrue;
        }

        /// <summary>
        /// Return 1(all is correct), 0(partial correct), -1(all is wrong)
        /// </summary>
        public object Compare(XmlAnswer<int[]> answer)
        {
            var list = Options.Where(x => answer.Answer.Contains(x.Id)).ToList();

            if (list.All(x => x.IsTrue))
            {
                return 1;
            }
            if (list.Any(x => x.IsTrue))
            {
                return 0;
            }
            return -1;
        }

        public override bool Create(IEnumerable<XmlTestOption> options, XmlQuestionType listType)
        {
            ListType = listType;

            var solutions = options.Count(x => x.IsTrue);

            if(solutions < 1)
            {
                ErrorList.Add("Test must contain a solution");

                return false;
            }

            switch (ListType)
            {
                case XmlQuestionType.Checkbox:
                    break;
                default:
                    if(solutions > 1)
                    {
                        ErrorList.Add("Radiobuttons and Combobox must contain only one solution");

                        return false;
                    }
                    break;
            }

            return _Initialize(options);
        }


    }
}
