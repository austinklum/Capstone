using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImmersiveQuiz.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }

        public string Content { get; set; }

        public int QuestionId { get; set; }
    }
}
