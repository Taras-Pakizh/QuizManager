using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.DBModels
{
    public class Quiz
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string Name { get; set; }

        public QuizType Type { get; set; }

        public UserDataType UserData { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? TimeLimit { get; set; }

        public int QuestionCount { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public Quiz()
        {
            Groups = new List<Group>();
        }
    }
}
