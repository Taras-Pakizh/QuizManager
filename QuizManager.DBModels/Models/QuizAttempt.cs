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
    public class QuizAttempt
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Quiz Quiz { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        public double Mark { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
