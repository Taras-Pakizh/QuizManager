using QuizManager.DBModels;
using QuizManager.ModelViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizManager.Logic
{
    public class AttemptFilters
    {
        public Quiz Quiz { get; set; }
        private Func<QuizAttempt, Quiz, bool> _QuizFilter = (attemp, quiz) =>
        {
            if (quiz == null || quiz.Id == attemp.Quiz.Id) 
                return true;
            return false;
        };

        public ApplicationUser User { get; set; }
        private Func<QuizAttempt, ApplicationUser, bool> _UserFilter = (attemp, user) =>
        {
            if (user == null || user.Id == attemp.User.Id)
                return true;
            return false;
        };

        [DataType(DataType.Date)]
        public DateTime? MinLimit { get; set; }
        private Func<QuizAttempt, DateTime?, bool> _MinLimitFilter = (attemp, minLimit) =>
        {
            if (minLimit == null || minLimit <= attemp.Time)
                return true;
            return false;
        };

        [DataType(DataType.Date)]
        public DateTime? MaxLimit { get; set; }
        private Func<QuizAttempt, DateTime?, bool> _MaxLimitFilter = (attemp, maxLimit) =>
        {
            if (maxLimit == null || maxLimit >= attemp.Time)
                return true;
            return false;
        };

        public Group Group { get; set; }
        private Func<QuizAttempt, Group, bool> _GroupFilter = (attempt, group) =>
        {
            if (group == null)
                return true;
            else
            {
                if (attempt.Group == null)
                    return false;
                if (attempt.Group.Id == group.Id)
                    return true;
            }
            return false;
        };

        public double? MinMark { get; set; }
        private Func<QuizAttempt, double?, bool> _MinMarkFilter = (attemp, mark) =>
        {
            if (mark == null || attemp.Mark >= mark)
                return true;
            return false;
        };

        public double? MaxMark { get; set; }
        private Func<QuizAttempt, double?, bool> _MaxMarkFilter = (attemp, mark) =>
        {
            if (mark == null || attemp.Mark <= mark)
                return true;
            return false;
        };

        public int Page { get; set; }

        public readonly static int PageSize = 10;

        private bool _Filter(QuizAttempt attempt)
        {
            if(_QuizFilter(attempt, Quiz) &&
                    _UserFilter(attempt, User) &&
                    _MinLimitFilter(attempt, MinLimit) &&
                    _MaxLimitFilter(attempt, MaxLimit) &&
                    _GroupFilter(attempt, Group) &&
                    _MinMarkFilter(attempt, MinMark) &&
                    _MaxMarkFilter(attempt, MaxMark))
            {
                return true;
            }
            return false;
        }

        public AttemptFilterView Filter(QuizContext cx, ApplicationUser user)
        {
            var users = new HashSet<ApplicationUser>();
            foreach (var group in cx.Groups.Where(x => x.Creator.Id == user.Id).ToList())
            {
                foreach (var applicationUser in group.ApplicationUsers)
                {
                    users.Add(applicationUser);
                }
            }

            var userIds = users.Select(x => x.Id).ToList();

            var filteringData = cx.QuizAttempts.
                Where(x => x.User.Id == user.Id ||
                userIds.Contains(user.Id) || 
                x.Quiz.User.Id == user.Id).ToList();

            filteringData = filteringData.
                Where(x => _Filter(x)).
                ToList();

            int pagesCount = 0;

            int lastPagePushSize = filteringData.Count % PageSize;

            if (lastPagePushSize == 0)
            {
                pagesCount = filteringData.Count / PageSize;
                lastPagePushSize = PageSize;
                if(pagesCount == 0)
                {
                    pagesCount = 1;
                }
            }
            else
            {
                pagesCount = filteringData.Count / PageSize + 1;
            }

            var data = filteringData.GetRange((Page - 1) * PageSize, 
                Page == pagesCount ? lastPagePushSize : PageSize);

            var result = new AttemptFilterView()
            {
                Filters = this,
                Data = data,
                Quizzes = cx.Quizzes.Where(x=>x.User.Id == user.Id).ToList(),
                Groups = cx.Groups.Where(x=>x.Creator.Id == user.Id).ToList(),
                Users = users,
                PagesCount = pagesCount
            };
            
            return result;
        }

    }
}