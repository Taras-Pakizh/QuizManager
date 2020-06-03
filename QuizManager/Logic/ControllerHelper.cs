using QuizManager.DBModels;
using QuizManager.Helpers;
using QuizManager.Helpers.Models;
using QuizManager.ModelViews;
using QuizManager.XmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Logic
{
    public class ControllerHelper
    {
        private QuizContext cx;

        public ControllerHelper(QuizContext context)
        {
            cx = context;
        }

        public List<TestView> CreateTestViews(IEnumerable<Question> questions, Quiz quiz)
        {
            var testViews = new List<TestView>();

            foreach (var question in questions)
            {
                var testView = new TestView()
                {
                    Quiz = quiz,
                    Question = question,
                    Model = XmlBase.Deserialize(question.XmlObject, question.TypeName)
                };

                testViews.Add(testView);
            }

            return testViews;
        }
        public TestView CreateTestView(Question question)
        {
            var testView = new TestView()
            {
                Quiz = question.Quiz,
                Question = question,
                Model = XmlBase.Deserialize(question.XmlObject, question.TypeName)
            };

            return testView;
        }


        public double ExamineQuiz(Quiz quiz, IEnumerable<Answer> answers)
        {
            var scoredPoints = answers.Sum(x => x.Mark);

            double maxPoints = 0;

            if(quiz.Type == QuizType.Adaptive)
            {
                maxPoints = answers.Select(x =>
                {
                    var section = x.Question.Section;

                    return x.Question.Value * section.Difficulty;
                }).Sum();
            }
            else
            {
                maxPoints = answers.Select(x => x.Question.Value).Sum();
            }

            return Math.Round(quiz.Value * (scoredPoints / maxPoints), 2);
        }

        public Answer ExamineQuestion(Question question, XmlBase xmlAnswer)
        {
            var answer = new Answer()
            {
                Question = question,
                XmlObject = XmlBase.SerializeAbstract(xmlAnswer),
            };

            XmlBase questionXml = XmlBase.Deserialize(question.XmlObject, question.TypeName);

            double mark = 0;

            if (((IParseAnswer)xmlAnswer).IsValid())
            {
                mark = ((IXmlTask)questionXml).Compare(xmlAnswer, question.Value);
            }

            var quiz = question.Quiz;

            if(quiz.Type == QuizType.Adaptive)
            {
                var section = question.Section;

                answer.Mark = mark * section.Difficulty;
            }
            else
            {
                answer.Mark = mark;
            }

            answer.TypeName = ((IAnswerName)questionXml).GetTypeName();

            return answer;
        }

        /// <summary>
        /// Returns next section for adaptive quiz
        /// for first section get medium
        /// </summary>
        public Section GetSection_Adaptive(Quiz quiz, Section prevSection, TestSave testSave, out bool isFinish, bool isFirst = false)
        {
            isFinish = false;

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).
                    OrderBy(y => y.Difficulty).ToList();

            if (isFirst)
            {
                return sections[sections.Count / 2];
            }
            else
            {
                var passedSectionIds = testSave.Saves.Select(x => x.Key).ToList();

                passedSectionIds.Remove(prevSection.Id);

                sections.RemoveAll(x => passedSectionIds.Contains(x.Id));
            }

            var sectionSave = testSave.Saves[prevSection.Id];

            var answers = new List<Answer>();

            foreach(var questionIdAnswers in sectionSave.Answers)
            {
                var answer = ExamineQuestion(cx.Questions.Find(questionIdAnswers.Key), 
                    questionIdAnswers.Value);

                answers.Add(answer);
            }

            double maxPoints = answers.Sum(x => x.Question.Value) * prevSection.Difficulty;

            double scoredPoints = answers.Sum(x => x.Mark) * prevSection.Difficulty;

            var coef = scoredPoints / maxPoints;

            var passedSections = testSave.Saves.Select(x => cx.Sections.Find(x.Key)).ToList();

            passedSections.RemoveAll(x => x.Id == prevSection.Id);

            var section = Adaptive_DesitionMaker(coef, prevSection, sections, passedSections);

            if(section == null)
            {
                isFinish = true;
            }

            return section;
        }
        public Question GetQuestion_Adaptive(Quiz quiz, Question prevQuestion, TestSave testSave, out bool isFinish, bool isFirst = false)
        {
            isFinish = false;

            var availableQuestions = cx.Questions.Where(x => x.Quiz.Id == quiz.Id).
                OrderBy(y => y.Difficulty).ToList();

            if (isFirst)
            {
                return availableQuestions[availableQuestions.Count / 2];
            }
            else
            {
                var passedQuestionIds = testSave.QuestionSaves.Select(x => x.Key).ToList();

                availableQuestions.RemoveAll(x => passedQuestionIds.Contains(x.Id));
            }

            var questionSave = testSave.QuestionSaves[prevQuestion.Id];

            var answer = ExamineQuestion(
                cx.Questions.Find(prevQuestion.Id),
                questionSave.Answer);

            var coef = answer.Mark / answer.Question.Value;

            var passedQuestions = testSave.QuestionSaves.Select(x => cx.Questions.Find(x.Key)).ToList();

            passedQuestions.RemoveAll(x => x.Id == prevQuestion.Id);

            var question = Adaptive_DesitionMaker(coef, prevQuestion, availableQuestions, passedQuestions);

            if (question == null)
            {
                isFinish = true;
            }

            return question;
        }

        /// <summary>
        /// Returns null if end
        /// </summary>
        public Section Adaptive_DesitionMaker(double coef, Section prevSection, List<Section> availableSections, List<Section> passedSections)
        {
            availableSections = availableSections.OrderBy(x => x.Difficulty).ToList();

            if(availableSections.Count == 1)
            {
                return null;
            }

            if(coef >= 0.75)
            {
                return availableSections.FirstOrDefault(x => x.Difficulty > prevSection.Difficulty);
            }
            else if(coef >= 0.5)
            {
                if(passedSections.Any(x=>x.Difficulty > prevSection.Difficulty))
                {
                    return null;
                }

                return availableSections.FirstOrDefault(x => x.Difficulty > prevSection.Difficulty);
            }
            else if(coef >= 0.25)
            {
                if(passedSections.Any(x=>x.Difficulty < prevSection.Difficulty))
                {
                    return null;
                }

                return availableSections.LastOrDefault(x => x.Difficulty < prevSection.Difficulty);
            }
            else
            {
                return availableSections.LastOrDefault(x => x.Difficulty < prevSection.Difficulty);
            }
        }
        public Question Adaptive_DesitionMaker(double coef, Question prevQuestion, List<Question> availableQuestions, List<Question> passedQuestions)
        {
            availableQuestions = availableQuestions.
                OrderBy(x => x.Difficulty).ToList();

            //because available not contains prevQuestion
            if (availableQuestions.Count == 0)
            {
                return null;
            }

            if (coef >= 0.75)
            {
                return availableQuestions.FirstOrDefault(x => x.Difficulty > prevQuestion.Difficulty);
            }
            else if (coef >= 0.5)
            {
                if (passedQuestions.Any(x => x.Difficulty > prevQuestion.Difficulty))
                {
                    return null;
                }

                return availableQuestions.FirstOrDefault(x => x.Difficulty > prevQuestion.Difficulty);
            }
            else if (coef >= 0.25)
            {
                if (passedQuestions.Any(x => x.Difficulty < prevQuestion.Difficulty))
                {
                    return null;
                }

                return availableQuestions.LastOrDefault(x => x.Difficulty < prevQuestion.Difficulty);
            }
            else
            {
                return availableQuestions.LastOrDefault(x => x.Difficulty < prevQuestion.Difficulty);
            }
        }

        /// <summary>
        /// Using for navigation buttons, finish button and end of time (quiz, section)
        /// </summary>
        public void ChooseAction(Section prevSection, string btnSubmit, TestSave testSave, out bool IsFinish, out Section section)
        {
            IsFinish = false;

            section = null;

            var quiz = cx.Quizzes.Find(prevSection.Quiz.Id);

            int nextOrder = 0;

            if (btnSubmit == "Next")
            {
                nextOrder = prevSection.Order + 1;
            }
            else if (btnSubmit == "Previous")
            {
                nextOrder = prevSection.Order - 1;
            }
            else if (btnSubmit == "Finish" || btnSubmit == "QuizTime")
            {
                IsFinish = true;

                return;
            }
            else if (btnSubmit == "SectionTime")
            {
                if (prevSection.Order == cx.Sections.Where(x => x.Quiz.Id == quiz.Id).Max(y => y.Order))
                {
                    IsFinish = true;
                }
                else
                {
                    nextOrder = prevSection.Order + 1;
                }
            }

            if(quiz.Type == QuizType.Adaptive)
            {
                section = this.GetSection_Adaptive(quiz, prevSection, testSave, out IsFinish);
            }
            else
            {
                section = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).Single(y => y.Order == nextOrder);
            }
        }
        public void ChooseAction(Question prevQuestion, string btnSubmit, TestSave testSave, out bool IsFinish, out Question question)
        {
            IsFinish = false;

            question = null;

            var quiz = cx.Quizzes.Find(prevQuestion.Quiz.Id);

            var prevOrder = testSave.QuestionOrders.IndexOf(prevQuestion.Id);

            int nextOrder = prevOrder;

            if (btnSubmit == "Next")
            {
                nextOrder++;
            }
            else if (btnSubmit == "Previous")
            {
                nextOrder--;
            }
            else if (btnSubmit == "Finish" || btnSubmit == "QuizTime")
            {
                IsFinish = true;

                return;
            }
            else if (btnSubmit == "QuestionTime")
            {
                if(prevOrder + 1 == testSave.QuestionOrders.Count)
                {
                    IsFinish = true;

                    return;
                }
                else
                {
                    nextOrder++;
                }
            }
            else if(Int32.TryParse(btnSubmit, out int questionId))
            {
                nextOrder = testSave.QuestionOrders.IndexOf(questionId);
            }
            
            if (quiz.Type == QuizType.Adaptive)
            {
                question = this.GetQuestion_Adaptive(
                    quiz, prevQuestion, testSave, out IsFinish);
            }
            else
            {
                question = cx.Questions.Find(
                    testSave.QuestionOrders[nextOrder]);
            }
        }

        public SectionView SetNavigation(SectionView model, Section section, Quiz quiz)
        {
            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).OrderBy(y => y.Order).ToList();

            if (section.Order == sections.Max(x => x.Order))
            {
                model.IsFinish = true;
                if (sections.Count != 1)
                {
                    model.PrevVisibility = true;
                }
            }
            else
            {
                model.PrevVisibility = true;
                model.NextVisibility = true;
                if (section.Order == sections.Min(x => x.Order))
                {
                    model.PrevVisibility = false;
                }
            }

            if(quiz.Type == QuizType.Adaptive)
            {
                model.PrevVisibility = false;

                model.IsFinish = false;

                model.NextVisibility = true;
            }

            if(quiz.TimeLimitType == QuizTimeLimitType.SectionLimited)
            {
                model.PrevVisibility = false;
            }

            return model;
        }
        public PerQuestionView SetNavigation(PerQuestionView model, Question question, TestSave testSave)
        {
            var quiz = question.Quiz;

            if (testSave.QuestionOrders.IndexOf(question.Id) == 
                testSave.QuestionOrders.Count - 1)
            {
                model.IsFinish = true;
                if (testSave.QuestionOrders.Count > 1)
                {
                    model.PrevVisibility = true;
                }
            }
            else
            {
                model.PrevVisibility = true;
                model.NextVisibility = true;
                if (testSave.QuestionOrders.IndexOf(question.Id) == 0)
                {
                    model.PrevVisibility = false;
                }
            }

            if (quiz.Type == QuizType.Adaptive)
            {
                model.PrevVisibility = false;

                model.IsFinish = false;

                model.NextVisibility = true;
            }

            if (quiz.TimeLimitType == QuizTimeLimitType.QuestionLimited)
            {
                model.PrevVisibility = false;
            }

            return model;
        }

        public bool IsQuizValid(int quizId, out List<string> Errors)
        {
            var isValid = true;

            Errors = new List<string>();

            var quiz = cx.Quizzes.Find(quizId);

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            foreach(var section in sections)
            {
                var isSectionValid = IsSectionValid(section.Id, out List<string> sectionErrors);

                if (!isSectionValid)
                {
                    isValid = isSectionValid;

                    Errors.AddRange(sectionErrors);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validate section:
        /// Count of question - fix count
        /// is all questions is finished
        /// </summary>
        public bool IsSectionValid(int sectionId, out List<string> Errors)
        {
            var result = true;

            var section = cx.Sections.Find(sectionId);

            Errors = new List<string>();

            var questions = cx.Questions.Where(x => x.Section.Id == section.Id).ToList();

            if(questions.Count(x=>x.Obligation == QuestionObligation.Fixed) > section.QuestionCount)
            {
                result = false;

                Errors.Add("Section № " + section.Order + ". Number of fixed questions is bigger than whole section question count");
            }

            if (questions.Count() < section.QuestionCount)
            {
                result = false;

                Errors.Add("Section № " + section.Order + ". Section has not enough questions.");
            }

            foreach (var question in questions.OrderBy(x=>x.OrderNumber))
            {
                if(question.XmlValue == null || question.Text == null)
                {
                    result = false;

                    Errors.Add("Section № " + section.Order + ". Question № " + question.OrderNumber + " not finished");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns randomly generated questions of sections
        /// </summary>
        public List<Question> GetRandomSectionQuestions(int sectionId)
        {
            var section = cx.Sections.Find(sectionId);

            var questions = cx.Questions.Where(x => x.Section.Id == section.Id).ToList();

            var fixedQuestions = questions.Where(x => x.Obligation == QuestionObligation.Fixed).ToList();

            var randomQuestions = questions.Where(x => x.Obligation == QuestionObligation.Random).ToList();

            var randomCount = section.QuestionCount - fixedQuestions.Count;

            var selectedRandom = new List<Question>();

            var random = new Random();

            for(int i = 0; i < randomCount; ++i)
            {
                var index = random.Next(0, randomQuestions.Count - 1);

                selectedRandom.Add(randomQuestions[index]);

                randomQuestions.RemoveAt(index);
            }

            fixedQuestions.AddRange(selectedRandom);

            return fixedQuestions;
        }

        public List<Question> GetRandomQuizQuestions(int quizId)
        {
            var quiz = cx.Quizzes.Find(quizId);

            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            var result = new List<Question>();

            foreach(var section in sections)
            {
                result.AddRange(GetRandomSectionQuestions(section.Id));
            }

            return result;
        }

        /// <summary>
        /// to initialize all test (need for time limit save)
        /// Not using for Adaptive quiz
        /// </summary>
        public TestSave CreateWholeTest(Quiz quiz)
        {
            var sections = cx.Sections.Where(x => x.Quiz.Id == quiz.Id).ToList();

            if (quiz.TestingType == QuizTestingType.PerSection)
            {
                var views = new List<SectionView>();

                foreach (var section in sections)
                {
                    var view = new SectionView()
                    {
                        QuizId = quiz.Id,
                        SectionId = section.Id,
                        Section = section,
                        SectionData = new List<AnswerBaseView>(),
                    };

                    var questions = GetRandomSectionQuestions(section.Id);

                    view.Tests = CreateTestViews(questions, quiz);

                    views.Add(view);
                }

                return new TestSave(cx, views);
            }
            else if(quiz.TestingType == QuizTestingType.PerQuestion)
            {
                var views = new List<PerQuestionView>();

                foreach(var section in sections)
                {
                    var questions = GetRandomSectionQuestions(section.Id);

                    foreach(var question in questions)
                    {
                        var view = new PerQuestionView()
                        {
                            Question = question,
                            Test = CreateTestView(question),
                            QuestionData = new AnswerBaseView(),
                        };

                        views.Add(view);
                    }
                }

                return new TestSave(cx, views);
            }

            //Are you added new enum?
            throw new NotImplementedException();
        }


        public string LinkGenerator()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "A");
            GuidString = GuidString.Replace("+", "B");
            GuidString = GuidString.Replace("/", "C");

            return GuidString;
        }

        
        public RedactorView SaveXmlQuestion(RedactorView view)
        {
            var question = cx.Questions.Find(view.Question.Id);

            question.Text = view.Question.Text;
            question.Value = view.Question.Value;
            question.TypeName = view.Model.GetType().Name;
            question.XmlObject = XmlBase.SerializeAbstract(view.Model);

            cx.SaveChanges();

            view.Question = question;
            view.Quiz = question.Quiz;
            view.Section = question.Section;

            return view;
        }
    }
}