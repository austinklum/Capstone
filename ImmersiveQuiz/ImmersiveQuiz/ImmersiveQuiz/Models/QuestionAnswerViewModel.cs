using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImmersiveQuiz.Models
{
    public class QuestionAnswerViewModel
    {
        public List<Answer> Answers { get; set; }

        public SelectList Questions { get; set; }

        public string SearchQuestionId { get; set; }

        public string SearchAnswerContent { get; set; }
    }
}
