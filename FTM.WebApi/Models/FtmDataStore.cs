using FTM.WebApi.Entities;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.WebApi.Models
{
    public class FtmDataStore : IDataStore
    {
        private FtmDbContext _context;
        public FtmDataStore(FtmDbContext context)
        {
            this._context = context;
        }
        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task StoreAsync<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
