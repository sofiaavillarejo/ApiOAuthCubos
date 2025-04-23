using ApiOAuthCubos.Data;
using ApiOAuthCubos.Models;
using ApiOAuthCubos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ApiOAuthCubos.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;
        private ServiceStorageBlob service;

        public RepositoryCubos(CubosContext context, ServiceStorageBlob service)
        {
            this.context = context;
            this.service = service;
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
            string containerUrl = this.service.GetContainerUrl("cubos");

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

        public async Task<UserModel> PerfilUsuarioBlobAsync(int id)
        {
            User usuario = await this.context.Users.Where(x => x.IdUsuario == id).FirstOrDefaultAsync();

            UserModel model = new UserModel
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Imagen = usuario.Imagen
            };

            if (!string.IsNullOrEmpty(usuario.Imagen))
            {
                string containerUrl = this.service.GetContainerUrl("cubos");

                if (!usuario.Imagen.StartsWith("http"))
                {
                    string imagePath = usuario.Imagen;
                    if (!imagePath.StartsWith("USUARIOS/"))
                    {
                        imagePath = "USUARIOS/" + imagePath;
                    }

                    model.Imagen = containerUrl + "/" + imagePath;
                }
                else
                {
                    model.Imagen = usuario.Imagen;
                }
            }

            return model;
        }

        public async Task<List<CompraCubo>> GetCompraUsuarioAsync(int id)
        {
            return await this.context.ComprasCubos
                .Where(x => x.IdUsuario == id)
                .ToListAsync();
        }
        public async Task<int> GetMaxPedidoIdAsync()
        {
            if (!await this.context.ComprasCubos.AnyAsync())
            {
                return 1;
            }
            return await this.context.ComprasCubos.MaxAsync(x => x.IdPedido) + 1;
        }

        public async Task RealizarPedido(int idUsuario, List<int> idsCubos)
        {
            int idPedido = await this.GetMaxPedidoIdAsync();
            DateTime fechaPedido = DateTime.Now;

            List<CompraCubo> compras = new List<CompraCubo>();

            foreach (int idCubo in idsCubos)
            {
                CompraCubo compra = new CompraCubo
                {
                    IdPedido = idPedido,
                    IdCubo = idCubo,
                    IdUsuario = idUsuario,
                    FechaPedido = fechaPedido
                };

                compras.Add(compra);
                idPedido++;
            }

            await this.context.ComprasCubos.AddRangeAsync(compras);
            await this.context.SaveChangesAsync();
        }


    }
}
