using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlPollOption : XmlBase
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }
}
