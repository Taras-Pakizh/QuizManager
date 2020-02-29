using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestOption : XmlOption
    {
        public bool IsTrue { get; set; }
    }
}
