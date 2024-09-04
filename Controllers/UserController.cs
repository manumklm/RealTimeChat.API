using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.BLL.Repository;
using RealTimeChat.DTO;
using RealTimeChat.DTO.General;
using System.Security.Claims;

namespace RealTimeChat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsermasterCore _ur;

        public UserController(IUsermasterCore ur)
        {
            _ur = ur;
        }

        // POST api/User/Authenticate
        [HttpPost("Authenticate")]
        public async Task<ResponseModel<TokenModel>> VerifyUser(UserDTO dto)
        {
            
            var result = await _ur.UserVerify(dto);
            return result;

        }

        // GET: api/User/List
        [HttpGet("List")]
        [Authorize]
        public async Task<ResponseModel<List<UsermasterModel>>> GetUserList()
        {
            var result = await _ur.GetUserList();
            return result;
        }

        // POST api/User/Create
        [HttpPost("Create")]
        public async Task<ResponseModel<bool>> CreateUser(UsermasterModel model)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = "0"; // claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _ur.CreateUser(model, userId);
            return result;

        }

        // POST api/User/Update
        [HttpPost("Update")]
        public async Task<ResponseModel<bool>> UpdateUser(UsermasterModel model)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = "0";// claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _ur.UpdateUser(model, userId);
            return result;

        }

        // POST api/User/Delete
        [HttpPost("Delete")]
        public async Task<ResponseModel<bool>> DeleteUser(int Id)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = "0";// claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _ur.DeleteUser(Id, userId);
            return result;

        }
    }
}
