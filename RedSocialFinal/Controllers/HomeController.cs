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

namespace RedSocialFinal.Controllers
{
    public class HomeController : Controller
    {

        private readonly MyContext _context;


        public HomeController(MyContext context)
        {
            _context = context;
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
                return RedirectToAction("Index", "Usuarios");

            }
            else if(usuario.esAdmin == false && usuario.bloqueado != true)
            {
                return RedirectToAction("Index", "posts");

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
    }
}