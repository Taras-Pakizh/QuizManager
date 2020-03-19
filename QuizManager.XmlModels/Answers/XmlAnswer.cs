using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public abstract class XmlAnswer<T> : XmlBase
    {   
        public T Answer { get; set; }
    }
}
