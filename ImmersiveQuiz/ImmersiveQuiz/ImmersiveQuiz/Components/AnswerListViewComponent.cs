using ImmersiveQuiz.Data;
using ImmersiveQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImmersiveQuiz.Components
{
    public class AnswerListViewComponent : ViewComponent
    {
        private readonly QuestionContext _questionContext;
        private readonly AnswerContext _answerContext;
        
        public AnswerListViewComponent(QuestionContext questionContext, AnswerContext answerContext)
        {
            _questionContext = questionContext;
            _answerContext = answerContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string searchQuestionId, string searchAnswerContent)
        {
            IQueryable<int> questionQuery = from question in _questionContext.Question
                                            orderby question.QuestionId
                                            select question.QuestionId;

            var answers = from m in _answerContext.Answer
                            select m;

            if (!string.IsNullOrEmpty(searchAnswerContent))
            {
                answers = answers.Where(s => s.Content.Contains(searchAnswerContent));
            }

            if (!string.IsNullOrEmpty(searchQuestionId))
            {
                answers = answers.Where(x => x.QuestionId == int.Parse(searchQuestionId));
            }

            var vm = new QuestionAnswerViewModel
            {
                SearchQuestionId = searchQuestionId,
                SearchAnswerContent = searchAnswerContent,
                Questions = new SelectList(await questionQuery.Distinct().ToListAsync()),
                Answers = await answers.ToListAsync()
            };

            return View(vm);
        }
    }
}
