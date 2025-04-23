using System.Security.Claims;
using ApiOAuthCubos.Models;
using Newtonsoft.Json;

namespace ApiOAuthCubos.Helpers
{
    public class HelperUserToken
    {
        private IHttpContextAccessor contextAccessor;
        public HelperUserToken(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public UserModel GetUser()
        {
            Claim claim = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "UserData");
            string json = claim.Value; //-> EmpleadoModel
            string jsonUser = HelperCryptography.DecryptString(json);
            UserModel model = JsonConvert.DeserializeObject<UserModel>(jsonUser);
            return model;
        }

    }
}
