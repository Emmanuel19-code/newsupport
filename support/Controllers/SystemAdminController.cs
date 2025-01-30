using Microsoft.AspNetCore.Mvc;
using support.Domain;
using support.Service;


namespace support.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAdminController : ControllerBase
    {
        private readonly ISystemAdmin _systemAdmin;
        public SystemAdminController(ISystemAdmin systemAdmin)
        {
            _systemAdmin = systemAdmin;
        }

        //[Authorize("SuperAdmin")]
        [HttpPost("add_system_admin")]
        public async Task<ApiDataResponse<CreatedResponse>> AddSystemAdmin(NewSystemAdmin request)
        {
            var response = await _systemAdmin.AddSystemAdmin(request);
            return response;
        }

        [HttpPost("login_access")]
        public async Task<ApiDataResponse<string>> Login(AccessRequest request)
        {
            var response = await _systemAdmin.AccessSystem(request);
            return response;
        }

        [HttpGet("reset_password/{adminUserName}")]
        public async Task<ApiDataResponse<string>> ResetPassword(string adminUserName)
        {
            var response = await _systemAdmin.ResetAdminPassword(adminUserName);
            return response;
        }

        [HttpGet("admin")]
        public async Task<ApiDataResponse<List<SystemAdmins>>> AllAdmins()
        {
            var response = await _systemAdmin.AllAdmin();
            return response;
        }

        [HttpPost("start_supporting")]
        public async Task<ApiResponse> StartSupport(CreateConversationRequest request, Guid TicketId)
        {
            var response = await _systemAdmin.StartConversation(request,TicketId);
            return response;
        }
    }

}
/*
"adminUserName": "Emmanuel2875",
"adminPassword": "OWMq5oc[?n3T"
"newpassword" : G<[+[L$8O%aw


userPassword = 8LO.;u}T<,mb,
userName = TGTS7188
*/