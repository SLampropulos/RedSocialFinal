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
    public class ReaccionsController : Controller
    {
        private readonly MyContext _context;

        public ReaccionsController(MyContext context)
        {
            _context = context;
            _context.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                  .Include(u => u.amigosMios).Load();

            _context.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

            _context.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

            _context.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

            _context.Tags.Include(u => u.Posts).Load();
        }

        // GET: Reaccions
        public async Task<IActionResult> Index()
        {
            var myContext = _context.reacciones.Include(r => r.post).Include(r => r.usuario);
            return View(await myContext.ToListAsync());
        }

        // GET: Reaccions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reacciones == null)
            {
                return NotFound();
            }

            var reaccion = await _context.reacciones
                .Include(r => r.post)
                .Include(r => r.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reaccion == null)
            {
                return NotFound();
            }

            return View(reaccion);
        }

        // GET: Reaccions/Create
        public IActionResult Create()
        {
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id");
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id");
            return View();
        }

        // POST: Reaccions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,tipo,idUsuario,idPost")] Reaccion reaccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reaccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", reaccion.idPost);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", reaccion.idUsuario);
            return View(reaccion);
        }

        // GET: Reaccions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reacciones == null)
            {
                return NotFound();
            }

            var reaccion = await _context.reacciones.FindAsync(id);
            if (reaccion == null)
            {
                return NotFound();
            }
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", reaccion.idPost);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", reaccion.idUsuario);
            return View(reaccion);
        }

        // POST: Reaccions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,tipo,idUsuario,idPost")] Reaccion reaccion)
        {
            if (id != reaccion.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reaccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaccionExists(reaccion.id))
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
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", reaccion.idPost);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", reaccion.idUsuario);
            return View(reaccion);
        }

        // GET: Reaccions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reacciones == null)
            {
                return NotFound();
            }

            var reaccion = await _context.reacciones
                .Include(r => r.post)
                .Include(r => r.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reaccion == null)
            {
                return NotFound();
            }

            return View(reaccion);
        }

        // POST: Reaccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reacciones == null)
            {
                return Problem("Entity set 'MyContext.reacciones'  is null.");
            }
            var reaccion = await _context.reacciones.FindAsync(id);
            if (reaccion != null)
            {
                _context.reacciones.Remove(reaccion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaccionExists(int id)
        {
          return (_context.reacciones?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
