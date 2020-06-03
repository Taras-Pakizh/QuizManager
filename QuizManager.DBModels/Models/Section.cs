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
    public class Section
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Quiz Quiz { get; set; }

        public int Difficulty { get; set; }

        public SectionTimeLimitType TimeLimitType { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? TimeLimit { get; set; }

        public int QuestionCount { get; set; }
    }
}
