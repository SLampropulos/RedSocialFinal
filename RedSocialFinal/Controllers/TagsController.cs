using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedSocialFinal.Data;
using RedSocialFinal.Models;

namespace RedSocialFinal.Controllers
{
    public class TagsController : Controller
    {
        private readonly MyContext _context;

        public TagsController(MyContext context)
        {
            _context = context;
            _context.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                  .Include(u => u.amigosMios).Load();

            _context.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

            _context.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

            _context.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

            _context.Tags.Include(u => u.Posts).Load();
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
              return _context.Tags != null ? 
                          View(await _context.Tags.ToListAsync()) :
                          Problem("Entity set 'MyContext.Tags'  is null.");
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,palabra")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,palabra")] Tag tag)
        {
            if (id != tag.id)
            {
                return NotFound();
            }

            Tag tagActual = _context.Tags.Where(p => p.id == id).FirstOrDefault();

            tagActual.id = tag.id;
            tagActual.palabra = tag.palabra;
            
            try
            {
                _context.Update(tagActual);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(tag.id))
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

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tags == null)
            {
                return Problem("Entity set 'MyContext.Tags'  is null.");
            }
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
          return (_context.Tags?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
