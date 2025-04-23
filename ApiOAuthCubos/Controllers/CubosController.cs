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
        public async Task<ActionResult<UserModel>> PerfilUsuario(int iduser)
        {
            UserModel model = this.helper.GetUser();
            return model;
        }

    }
}
