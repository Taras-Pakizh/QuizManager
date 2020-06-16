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

        public bool? IsOnlyMine { get; set; }
        private Func<QuizAttempt, bool?, ApplicationUser, bool> _IsOnlyMine = (attemp, isMine, user) =>
        {
            if(isMine == true)
            {
                if (attemp.User.Id == user.Id)
                    return true;
                return false;
            }
            return true;
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
                    _MaxMarkFilter(attempt, MaxMark) &&
                    _IsOnlyMine(attempt, IsOnlyMine, CurrentUser))
            {
                return true;
            }
            return false;
        }

        public ApplicationUser CurrentUser { get; set; }

        public AttemptFilterView Filter(QuizContext cx, ApplicationUser user)
        {
            CurrentUser = user;

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

            //last has all size OR Count == 0 
            if (lastPagePushSize == 0)
            {
                pagesCount = filteringData.Count / PageSize;
                //Count == 0
                if(pagesCount == 0)
                {
                    pagesCount = 1;
                }
                else
                {
                    lastPagePushSize = PageSize;
                }
            }
            else
            {
                pagesCount = filteringData.Count / PageSize + 1;
            }

            if(Page > pagesCount)
            {
                //bag
                Page = 1;
            }

            var data = filteringData.GetRange((Page - 1) * PageSize, 
                Page == pagesCount ? lastPagePushSize : PageSize);

            //quizzes - my + which i passed
            var attempts = cx.QuizAttempts.Where(x => x.User.Id == CurrentUser.Id).ToList();
            var quizIds = attempts.Select(x => x.Quiz.Id).ToList();

            //groups - my + opened!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var openedIds = user.Groups.Select(x => x.Id).ToList();

            var result = new AttemptFilterView()
            {
                Filters = this,
                Data = data,
                Quizzes = cx.Quizzes.Where(x=>x.User.Id == user.Id ||
                        quizIds.Contains(x.Id)).ToList(),
                Groups = cx.Groups.Where(x=>x.Creator.Id == user.Id || 
                        openedIds.Contains(x.Id)),
                Users = users,
                PagesCount = pagesCount
            };
            
            return result;
        }

    }
}