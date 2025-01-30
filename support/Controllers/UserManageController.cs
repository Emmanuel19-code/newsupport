using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using support.Domain;
using support.Service;

namespace support.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManageController : ControllerBase
    {
        private readonly IUserManageService _userManageService;
        public UserManageController(IUserManageService userManageService)
        {
                _userManageService = userManageService;
        }

        [HttpPost("create")]
        public async Task<ApiResponse> AddNewSupport(AddUserDto request)
        {
            var response = await _userManageService.AddUser(request);
            return response;
        }

        [HttpPost("access")]
        public async Task<ApiDataResponse<string>> LoginAccess(AccessDetails request)
        {
            var response = await _userManageService.AccessSystem(request);
            return response;
        }

        [HttpGet("refreshtoken/{Id}")]
        public async Task<ApiDataResponse<string>> RefreshToken(Guid Id)
        {
            var response = await _userManageService.RefreshAccessToken(Id);
            return response;
        }

        [Authorize("Admin")]
        

        [HttpPost("create_ticket")]
        public async Task<ApiResponse> CreateTicket(CreateTicket request)
        {
            var response = await _userManageService.CreateTicket(request);
            return response;
        }
    }
}