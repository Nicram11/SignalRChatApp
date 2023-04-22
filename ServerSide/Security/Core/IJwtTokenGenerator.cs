namespace ServerSide.Security.Core
{
    public interface IJwtTokenGenerator<TUser> where TUser : class
    {
        public string Generate(TUser user);
    }
}
