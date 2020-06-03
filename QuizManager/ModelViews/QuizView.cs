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

        public QuizTestingType TestingType { get; set; }

        public TimeSpan? TimeLimit { get; set; }

        public double Value { get; set; }

        public QuizTimeLimitType TimeLimitType { get; set; }
    }
}