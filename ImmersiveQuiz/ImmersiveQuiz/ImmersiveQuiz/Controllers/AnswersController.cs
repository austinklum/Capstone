using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImmersiveQuiz.Data;
using ImmersiveQuiz.Models;

namespace ImmersiveQuiz.Controllers
{
    public class AnswersController : Controller
    {
        private readonly AnswerContext _answerContext;
        private readonly QuestionContext _questionContext;


        public AnswersController(AnswerContext context, QuestionContext questionContext)
        {
            _answerContext = context;
            _questionContext = questionContext;
        }

        // GET: Answers
        public async Task<IActionResult> Index()
        {
            return View(await _answerContext.Answer.ToListAsync());
        }

        // GET: Answers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _answerContext.Answer
                .FirstOrDefaultAsync(m => m.AnswerId == id);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // GET: Answers/Create
        public IActionResult Create(int? questionId)
        {
            Answer answer = new Answer();

            if (questionId.HasValue)
            {
                answer.QuestionId = questionId.Value;
            }

            return View(answer);
        }

        // POST: Answers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnswerId,Content,QuestionId")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                _answerContext.Add(answer);
                await _answerContext.SaveChangesAsync();

                Question question = _questionContext.Question.First(q => q.QuestionId == answer.QuestionId);
                return RedirectToAction("Details", "Locations", new { id = question.LocationId });
            }
            return View(answer);
        }

        // GET: Answers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _answerContext.Answer.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnswerId,Content,QuestionId")] Answer answer)
        {
            if (id != answer.AnswerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _answerContext.Update(answer);
                    await _answerContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerExists(answer.AnswerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                Question question = _questionContext.Question.First(q => q.QuestionId == answer.QuestionId);
                return RedirectToAction("Details", "Locations", new { id = question.LocationId });
            }
            return View(answer);
        }

        // GET: Answers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _answerContext.Answer
                .FirstOrDefaultAsync(m => m.AnswerId == id);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var answer = await _answerContext.Answer.FindAsync(id);
            _answerContext.Answer.Remove(answer);
            await _answerContext.SaveChangesAsync();
            Question question = _questionContext.Question.First(q => q.QuestionId == answer.QuestionId);
            return RedirectToAction("Details", "Locations", new { id = question.LocationId });
        }

        private bool AnswerExists(int id)
        {
            return _answerContext.Answer.Any(e => e.AnswerId == id);
        }
    }
}
