namespace Api.Bff.Domain.Configuration
{
    public class CognitoSettings
    {
        public string? DomainUrl { get; set; }
        public string? ClientId { get; set; }
        public string? Region { get; set; }
        public string? UserPoolId { get; set; }
    }
}