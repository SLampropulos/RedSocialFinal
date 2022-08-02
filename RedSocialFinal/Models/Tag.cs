using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace RedSocialFinal.Models
{
	public class Tag
	{

		public int id { get; set; }
		public string palabra { get; set; }

		public ICollection<Post> Posts { get; } = new List<Post>();
		public List<TagPost> TagPost { get; set; }

		public Tag()
		{ }

		public Tag(int id, string palabra)
		{
		this.id = id;
		this.palabra = palabra;		
		}

		public Tag(string palabra)
        {
			this.palabra= palabra;
        }
	}
}
