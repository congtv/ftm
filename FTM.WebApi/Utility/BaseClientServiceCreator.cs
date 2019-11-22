using FTM.WebApi.Common;
using FTM.WebApi.Entities;
using FTM.WebApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using System.Linq;
using System.Threading;

namespace FTM.WebApi.Utility
{
    public class BaseClientServiceCreator
    {
        public static BaseClientService.Initializer Create(FtmDbContext context, ClientInfo clientInfo, FtmDataStore dataStore)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientInfo.ClientId,
                    ClientSecret = clientInfo.ClientSecret,
                },
                DataStore = dataStore, // match the one defined in OnAuthorizationCodeReceived method
            });
            var ftmToken = context.FtmTokenResponses.First(x => x.UserId == Constains.UserId);

#pragma warning disable CS0612 // Type or member is obsolete
            var tokenResponse = ftmToken.GetTokenResponseInfo();
#pragma warning restore CS0612 // Type or member is obsolete
            var userCredential = new UserCredential(flow, Constains.UserId, tokenResponse);
            return new BaseClientService.Initializer() { HttpClientInitializer = userCredential };
        }
    }
}