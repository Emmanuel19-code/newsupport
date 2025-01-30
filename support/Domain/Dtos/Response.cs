namespace support.Domain
{
    public class ApiResponse
    {
        
        public bool Success {get;set;}
        public string Message {get;set;}
        public int StatusCode {get;set;}
    }

    public class ApiDataResponse<T> : ApiResponse
    {
       public T Data {get;set;}
    }
}