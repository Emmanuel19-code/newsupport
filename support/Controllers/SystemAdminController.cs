using Microsoft.AspNetCore.Authorization;
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
        [Authorize("SuperAdmin")]
        [HttpPut("reset_password/{adminUserName}")]
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

        [HttpPost("assign_ticket")]
        public async Task<ApiResponse> AssignTicketToAdmin(AssignedToAdmin request)
        {
            var response = await _systemAdmin.AssignToAdmin(request);
            return response;
        }

        [HttpGet("received_tickets")]
        public async Task<List<Ticket>> ReceivedTickets()
        {
            var response  = await _systemAdmin.AllReceivedTickets();
            return response;
        }

        [HttpGet("my_assigned_ticket/{AdminUserName}")]
        public async Task<List<Ticket>> MyAssignedTicket(string AdminUserName)
        {
            var response = await _systemAdmin.MyAssignedTicket(AdminUserName);
            return response;
        }

        [HttpGet("supportComapnies")]
        public async Task<ApiDataResponse<List<CompanyProfile>>> AllRegistered()
        {
            var response = await _systemAdmin.GetSupportingCompanies();
            return response;
        }

        [HttpPut("terminate_support/{companyId}")]
        public async Task<ApiResponse> TerminateComapnySupport(Guid companyId)
        {
            var response = await _systemAdmin.TerminateSupport(companyId);
            return response;
        }

        [HttpPut("renew_support/{companyId}")]
        public async Task<ApiResponse> RenewCompanySupport(Guid companyId)
        {
             var response = await _systemAdmin.RenewSupport(companyId);
            return response;
        }

        [HttpPost("add_new_support_company")]
        public async Task<ApiResponse> AddNewCompany(AddUserDto request)
        {
            var response = await _systemAdmin.AddCompany(request);
            return response;
        }

        
    }

}
/*
"adminUserName": "Emmanuel4887",
"adminPassword": "{rgX[07BAePf"
"newpassword" : l+6$%rm%6Z5$

"adminUserName": "Solomon3494",
"adminPassword": "f+cn-rKe5CL1"

userPassword = p}.5zB!G#:|x,
userName = TGTS2933

company = GIS9113
password = 2[]YSZONd^rY

company = SAPCLIENT
password = O|i1viNS<Zk

*/