using Microsoft.AspNetCore.Mvc;
using RedSocialFinal.Models;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedSocialFinal.Data;
using RedSocialFinal.Models;
using Microsoft.AspNetCore.Http;

namespace RedSocialFinal.Controllers
{
    public class HomeController : Controller
    {

        private readonly MyContext _context;


        public HomeController(MyContext context)
        {
            _context = context;
            _context.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                  .Include(u => u.amigosMios).Load();

            _context.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

            _context.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

            _context.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

            _context.Tags.Include(u => u.Posts).Load();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult MiPerfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("Id");
            var usuarioActual = _context.usuarios.Where(p => p.id == idUsuario).FirstOrDefault();

            if (idUsuario == null || _context.usuarios == null)
            {
                return NotFound();
            }

            if (usuarioActual == null)
            {
                return NotFound();
            }

            return View(usuarioActual);
            
        }


        /* -----------------------------------------Login--------------------------------------------------*/

        // POST: Home/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombre, string pass)
        {
            if (nombre == null || pass == null)
            {
                return NotFound();
            }
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.nombre == nombre);

            if (usuario.intentosFallidos == 3)
            {
                usuario.bloqueado = true;
                usuario.intentosFallidos = 3;
                _context.usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }


            if (usuario.nombre.Equals(nombre) && usuario.pass.Equals(pass) && usuario.bloqueado != true && usuario.esAdmin != false)
            {
                usuario.intentosFallidos = 0;
                _context.usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("Id",usuario.id);
                return RedirectToAction("Index", "Usuarios");

            }
            else if(usuario.nombre.Equals(nombre) && usuario.pass.Equals(pass) && usuario.bloqueado != true && usuario.esAdmin == false && usuario.bloqueado != true)
            {
                usuario.intentosFallidos = 0;
                _context.usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("Id", usuario.id);
                return RedirectToAction("PostAmigos", "Home");
            }
         
            
            if (usuario.nombre.Equals(nombre) && !usuario.pass.Equals(pass))
            {
                usuario.intentosFallidos++;
                _context.usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Login","Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /* -----------------------------------------registro--------------------------------------------------*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar([Bind("id,dni,nombre,apellido,mail,pass,0,0,0")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return RedirectToAction("Registrar", "Home");
        }
        /* -----------------------------------------posts de mis amigos--------------------------------------------------*/

        public async Task<IActionResult> PostAmigos()
        {
            int? idUsuario = HttpContext.Session.GetInt32("Id");
            var usuarioActual = _context.usuarios.Where(p => p.id == idUsuario).FirstOrDefault();

            List<Post> postList = new List<Post>();
            foreach (UsuarioAmigo amigo in usuarioActual.misAmigos)
            {
                foreach (Post post in amigo.amigo.misPost)
                {
                    postList.Add(post);
                }
            }

            return View(postList.ToList());
        }

        /* -----------------------------------------posts  propios--------------------------------------------------*/
        public async Task<IActionResult> MisPublicaciones()
        {
            int? idUsuario = HttpContext.Session.GetInt32("Id");
            var postsUsuarioActual = _context.posts.Where(p => p.idUsuario == idUsuario);

            return View(await postsUsuarioActual.ToListAsync());

        }

        /* -----------------------------------------Postear--------------------------------------------------*/

        public IActionResult Postear(int id)
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Postear([Bind("contenido,fecha,idUsuario")] Post post)
        {
            int? usuarioActual = HttpContext.Session.GetInt32("Id");
            post.idUsuario = (int)usuarioActual;
            post.fecha = DateTime.Now;
            _context.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("MisPublicaciones", "Home");

            /*ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", post.idUsuario);
            return View(post);*/
        }

        /* -----------------------------------------Eliminar Post--------------------------------------------------*/
        // GET: Posts/Delete/5
        public async Task<IActionResult> EliminarPost(int? id)
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
        public async Task<IActionResult> EliminarPost(int id)
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
            return RedirectToAction("MisPublicaciones", "Home");
        }

        /* -----------------------------------------Modificar Post--------------------------------------------------*/

        // GET: Posts/Edit/5
        public async Task<IActionResult> ModificarPost(int? id)
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
        public async Task<IActionResult> ModificarPost(int id, [Bind("id,contenido,fecha,idUsuario")] Post post)
        {
            if (id != post.id)
            {
                return NotFound();
            }

            int? usuarioActual = HttpContext.Session.GetInt32("Id");

            Post postActual = _context.posts.Where(p => p.id == id).FirstOrDefault();

            postActual.id = post.id;
            postActual.contenido = post.contenido;
            postActual.fecha = post.fecha;
            postActual.idUsuario = (int)usuarioActual;

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
            return RedirectToAction("MisPublicaciones", "Home");

        }

        /* -----------------------------------------ver comentarios de post especifico--------------------------------------------------*/
        // GET: home/Comentarios/5
        public async Task<IActionResult> Comentarios(int? id)
        {
            if (id == null || _context.posts == null)
            {
                return NotFound();
            }

            var myContext = _context.comentarios.Where(p => p.idPost == id);
            return View(await myContext.ToListAsync());         
        }

        /* -----------------------------------------Comentar--------------------------------------------------*/
        // GET: Comentarios/Create
        public IActionResult Comentar(int id)
        {
            HttpContext.Session.SetInt32("Post", id);
            //ViewData["idPost"] = new SelectList(_context.posts, "id", "id");
            // ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id");

            return View();
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comentar([Bind("contenido,fecha,idUsuario,idPost")] Comentario comentario)
        {
           
            int? idUsuario = HttpContext.Session.GetInt32("Id");
            int? idPost = HttpContext.Session.GetInt32("Post");
            comentario.idUsuario = (int)idUsuario;
            comentario.idPost = (int)idPost;
            comentario.fecha = DateTime.Now;
           
            _context.Add(comentario);
            await _context.SaveChangesAsync();
            return RedirectToAction("PostAmigos", "Home");

            /* ViewData["idPost"] = new SelectList(_context.posts, "id", "id", comentario.idPost);
             ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "id", comentario.idUsuario);
             return View(comentario);*/
        }
        /* -----------------------------------------Editar perfil--------------------------------------------------*/

        public async Task<IActionResult> EditarPerfil(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(int id, [Bind("id,dni,nombre,apellido,mail,pass,0,0,0")] Usuario usuario, string nuevaContraseña, string confirmacionContraseña)
        {
            if (id != usuario.id)
            {
                return NotFound();
            }
            int? idUsuario = HttpContext.Session.GetInt32("Id");
            Usuario usuarioActual = _context.usuarios.Where(p => p.id == idUsuario).FirstOrDefault();
            
            usuarioActual.nombre = usuario.nombre;
            usuarioActual.apellido = usuario.apellido;
            usuarioActual.dni = usuario.dni;
            usuarioActual.mail = usuario.mail;
            usuarioActual.pass = nuevaContraseña;
            usuarioActual.esAdmin = usuario.esAdmin;
            usuarioActual.bloqueado = usuario.bloqueado;
            usuarioActual.intentosFallidos = usuario.intentosFallidos;
            if (!usuario.pass.Equals(usuario.pass) || (!nuevaContraseña.Equals(confirmacionContraseña)))
            {
                return RedirectToAction("MiPerfil", "Home");
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioActual);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("MiPerfil", "Home");

        }

        /* -----------------------------------------Reaccionar--------------------------------------------------*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reaccionar(int idPost)
        {
            Post post = null;
            post = _context.posts.Where(u => u.id == idPost).FirstOrDefault();

            int? idUsuario = HttpContext.Session.GetInt32("Id");
            Usuario usuarioActual = _context.usuarios.Where(p => p.id == idUsuario).FirstOrDefault();

            if (post != null)
            {
                Reaccion reaccion = post.reacciones.Where(U => U.idUsuario == idUsuario).FirstOrDefault();
                if (reaccion != null) return RedirectToAction("PostAmigos", "Home"); ;

                try
                {
                    Reaccion r = new Reaccion(1, (int)idPost, usuarioActual.id);
                    post.reacciones.Add(r);
                    _context.posts.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("PostAmigos", "Home");
                }
                catch (Exception ex)
                {
                    
                }
            }
            return RedirectToAction("PostAmigos", "Home");

        }
        /* -----------------------------------------Eliminar Reacción--------------------------------------------------*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarReaccion(int idPost)
        {
            Post post = null;
            post = _context.posts.Where(U => U.id == idPost).FirstOrDefault();

            int? idUsuario = HttpContext.Session.GetInt32("Id");
            Usuario usuarioActual = _context.usuarios.Where(p => p.id == idUsuario).FirstOrDefault();

            if (post == null) return RedirectToAction("PostAmigos", "Home");

            try
            {
                Reaccion reaccion = post.reacciones.Where(U => U.idUsuario == usuarioActual.id).FirstOrDefault();

                post.reacciones.Remove(reaccion);
                usuarioActual.misReacciones.Remove(reaccion);
                _context.reacciones.Remove(reaccion);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("PostAmigos", "Home");
            }
            return RedirectToAction("PostAmigos", "Home");

        }
        /* -----------------------------------------Agregar Tag--------------------------------------------------*/
        // GET: Tags/Create
        public IActionResult AgregarTag(int? id)
        {
            HttpContext.Session.SetInt32("IdPost", (int)id);
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarTag([Bind("palabra")] Tag tag)
        {
            int? idPost = HttpContext.Session.GetInt32("IdPost");
            Post post = null;
            post = _context.posts.Where(U => U.id == idPost).FirstOrDefault();

            if (_context.Tags.Where(u => u.palabra.Equals(tag.palabra)).FirstOrDefault() == null)
            {
                _context.Add(tag);
                post.Tags.Add(tag);
                _context.Update(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MisPublicaciones", "Home");

        }


        /* -----------------------------------------Buscar Post--------------------------------------------------*/
        public async Task<IActionResult> BuscarPosts(List<Post>? post)
        {
            
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarPosts(string? contenido, DateTime? fechaDesde, DateTime? fechaHasta, string? tag, bool orderFecha, bool nombreAsc, bool nombreDesc)
        {

            if (contenido == null && (fechaDesde == null || fechaHasta==null) && tag == null)
            {
                if (orderFecha && nombreAsc && nombreDesc)
                {
                    return View(await _context.posts.ToListAsync());
                }
                else if (orderFecha)
                {
                    return View(await _context.posts.OrderBy(u => u.fecha).ToListAsync());
                }
                else if (nombreAsc)
                {
                    return View(await _context.posts.OrderBy(u => u.usuario.nombre).ToListAsync());
                }
                else if (nombreDesc)
                {
                    return View(await _context.posts.OrderByDescending(u => u.usuario.nombre).ToListAsync());
                }
                return View(await _context.posts.ToListAsync());
            }

                List<Post> bPost = new List<Post>();

            var query = from Post in _context.posts
                        where Post.contenido == contenido ||
                        (Post.fecha >= fechaDesde &&
                        Post.fecha <= fechaHasta)
                        select Post;
            bPost = await query.ToListAsync();

            if (tag != null)
            {
                foreach (Post p in _context.posts)
                {
                    if (p.Tags.Where(u => u.palabra.Equals(tag)).FirstOrDefault() != null)
                    {
                        if (!bPost.Contains(p))
                        {
                            bPost.Add(p);
                            break;
                        }
                      
                    }
                    
                }
            }
            if (orderFecha && nombreAsc && nombreDesc) 
            {
                return View(bPost.ToList());
            }else if (orderFecha) 
            {
                return View(bPost.OrderBy(u => u.fecha).ToList());
            }else if (nombreAsc)
            {
                return View(bPost.OrderBy(u => u.usuario.nombre).ToList());
            }else if (nombreDesc)
            {
                return View(bPost.OrderByDescending(u => u.usuario.nombre).ToList());
            }

                return View(bPost.ToList());
        }



        /* -----------------------------------------Metodos auxiliares--------------------------------------------------*/
        private bool UsuarioExists(int id)
        {
            return (_context.usuarios?.Any(e => e.id == id)).GetValueOrDefault();
        }

        private bool PostExists(int id)
        {
            return (_context.posts?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}