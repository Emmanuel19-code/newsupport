namespace support.Domain
{
    public class NewSystemAdmin
    {
        public string AssignedTo {get;set;}
        public string AdminUserEmail {get;set;}
        public string Role {get;set;}
    }
    public class CreatedResponse
    {
        public string AdminUserName {get;set;}
        public string AdminPassword {get;set;} 
    }
    public class AccessRequest
    {
        public string AdminUserName {get;set;}
        public string AdminPassword {get;set;} 
    }
    public class SystemAdmins
    {
       public Guid Id {get;set;}
        public string AdminUserName {get;set;}
        public string AdminUserEmail {get;set;}
        public string AssignedTo {get;set;}
        public string Role {get;set;} 
        public DateTime LastLoggedIn {get;set;}
    }
    public class AssignedToAdmin
    {
        public string AdminUserName { get; set; }
        public Guid TicketId { get; set; }
    }
}