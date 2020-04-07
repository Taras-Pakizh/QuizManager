using QuizManager.DBModels;
using QuizManager.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers
{
    public class TestSave
    {
        private QuizContext _cx { get; set; }

        public TestSave(QuizContext context)
        {
            _cx = context;
        }

        /// <summary>
        /// SectionId - SectionSave
        /// </summary>
        public Dictionary<int, SectionSave> Saves { get; set; } 
            = new Dictionary<int, SectionSave>();

        public void Update(SectionView view)
        {
            if (Saves.Keys.Contains(view.SectionId))
            {
                Saves.Remove(view.SectionId);
            }

            Saves.Add(view.SectionId, new SectionSave(view, _cx));
        }

    }
}