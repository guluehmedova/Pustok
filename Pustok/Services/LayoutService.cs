using Microsoft.EntityFrameworkCore;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Services
{
    public class LayoutService
    {
        private PustokContext _context;
        public LayoutService(PustokContext context)
        {
            _context = context;
        }
        public async Task<List<Setting>> GetSettings()
        {
            return await _context.Settings.ToListAsync();
        }
        public async Task<List<SocialMedia>> SocialMedias()
        {
            return await _context.SocialMedias.ToListAsync();
        }
    }
}
