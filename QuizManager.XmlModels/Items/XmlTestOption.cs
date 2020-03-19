using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlTestOption : XmlBase
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool IsTrue { get; set; }
    }
}
