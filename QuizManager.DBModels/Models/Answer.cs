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
    public class Answer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual QuizAttempt Attempt { get; set; }

        public virtual Question Question { get; set; }

        public double Mark { get; set; }

        [Column(TypeName = "xml")]
        public string XmlAnswer { get; set; }

        [NotMapped]
        public XElement XmlObject
        {
            get { return XElement.Parse(XmlAnswer); }

            set { XmlAnswer = value.ToString(); }
        }
    }
}
