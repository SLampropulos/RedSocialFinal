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
    public class ComentariosController : Controller
    {
        private readonly MyContext _context;

        public ComentariosController(MyContext context)
        {
            _context = context;
        }

        // GET: Comentarios
        public async Task<IActionResult> Index()
        {
            var myContext = _context.comentarios.Include(c => c.post).Include(c => c.usuario);
            return View(await myContext.ToListAsync());
        }

        // GET: Comentarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.comentarios == null)
            {
                return NotFound();
            }

            var comentario = await _context.comentarios
                .Include(c => c.post)
                .Include(c => c.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // GET: Comentarios/Create
        public IActionResult Create()
        {
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id");
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id");
            return View();
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,contenido,fecha,idUsuario,idPost")] Comentario comentario)
        {
            
                _context.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
           /* ViewData["idPost"] = new SelectList(_context.posts, "id", "id", comentario.idPost);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", comentario.idUsuario);
            return View(comentario);*/
        }

        // GET: Comentarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.comentarios == null)
            {
                return NotFound();
            }

            var comentario = await _context.comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", comentario.idPost);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", comentario.idUsuario);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,contenido,fecha,idUsuario,idPost")] Comentario comentario)
        {
            if (id != comentario.id)
            {
                return NotFound();
            }

            Comentario comentarioActual = _context.comentarios.Where(p => p.id == id).FirstOrDefault();
            
            comentarioActual.id = comentario.id;
            comentarioActual.contenido = comentario.contenido;
            comentarioActual.fecha = comentario.fecha;
            comentarioActual.idUsuario = comentario.idUsuario;
            comentarioActual.idPost = comentario.idPost;

            try
            {
                _context.Update(comentarioActual);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentarioExists(comentario.id))
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

        // GET: Comentarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.comentarios == null)
            {
                return NotFound();
            }

            var comentario = await _context.comentarios
                .Include(c => c.post)
                .Include(c => c.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.comentarios == null)
            {
                return Problem("Entity set 'MyContext.comentarios'  is null.");
            }
            var comentario = await _context.comentarios.FindAsync(id);
            if (comentario != null)
            {
                _context.comentarios.Remove(comentario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentarioExists(int id)
        {
          return (_context.comentarios?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
