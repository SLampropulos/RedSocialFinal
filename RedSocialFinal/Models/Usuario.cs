using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedSocialFinal.Models
{
    public class Usuario
    {

        public int id { get; set; }
        public string dni { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string mail { get; set; }
        public string pass { get; set; }
        public int intentosFallidos { get; set; }
        public bool bloqueado { get; set; }
        public bool esAdmin { get; set; }



        public List<Post> misPost = new List<Post>();

        public List<Comentario> misComentarios = new List<Comentario>();

        public List<Reaccion> misReacciones = new List<Reaccion>();

        public virtual ICollection<UsuarioAmigo> misAmigos { get; set; } = new List<UsuarioAmigo>();
        public virtual ICollection<UsuarioAmigo> amigosMios { get; set; } = new List<UsuarioAmigo>();


        public Usuario()
        {}


        public Usuario(int id, string dni, string nombre, string apellido, string mail, string pass, bool esAdmin, bool bloqueado, int intentosFallidos) 
        {
            this.id = id;
            this.dni = dni;
            this.nombre = nombre;
            this.apellido = apellido;
            this.mail = mail;
            this.pass = pass;
            this.esAdmin = esAdmin;
            this.bloqueado = bloqueado;
            this.intentosFallidos = intentosFallidos;

        }
        public Usuario(string dni, string nombre, string apellido, string mail, string pass, bool esAdmin, bool bloqueado, int intentosFallidos) 
        {
            this.dni = dni;
            this.nombre = nombre;
            this.apellido = apellido;
            this.mail = mail;
            this.pass = pass;
            this.esAdmin = esAdmin;
            this.bloqueado = bloqueado;
            this.intentosFallidos = intentosFallidos;

        }
    }
}
