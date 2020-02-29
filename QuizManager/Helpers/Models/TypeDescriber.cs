using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers.Models
{
    public class TypeDescriber
    {
        public IEnumerable<QuestionType> Names { get; set; }

        public Dictionary<QuizType, Type> Types { get; set; }
    }
}