using ApiOAuthCubos.Helpers;
using ApiOAuthCubos.Models;
using ApiOAuthCubos.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiOAuthCubos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperUserToken helper;

        public CubosController(RepositoryCubos repo, HelperUserToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpGet("Cubos")]
        public async Task<ActionResult<List<Cubo>>> GetCubos()
        {
            return await this.repo.GetCubosAsync();
        }

        [Authorize]
        [HttpGet("CubosBlob")]
        public async Task<ActionResult<List<Cubo>>> GetCubosBlob()
        {
            return await this.repo.GetCubosBlobAsync();
        } 

        [HttpGet("CubosMarca")]
        public async Task<ActionResult<List<Cubo>>> GetCubosPorMarca(string marca)
        {
            return await this.repo.GetCuboByMarca(marca);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUsuario(User user)
        {
            await this.repo.CreateUsuarioAsync(user.Nombre, user.Email, user.Pass);
            return Ok();
        }

        [Authorize]
        [HttpGet("PerfilUser")]
        public async Task<ActionResult<UserModel>> PerfilUsuario()
        {
            UserModel model = this.helper.GetUser();
            return model;
        }

        [Authorize]
        [HttpGet("PerfilBlob")]
        public async Task<ActionResult<UserModel>> PerfilUsuarioBlob()
        {

            UserModel model = this.helper.GetUser();
            var userblob = await this.repo.PerfilUsuarioBlobAsync(model.IdUsuario);
            return userblob;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<UserModel>> GetCompraUsuario()
        {

            UserModel model = this.helper.GetUser();
            var compras = await this.repo.GetCompraUsuarioAsync(model.IdUsuario);
            return Ok(compras);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> RealizarPedido(List<int> idsCubos)
        {

            UserModel model = this.helper.GetUser();


            if (idsCubos == null || idsCubos.Count == 0)
            {
                return BadRequest("Selecciona mas de un cubo");
            }

            await this.repo.RealizarPedido(model.IdUsuario, idsCubos);

            return Ok(new
            {
                message = "Compra realizada",
                userId = model.IdUsuario,
                items = idsCubos.Count
            });


        }


    }
}
