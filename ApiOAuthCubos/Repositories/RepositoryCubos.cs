using ApiOAuthCubos.Data;
using ApiOAuthCubos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ApiOAuthCubos.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            var consulta = from datos in this.context.Cubos select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Cubo>> GetCuboByMarca(string marca)
        {
            return await this.context.Cubos.Where(c => c.Marca == marca).ToListAsync();
        }

        public async Task<int> GetMaxIdUser()
        {
            if (this.context.Users.Count() == 0)
            {
                return 1;
            }else
            {
                return await this.context.Users.MaxAsync(u => u.IdUsuario) + 1;
            }
        }

        public async Task CreateUsuarioAsync(string nombre, string email, string pass)
        {
            User user = new User();
            user.IdUsuario = await this.GetMaxIdUser();
            user.Nombre = nombre;
            user.Email = email;
            user.Pass = pass;
            user.Imagen = "";
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
        }
        public async Task<User> LoginUserAsync(string email, string pass)
        {
            return await this.context.Users.Where(x => x.Email == email && x.Pass == pass).FirstOrDefaultAsync();
        }

        public async Task<User> PerfilUsuario(int id)
        {
            return await this.context.Users.Where(x => x.IdUsuario == id).FirstOrDefaultAsync();
        }

        public async Task<List<Cubo>> GetCubosBlobAsync()
        {
            List<Cubo> cubos = await this.context.Cubos.ToListAsync();
            string containerUrl = this.service.GetContainerUrl("examenaga");

            foreach (Cubo c in cubos)
            {
                if (!c.Imagen.StartsWith("http"))
                {
                    string imagePath = c.Imagen;
                    if (!imagePath.StartsWith("CUBOS/"))
                    {
                        imagePath = "CUBOS/" + imagePath;
                    }

                    c.Imagen = containerUrl + "/" + imagePath;
                }
            }

            return cubos;
        }

    }
}
