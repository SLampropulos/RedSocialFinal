using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RedSocialFinal.Data;

namespace RedSocialFinal.Models
{
    public class RedSocial
    {
        private MyContext contexto;
        public Usuario usuarioActual { get; set; }
       
        public RedSocial()
        { 
            inicializarAtributos();
        }

        private void inicializarAtributos()
        {
            try
            {
                contexto = new MyContext();
                contexto.usuarios.Include(u => u.misPost).Include(u => u.misComentarios).Include(u => u.misReacciones).Include(u => u.misAmigos)
                    .Include(u => u.amigosMios).Load();
                
                contexto.posts.Include(u => u.usuario).Include(u => u.comentarios).Include(u => u.reacciones).Include(u => u.Tags).Load();

                contexto.comentarios.Include(u => u.usuario).Include(u => u.post).Load();

                contexto.reacciones.Include(u => u.post).Include(u => u.usuario).Load();

                contexto.Tags.Include(u => u.Posts).Load();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool iniciarSesion(string nombre, string pass)
        {
            bool usuarioEncontrado = false;

            Usuario us1 = contexto.usuarios.Where(U => U.nombre.Equals(nombre)).FirstOrDefault();
                
            if(us1 == null) return false;
                if (us1.intentosFallidos == 3)
                {
                    us1.bloqueado = true;
                    us1.intentosFallidos = 3;
                    contexto.usuarios.Update(us1); 
                    contexto.SaveChanges();
                }

            if (us1.nombre.Equals(nombre) && us1.pass.Equals(pass) && us1.bloqueado != true)
            {
                this.usuarioActual = us1;
                usuarioEncontrado = true;
                us1.intentosFallidos = 0;
                contexto.usuarios.Update(us1);
                contexto.SaveChanges();
            }
            else if (us1.nombre.Equals(nombre) && !us1.pass.Equals(pass))
            {
                us1.intentosFallidos++;
                contexto.usuarios.Update(us1);
                contexto.SaveChanges();
                }
            return usuarioEncontrado;
        }

        //Cerrar sesión
        public void cerrarSesion()
        {
            usuarioActual = null;
        }

        //======================================MANEJO DE USUARIOS=========================================
        public List<Usuario> getUsuarios()
        {
            List<Usuario> usuarios = contexto.usuarios.ToList<Usuario>();
            return usuarios;
        }

        public Usuario getUsuario(int idUsur)
        {
            return contexto.usuarios.Where(U => U.id.Equals(idUsur)).FirstOrDefault();
        }

        public bool registrarUsuario(string Dni, string Nombre, string Apellido, string Mail, string Password, bool admin)
        {
            //comprobación para que no me agreguen usuarios con DNI duplicado
            bool esValido = true;
            Usuario usuario = null;
            usuario = contexto.usuarios.Where(U => U.nombre.Equals(Dni)).FirstOrDefault();
            
            if (usuario == null)
            {
                try
                {
                    //Ahora sí lo agrego en la lista
                    Usuario nuevo = new Usuario(Dni, Nombre, Apellido, Mail, Password, admin, false, 0);
                    contexto.usuarios.Add(nuevo);
                    contexto.SaveChanges();
                    return true;
                }
                catch(Exception e)
                {
                    //algo salió mal con la query porque no generó un id válido
                    return false;
                }
            }
            else
                return false;
        }

        public bool eliminarUsuario(int Id)
        {
            Usuario usuario = null;
            usuario = contexto.usuarios.Where(U => U.id == Id).FirstOrDefault();
            if (usuario != null)
            {
                try
                {
                    //Ahora sí lo elimino en la lista
                    /*
                    for (int i = 0; i < usuarios.Count; i++)
                        if (usuarios[i].id == Id)
                            usuarios.RemoveAt(i);
                    */
                    contexto.usuarios.Remove(usuario);
                    contexto.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                //algo salió mal con la query porque no generó 1 registro
                return false;
            }
        }

        public bool modificarUsuario(int Id, string Dni, string Nombre, string Apellido, string Mail, string Password, bool EsADM, bool Bloqueado, int IntentosFallidos)
        {
            Usuario usuario = null;
            usuario = contexto.usuarios.Where(U => U.nombre.Equals(Id)).FirstOrDefault();

            if (Bloqueado == false)
            {
                IntentosFallidos = 0;
            }

            //primero me aseguro que lo pueda agregar a la base
            if (usuario != null)
            {
                try
                {
                    //Ahora sí lo MODIFICO en la lista

                    usuario.nombre = Nombre;
                    usuario.apellido = Apellido;
                    usuario.dni = Dni;
                    usuario.mail = Mail;
                    usuario.pass = Password;
                    usuario.esAdmin = EsADM;
                    usuario.bloqueado = Bloqueado;
                    usuario.intentosFallidos = IntentosFallidos;
                    contexto.usuarios.Update(usuario);
                    contexto.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                //algo salió mal con la query porque no generó 1 registro
                return false;
            }
        }

        //===========================================MANEJO DE AMIGOS==================================================

        public bool agregarAmigo(int id)
        {
            Usuario usuario = null;
            usuario = contexto.usuarios.Where(U => U.id == id).FirstOrDefault();
            UsuarioAmigo aux = usuarioActual.misAmigos.Where(U => U.amigo.Equals(usuario)).FirstOrDefault();
            if (usuario != null &&  aux == null)
            {
                try
                {
                    UsuarioAmigo amigo = new UsuarioAmigo(usuarioActual,usuario);
                    UsuarioAmigo amigo2 = new UsuarioAmigo(usuario, usuarioActual);
                    usuarioActual.misAmigos.Add(amigo);
                    usuarioActual.amigosMios.Add(amigo2);
                    contexto.Update(usuarioActual);
                    contexto.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;

            //if (DB.registrarAmigo(usuarioActual.id, id))
            //{
            //    foreach (Usuario u in usuarios)
            //    {

            //        if (!usuarioActual.amigos.Contains(u) && (u.id == id))
            //        {
            //            u.amigos.Add(usuarioActual);
            //            usuarioActual.amigos.Add(u);
            //            return true;
            //        }

            //    }
            //}
            //return false;
        }

        public void quitarAmigo(int id)
        {
            Usuario usuario = null;
            usuario = contexto.usuarios.Where(U => U.id == id).FirstOrDefault(); 

            
            if(usuario != null)
            {
                UsuarioAmigo amigo = usuarioActual.misAmigos.Where(U => U.amigo.Equals(usuario)).FirstOrDefault();
                UsuarioAmigo amigo2 = usuarioActual.amigosMios.Where(U => U.user.Equals(usuario)).FirstOrDefault();

                if (amigo !=null)
                {
                    usuarioActual.misAmigos.Remove(amigo);
                    usuarioActual.amigosMios.Remove(amigo2);
                    contexto.SaveChanges();
                }
            }
            return;

            //if (DB.eliminarAmigo(id, usuarioActual.id))
            //{
            //    foreach (Usuario u in usuarios)
            //    {

            //        if (u.id == id)
            //        {
            //            //Se elimina el amigo
            //            int aux = usuarios.FindIndex(usuario => usuario.id == usuarioActual.id);
            //            usuarios[aux].amigos.Remove(u);

            //            //El usuario que fue eliminado, tambien elimina al usuario que lo elimino
            //            int aux2 = usuarios.FindIndex(usuario => usuario.id == u.id);
            //            usuarios[aux2].amigos.Remove(usuarioActual);

            //        }
            //    }
            //}

        }

        //===========================================MANEJO DE REACCIONES==================================================
        public bool reaccionar(int idPost, int tipoReaccion, int idUsuario)
        {
            Post post = null;
            post = contexto.posts.Where(U => U.id.Equals(idPost)).FirstOrDefault();

            if(post != null)
            {
                Reaccion reaccion = post.reacciones.Where(U => U.idUsuario.Equals(idUsuario)).FirstOrDefault();
                if (reaccion != null) return false;

                try
                {
                    Reaccion r = new Reaccion(tipoReaccion,idPost,usuarioActual.id);
                    post.reacciones.Add(r);
                    contexto.posts.Update(post);
                    contexto.SaveChanges();
                    return true;
                }catch(Exception ex)
                {
                    return false;
                }
            }
            return false;

            //Post PostAModif = null;
            //foreach (Post p in posts)
            //{
            //    if (p.id == idPost)
            //    {
            //        PostAModif = p;
            //    }
            //}
            //if (PostAModif != null)
            //{
            //    foreach (Reaccion r in PostAModif.reacciones)
            //    {
            //        if (r.usuario.id == idUsuario) aca no compara con el usuario actual??
            //        {
            //            return false;
            //        }
            //    }
            //}
            //int idNuevaReaccion = DB.Reaccionar(tipoReaccion, idPost, idUsuario);
            //if (idNuevaReaccion != -1)
            //{
            //    Reaccion Nueva = new Reaccion(idNuevaReaccion, tipoReaccion, PostAModif.id, idUsuario);
            //    PostAModif.reacciones.Add(Nueva);
            //    usuarioActual.misReacciones.Add(Nueva);
            //    return true;
            //}
            //return false;
        }


        public void modificarReaccion(int idReaccion, int tipo, int idPost)
        {
            Reaccion reaccion = null;
            reaccion = contexto.reacciones.Where(U => U.id.Equals(idReaccion)).FirstOrDefault();

            if (reaccion == null) return;

            Post post = contexto.posts.Where(U => U.id == idPost).FirstOrDefault();
            try
            {
                post.reacciones.Remove(reaccion);
                contexto.SaveChanges();
            }catch(Exception ex)
            {
                return;
            }
            //if (DB.modificarReaccion(idReaccion, tipo))
            //{
            //    foreach (Post p in posts)
            //    {
            //        if (p.id == idPost)
            //        {
            //            //busco el indice de la reaccion en la lista de posts
            //            int aux = p.reacciones.FindIndex((reaccion) => reaccion.id == idReaccion);
            //            p.reacciones[aux].tipo = tipo;
            //        }
            //    }

            //}

        }

        public void quitarReaccion(int idPost)
        {
            Post post = null;
            post = contexto.posts.Where(U => U.id==idPost).FirstOrDefault();

            if(post == null) return;

            try
            {
                Reaccion reaccion = post.reacciones.Where(U => U.idUsuario == usuarioActual.id).FirstOrDefault();
                
                post.reacciones.Remove(reaccion);
                usuarioActual.misReacciones.Remove(reaccion);
                contexto.reacciones.Remove(reaccion);
                contexto.SaveChanges();
            }catch (Exception ex)
            {
                return;
            }

            //int idReaccion = 0;
            //foreach (Post p in posts) {
            //    bool salir = false;
            //    if(p.id == idPost)
            //    {
            //        foreach(Reaccion r in p.reacciones)
            //        {

            //            if(r.usuario.id == usuarioActual.id)
            //            {
            //                idReaccion = r.id;
            //                salir = true;
            //                break;
            //            }
            //        }
            //        if (salir) break;
            //        return;
            //    }
            //}
            //if (DB.eliminarReaccion(idReaccion))
            //{
            //    //Borro reaccion de la lista
            //    foreach (Post p in posts)
            //    {
            //        if (p.id == idPost)
            //        {
            //            //busco el indice de la reaccion en la lista de posts
            //            int aux = p.reacciones.FindIndex((reaccion) => reaccion.id == idReaccion);
            //            p.reacciones.RemoveAt(aux);
            //            usuarioActual.misReacciones.RemoveAt(aux);
            //        }
            //    }
            //}

        }

        //===========================================MANEJO DE POSTEOS==================================================
        public void postear(string contenido, DateTime fecha, int idUsuario, List<Tag> tag)
        {
            try
            {
                Post nuevo = new Post(fecha,contenido,idUsuario);
                foreach (Tag t in tag)
                {
                    if (contexto.Tags.Where(u => u.palabra.Equals(t.palabra)).FirstOrDefault() == null)
                    {
                        contexto.Tags.Add(t);
                    }
                    nuevo.Tags.Add(t);
                }
                contexto.posts.Add(nuevo);
                usuarioActual.misPost.Add(nuevo);
                contexto.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            //int idNuevoPost;
            //idNuevoPost = DB.Postear(contenido, fecha, idUsuario);

            //if (idNuevoPost != -1)
            //{
            //    //Ahora sí lo agrego en la lista
            //    Post nuevo = new Post(idNuevoPost,
            //                                fecha,
            //                                contenido,
            //                                idUsuario);
            //    nuevo.usuario = usuarioActual;
            //    foreach (Tag t in tag)
            //    {
            //        t.id = DB.altaTag(t.palabra,idNuevoPost);
            //        DB.altaRelacionarTagPost(idNuevoPost,t.id);

            //        t.posts.Add(nuevo);
            //        nuevo.tags.Add(t);

            //        if (!tags.Contains(t))
            //        {
            //            tags.Add(t);
            //        }
            //    }
             
            //    posts.Add(nuevo);
            //    usuarioActual.misPost.Add(nuevo);

            //}

        }

        public bool modificarPost(int idPost, string comentario)
        {
            Post post = null;
            post = contexto.posts.Where(U => U.id==idPost).FirstOrDefault();

            if(post==null) return false;

            try
            {
                post.contenido = comentario;
                contexto.posts.Update(post);
                contexto.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            return false;
            //int aux = posts.FindIndex(p => p.id == idPost);
            //Post post = posts[aux];

            //if (!post.usuario.Equals(usuarioActual)) return false;

            //if (DB.modificarPost(idPost, comentario))
            //{
            //    post.contenido = comentario;
            //    return true;
            //}
            //return false;
        }

        public bool eliminarPost(int idPost)
        {
            Post post = null;
            Usuario aux = null;
            post = contexto.posts.Where(U => U.id == idPost).FirstOrDefault();
            aux = contexto.usuarios.Where(U => U.id == post.idUsuario).FirstOrDefault();            
            if(post == null) return false;
            if(!usuarioActual.misPost.Contains(post) && !usuarioActual.esAdmin) return false;

            //elimino las reacciones
            var queryReaccion = from Reaccion in contexto.reacciones
                        where Reaccion.idPost == idPost
                        select Reaccion;

            foreach(Reaccion r in queryReaccion)
            {
                contexto.reacciones.Remove(r);
            }
            
            //elimino los comentarios
            var queryComentario = from Comentario in contexto.comentarios
                                where Comentario.idPost == idPost
                                select Comentario;

            foreach (Comentario c in queryComentario)
            {
                contexto.comentarios.Remove(c);
            }

            contexto.posts.Remove(post);
            aux.misPost.Remove(post);
            contexto.usuarios.Update(aux);
            contexto.SaveChanges();

            return true;
            ////Busco el post a eliminar
            //int auxPost = posts.FindIndex(p => p.id == idPost);

            ////busco al usuario en la lista de usuarios
            //int aux = usuarios.FindIndex(usuario => usuario.id == posts[auxPost].idUsuario);
            //if (!usuarioActual.misPost.Contains(posts[auxPost]) && !usuarioActual.esAdmin) return false;
            //    //busco la reaccion correspondiente al post 
            //    Reaccion reaccionEliminar;
            //if (posts[auxPost].reacciones != null || posts[auxPost].reacciones.Count >0)
            //{
            //    //elimino la reaccion correspondiente al post
            //    reaccionEliminar = usuarios[aux].misReacciones.Find(x => x.post.Equals(posts[auxPost]));
            //    usuarios[aux].misReacciones.Remove(reaccionEliminar);
            //    if(reaccionEliminar != null) DB.eliminarReaccion(reaccionEliminar.id);
            //}

            //if (posts[auxPost].comentarios != null)
            //{
            //    foreach (Comentario c in posts[auxPost].comentarios)
            //    {
            //        // Se elimina el comentario del post
            //        usuarios[aux].misComentarios.Remove(c);
            //        DB.eliminarComentario(c.id);
            //    }

            //}
            //if (posts[auxPost].tags != null && posts[auxPost].tags.Count > 0)
            //{
            //    foreach(Tag tag in posts[auxPost].tags)
            //    {
            //        tags.Remove(tag);
            //        DB.bajaRelacionTag_post(idPost, tag.id);
            //        DB.bajaTag(tag.id);
            //    }
                
            //}
                
            //DB.eliminarPost(idPost);
            //usuarios[aux].misPost.Remove(posts[auxPost]); // borro el post de la lista de posts del usuario
            //posts.RemoveAt(auxPost); //borro el post de la lista de posts

            //return true;
        }

        //===========================================MOSTRAR DATOS==================================================

        public Usuario mostrarDatos()
        {
            return usuarioActual;
        }

        //Mostar posts
        public List<Post> mostrarPost()
        {
            return contexto.posts.ToList();
        }
        public List<Tag> mostrarTag()
        {
            return contexto.Tags.ToList();
        }

        //Mostrar amigos
        public List<Usuario> mostrarAmigos()
        {
            List<Usuario> usuarios = new List<Usuario>();
            List<UsuarioAmigo> amigos = usuarioActual.misAmigos.ToList();
            
            foreach (UsuarioAmigo amigo in amigos)
            {
                usuarios.Add(amigo.amigo);
            }
            
            return usuarios;
        }

        //Mostrar posts amigo
        public List<Post> mostrarPostAmigo()
        {
            List<Post> postList = new List<Post>();
            foreach (UsuarioAmigo amigo in usuarioActual.misAmigos)
            {
                foreach (Post post in amigo.amigo.misPost)
                {
                    postList.Add(post);
                }
            }
            return postList.ToList();

            //List<Post> postAmigo = new List<Post>();
            //foreach (Usuario amigo in usuarioActual.amigos)
            //{
            //    foreach (Post post in amigo.misPost)
            //    {
            //        postAmigo.Add(post);
            //    }
            //}
            //return postAmigo;
        }

        public List<Tag> mostrarTagsDePost(int idPost)
        {
            List<Tag> tags = new List<Tag>();
            Post post = buscarPost(idPost);

            tags = post.Tags.ToList();

            return tags;
        } 

        //Buscar posts
        public List<Post> buscarPosts(string contenido, DateTime fechaDesde, DateTime fechaHasta, List<Tag> t)
        {
            List<Post> bPost = new List<Post>();

            var query = from Post in contexto.posts
                            where Post.contenido == contenido ||
                            (Post.fecha >= fechaDesde &&
                            Post.fecha <= fechaHasta)
                        select Post;

            if (t.Count() != 0) {
                foreach (Post p in contexto.posts)
                {
                    foreach (Tag tag in t)
                    {
                        if (p.Tags.Where(u => u.palabra.Equals(tag.palabra)).FirstOrDefault() != null)
                        {
                            bPost.Add(p);
                            break;
                        }
                    }                
                }
            }

            foreach (Post post in query)
            {
                bPost.Add((Post)post);

            }

            return bPost.ToList();
            //List<Post> bPost = new List<Post>();
            //foreach (Post post in posts)
            //{
            //    if (post.contenido.Equals(contenido))
            //    {
            //        bPost.Add(post);
            //    }
            //    else if (post.fecha >= fechaDesde && post.fecha <= fechaHasta)
            //    {
            //        bPost.Add(post);
            //    }
            //    else
            //    {
            //        foreach (Tag p in t)
            //        {
            //            foreach (Tag q in post.tags)
            //            {
            //                if (q.palabra == p.palabra)
            //                {
            //                    bPost.Add(post);
            //                }
            //            }
            //        }
            //    }
            //}
            //return bPost;
        }
        public Post buscarPost(int idPost)
        {
            Post post = null;
            post = contexto.posts.Where(U => U.id == idPost).FirstOrDefault();
            return post;

            //Post post = null;
            //foreach(Post postActual in posts)
            //{
            //    if(idPost == postActual.id)
            //    {
            //        post = postActual;
            //        break;
            //    }
            //}
            //return post;
        }


        //===========================================MANEJO DE COMENTARIOS==================================================

        public void comentar(int idPost, String contenido, DateTime fecha)
        {
            Post post = this.buscarPost(idPost);

            if(post == null) return;
            try
            {
                Comentario comentario = new Comentario(fecha, contenido, usuarioActual.id, idPost);
                contexto.comentarios.Add(comentario);
                post.comentarios.Add(comentario);
                contexto.posts.Update(post);
                contexto.SaveChanges();
            }catch(Exception ex)
            {
                return;
            }


            //int idNuevoComentario;
            //idNuevoComentario = DB.registrarComentario(fecha, contenido, usuarioActual.id, idPost);
            //if (idNuevoComentario != -1)
            //{
            //    //Ahora sí lo agrego en la lista
            //    Comentario nuevo = new Comentario(idNuevoComentario, fecha, contenido, usuarioActual.id, idPost);
            //    nuevo.usuario = usuarioActual;

            //    int aux = posts.FindIndex(p => p.id == idPost);
            //    nuevo.post = posts[aux];

            //    comentarios.Add(nuevo);
            //    posts[aux].comentarios.Add(nuevo);

            //}
        }

        //Modificar comentario
        public void modificarComentario(int idComentario, string nuevoComentario)
        {
            Comentario comentario = null;
            comentario = contexto.comentarios.Where(U => U.id == idComentario).FirstOrDefault();

            if (comentario == null) return;

            try
            {
                comentario.contenido = nuevoComentario;
                contexto.comentarios.Update(comentario);
                contexto.SaveChanges();
            }catch(Exception ex)
            {
                return;
            }

            //if (DB.modificarComentario(idComentario, nuevoComentario))
            //{
            //    int aux = comentarios.FindIndex(c => c.id == idComentario);

            //    if (comentarios[aux].usuario.Equals(usuarioActual))
            //    {
            //        comentarios[aux].contenido = nuevoComentario;
            //    }
            //}

        }

        //Borrar comentario
        public void quitarComentario(int idPost, int idComentario)
        {
            Post post = buscarPost(idPost);
            if (post == null) return;

            Comentario comentario = post.comentarios.Where(U =>U.id == idComentario).FirstOrDefault();
            if(comentario == null) return;

            try
            {
                usuarioActual.misComentarios.Remove(comentario);
                post.comentarios.Remove(comentario);

                contexto.usuarios.Update(usuarioActual);
                contexto.posts.Update(post);
                contexto.comentarios.Remove(comentario);
                contexto.SaveChanges();

            }catch(Exception ex)
            {
                return;
            }

            //int auxCom = comentarios.FindIndex(c => c.id == idComentario);
            //if (!comentarios[auxCom].usuario.Equals(usuarioActual) && !usuarioActual.esAdmin) return;

            //if (DB.eliminarComentario(idComentario))
            //{
            //    int aux2 = posts.FindIndex(post => post.id == idPost);

            //    posts[aux2].comentarios.Remove(comentarios[auxCom]);

            //    int aux = usuarios.FindIndex(usuario => usuario.id == usuarioActual.id);
            //    usuarios[aux].misComentarios.Remove(comentarios[auxCom]);

            //    comentarios.Remove(comentarios[auxCom]);
            //}

        }
        //===========================================MANEJO DE TAGS==================================================

        public bool eliminarTag(int idTag)
        {
            Tag tag = null;
            tag = contexto.Tags.Where(U =>U.id == idTag).FirstOrDefault();

            if(tag == null) return false;

            try
            {
                List<Post> posts = contexto.posts.Where(U =>U.Tags.Contains(tag)).ToList();
                foreach(Post post in posts)
                {
                    post.Tags.Remove(tag);
                    contexto.posts.Update(post);
                }
                contexto.Tags.Remove(tag);
                contexto.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            return false;
            //int idPost = -1;
            //bool borro = false;
            //Tag aux = null;
            

            //foreach (Tag tag in tags)
            //{
            //    if (tag.id == idTag)
            //    {
            //        aux = tag;
            //        idPost = tag.idPost;
            //        break;
            //    }
            //}

            //foreach (Post post in posts)
            //{
            //    if (post.id == idPost)
            //    {
            //        post.tags.Remove(aux);
                    
            //    }
   
            //}
            //tags.Remove(aux);


            // DB.bajaRelacionTag_post(idTag);

            //borro = DB.bajaTag(idTag);

            //if (!borro) return borro;


            //return borro;
        }

        public void cerrarPrograma()
        {
            contexto.Dispose();
        }

        

    }

}