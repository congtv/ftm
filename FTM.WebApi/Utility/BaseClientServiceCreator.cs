using FTM.WebApi.Common;
using FTM.WebApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Services;
using System.Threading;

namespace FTM.WebApi.Utility
{
    public class BaseClientServiceCreator
    {
        public static BaseClientService.Initializer Create(ClientInfo clientInfo, FtmDataStore dataStore)
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
            var tokenResponse = flow.LoadTokenAsync(Constains.UserId, CancellationToken.None).Result;
            var userCredential = new UserCredential(flow, Constains.UserId, tokenResponse);
            return new BaseClientService.Initializer() { HttpClientInitializer = userCredential };
        }
    }
}