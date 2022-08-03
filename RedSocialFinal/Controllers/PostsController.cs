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
    public class PostsController : Controller
    {
        private readonly MyContext _context;

        public PostsController(MyContext context)
        {
            _context = context;
            _context.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                    .Include(u => u.amigosMios).Load();

            _context.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

            _context.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

            _context.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

            _context.Tags.Include(u => u.Posts).Load();
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var myContext = _context.posts.Include(p => p.usuario).Include(p => p.reacciones);
            return View(await myContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

            var post = await _context.posts
                .Include(p => p.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,contenido,fecha,idUsuario")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", post.idUsuario);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

            var post = await _context.posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", post.idUsuario);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,contenido,fecha,idUsuario")] Post post)
        {
            if (id != post.id)
            {
                return NotFound();
            }

            Post postActual = _context.posts.Where(p => p.id == id).FirstOrDefault();
           
            postActual.id = post.id;
            postActual.contenido = post.contenido;
            postActual.fecha = post.fecha;
            postActual.idUsuario = post.idUsuario;

            try
            {
               _context.Update(postActual);
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (!PostExists(post.id))
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

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

            var post = await _context.posts
                .Include(p => p.usuario)
                .FirstOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.posts == null)
            {
                return Problem("Entity set 'MyContext.posts'  is null.");
            }
            var post = await _context.posts.FindAsync(id);
            if (post != null)
            {
                _context.posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.posts?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
