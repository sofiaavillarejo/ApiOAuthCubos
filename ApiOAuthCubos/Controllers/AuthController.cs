using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiOAuthCubos.Helpers;
using ApiOAuthCubos.Models;
using ApiOAuthCubos.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ApiOAuthCubos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperActionServicesOAuth helper;

        public AuthController(RepositoryCubos repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> LoginUser(LoginModel model)
        {
            User user = await this.repo.LoginUserAsync(model.Email, model.Passwd);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);
                UserModel modelEmp = new UserModel();
                modelEmp.IdUsuario = user.IdUsuario;
                modelEmp.Nombre = user.Nombre;
                modelEmp.Email = user.Email;
                modelEmp.Imagen = user.Imagen;

                string jsonEmpleado =
                    JsonConvert.SerializeObject(modelEmp);
                string jsonCrifado =
                    HelperCryptography.EncryptString(jsonEmpleado);
                Claim[] info = new[]
                {
                    new Claim("UserData", jsonCrifado)
                };
                JwtSecurityToken token = new JwtSecurityToken(
                    claims: info,
                     issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    notBefore: DateTime.UtcNow
                );

                return Ok(new { response = new JwtSecurityTokenHandler().WriteToken(token) });
            }


        }
    }
}
