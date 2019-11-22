using FTM.WebApi.Entities;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Models
{
    public class FtmDataStore : IDataStore
    {
        private FtmDbContext context;
        private readonly Task CompletedTask = Task.FromResult(0);

        public FtmDataStore(FtmDbContext context)
        {
            this.context = context;
        }

        public Task ClearAsync()
        {
            context.FtmTokenResponses.RemoveRange(context.FtmTokenResponses.ToArray());
            context.SaveChanges();
            return CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var delete = context.FtmTokenResponses.FirstOrDefault(x => x.UserId == key);
            if(delete != null)
            {
                context.FtmTokenResponses.Remove(delete);
                context.SaveChanges();
            }
            else
            {
                ClearAsync();
            }
            return CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            try
            {
                var result = context.FtmTokenResponses.FirstOrDefault(x => x.UserId == key);
                if (result != null)
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    var tokenResponse = NewtonsoftJsonSerializer.Instance.Serialize(result.GetTokenResponseInfo());
#pragma warning restore CS0612 // Type or member is obsolete
                    tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(tokenResponse));
                }
                else
                {
                    tcs.SetResult(default(T));
                }
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var ftmTokenResponse = context.FtmTokenResponses.FirstOrDefault(x => x.UserId == key);
            if (ftmTokenResponse == null)
            {
                ftmTokenResponse = new FtmTokenResponse();
                context.FtmTokenResponses.Add(ftmTokenResponse);
            }
            ftmTokenResponse.SetTokenResponseInfo(value as TokenResponse);
            ftmTokenResponse.UserId = key;
            context.SaveChanges();

            return CompletedTask;
        }
    }
}