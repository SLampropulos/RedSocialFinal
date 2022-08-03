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
    public class UsuarioAmigoesController : Controller
    {
        private readonly MyContext _context;

        public UsuarioAmigoesController(MyContext context)
        {
            _context = context;
            _context.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                  .Include(u => u.amigosMios).Load();

            _context.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

            _context.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

            _context.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

            _context.Tags.Include(u => u.Posts).Load();
        }

        // GET: UsuarioAmigoes
        public async Task<IActionResult> Index()
        {
            var myContext = _context.UsuarioAmigo.Include(u => u.amigo).Include(u => u.user);
            return View(await myContext.ToListAsync());
        }

        // GET: UsuarioAmigoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioAmigo == null)
            {
                return NotFound();
            }

            var usuarioAmigo = await _context.UsuarioAmigo
                .Include(u => u.amigo)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m.num_usr == id);
            if (usuarioAmigo == null)
            {
                return NotFound();
            }

            return View(usuarioAmigo);
        }

        // GET: UsuarioAmigoes/Create
        public IActionResult Create()
        {
            ViewData["num_usr2"] = new SelectList(_context.usuarios, "id", "id");
            ViewData["num_usr"] = new SelectList(_context.usuarios, "id", "id");
            return View();
        }

        // POST: UsuarioAmigoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("num_usr,num_usr2")] UsuarioAmigo usuarioAmigo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioAmigo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["num_usr2"] = new SelectList(_context.usuarios, "id", "id", usuarioAmigo.num_usr2);
            ViewData["num_usr"] = new SelectList(_context.usuarios, "id", "id", usuarioAmigo.num_usr);
            return View(usuarioAmigo);
        }

        // GET: UsuarioAmigoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioAmigo == null)
            {
                return NotFound();
            }

            var usuarioAmigo = await _context.UsuarioAmigo.FindAsync(id);
            if (usuarioAmigo == null)
            {
                return NotFound();
            }
            ViewData["num_usr2"] = new SelectList(_context.usuarios, "id", "id", usuarioAmigo.num_usr2);
            ViewData["num_usr"] = new SelectList(_context.usuarios, "id", "id", usuarioAmigo.num_usr);
            return View(usuarioAmigo);
        }

        // POST: UsuarioAmigoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("num_usr,num_usr2")] UsuarioAmigo usuarioAmigo)
        {
            if (id != usuarioAmigo.num_usr)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioAmigo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioAmigoExists(usuarioAmigo.num_usr))
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
            ViewData["num_usr2"] = new SelectList(_context.usuarios, "id", "id", usuarioAmigo.num_usr2);
            ViewData["num_usr"] = new SelectList(_context.usuarios, "id", "id", usuarioAmigo.num_usr);
            return View(usuarioAmigo);
        }

        // GET: UsuarioAmigoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioAmigo == null)
            {
                return NotFound();
            }

            var usuarioAmigo = await _context.UsuarioAmigo
                .Include(u => u.amigo)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m.num_usr == id);
            if (usuarioAmigo == null)
            {
                return NotFound();
            }

            return View(usuarioAmigo);
        }

        // POST: UsuarioAmigoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioAmigo == null)
            {
                return Problem("Entity set 'MyContext.UsuarioAmigo'  is null.");
            }
            var usuarioAmigo = await _context.UsuarioAmigo.FindAsync(id);
            if (usuarioAmigo != null)
            {
                _context.UsuarioAmigo.Remove(usuarioAmigo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioAmigoExists(int id)
        {
          return (_context.UsuarioAmigo?.Any(e => e.num_usr == id)).GetValueOrDefault();
        }
    }
}
