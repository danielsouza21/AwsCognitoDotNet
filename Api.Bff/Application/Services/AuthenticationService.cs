using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime.Internal;
using Api.Bff.Domain.Configuration;
using Api.Bff.Domain.Services;
using Microsoft.Extensions.Options;

namespace Api.Bff.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly CognitoUserPool _cognitoUserPool;
        private readonly CognitoSettings _cognitoSettings;

        public AuthenticationService(IOptions<CognitoSettings> cognitoSettings, AmazonCognitoIdentityProviderClient cognitoClient, CognitoUserPool cognitoUserPool)
        {
            _cognitoClient = cognitoClient;
            _cognitoUserPool = cognitoUserPool;
            _cognitoSettings = cognitoSettings.Value;
        }

        public async Task<object> LoginWithUsernamePasswordAsync(string username, string password)
        {
            var user = new CognitoUser(username, _cognitoUserPool.ClientID, _cognitoUserPool, _cognitoClient);

            var authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            var response = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

            return response.AuthenticationResult;
        }

        public async Task<object> RegisterUserAsync(string username, string password, string email)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = username,
                Password = password,
                UserAttributes =
                [
                    new() { Name = "email", Value = email }
                ]
            };

            return await _cognitoClient.SignUpAsync(signUpRequest);
        }

        public async Task ConfirmEmailCode(string username, string code)
        {
            var confirmRequest = new ConfirmSignUpRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = username,
                ConfirmationCode = code
            };

            await _cognitoClient.ConfirmSignUpAsync(confirmRequest);
        }

        public async Task ResendConfirmationCode(string username)
        {
            var resendRequest = new ResendConfirmationCodeRequest
            {
                ClientId = _cognitoSettings.ClientId,
                Username = username
            };

            await _cognitoClient.ResendConfirmationCodeAsync(resendRequest);
        }
    }
}
