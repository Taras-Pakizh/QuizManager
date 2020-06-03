using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.ModelViews
{
    public class GroupQuizView
    {
        public Group Group { get; set; }

        public Quiz SelectedQuiz { get; set; }

        public List<Quiz> Quizzes { get; set; }
    }
}