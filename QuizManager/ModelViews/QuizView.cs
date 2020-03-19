using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class QuizView
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public QuizType Type { get; set; }

        public UserDataType UserData { get; set; }

        public TimeSpan? TimeLimit { get; set; }

        public int QuestionCount { get; set; }
    }
}