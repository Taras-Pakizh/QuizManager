using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public class QuizReference
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public virtual Quiz Quiz { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }

        public int AttemptCount { get; set; }

        public ReferenceType Type { get; set; }
    }
}
