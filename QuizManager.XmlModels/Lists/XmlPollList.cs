using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizManager.XmlModels.Answers;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollList : XmlList<XmlPollOption>
    {
        
    }
}
