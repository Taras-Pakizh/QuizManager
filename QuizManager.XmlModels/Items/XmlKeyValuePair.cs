using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels
{
    [Serializable]
    public class XmlKeyValuePair<K, V>:XmlBase
        where K : IComparable
        where V : IComparable
    {
        public K Key { get; set; }

        public V Value { get; set; }

        public XmlKeyValuePair() { }

        public XmlKeyValuePair(K key, V value)
        {
            Key = key;

            Value = value;
        }

        public override bool Equals(object obj)
        {
            var instance = obj as XmlKeyValuePair<K, V>;

            if(instance == null)
            {
                return false;
            }
            
            if(Key.CompareTo(instance.Key) == 0 &&
                Value.CompareTo(instance.Value) == 0)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
