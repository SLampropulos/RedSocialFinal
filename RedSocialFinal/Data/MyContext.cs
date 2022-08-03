using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RedSocialFinal.Models;

namespace RedSocialFinal.Data
{
    public class MyContext : DbContext
    {
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Comentario> comentarios { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Reaccion> reacciones { get; set; }

        public MyContext(DbContextOptions<MyContext> optionsBuilder) : base(optionsBuilder) { }

        public MyContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("MyContext");
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //nombre de la tabla
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios")
                .HasKey(u => u.id);
            //Agregamos las tablas nuevas
            modelBuilder.Entity<Post>()
                .ToTable("Posts")
                .HasKey(d => d.id);
            modelBuilder.Entity<Comentario>()
                .ToTable("Comentarios")
                .HasKey(p => p.id);
            modelBuilder.Entity<Tag>()
                .ToTable("Tags")
                .HasKey(d => d.id);
            modelBuilder.Entity<Reaccion>()
                .ToTable("Reacciones")
                .HasKey(d => d.id);          
            modelBuilder.Entity<UsuarioAmigo>()
                .ToTable("Usuario_Amigo")
                .HasKey(k => new { k.num_usr, k.num_usr2 });
            modelBuilder.Entity<TagPost>()
                .ToTable("Tag_Post")
                .HasKey(k => new { k.idTag, k.idPost });


            //==================== RELACIONES============================
            //DEFINICIÓN DE LA RELACIÓN USUARIO ONE TO MANY -> POST
            modelBuilder.Entity<Usuario>()
                .HasMany(U => U.misPost)
                .WithOne(D => D.usuario)
                .HasForeignKey(D => D.idUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<Post>()
                .HasOne(U => U.usuario)
                .WithMany(D => D.misPost)
                .HasForeignKey(D => D.idUsuario)
                .OnDelete(DeleteBehavior.Restrict);*/


            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> COMENTARIO
            modelBuilder.Entity<Usuario>()
               .HasMany(U => U.misComentarios)
               .WithOne(D => D.usuario)
               .HasForeignKey(D => D.idUsuario)
               .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<Comentario>()
                .HasOne(U => U.usuario)
                .WithMany(D => D.misComentarios)
                .HasForeignKey(D => D.idUsuario)
                .OnDelete(DeleteBehavior.Restrict);*/


            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> REACCION
            modelBuilder.Entity<Usuario>()
               .HasMany(U => U.misReacciones)
               .WithOne(D => D.usuario)
               .HasForeignKey(D => D.idUsuario)
               .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<Reaccion>()
                .HasOne(U => U.usuario)
                .WithMany(D => D.misReacciones)
                .HasForeignKey(D => D.idUsuario)
                .OnDelete(DeleteBehavior.Restrict);*/


            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY COMENTARIO -> POST
            modelBuilder.Entity<Post>()
                .HasMany(U => U.comentarios)
                .WithOne(D => D.post)
                .HasForeignKey(D => D.idPost)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comentario>()
                .HasOne(U => U.post)
                .WithMany(D => D.comentarios)
                .HasForeignKey(D => D.idPost)
                .OnDelete(DeleteBehavior.Restrict);


            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY REACCION -> POST
            modelBuilder.Entity<Post>()
                .HasMany(U => U.reacciones)
                .WithOne(D => D.post)
                .HasForeignKey(D => D.idPost)
                .OnDelete(DeleteBehavior.Cascade);

           modelBuilder.Entity<Reaccion>()
                .HasOne(U => U.post)
                .WithMany(D => D.reacciones)
                .HasForeignKey(D => D.idPost)
                .OnDelete(DeleteBehavior.Restrict);

            //DEFINICIÓN DE LA RELACIÓN MANY TO MANY TAG <-> POST
            modelBuilder.Entity<Post>()
                .HasMany(U => U.Tags)
                .WithMany(P => P.Posts)
                .UsingEntity<TagPost>(
                    eup => eup.HasOne(up => up.tag).WithMany(p => p.TagPost).HasForeignKey(u => u.idTag),
                    eup => eup.HasOne(up => up.post).WithMany(u => u.TagPost).HasForeignKey(u => u.idPost),
                    eup => eup.HasKey(k => new { k.idTag, k.idPost })
                );

            //DEFINICIÓN DE LA RELACIÓN MANY TO MANY USUARIO <-> AMIGO
            modelBuilder.Entity<UsuarioAmigo>()
               .HasOne(UA => UA.user)
               .WithMany(U => U.misAmigos)
               .HasForeignKey(u => u.num_usr)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UsuarioAmigo>()
                .HasOne(UA => UA.amigo)
                .WithMany(U => U.amigosMios)
                .HasForeignKey(u => u.num_usr2)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UsuarioAmigo>().HasKey(k => new { k.num_usr, k.num_usr2 });

            //propiedades de los datos
            modelBuilder.Entity<Usuario>(
                usr =>
                {
                    usr.Property(u => u.dni).HasColumnType("varchar(50)");
                    usr.Property(u => u.nombre).HasColumnType("varchar(50)");
                    usr.Property(u => u.apellido).HasColumnType("varchar(50)");
                    usr.Property(u => u.mail).HasColumnType("varchar(255)");
                    usr.Property(u => u.pass).HasColumnType("varchar(50)");
                    usr.Property(u => u.intentosFallidos).HasColumnType("int");
                    usr.Property(u => u.bloqueado).HasColumnType("bit");
                    usr.Property(u => u.esAdmin).HasColumnType("bit");
                });

            modelBuilder.Entity<Reaccion>(
                usr =>
                {
                    usr.Property(u => u.tipo).HasColumnType("int");
                    usr.Property(u => u.idUsuario).HasColumnType("int");
                    usr.Property(u => u.idPost).HasColumnType("int");
                });

            modelBuilder.Entity<Tag>(
                usr =>
                {
                    usr.Property(u => u.palabra).HasColumnType("varchar(50)");
                });

            modelBuilder.Entity<Post>(
               usr =>
               {
                   usr.Property(u => u.contenido).HasColumnType("varchar(255)");
                   usr.Property(u => u.fecha).HasColumnType("datetime");
                   usr.Property(u => u.idUsuario).HasColumnType("int");
               });

            modelBuilder.Entity<Comentario>(
               usr =>
               {
                   usr.Property(u => u.contenido).HasColumnType("varchar(255)");
                   usr.Property(u => u.fecha).HasColumnType("datetime");
                   usr.Property(u => u.idUsuario).HasColumnType("int");
                   usr.Property(u => u.idPost).HasColumnType("int");
               });

           modelBuilder.Entity<UsuarioAmigo>(
              usr =>
              {
                  usr.Property(u => u.num_usr).HasColumnType("int");
                  usr.Property(u => u.num_usr2).HasColumnType("int");
              });

            modelBuilder.Entity<TagPost>(
              usr =>
              {
                  usr.Property(u => u.idTag).HasColumnType("int");
                  usr.Property(u => u.idPost).HasColumnType("int");
              });
        

            
            //AGREGO ALGUNOS DATOS DE PRUEBA
            modelBuilder.Entity<Usuario>().HasData(
                new { id = 1, dni = "11111111", nombre = "111", apellido = "111", mail = "111@111", pass = "111", intentosFallidos = 0, bloqueado = false, esAdmin = true },
                new { id = 2, dni = "22222222", nombre = "222", apellido = "222", mail = "222@222", pass = "222", intentosFallidos = 0, bloqueado = false, esAdmin = false },
                new { id = 3, dni = "33333333", nombre = "333", apellido = "333", mail = "333@333", pass = "333", intentosFallidos = 0, bloqueado = false, esAdmin = false },
                new { id = 4, dni = "44444444", nombre = "444", apellido = "444", mail = "444@444", pass = "444", intentosFallidos = 0, bloqueado = false, esAdmin = false },
                new { id = 5, dni = "55555555", nombre = "555", apellido = "555", mail = "555@555", pass = "555", intentosFallidos = 3, bloqueado = true, esAdmin = false });
            modelBuilder.Entity<Post>().HasData(
                new { id = 1, contenido = "111", fecha = DateTime.Now, idUsuario = 1 },
                new { id = 2, contenido = "222", fecha = DateTime.Now, idUsuario = 1 },
                new { id = 3, contenido = "333", fecha = DateTime.Now, idUsuario = 2 },
                new { id = 4, contenido = "444", fecha = DateTime.Now, idUsuario = 3 },
                new { id = 5, contenido = "555", fecha = DateTime.Now, idUsuario = 4 });
            modelBuilder.Entity<Comentario>().HasData(
                new { id = 1, contenido = "111", fecha = DateTime.Now, idUsuario = 1, idPost = 1 },
                new { id = 2, contenido = "222", fecha = DateTime.Now, idUsuario = 5, idPost = 1 },
                new { id = 3, contenido = "333", fecha = DateTime.Now, idUsuario = 2, idPost = 2 },
                new { id = 4, contenido = "444", fecha = DateTime.Now, idUsuario = 3, idPost = 3 },
                new { id = 5, contenido = "555", fecha = DateTime.Now, idUsuario = 4, idPost = 4 });
            modelBuilder.Entity<Reaccion>().HasData(
               new { id = 1, tipo = 1, idUsuario = 1, idPost = 1 },
               new { id = 2, tipo = 1, idUsuario = 5, idPost = 1 },
               new { id = 3, tipo = 1, idUsuario = 2, idPost = 2 },
               new { id = 4, tipo = 1, idUsuario = 3, idPost = 3 },
               new { id = 5, tipo = 1, idUsuario = 4, idPost = 4 });
            modelBuilder.Entity<Tag>().HasData(
               new { id = 1, palabra = "111" },
               new { id = 2, palabra = "222" },
               new { id = 3, palabra = "333" },
               new { id = 4, palabra = "444" },
               new { id = 5, palabra = "555" });
            modelBuilder.Entity<TagPost>().HasData(
               new { idTag = 1, idPost = 1 },
               new { idTag = 2, idPost = 1 },
               new { idTag = 3, idPost = 2 },
               new { idTag = 4, idPost = 3 },
               new { idTag = 5, idPost = 4 });
            modelBuilder.Entity<UsuarioAmigo>().HasData(
               new { num_usr = 1, num_usr2 = 2 },
               new { num_usr = 1, num_usr2 = 3 },
               new { num_usr = 2, num_usr2 = 1 },
               new { num_usr = 2, num_usr2 = 4 },
               new { num_usr = 4, num_usr2 = 2 },
               new { num_usr = 4, num_usr2 = 3 },
               new { num_usr = 4, num_usr2 = 5 },
               new { num_usr = 3, num_usr2 = 4 },
               new { num_usr = 5, num_usr2 = 4 });

          

           
        }
        public DbSet<RedSocialFinal.Models.UsuarioAmigo>? UsuarioAmigo { get; set; }
        public DbSet<RedSocialFinal.Models.TagPost>? TagPost { get; set; }
    }
}
