using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImmersiveQuiz.Models
{
    public class QuestionLocationViewModel
    {
        public List<Question> Questions { get; set; }

        public SelectList Locations { get; set; }
        
        public string LocationId { get; set; }
        
        public string Search { get; set; }
    }
}
