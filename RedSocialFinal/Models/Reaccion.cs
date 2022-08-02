using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedSocialFinal.Models
{
    public class Reaccion
    {
        public int id { get; set; }
        public int tipo { get; set; }
        public Post post { get; set; }
        public Usuario usuario{ get; set; }
        public int idUsuario { get; set; }
        public int idPost { get; set; }


        public Reaccion()
        { }

        public Reaccion(int id, int tipo, int idPost, int idUsuario)
        {
            this.id = id;
            this.tipo = tipo;
            this.idPost = idPost;
            this.idUsuario = idUsuario;
        }
        public Reaccion(int tipo, int idPost, int idUsuario)
        {
            this.id = id;
            this.tipo = tipo;
            this.idPost = idPost;
            this.idUsuario = idUsuario;
        }

    }
}
