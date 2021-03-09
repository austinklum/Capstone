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
    public class QuestionListViewComponent : ViewComponent
    {
        private readonly QuestionContext _questionContext;
        private readonly LocationContext _locationContext;
        
        public QuestionListViewComponent(QuestionContext questionContext, LocationContext locationContext)
        {
            _questionContext = questionContext;
            _locationContext = locationContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string locationId, string search)
        {
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
                LocationId = locationId,
                Search = search,
                Locations = new SelectList(await locationQuery.Distinct().ToListAsync()),
                Questions = await questions.ToListAsync()
            };

            return View(vm);
        }
    }
}
