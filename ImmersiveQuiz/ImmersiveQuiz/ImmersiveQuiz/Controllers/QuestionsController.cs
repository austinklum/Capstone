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
    public class QuestionsController : Controller
    {
        private readonly QuestionContext _questionContext;
        private readonly LocationContext _locationContext;

        public QuestionsController(QuestionContext context, LocationContext locationContext)
        {
            _questionContext = context;
            _locationContext = locationContext;
        }

        // GET: Questions
        public async Task<IActionResult> Index(string locationId, string search)
        {
            // Use LINQ to get list of genres.
            IQueryable<int> locationQuery = from location in _locationContext.Location
                                               orderby location.Name
                                               select location.LocationId;

            var questions = from m in _questionContext.Question
                            select m;

            if (!string.IsNullOrEmpty(search))
            {
                questions = questions.Where(s => s.Content.Contains(search));
            }

            if (!string.IsNullOrEmpty(locationId))
            {
                questions = questions.Where(x => x.LocationId == int.Parse(locationId));
            }

            var vm = new QuestionLocationViewModel
            {
                Locations = new SelectList(await locationQuery.Distinct().ToListAsync()),
                Questions = await questions.ToListAsync()
            };

            return View(vm);
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionContext.Question
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create(int? locationId)
        {
            Question question = new Question();

            if (locationId.HasValue)
            {
                question.LocationId = locationId.Value;
            }

            return View(question);
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionId,Content,CorrectAnswerId,LocationId")] Question question)
        {
            if (ModelState.IsValid)
            {
                _questionContext.Add(question);
                await _questionContext.SaveChangesAsync();
                return RedirectToAction("Details", "Locations", new { id = question.LocationId });
            }
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionContext.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionId,Content,CorrectAnswerId,LocationId")] Question question)
        {
            if (id != question.QuestionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _questionContext.Update(question);
                    await _questionContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _questionContext.Question
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _questionContext.Question.FindAsync(id);
            _questionContext.Question.Remove(question);
            await _questionContext.SaveChangesAsync();
            return RedirectToAction("Details", "Locations", new { id = question.LocationId });
        }

        private bool QuestionExists(int id)
        {
            return _questionContext.Question.Any(e => e.QuestionId == id);
        }
    }
}
