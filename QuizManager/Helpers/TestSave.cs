using QuizManager.DBModels;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers
{
    public class TestSave
    {
        public readonly string Id;

        private QuizContext _cx { get; set; }

        private string _GenerateId()
        {
            Guid g = Guid.NewGuid();
            return Convert.ToBase64String(g.ToByteArray());
        }

        public TestSave(QuizContext context)
        {
            _cx = context;

            Id = _GenerateId();
        }

        /// <summary>
        /// For default initializing
        /// </summary>
        public TestSave(QuizContext context, List<SectionView> sectionViews)
        {
            _cx = context;

            Id = _GenerateId();

            foreach(var item in sectionViews)
            {
                var sectionSave = new SectionSave()
                {
                    View = item,
                    Answers = new Dictionary<int, XmlModels.XmlBase>()
                };

                foreach(var test in item.Tests)
                {
                    var answerName = ((IAnswerName)test.Model).GetTypeName();

                    var answerInstance = (XmlBase)Activator.CreateInstance(XmlBase.GetType(answerName));

                    sectionSave.Answers.Add(test.Question.Id, answerInstance);
                }

                Saves.Add(item.Section.Id, sectionSave);
            }
        }

        public TestSave(QuizContext context, List<PerQuestionView> questionViews)
        {
            _cx = context;

            Id = _GenerateId();

            foreach(var item in questionViews)
            {
                var answerName = ((IAnswerName)item.Test.Model).GetTypeName();

                var answerInstance = (XmlBase)Activator.CreateInstance(XmlBase.GetType(answerName));

                var emptySave = new QuestionSave()
                {
                    View = item,
                    QuestionId = item.Question.Id,
                    Answer = answerInstance
                };

                QuestionSaves.Add(item.Question.Id, emptySave);
            }

            QuestionOrders = questionViews.Select(x => new
            {
                QuestionId = x.Question.Id,
                SectionOrder = x.Question.Section.Order,
                QuestionOrder = x.Question.OrderNumber
            }).ToList().
            OrderBy(y => y.SectionOrder + "_" + y.QuestionOrder)
            .ToList().Select(z => z.QuestionId).ToList();

            for(int i = 0; i < QuestionOrders.Count; ++i)
            {
                var item = questionViews.Single(x => x.Question.Id == QuestionOrders[i]);

                item.QuestionOrder = i;
            }
        }

        /// <summary>
        /// SectionId - SectionSave
        /// </summary>
        public Dictionary<int, SectionSave> Saves { get; set; } 
            = new Dictionary<int, SectionSave>();

        /// <summary>
        /// Question.Id - QuestionSave
        /// </summary>
        public Dictionary<int, QuestionSave> QuestionSaves { get; set; }
            = new Dictionary<int, QuestionSave>();

        /// <summary>
        /// Question.Id - order
        /// </summary>
        public List<int> QuestionOrders { get; set; }
            = new List<int>();


        public void Update(SectionView view)
        {
            if (Saves.Keys.Contains(view.SectionId))
            {
                Saves.Remove(view.SectionId);
            }

            Saves.Add(view.SectionId, new SectionSave(view, _cx));
        }

        public void Update(PerQuestionView view)
        {
            if (QuestionSaves.Keys.Contains(view.Question.Id))
            {
                QuestionSaves.Remove(view.Question.Id);
            }

            QuestionSaves.Add(view.Question.Id, new QuestionSave(view, _cx));
        }
    }
}