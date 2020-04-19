﻿using QuizManager.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizManager.ModelViews
{
    public class RedactorContainerView
    {
        public Quiz Quiz { get; set; }

        public Section Section { get; set; }

        public Question Question { get; set; }
    }
}