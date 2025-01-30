namespace support.Domain
{
    public class AddUserDto
    {
        public required string CompanyName {get;set;}
        public int SupportMonth {get;set;}
    }
    public class AccessDetails
    {
        public string UserName {get;set;}
        public string Password {get;set;}
    }
    public class UserProfile
    {
        public Guid Id {get;set;}
        public string CompanyName {get;set;}
        public string UserName {get;set;}
        public string Password {get;set;}
        public bool Supporting {get;set;} = true;
        public DateTime SupportExpiry {get;set;}
    }
}