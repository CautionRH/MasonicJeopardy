using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jeopordy
{
    class Category
    {

        public List<QuestionAndAnswer> questions;
        public string CategoryName;
        public Category()
        {
            questions = new List<QuestionAndAnswer>();
        }

        public void AddQuestion(string question, string answer, int value)
        {
            QuestionAndAnswer qa = new QuestionAndAnswer();
            qa.Question = question;
            qa.Answer = answer;
            qa.Value = value;
            AddQuestion(qa);
        }

        public void AddQuestion(QuestionAndAnswer qa)
        {
            questions.Add(qa);
        }
    }
}
