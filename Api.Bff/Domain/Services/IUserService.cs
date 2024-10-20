using Amazon.CognitoIdentityProvider.Model;

namespace Api.Bff.Domain.Services
{
    public interface IUserService
    {
        Task<GetUserResponse> GetUserAsync(string accessToken);
    }
}