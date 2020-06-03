using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlMatchingMultyAnswer : XmlAnswer<List<XmlKeyValuePair<int, int>>>
    {
        public override bool IsValid()
        {
            if(Answer == null)
            {
                return false;
            }
            return true;
        }

        //pair of values (RowId_ColId)
        public override void ParseAnswer(List<string> values)
        {
            if(values == null)
            {
                return;
            }

            var result = new List<XmlKeyValuePair<int, int>>();
            
            foreach(var item in values)
            {
                var ids = item.Split(new char[] { '_' });

                result.Add(
                    new XmlKeyValuePair<int, int>(
                        Int32.Parse(ids[0]),
                        Int32.Parse(ids[1])
                    )
                );
            }

            Answer = result;
        }
    }
}
