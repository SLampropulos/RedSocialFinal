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

        /* -----------------------------------------posts de propios--------------------------------------------------*/
        public async Task<IActionResult> MisPublicaciones()
        {
            int? idUsuario = HttpContext.Session.GetInt32("Id");
            var postsUsuarioActual = _context.posts.Where(p => p.idUsuario == idUsuario);

            return View(await postsUsuarioActual.ToListAsync());

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

            bool UsuarioExists(int id)
            {
                return (_context.usuarios?.Any(e => e.id == id)).GetValueOrDefault();
            }
        }
    }
}