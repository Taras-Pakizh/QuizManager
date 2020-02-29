using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    public interface IXmlTask<T>
    {
        object Compare(XmlAnswer<T> answer);
    }
}
