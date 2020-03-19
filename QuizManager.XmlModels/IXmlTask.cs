using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    public interface IXmlTask
    {
        double Compare(XmlBase answer, double Value);
    }
}
