namespace Api.Bff.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<object> LoginWithUsernamePasswordAsync(string username, string password);
        Task<object> RegisterUserAsync(string username, string password, string email);
        Task ConfirmEmailCode(string username, string code);
        Task ResendConfirmationCode(string username);
    }
}