using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class TestMatchingRedactorView : RedactorView
    {
        public XmlMatching _XmlModel { get; set; }

        public List<string> Ids { get; set; }

        public List<XmlKeyValuePair<int, int>> ParseAnswers()
        {
            var result = new List<XmlKeyValuePair<int, int>>();

            if (_XmlModel.MatchingType == XmlMatchingType.Multy)
            {
                if (Ids == null)
                {
                    return new List<XmlKeyValuePair<int, int>>();
                }

                foreach (var item in Ids)
                {
                    var ids = item.Split(new char[] { ' ' });

                    result.Add(
                        new XmlKeyValuePair<int, int>(
                            Int32.Parse(ids[0]),
                            Int32.Parse(ids[1])
                        )
                    );
                }
            }

            if(_XmlModel.MatchingType == XmlMatchingType.Single)
            {
                for(int i = 0; i < _XmlModel.Rows.Count; ++i)
                {
                    result.Add(new XmlKeyValuePair<int, int>(
                            _XmlModel.Rows[i].Id,
                            _XmlModel.Columns[i].Id
                        )
                    );
                }
            }

            return result;
        }

        public override XmlBase Model
        {
            get { return _XmlModel; }
            set
            {
                if (value is XmlMatching)
                {
                    _XmlModel = value as XmlMatching;
                }
                else { throw new Exception(this.GetType().Name + ": incorrect type"); }
            }
        }
    }
}