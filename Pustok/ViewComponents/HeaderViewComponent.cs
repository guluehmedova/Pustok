using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewComponents
{
    public class HeaderViewComponent: ViewComponent
    {
        private PustokContext _context;
        public HeaderViewComponent(PustokContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderViewModel headerViewModel = new HeaderViewModel
            {
                Genres = await _context.Genres.ToListAsync(),
                Settings = await _context.Settings.ToListAsync()
            };
            return View(headerViewModel);
        }
    }
}
