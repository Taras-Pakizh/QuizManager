using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    public interface IParseAnswer
    {
        void ParseAnswer(List<string> values);
    }

    [Serializable]
    public abstract class XmlAnswer<T> : XmlBase, IParseAnswer
    {   
        public T Answer { get; set; }

        public abstract void ParseAnswer(List<string> values);
    }
}
