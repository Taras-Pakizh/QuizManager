using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public class GroupAllowance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual Group Group { get; set; }

        public ReferenceType Type { get; set; }

        public int AttemptCount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }
    }
}
