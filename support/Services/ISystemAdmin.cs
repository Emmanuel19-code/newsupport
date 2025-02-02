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
    }
}