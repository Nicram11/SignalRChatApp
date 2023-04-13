namespace ServerSide.Security.Models
{
    public class CreateUserResult
    {
        public static bool SUCCESSED = true;
        public static bool FAILED = false;

        public bool Success { get; set; }

        public CreateUserResult(bool result)
        {
            Success = result;
        }
    }
}
