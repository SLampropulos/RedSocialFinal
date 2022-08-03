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
    public class TagPostsController : Controller
    {
        private readonly MyContext _context;

        public TagPostsController(MyContext context)
        {
            _context = context;
            _context.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                  .Include(u => u.amigosMios).Load();

            _context.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

            _context.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

            _context.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

            _context.Tags.Include(u => u.Posts).Load();
        }

        // GET: TagPosts
        public async Task<IActionResult> Index()
        {
            var myContext = _context.TagPost.Include(t => t.post).Include(t => t.tag);
            return View(await myContext.ToListAsync());
        }

        // GET: TagPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TagPost == null)
            {
                return NotFound();
            }

            var tagPost = await _context.TagPost
                .Include(t => t.post)
                .Include(t => t.tag)
                .FirstOrDefaultAsync(m => m.idTag == id);
            if (tagPost == null)
            {
                return NotFound();
            }

            return View(tagPost);
        }

        // GET: TagPosts/Create
        public IActionResult Create()
        {
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id");
            ViewData["idTag"] = new SelectList(_context.Tags, "id", "id");
            return View();
        }

        // POST: TagPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idTag,idPost")] TagPost tagPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tagPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", tagPost.idPost);
            ViewData["idTag"] = new SelectList(_context.Tags, "id", "id", tagPost.idTag);
            return View(tagPost);
        }

        // GET: TagPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TagPost == null)
            {
                return NotFound();
            }

            var tagPost = await _context.TagPost.FindAsync(id);
            if (tagPost == null)
            {
                return NotFound();
            }
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", tagPost.idPost);
            ViewData["idTag"] = new SelectList(_context.Tags, "id", "id", tagPost.idTag);
            return View(tagPost);
        }

        // POST: TagPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idTag,idPost")] TagPost tagPost)
        {
            if (id != tagPost.idTag)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tagPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagPostExists(tagPost.idTag))
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
            ViewData["idPost"] = new SelectList(_context.posts, "id", "id", tagPost.idPost);
            ViewData["idTag"] = new SelectList(_context.Tags, "id", "id", tagPost.idTag);
            return View(tagPost);
        }

        // GET: TagPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TagPost == null)
            {
                return NotFound();
            }

            var tagPost = await _context.TagPost
                .Include(t => t.post)
                .Include(t => t.tag)
                .FirstOrDefaultAsync(m => m.idTag == id);
            if (tagPost == null)
            {
                return NotFound();
            }

            return View(tagPost);
        }

        // POST: TagPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TagPost == null)
            {
                return Problem("Entity set 'MyContext.TagPost'  is null.");
            }
            var tagPost = await _context.TagPost.FindAsync(id);
            if (tagPost != null)
            {
                _context.TagPost.Remove(tagPost);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagPostExists(int id)
        {
          return (_context.TagPost?.Any(e => e.idTag == id)).GetValueOrDefault();
        }
    }
}
