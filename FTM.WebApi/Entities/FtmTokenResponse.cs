using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.WebApi.Entities
{
    public class FtmTokenResponse : TokenResponse
    {
        [Key]
        public string UserId { get; set; }

        public void SetTokenResponseInfo(TokenResponse token)
        {
            AccessToken = token.AccessToken;
            ExpiresInSeconds = token.ExpiresInSeconds;
            IdToken = token.IdToken;
#pragma warning disable CS0618 // Type or member is obsolete
            Issued = token.Issued;
#pragma warning restore CS0618 // Type or member is obsolete
            IssuedUtc = token.IssuedUtc;
            RefreshToken = token.RefreshToken;
            Scope = token.Scope;
            TokenType = token.TokenType;
        }

        [Obsolete]
        public TokenResponse GetTokenResponseInfo()
        {
            return new TokenResponse()
            {
                AccessToken = this.AccessToken,
                ExpiresInSeconds = this.ExpiresInSeconds,
                IdToken = this.IdToken,
                Issued = this.Issued,
                IssuedUtc = this.IssuedUtc,
                RefreshToken = this.RefreshToken,
                Scope = this.Scope,
                TokenType = this.TokenType,
            };
        }
    }
}