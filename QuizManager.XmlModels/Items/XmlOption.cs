using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlOption : XmlBase
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            var value = obj as XmlOption;

            if (value == null)
            {
                return false;
            }

            var result = Text.CompareTo(value.Text);

            if (result == 0)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}
