using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedSocialFinal.Models
{
	public class Comentario


	{
		public int id { get; set; }
		public string contenido { get; set; }
		public DateTime fecha { get; set; }
		public int idUsuario { get; set; }
		public int idPost { get; set; }
		public Post post { get; set; }
		public Usuario usuario { get; set; }


		public Comentario()
		{}

		public Comentario(int id, DateTime fecha, string contenido,  int idUsuario, int idPost)
		{

			this.id = id;
			this.contenido = contenido;
			this.fecha = fecha;
			this.idPost = idPost;
			this.idUsuario = idUsuario;

		}

		public Comentario(DateTime fecha, string contenido, int idUsuario, int idPost)
		{

			this.contenido = contenido;
			this.fecha = fecha;
			this.idPost = idPost;
			this.idUsuario = idUsuario;

		}
	}
}
