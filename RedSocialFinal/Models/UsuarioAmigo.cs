using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedSocialFinal.Models
{
    public class UsuarioAmigo
    {
        public int num_usr { get; set; }
        
        public Usuario user { get; set; }
        public int num_usr2 { get; set; }
       
        public Usuario amigo { get; set; }

        public UsuarioAmigo() { }

        public UsuarioAmigo(Usuario ppal, Usuario segundo)
        {
            user = ppal;
            amigo = segundo;
        }
    }
}
