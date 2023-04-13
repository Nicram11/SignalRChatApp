namespace ServerSide.Security.Core
{
    public interface ISignInManager<TUser>
    {
        Task<string> PasswordSignInAsync(string userName, string password);
    }
}
