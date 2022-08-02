using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RedSocialFinal.Models
{
    public class TagPost
    {
        public int idTag { get; set; }
        public Tag tag { get; set; }
        public int idPost { get; set; }
        public Post post { get; set; }

        public TagPost() { }
    }
}
