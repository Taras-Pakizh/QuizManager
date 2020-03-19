﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizManager.XmlModels.Answers
{
    [Serializable]
    public class XmlMatchingMultyAnswer : XmlAnswer<int[][]>
    {
        public List<XmlMultyAnswer> Answers
        {
            get
            {
                return Answer.Select(x => new XmlMultyAnswer() 
                {
                    Answer = x 
                }).ToList();
            }
        }
    }
}
