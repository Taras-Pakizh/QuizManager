using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizManager.Helpers
{
    public static class StringExtentions
    {
        public static bool IsNullOrEmptry(this string obj)
        {
            if(obj == null || obj == "")
            {
                return true;
            }
            return false;
        }
    }
}