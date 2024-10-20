using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Api.Bff.Domain.Services;

namespace Api.Bff.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;

        public UserService(AmazonCognitoIdentityProviderClient cognitoClient)
        {
            _cognitoClient = cognitoClient;
        }

        public async Task<GetUserResponse> GetUserAsync(string accessToken)
        {
            var request = new GetUserRequest
            {
                AccessToken = accessToken
            };

            try
            {
                var response = await _cognitoClient.GetUserAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar informações do usuário: {ex.Message}");
                throw;
            }
        }
    }
}
