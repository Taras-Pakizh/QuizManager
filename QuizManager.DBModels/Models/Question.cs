using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuizManager.DBModels
{
    public class Question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Quiz Quiz { get; set; }

        public QuestionType? Type { get; set; }

        public QuestionObligation Obligation { get; set; }

        public int OrderNumber { get; set; }

        public string Text { get; set; }

        public string TypeName { get; set; }

        public double Value { get; set; }

        [Column(TypeName = "xml")]
        public string XmlValue { get; set; }

        [NotMapped]
        public XElement XmlObject
        {
            get { return XElement.Parse(XmlValue); }

            set { XmlValue = value.ToString(); }
        }
    }
}
