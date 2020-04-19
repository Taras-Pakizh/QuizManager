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

        bool IsValid();
    }

    [Serializable]
    public abstract class XmlAnswer<T> : XmlBase, IParseAnswer
    {   
        public T Answer { get; set; }

        public abstract bool IsValid();

        public abstract void ParseAnswer(List<string> values);
    }
}
