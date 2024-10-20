namespace Api.Bff.Dtos
{
    public class ConfirmEmailRequest
    {
        public string? Username { get; set; }
        public string? ConfirmationCode { get; set; }
    }
}
