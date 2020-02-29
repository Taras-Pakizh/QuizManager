using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollResList : XmlList<XmlPollResOption>, 
        IXmlTask<int>, IXmlTask<int[]>
    {
        public object Compare(XmlAnswer<int> answer)
        {
            return Options.Single(x => x.Id == answer.Answer);
        }

        public object Compare(XmlAnswer<int[]> answer)
        {
            return Options.Where(x => answer.Answer.Contains(x.Id)).ToList();
        }

        public override bool Create(IEnumerable<XmlPollResOption> options, XmlQuestionType listType)
        {
            ListType = listType;

            var res = _IsInitialize(options);

            if (!res)
            {
                return false;
            }

            return _Initialize(options);
        }

        private bool _IsInitialize(IEnumerable<XmlPollResOption> options)
        {
            var IsId = options.Any(x => x.OptionId == 0);

            if (IsId)
            {
                ErrorList.Add("All options must be initialized with category");

                return false;
            }

            var IsValue = options.Any(x => x.Value == 0);

            if (IsValue)
            {
                ErrorList.Add("All Options must have value");

                return false;
            }

            return true;
        }
    }
}
