using support.Domain;

namespace support.Service
{
    public interface ISystemAdmin
    {
        Task<ApiDataResponse<CreatedResponse>> AddSystemAdmin(NewSystemAdmin request);
        Task<ApiDataResponse<string>> AccessSystem(AccessRequest request);
        Task<ApiDataResponse<string>> ResetAdminPassword(string AdminUserName);
        Task<ApiDataResponse<List<SystemAdmins>>> AllAdmin();
        Task<ApiResponse> StartConversation(CreateConversationRequest request,Guid TicketId);
        Task<ApiResponse> AssignToAdmin(AssignedToAdmin request);
        Task<List<Ticket>> AllReceivedTickets();
        Task<List<Ticket>> MyAssignedTicket(string AdminUserName);
        Task<ApiDataResponse<List<CompanyProfile>>>  GetSupportingCompanies();
        Task<ApiResponse> TerminateSupport(Guid companyId);
        Task<ApiResponse> RenewSupport(Guid companyId);
        Task<ApiResponse> AddCompany(AddUserDto request);
    }
}