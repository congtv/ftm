using System.ComponentModel.DataAnnotations;

namespace FTM.WebApi.Entities
{
    public class CredentialInfo
    {
        [Key]
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
        public string IdToken { get; set; }
        public string Issued { get; set; }
        public string IssuedUtc { get; set; }

    }
}
