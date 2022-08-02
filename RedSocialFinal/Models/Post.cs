using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedSocialFinal.Models
{
    public class Post
    {
        public int id { get; set; }
        public Usuario usuario {get; set;}
        public string contenido {get; set;}
        public List<Comentario> comentarios {get; set;}
        public List<Reaccion> reacciones {get; set;}
        public DateTime fecha {get; set;}
        public int idUsuario {get; set;}

        public ICollection<Tag> Tags { get; } = new List<Tag>();
        public List<TagPost> TagPost { get; set; }


        public Post()
        {}
        public Post(int id, DateTime fecha, string contenido, int idUsuario)
        {
            this.id = id;
            this.contenido = contenido;
            this.comentarios = new List<Comentario>(); 
            this.reacciones = new List<Reaccion>();
            this.fecha = fecha;
            this.idUsuario = idUsuario;

        }
        public Post(DateTime fecha, string contenido, int idUsuario)
        {
            this.contenido = contenido;
            this.comentarios = new List<Comentario>(); 
            this.reacciones = new List<Reaccion>();
            this.fecha = fecha;
            this.idUsuario = idUsuario;

        }
    }
    
}
